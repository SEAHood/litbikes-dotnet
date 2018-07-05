using LitBikes.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Timers;
using LitBikes.Game.Engine;
using LitBikes.Model.Dtos;

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

        private ClientEventHandler _clientEventHandler;
        
        public GameController(int _minPlayers, int gameSize, ClientEventController clientEventController)
        {
            var gameEventController = new GameEventController();
            _clientEventHandler = new ClientEventHandler();
            SetupGameEventHandlers(gameEventController);
            SetupClientEventHandlers(clientEventController);

            minPlayers = _minPlayers;
            game = new GameEngine(gameEventController, gameSize);
            sessionPlayers = new Dictionary<Guid, int>();
            // botController = new BotController(this);

            broadcastWorldTimer.Interval = broadcastWorldInterval;
            broadcastWorldTimer.Elapsed += (sender, e) => BroadcastWorldUpdate();
            broadcastWorldTimer.Start();

        }

        private void SetupClientEventHandlers(ClientEventController clientEventController)
        {
            clientEventController.Event += (sender, args) =>
            {
                switch (args.Event)
                {
                    case ClientEvent.Hello:
                        _clientEventHandler.ClientHello();
                        break;
                    case ClientEvent.ChatMessage:
                        _clientEventHandler.ClientChatMessageEvent("something");
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
                        ScoreUpdated(null);
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
            String crashedInto = player.IsCrashedIntoSelf() ? "their own trail!" : player.GetCrashedInto().GetName();
            String playerCrashedMessage = player.GetName() + " crashed into " + crashedInto;
            //LOG.info(playerCrashedMessage);
            ChatMessageDto dto = new ChatMessageDto(null, null, playerCrashedMessage, true);
            BroadcastData("chat-message", dto);
            BroadcastWorldUpdate();
        }

        public void PlayerSpawned(int pid)
        {
            BroadcastWorldUpdate();
        }

        public void ScoreUpdated(List<ScoreDto> scores)
        {
            BroadcastData("score-update", scores);
        }

        public void RoundStarted()
        {
            String msg = "Round started!";
            ChatMessageDto dto = new ChatMessageDto(null, null, msg, true);
            BroadcastData("chat-message", dto);
            BroadcastData("score-update", game.GetScores());
        }

        public void RoundEnded()
        {
            game.StartRound();

            String msg = "Round ended!";
            ChatMessageDto dto = new ChatMessageDto(null, null, msg, true);
            BroadcastData("chat-message", dto);
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
            //ioServer.getBroadcastOperations().sendEvent("world-update", game.getWorldDto());
            //botController.doUpdate(game.getPlayers(), game.getArena());
        }

        public void BroadcastData(String key, Object obj)
        {
            //ioServer.getBroadcastOperations().sendEvent(key, obj);
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
