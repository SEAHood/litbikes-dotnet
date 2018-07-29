using LitBikes.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LitBikes.Model;
using LitBikes.Model.Dtos.FromClient;
using LitBikes.Game.Controller;

namespace LitBikes.Ai
{
    public class BotController
    {

        private List<Bot> _bots;
        private IClientEventReceiver _clientEventReceiver;
        private ServerEventSender _serverEventSender;

        private GameController _gameController;
        private Thread _tickThread;
        private DateTime _nextTickTime;

        private const int AiTickMs = 100;
        private const int AiRespawnMs = 3000;


        //public double AiTickMs { get; private set; }

        public BotController(GameController gameController, IClientEventReceiver clientEventReceiver)
        {
            _gameController = gameController;
            _clientEventReceiver = clientEventReceiver;
            _bots = new List<Bot>();  
            //_serverEventSender = serverEventSender;
        }

        public void Start()
        {
            _tickThread = new Thread(delegate ()
            {
                while (true)
                {
                    if (DateTime.UtcNow <= _nextTickTime) continue;
                    _nextTickTime = DateTime.UtcNow.AddMilliseconds(AiTickMs);
                    _bots.ToList().ForEach(b =>
                    {
                        if (!b.HasBike()) return;
                        if (!b.IsCrashed())
                        {
                            var result = b.PredictCollision();
                            if (result != null)
                                SendUpdate(b.GetId(), result);
                        }
                        else
                        {
                            if (b.RespawnTime == null)
                                b.RespawnTime = DateTime.UtcNow.AddMilliseconds(AiRespawnMs);
                            else if (b.RespawnTime <= DateTime.UtcNow)
                            {
                                SendRespawnRequest(b.GetId());
                                b.RespawnTime = null;
                            }
                        }
                    });
                }
            });
            _tickThread.Start();
        }

        private List<Bot> GetBots()
        {
            return _bots.ToList();
        }

        private void DeployBots(int count)
        {
            Console.WriteLine($"Creating {count} bots");
            for (var i = 0; i < count; i++)
            {
                var player = _gameController.CreateBot();
                var bot = new Bot(player.GetId());
                _bots.Add(bot);
            }

            //LOG.info("Current bot count: " + bots.size());
        }

        private void DestroyBots(int count)
        {
            // TODO this is bad i think
            //LOG.info("Destroying " + count + " bots");
            var doomedBots = _bots.Take(count).ToList();

            foreach (var b in doomedBots)
            {
                _gameController.BotDestroyed(b.GetId());
                _bots.Remove(b);
            }

            //LOG.info("Current bot count: " + bots.size());
        }

        public int GetBotCount()
        {
            return _bots.Count;
        }

        public void SetBotCount(int botCount)
        {
            if (botCount < 0)
                return;
            //LOG.info("Setting bot count to " + botCount);
            var currentBotCount = _bots.Count;
            var botDeficit = botCount - currentBotCount;
            if (botDeficit > 0)
                DeployBots(botDeficit);
            else if (botDeficit < 0)
                DestroyBots(Math.Abs(botDeficit));
        }

        public void DoUpdate(List<Player> players, Arena arena)
        {
            foreach (var bot in _bots)
            {
                var botPlayer = players.FirstOrDefault(p => p.GetId() == bot.GetId());
                if (botPlayer != null)
                    bot.SetCrashed(botPlayer.IsCrashed());
                bot.UpdateWorld(players, arena);
            }
        }

        /*public void sendWorldRequest(BotIOClient client)
        {
            _clientEventReceiver.
            gameController.clientRequestWorldEvent(client);
        }*/

        public void SendUpdate(Guid playerId, ClientUpdateDto updateDto)
        {
            _clientEventReceiver.Update(playerId, updateDto);
        }

        public void SendRespawnRequest(Guid botId)
        {
            _clientEventReceiver.RequestRespawn(botId);
        }
    }
}
