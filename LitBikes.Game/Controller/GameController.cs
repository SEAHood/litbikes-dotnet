using LitBikes.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Timers;
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
        
        public GameController(int _minPlayers, int gameSize, ClientEventController clientEventController)
        {
            var gameEventController = new GameEventController();
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

            minPlayers = _minPlayers;
            game = new GameEngine(gameSize);
            sessionPlayers = new Dictionary<Guid, int>();
            // botController = new BotController(this);

            broadcastWorldTimer.Interval = broadcastWorldInterval;
            broadcastWorldTimer.Elapsed += new ElapsedEventHandler(BroadcastWorldUpdate);
            broadcastWorldTimer.Start();

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

        public Bot BotCreated()
        {
            /*String botName = "BOT#" + String.format("%04d", random.nextInt(10000));
            int pid = pidGen++;
            Player player = game.playerJoin(pid, botName, false);
            Bot bot = new Bot(player.getId(), game.getPlayers(), game.getArena());
            bot.setName(botName);
            bot.setBike(player.getBike());
            sessionPlayers.put(bot.getSessionId(), pid);
            return bot;*/
        }

        public void BotDestroyed(Bot bot)
        {
            /*game.dropPlayer(bot.getId());
            sessionPlayers.remove(bot.getSessionId());*/
        }

        private void BalanceBots()
        {
            /*int totalHumans = (int)game.getPlayers().stream()
                    .filter(p->p.isHuman())
                    .count();
            int requiredBots = Math.max(0, minPlayers - totalHumans);
            LOG.info("Balancing game - " + totalHumans + " humans and " + requiredBots + " bots");
            botController.setBotCount(requiredBots);*/
        }

        // CLIENT EVENTS
        public void ClientJoiningGame(ClientGameJoinDto gameJoinDto)
        {
            /*if (!gameJoinDto.isValid())
            {
                // TODO Implement some error handling
                client.sendEvent(C_ERROR, "invalid name");
                return;
            }

            int pid = sessionPlayers.get(client.getSessionId());
            String name = gameJoinDto.name;
            Player player = game.playerJoin(pid, name, true);

            GameSettingsDto gameSettings = new GameSettingsDto();
            gameSettings.gameTickMs = game.getGameTickMs();

            GameJoinDto dto = new GameJoinDto();
            dto.player = player.getDto();
            dto.scores = game.getScores();

            balanceBots();

            client.sendEvent(C_JOINED_GAME, dto);*/
        }

        public void ClientHello()
        {
            /*int pid = pidGen++;
            sessionPlayers.put(client.getSessionId(), pid);

            GameSettingsDto gameSettings = new GameSettingsDto();
            gameSettings.gameTickMs = game.getGameTickMs();

            HelloDto dto = new HelloDto();
            dto.gameSettings = gameSettings;
            dto.world = game.getWorldDto();

            client.sendEvent(C_HELLO, dto);*/
        }

        public void ClientDisconnectEvent()
        {
            /*try
            {
                Integer pid = sessionPlayers.get(client.getSessionId());
                if (pid == null)
                    return;//throw new Exception("sessionPlayers value was null.. this really shouldn't have happened");
                game.dropPlayer(pid);
            }
            catch (Exception e)
            {

            }
            balanceBots();*/
        }

        public void ClientChatMessageEvent(String message)
        {
            //var pid = sessionPlayers.get(client.getSessionId());
            //if (pid == null)
                //return;//throw new Exception("sessionPlayers value was null.. this really shouldn't have happened");
            var pid = 1; // TODO TEMPORARY
            var player = game.GetPlayer(pid);
            var colour = player.GetBike().GetColour();
            var sourceColour = $"rgba({colour.getRed()},{colour.getGreen()},{colour.getBlue()},%A%)";
            var dto = new ChatMessageDto(player.GetName(), sourceColour, message, false);

            BroadcastData("chat-message", dto);
        }


        public void ClientUpdateEvent(ClientUpdateDto updateDto)
        {
            /*Integer pid = sessionPlayers.get(client.getSessionId());
            if (pid == null)
                return;//throw new Exception("sessionPlayers value was null.. this really shouldn't have happened");

            if (game.HandleClientUpdate(updateDto))
            {
                broadcastWorldUpdate();
            }*/
        }


        public void ClientRequestRespawnEvent()
        {
            /*Integer pid = sessionPlayers.get(client.getSessionId());
            if (pid == null)
                throw new Exception("sessionPlayers value was null.. this really shouldn't have happened");

            Player player = game.getPlayer(pid);
            LOG.info("Respawn request from " + player.getName());
            game.requestRespawn(player);*/
        }


        public void ClientRequestUsePowerUpEvent()
        {
            /*Integer pid = sessionPlayers.get(client.getSessionId());
            if (pid == null)
                throw new Exception("sessionPlayers value was null.. this really shouldn't have happened");

            Player player = game.getPlayer(pid);
            if (player.getCurrentPowerUpType() == null)
                return; // player doesn't have a powerup

            LOG.info("PowerUp used by " + player.getName());
            game.requestUsePowerUp(player);
            broadcastWorldUpdate();*/
        }


        public void ClientHelloEvent()
        {
            /*LOG.info("Received hello");
            clientHello(client);*/
        }

        public void ClientRequestWorldEvent()
        {
            //SendWorldUpdate(client);
        }

        public void ClientKeepAliveEvent()
        {
            // todo time out client after 2 missed keep alives or something?
            //client.sendEvent("keep-alive-ack");
        }

        public void ClientRequestGameJoinEvent(ClientGameJoinDto gameJoinDto)
        {
            /*LOG.info("Received game join request event");
            clientJoiningGame(client, gameJoinDto);

            // TODO Make sure client isn't trying to rejoin - i.e. if they already have a name

            String newPlayerMessage = gameJoinDto.name + " joined!";
            ChatMessageDto dto = new ChatMessageDto(null, null, newPlayerMessage, true);
            broadcastData("chat-message", dto);*/
        }

	    // END CLIENT EVENTS

    }
}
