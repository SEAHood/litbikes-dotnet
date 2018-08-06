using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LitBikes.Model;
using LitBikes.Model.Dtos.FromClient;
using LitBikes.Util;

namespace LitBikes.Ai
{
    public class Bot
    {
        private readonly Guid _botId;
        private bool _crashed;
        private Bike _botBike;
	    private List<Player> _players;
        private Arena _arena;
        private readonly Random _rand = new Random();
        private readonly float _randomTurnChance;

        public DateTime? RespawnTime { get; set; }
        
        public Guid GetId() => _botId;

        public Bot(Guid botId)
        {
            _botId = botId;
            _players = new List<Player>();
            _randomTurnChance = NumberUtil.RandFloat(0.2f, 0.8f);
        }

        public bool HasBike()
        {
            return _botBike != null;
        }

        public void SetCrashed(bool crashed)
        {
            _crashed = crashed;
        }
        
        public void UpdateWorld(List<Player> players, Arena arena)
        {
            _arena = arena;
            _players = players;
            _botBike = players.FirstOrDefault(p => p.GetId() == _botId)?.GetBike();
        }
        
        public bool IsCrashed()
        {
            return _crashed;
        }

        public ClientUpdateDto ExhibitBehaviour()
        {
            var rand = NumberUtil.RandInt(0, (int)(_randomTurnChance * 100));
            if (rand == 0)
                return RandomDirectionDto();
            return null;
        }

        private ClientUpdateDto RandomDirectionDto()
        {
            var newVal = _rand.Next(0, 2) == 0 ? -1 : 1;
            var updateDto = new ClientUpdateDto { PlayerId = _botId };

            if (_botBike.GetDir().X != 0)
            {
                updateDto.XDir = 0;
                updateDto.YDir = newVal;
            }
            else if (_botBike.GetDir().Y != 0)
            {
                updateDto.XDir = newVal;
                updateDto.YDir = 0;
            }

            return updateDto;
        }

        public ClientUpdateDto PredictCollision()
        {
            var dDist = 20;
            var activePlayers = _players.Where(p => p.IsAlive()).ToList();

            var allTrails = new List<TrailSegment>();
            foreach (var p in activePlayers)
            {
                var isSelf = p.GetId() == _botId;
                allTrails.AddRange(p.GetBike().GetTrailSegmentList(!isSelf));
            }

            var incCollision = _botBike.Collides(allTrails, dDist, out _) || _arena.CheckCollision(_botBike, dDist);

            if (!incCollision) return null;

            var newVal = _rand.Next(0, 2) == 0 ? -1 : 1;
            var updateDto = new ClientUpdateDto {PlayerId = _botId};

            if (_botBike.GetDir().X != 0)
            {
                updateDto.XDir = 0;
                updateDto.YDir = newVal;
            }
            else if (_botBike.GetDir().Y != 0)
            {
                updateDto.XDir = newVal;
                updateDto.YDir = 0;
            }

            return RandomDirectionDto();
        }

        public void SetRespawnTime(DateTime dateTime)
        {
            RespawnTime = dateTime;
        }
    }
}
