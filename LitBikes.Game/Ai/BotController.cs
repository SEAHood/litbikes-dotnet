using LitBikes.Events;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private GameController _gameController;
        private Thread _tickThread;
        private DateTime _nextTickTime;

        private const int AiTickMs = 200;
        private const int AiRespawnMs = 3000;
        
        public BotController(GameController gameController, IClientEventReceiver clientEventReceiver)
        {
            _gameController = gameController;
            _clientEventReceiver = clientEventReceiver;
            _bots = new List<Bot>();
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
                            var behaviourResult = b.ExhibitBehaviour();
                            var collisionResult = b.PredictCollision();

                            // Prefer the behaviour result
                            if (behaviourResult != null)
                            {
                                SendUpdate(b.GetId(), behaviourResult);
                                return;
                            }

                            if (collisionResult != null)
                                SendUpdate(b.GetId(), collisionResult);
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

            Console.WriteLine("Current bot count: " + _bots.Count);
        }

        private void DestroyBots(int count)
        {
            // TODO this is bad i think
            Console.WriteLine($"Destroying {count} bots");
            var doomedBots = _bots.Take(count).ToList();

            foreach (var b in doomedBots)
            {
                _gameController.BotDestroyed(b.GetId());
                _bots.Remove(b);
            }
            Console.WriteLine($"Current bot count: {_bots.Count}");
        }

        public int GetBotCount()
        {
            return _bots.Count;
        }

        public void SetBotCount(int botCount)
        {
            if (botCount < 0)
                return;
            Console.WriteLine($"Setting bot count to {botCount}");
            var currentBotCount = _bots.Count;
            var botDeficit = botCount - currentBotCount;
            if (botDeficit > 0)
                DeployBots(botDeficit);
            else if (botDeficit < 0)
                DestroyBots(Math.Abs(botDeficit));
        }

        public void DoUpdate(List<Player> players, Arena arena)
        {
            foreach (var bot in _bots.ToList())
            {
                var botPlayer = players.FirstOrDefault(p => p.GetId() == bot.GetId());
                if (botPlayer != null)
                    bot.SetCrashed(botPlayer.IsCrashed());
                bot.UpdateWorld(players, arena);
            }
        }

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
