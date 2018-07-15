using LitBikes.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Timers;
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
	    //private BotController botController;
	    private int pidGen = 0;

        private static String C_HELLO = "hello";
        private static String C_JOINED_GAME = "joined-game";
        private static String C_ERROR = "error";

        private static int ROUND_TIME = 300;
        private static int ROUND_DELAY = 15;

        private int minPlayers;
        private Random random = new Random();

        private Timer broadcastWorldTimer;
        private int broadcastWorldInterval = 100; // MS

        private readonly ClientEventHandler _clientEventHandler;
        private IServerEventSender _eventSender;
        
        public GameController(IClientEventReceiver clientEventReceiver, IServerEventSender serverEventSender)
        {
            var gameEventController = new GameEventController();
            _clientEventHandler = new ClientEventHandler();
            SetupGameEventHandlers(gameEventController);
            SetupClientEventHandlers(clientEventReceiver);

            _eventSender = serverEventSender;
            minPlayers = 5;
            var gameSize = 600;
            _game = new GameEngine(gameEventController, gameSize);
            sessionPlayers = new Dictionary<Guid, int>();
            // botController = new BotController(this);

            broadcastWorldTimer = new Timer { Interval = broadcastWorldInterval };
            broadcastWorldTimer.Elapsed += (sender, e) => BroadcastWorldUpdate();
            broadcastWorldTimer.Start();
            
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
                        var dto = new ChatMessageDto(player.GetName(), sourceColour, messageDto.Message, false);
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
                        if (_game.HandleClientUpdate(updateDto))
                            BroadcastWorldUpdate();
                        break;
                    case ClientEvent.UsePowerup:
                        if (player.GetCurrentPowerUpType() == PowerUpType.Nothing)
                            return; // player doesn't have a powerup
                        _game.RequestUsePowerUp(player);
                        BroadcastWorldUpdate();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }


        private void RequestGameJoin(Guid playerId, ClientGameJoinDto dto)
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

            //balanceBots();
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
            //BalanceBots();
            _game.StartRound();
            broadcastWorldTimer.Start();
        }

        //// START GAME EVENTS

        public void PlayerCrashed(Player player)
        {
            var crashedInto = player.IsCrashedIntoSelf() ? "their own trail!" : player.GetCrashedInto().GetName();
            var playerCrashedMessage = player.GetName() + " crashed into " + crashedInto;
            SendServerMessage(playerCrashedMessage);
            BroadcastWorldUpdate();
        }

        private void SendServerMessage(string message)
        {
            var dto = new ChatMessageDto(null, null, message, true);
            _eventSender.SendEvent(ServerEvent.ChatMessage, dto);
        }

        public void PlayerSpawned()
        {
            BroadcastWorldUpdate();
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
        }

        public void RoundEnded()
        {
            const string msg = "Round ended!";
            SendServerMessage(msg);
            _game.StartRound();
        }
        // END GAME EVENTS


        public void BroadcastWorldUpdate()
        {
            _eventSender.SendEvent(ServerEvent.WorldUpdate, _game.GetWorldDto());
            //ioServer.getBroadcastOperations().sendEvent("world-update", game.getWorldDto());
            //botController.doUpdate(game.getPlayers(), game.getArena());
        }


        /*public Bot BotCreated()
        {
            /*String botName = "BOT#" + String.format("%04d", random.nextInt(10000));
            int pid = pidGen++;
            Player player = game.playerJoin(pid, botName, false);
            Bot bot = new Bot(player.getId(), game.getPlayers(), game.getArena());
            bot.setName(botName);
            bot.setBike(player.getBike());
            sessionPlayers.put(bot.getSessionId(), pid);
            return bot;#1#
        }

        public void BotDestroyed(Bot bot)
        {
            /*game.dropPlayer(bot.getId());
            sessionPlayers.remove(bot.getSessionId());#1#
        }*/

        private void BalanceBots()
        {
            /*int totalHumans = (int)game.getPlayers().stream()
                    .filter(p->p.isHuman())
                    .count();
            int requiredBots = Math.max(0, minPlayers - totalHumans);
            LOG.info("Balancing game - " + totalHumans + " humans and " + requiredBots + " bots");
            botController.setBotCount(requiredBots);*/
        }

        
    }
}
