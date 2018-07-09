using LitBikes.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Timers;
using LitBikes.Game.Engine;
using LitBikes.Model.Dtos;
using LitBikes.Events;
using LitBikes.Model.Enums;

namespace LitBikes.Game.Controller
{
    public class GameController// : GameEventListener
    {
        //private static Logger LOG = Log.getLogger(GameController.class);
	    private Dictionary<Guid, int> sessionPlayers; //session ID -> engine player ID
        private GameEngine game;
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
            game = new GameEngine(gameEventController, gameSize);
            sessionPlayers = new Dictionary<Guid, int>();
            // botController = new BotController(this);

            broadcastWorldTimer = new Timer { Interval = broadcastWorldInterval };
            broadcastWorldTimer.Elapsed += (sender, e) => BroadcastWorldUpdate();
            broadcastWorldTimer.Start();
            
        }

        private void SetupClientEventHandlers(IClientEventReceiver clientEventReceiver)
        {
            clientEventReceiver.Event += (sender, args) =>
            {
                switch (args.Event)
                {
                    case ClientEvent.Hello:
                        //int pid = pidGen++;
                        //sessionPlayers.put(client.getSessionId(), pid);
                        game.PlayerJoin(args.PlayerId, "Test Player", true);

                        var dto = new HelloDto
                        {
                            GameSettings = new GameSettingsDto
                            {
                                GameTickMs = game.GetGameTickMs()
                            },
                            World = game.GetWorldDto()
                        };

                        _eventSender.SendEvent(ServerEvent.Hello, dto);
                        //client.sendEvent(C_HELLO, dto);
                        break;
                    case ClientEvent.ChatMessage:
                        var player = game.GetPlayer(args.PlayerId);
                        _clientEventHandler.ClientChatMessageEvent(player, "something");
                        break;
                    case ClientEvent.KeepAlive:
                        _clientEventHandler.ClientKeepAliveEvent();
                        break;
                    case ClientEvent.RequestJoinGame:
                        _clientEventHandler.ClientRequestGameJoinEvent(null);
                        break;
                    case ClientEvent.RequestRespawn:
                        _clientEventHandler.ClientRequestRespawnEvent();
                        break;
                    case ClientEvent.Update:
                        _clientEventHandler.ClientUpdateEvent(null);
                        break;
                    case ClientEvent.UsePowerup:
                        _clientEventHandler.ClientRequestUsePowerUpEvent();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }

        private void SetupGameEventHandlers(GameEventController gameEventController)
        {
            gameEventController.Event += (sender, args) =>
            {
                switch (args.Event)
                {
                    case GameEvent.PlayerCrashed:
                        PlayerCrashed(null);
                        break;
                    case GameEvent.PlayerSpawned:
                        PlayerSpawned(1);
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
            game.Start();
            //BalanceBots();
            game.StartRound();
            broadcastWorldTimer.Start();
        }

        //// START GAME EVENTS
        //private void setupGameListeners()
        //{
        //    game.attachListener(this);
        //}


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

        public void PlayerSpawned(int pid)
        {
            BroadcastWorldUpdate();
        }

        public void ScoreUpdated()
        {
            _eventSender.SendEvent(ServerEvent.ScoreUpdate, game.GetScores());
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
            game.StartRound();
        }
        // END GAME EVENTS


        public void SendWorldUpdate()//SocketIOClient client)
        {
            /*if (client != null)
            {
                if (client instanceof BotIOClient ) 
				        ((BotIOClient)client).updateBot(game.getPlayers(), game.getArena());
			        else
				        client.sendEvent("world-update", game.getWorldDto());
            }*/
        }

        public void BroadcastWorldUpdate()
        {
            _eventSender.SendEvent(ServerEvent.WorldUpdate, game.GetWorldDto());
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
