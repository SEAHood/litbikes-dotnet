using LitBikes.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;
using LitBikes.Ai;
using LitBikes.Game.Engine;
using LitBikes.Model.Dtos;
using LitBikes.Events;
using LitBikes.Model.Dtos.FromClient;
using LitBikes.Model.Enums;

namespace LitBikes.Game.Controller
{
    public class GameController// : GameEventListener
    {
        //private static Logger LOG = Log.getLogger(GameController.class);
	    private Dictionary<Guid, int> sessionPlayers; //session ID -> engine player ID
        private readonly GameEngine _game;
	    private readonly BotController _botController;
	    private int pidGen = 0;

        private static String C_HELLO = "hello";
        private static String C_JOINED_GAME = "joined-game";
        private static String C_ERROR = "error";

        private static int ROUND_TIME = 300;
        private static int ROUND_DELAY = 15;

        private readonly int _minPlayers;
        private Random random = new Random();

        private readonly Timer _broadcastWorldTimer;
        private readonly int _broadcastWorldInterval = 25; // MS

        //private readonly ClientEventHandler _clientEventHandler;
        private readonly IServerEventSender _eventSender;
        
        public GameController(IClientEventReceiver clientEventReceiver, IServerEventSender serverEventSender)
        {
            var gameEventController = new GameEventController();
            //_clientEventHandler = new ClientEventHandler();
            SetupGameEventHandlers(gameEventController);
            SetupClientEventHandlers(clientEventReceiver);

            _eventSender = serverEventSender;
            _minPlayers = 10;

            _game = new GameEngine(gameEventController, new GameSettings
            {
                ArenaSize = 600,
                RoundDuration = 120,
                RoundCountdownDuration = 15
            });

            sessionPlayers = new Dictionary<Guid, int>();
            _botController = new BotController(this, clientEventReceiver);
            _botController.Start();

            _broadcastWorldTimer = new Timer { Interval = _broadcastWorldInterval };
            _broadcastWorldTimer.Elapsed += (sender, e) => BroadcastWorldUpdate();
            _broadcastWorldTimer.Start();
            
            Start();
        }

