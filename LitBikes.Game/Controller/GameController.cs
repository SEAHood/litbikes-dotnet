using LitBikes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LitBikes.Ai;
using LitBikes.Game.Engine;
using LitBikes.Model.Dtos;
using LitBikes.Events;
using LitBikes.Model.Dtos.FromClient;
using LitBikes.Model.Enums;

namespace LitBikes.Game.Controller
{
    public class GameController
    {
        private const int BroadcastWorldInterval = 25; // MS

        private readonly GameEngine _game;
	    private readonly BotController _botController;
        private readonly int _minPlayers;
        private readonly IServerEventSender _eventSender;
        private readonly Thread _worldUpdateThread;
        private readonly CancellationTokenSource _worldUpdateCancellation;
        private DateTime _nextWorldUpdate;
        private readonly Thread t;
        
        public GameController(IClientEventReceiver clientEventReceiver, IServerEventSender serverEventSender, GameSettings settings)
        {
            var gameEventController = new GameEventController();
            SetupGameEventHandlers(gameEventController);
            SetupClientEventHandlers(clientEventReceiver);

            _eventSender = serverEventSender;
            _minPlayers = settings.MinPlayers;

            _game = new GameEngine(gameEventController, settings);

            _botController = new BotController(this, clientEventReceiver);

            /*_worldUpdateThread = new Thread(delegate ()
            {
                while (true)
                {
                    Thread.Sleep(BroadcastWorldInterval);
                    BroadcastWorldUpdate();
                }
            });*/
            _worldUpdateCancellation = new CancellationTokenSource();
            _worldUpdateThread = new Thread(delegate ()
            {
                while (true)
                {
                    if (DateTime.UtcNow <= _nextWorldUpdate) continue;
                    //_worldUpdateCancellation.Cancel(); // Cancel update already in progress
                    Task.Run(() => BroadcastWorldUpdate());//, _worldUpdateCancellation.Token);
                    _nextWorldUpdate = DateTime.UtcNow.AddMilliseconds(BroadcastWorldInterval);
                }
            });

            t = new Thread(delegate()
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine($"{worldUpdatesPerSecond} world updates per second");
                    worldUpdatesPerSecond = 0;
                }
            });
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
                        var helloDto = new HelloDto
                        {
                            GameSettings = new GameSettingsDto
                            {
                                GameTickMs = _game.GetGameTickMs()
                            },
                            World = _game.GetWorldDto()
                        };

                        _eventSender.SendEvent(ServerEvent.Hello, helloDto, args.PlayerId);
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
                        _eventSender.SendEvent(ServerEvent.ChatMessage, dto, null);
                        break;
                    case ClientEvent.KeepAlive:
                        _eventSender.SendEvent(ServerEvent.KeepAliveAck, args.PlayerId);
                        break;
                    case ClientEvent.RequestJoinGame:
                        RequestGameJoin(args.PlayerId, (ClientGameJoinDto)args.Dto);
                        break;
                    case ClientEvent.RequestRespawn:
                        _game.RequestRespawn(player);
                        break;
                    case ClientEvent.Update:
                        var updateDto = (ClientUpdateDto)args.Dto;
                        _game.HandleClientUpdate(updateDto);
                        break;
                    case ClientEvent.UsePowerup:
                        if (player.GetCurrentPowerUpType() == PowerUpType.Nothing)
                            return; // player doesn't have a powerup
                        _game.RequestUsePowerUp(player);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }

        public void RequestGameJoin(Guid playerId, ClientGameJoinDto dto)
        {
            if (!dto.IsValid())
                return;

            var player = _game.PlayerJoin(playerId, dto.Name, true);
            var gameJoinDto = new GameJoinDto
            {
                Player = player.GetDto(),
                Scores = _game.GetScores()
            };

            BalanceBots();
            _eventSender.SendEvent(ServerEvent.JoinedGame, gameJoinDto, playerId);
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
            _game.Start();
            BalanceBots();
            _game.NewRound();
            _botController.Start();
            _worldUpdateThread.Start();
            t.Start();
        }

        //// START GAME EVENTS

        public void PlayerCrashed(Player player)
        {
            var crashedInto = player.IsCrashedIntoSelf() ? "their own trail!" : player.GetCrashedInto().GetName();
            var playerCrashedMessage = player.GetName() + " crashed into " + crashedInto;
            SendServerMessage(playerCrashedMessage);
            Console.WriteLine(playerCrashedMessage);
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
            _eventSender.SendEvent(ServerEvent.ChatMessage, dto, null);
        }

        public void PlayerSpawned()
        {
            //BroadcastWorldUpdate();
        }

        public void ScoreUpdated()
        {
            // Todo: send a score object instead, that contains list of scores, winner, etc
            _eventSender.SendListEvent(ServerEvent.ScoreUpdate, new List<IDto>(_game.GetScores()), null);
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
            _game.RoundEnded();
            _game.NewRound();
        }
        // END GAME EVENTS

        public int worldUpdatesPerSecond = 0;
        public void BroadcastWorldUpdate()
        {
            worldUpdatesPerSecond++;
            var startTime = DateTime.Now;
            //Console.WriteLine($"Sent game event at {startTime}");
            _eventSender.SendEvent(ServerEvent.WorldUpdate, _game.GetWorldDto(), null);
            _botController.DoUpdate(_game.GetPlayers(), _game.GetArena());
        }

        /*private ServerWorldDto GetWorldDiff(out ServerWorldDto currentWorldDto)
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
        }*/


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
