using LitBikes.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public BotController(GameController gameController, IClientEventReceiver clientEventReceiver)
        {
            _gameController = gameController;
            _clientEventReceiver = clientEventReceiver;
            _bots = new List<Bot>();  
            //_serverEventSender = serverEventSender;
        }

        private void deployBots(int count)
        {
            Console.WriteLine($"Creating {count} bots");
            for (var i = 0; i < count; i++)
            {
                var player = _gameController.CreateBot();
                var bot = new Bot(player.GetId());
                bot.AttachController(this);
                bot.Start();
                _bots.Add(bot);
            }
            //LOG.info("Current bot count: " + bots.size());
        }

        private void destroyBots(int count)
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
                deployBots(botDeficit);
            else if (botDeficit < 0)
                destroyBots(Math.Abs(botDeficit));
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