        private void SetupClientEventHandlers(IClientEventReceiver clientEventReceiver)
        {
            clientEventReceiver.Event += (sender, args) =>
            {
                var player = _game.GetPlayer(args.PlayerId);
                switch (args.Event)
                {
                    case ClientEvent.Hello:
                        //int pid = pidGen++;
                        //sessionPlayers.put(client.getSessionId(), pid);
                        //_game.PlayerJoin(args.PlayerId, "Test Player", true);

                        var helloDto = new HelloDto
                        {
                            GameSettings = new GameSettingsDto
                            {
                                GameTickMs = _game.GetGameTickMs()
                            },
                            World = _game.GetWorldDto()
                        };

                        _eventSender.SendEvent(ServerEvent.Hello, helloDto);
                        break;
                    case ClientEvent.ChatMessage:
                        var messageDto = (ClientChatMessageDto)args.Dto;
                        var colour = player.GetBike().GetColour();
                        var sourceColour = $"rgba({colour.R:X2},{colour.G:X2},{colour.B:X2},%A%)";
                        var dto = new ChatMessageDto
                        {
                            IsSystemMessage = false,
                            Message = messageDto.Message,
                            SourceColour = sourceColour,
                            Source = player.GetName(),
                            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                        };
                        _eventSender.SendEvent(ServerEvent.ChatMessage, dto);
                        break;
                    case ClientEvent.KeepAlive:
                        _eventSender.SendEvent(ServerEvent.KeepAliveAck);
                        break;
                    case ClientEvent.RequestJoinGame:
                        RequestGameJoin(args.PlayerId, (ClientGameJoinDto)args.Dto);
                        break;
                    case ClientEvent.RequestRespawn:
                        _game.RequestRespawn(player);
                        break;
                    case ClientEvent.Update:
                        var updateDto = (ClientUpdateDto)args.Dto;
                        _game.HandleClientUpdate(updateDto);//if ()
                            //BroadcastWorldUpdate();
                        break;
                    case ClientEvent.UsePowerup:
                        if (player.GetCurrentPowerUpType() == PowerUpType.Nothing)
                            return; // player doesn't have a powerup
                        _game.RequestUsePowerUp(player);
                        //BroadcastWorldUpdate();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }


        public void RequestGameJoin(Guid playerId, ClientGameJoinDto dto)
        {
            //var gameJoinDto = (ClientGameJoinDto) args.Dto;


            if (!dto.IsValid())
                return;
            //throw new InvalidPayloadException("Invalid name"); // TODO Refactor - error from IsValid()

            //int pid = sessionPlayers.get(client.getSessionId());
            //String name = gameJoinDto.name;
            var player = _game.PlayerJoin(playerId, dto.Name, true);

            //var gameSettings = new GameSettingsDto { GameTickMs = _game.GetGameTickMs() };

            var gameJoinDto = new GameJoinDto
            {
                Player = player.GetDto(),
                Scores = _game.GetScores()
            };

            BalanceBots();
            _eventSender.SendEvent(ServerEvent.JoinedGame, gameJoinDto);
        }

        private void SetupGameEventHandlers(GameEventController gameEventController)
        {
            gameEventController.Event += (sender, args) =>
            {
                switch (args.Event)
                {
                    case GameEvent.PlayerCrashed:
                        PlayerCrashed(args.Player);
                        break;
                    case GameEvent.PlayerSpawned:
                        PlayerSpawned();
                        break;
                    case GameEvent.ScoreUpdated:
                        ScoreUpdated();
                        break;
                    case GameEvent.GameStarted:
                        break;
                    case GameEvent.RoundStarted:
                        RoundStarted();
                        break;
                    case GameEvent.RoundEnded:
                        RoundEnded();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }

        public void Start()
        {
            //SetupGameListeners();
            _game.Start();
            BalanceBots();
            _game.NewRound();
            _broadcastWorldTimer.Start();
        }

        //// START GAME EVENTS

        public void PlayerCrashed(Player player)
        {
            var crashedInto = player.IsCrashedIntoSelf() ? "their own Trail!" : player.GetCrashedInto().GetName();
            var playerCrashedMessage = player.GetName() + " crashed into " + crashedInto;
            SendServerMessage(playerCrashedMessage);
            //BroadcastWorldUpdate();
        }

        private void SendServerMessage(string message)
        {
            var dto = new ChatMessageDto
            {
                IsSystemMessage = true,
                Message = message,
                SourceColour = null,
                Source = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
            _eventSender.SendEvent(ServerEvent.ChatMessage, dto);
        }

        public void PlayerSpawned()
        {
            //BroadcastWorldUpdate();
        }

        public void ScoreUpdated()
        {
            _eventSender.SendEvent(ServerEvent.ScoreUpdate, _game.GetScores());
        }

        public void RoundStarted()
        {
            const string msg = "Round started!";
            SendServerMessage(msg);
            ScoreUpdated();
            _game.RoundStarted();
        }

        public void RoundEnded()
        {
            const string msg = "Round ended!";
            SendServerMessage(msg);
            _game.NewRound();
        }
        // END GAME EVENTS


        private ServerWorldDto _lastWorldUpdate;
        public void BroadcastWorldUpdate()
        {
            var worldDto = GetWorldDiff(out var currentWorldDto);
            _lastWorldUpdate = currentWorldDto;
            _eventSender.SendEvent(ServerEvent.Dev, worldDto);
            _eventSender.SendEvent(ServerEvent.WorldUpdate, _game.GetWorldDto());
            _botController.DoUpdate(_game.GetPlayers(), _game.GetArena());
        }

        private ServerWorldDto GetWorldDiff(out ServerWorldDto currentWorldDto)
        {
            currentWorldDto = _game.GetWorldDto();
            if (_lastWorldUpdate == null) return currentWorldDto;

            var lastWorld = _lastWorldUpdate;
            var type = currentWorldDto.GetType();
            var diff = new ServerWorldDto();
            foreach (var prop in type.GetProperties())
            {
                var lastWorldProp = prop.GetValue(lastWorld);
                var thisWorldProp = prop.GetValue(currentWorldDto);
                prop.SetValue(diff, Equals(lastWorldProp, thisWorldProp) ? null : thisWorldProp);
            }
            return diff;
        }


        public Player CreateBot()
        {
            var botId = Guid.NewGuid();
            var botName = $"BOT#{botId.ToString().Substring(0, 4)}";
            var bot = _game.PlayerJoin(botId, botName, false);
            return bot;
        }

        public void BotDestroyed(Guid botId)
        {
            _game.DropPlayer(botId);
        }

        private void BalanceBots()
        {
            var totalHumans = _game.GetPlayers().Count(p => p.IsHuman());
            var requiredBots = Math.Max(0, _minPlayers - totalHumans);
            Console.WriteLine($"Balancing game - {totalHumans} humans and {requiredBots} bots");
            _botController.SetBotCount(requiredBots);
        }

        
    }
}
