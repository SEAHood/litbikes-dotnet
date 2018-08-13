using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using LitBikes.Model;
using LitBikes.Model.Dtos;
using LitBikes.Util;
using Nine.Geometry;

namespace LitBikes.Game.Engine
{
    class PowerUpKeeper
    {
        private readonly Dictionary<Guid, PowerUp> _playerPowerUps;
        private readonly List<PowerUp> _availablePowerUps;
        private readonly int _gameSize;

        private bool _isSpawning;
        private Thread _spawnThread;

        private readonly int SpawnDelayMin = 4000;
        private readonly int SpawnDelayMax = 7000;
        private readonly int DurationMin = 10000;
        private readonly int DurationMax = 20000;

        public PowerUpKeeper(int gameSize)
        {
            _gameSize = gameSize;
            _availablePowerUps = new List<PowerUp>();
            _playerPowerUps = new Dictionary<Guid, PowerUp>();
        }

        public void StartSpawner()
        {
            if (!_isSpawning)
            {
                _isSpawning = true;
                _spawnThread = new Thread(delegate ()
                {
                    while (true)
                    {
                        Thread.Sleep(NumberUtil.RandInt(SpawnDelayMin, SpawnDelayMax));
                        SpawnPowerUp();
                    }
                });
                _spawnThread.Start();
            }
        }

        public void StopSpawner()
        {
            if (_isSpawning)
            {
                _isSpawning = false;
            }
        }

        public void SpawnPowerUp()
        {
            PowerUpType type = PowerUpType.Slow;
            if (NumberUtil.RandInt(0, 2) == 0)
                type = PowerUpType.Rocket;
            else
                type = PowerUpType.Slow;

            var powerUpPos = new Vector2(NumberUtil.RandInt(0, _gameSize), NumberUtil.RandInt(0, _gameSize));
            PowerUp powerUp = new PowerUp(powerUpPos, type);
            _availablePowerUps.Add(powerUp);
            Console.WriteLine($"Spawned {type} powerup at {powerUpPos.X}, {powerUpPos.Y}");
            
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(NumberUtil.RandInt(DurationMin, DurationMax));
                if (!powerUp.IsCollected())
                    _availablePowerUps.Remove(powerUp);
            });
        }

        public void PlayerCollectedPowerUp(Player player, PowerUp powerUp)
        {
            var availablePowerUp = _availablePowerUps.FirstOrDefault(p => p.GetId() == powerUp.GetId());
            if (availablePowerUp == null || !availablePowerUp.Equals(powerUp))
                throw new Exception("PowerUp does not exist");

            _playerPowerUps.Remove(player.GetId());
            _playerPowerUps.Add(player.GetId(), availablePowerUp);

            _availablePowerUps.Remove(availablePowerUp);
        }

        public void PlayerRequestsUse(Player player, List<Player> playerList, List<TrailSegment> trails, int gameSize)
        {
            if (!_playerPowerUps.TryGetValue(player.GetId(), out var powerUp)) return;
            _playerPowerUps.Remove(player.GetId());
            switch (powerUp.GetPowerUpType())
            {
                case PowerUpType.Rocket:
                    var pos = player.GetBike().GetPosAsPoint();
                    var dir = player.GetBike().GetDir();

                    var wallAhead = pos; // This will change
                    if (dir.X == 0)
                    {
                        // moving on y axis
                        if (dir.Y > 0)
                            wallAhead = new Point(pos.X, gameSize);
                        else if (dir.Y < 0)
                            wallAhead = new Point(pos.X, 0);
                    }
                    else if (dir.Y == 0)
                    {
                        // moving on x axis
                        if (dir.X > 0)
                            wallAhead = new Point(gameSize, pos.Y);
                        else if (dir.X < 0)
                            wallAhead = new Point(0, pos.Y);
                    }


                    var ray = new LineSegment2D(pos.ToVector2(), wallAhead.ToVector2());
                    var impactPoint = Physics.FindClosestImpactPoint(pos, ray, trails);
                    if (impactPoint == null)
                    {
                        //LOG.warn("Impact point was null");
                        break;
                    }
                    if (impactPoint.GetTrailSegment() == null)
                    {
                        //LOG.warn("Trail segment point was null");
                        break;
                    }

                    var trailOwner = playerList
                        .FirstOrDefault(p => p.GetId() == impactPoint.GetTrailSegment()
                                                 .GetOwnerPid());

                    if (trailOwner == null)
                    {
                        //Console.warn("Trail exists without a player - this is a bug");
                        return;
                    }

                    trailOwner.BreakTrailSegment(impactPoint, 10);
                    //debug.addImpact(impactPoint);
                    break;
                case PowerUpType.Slow:
                    foreach (var p in playerList)
                    {
                        if (p.GetId() != player.GetId())
                        {
                            var b = p.GetBike();
                            var oldSpd = b.GetSpd();
                            b.SetSpd(0.5f);
                            p.UpdateBike(b);
                            p.SetEffect(PlayerEffect.Slowed);

                            Task.Factory.StartNew(() =>
                            {
                                Thread.Sleep(NumberUtil.RandInt(DurationMin, DurationMax));
                                b.SetSpd(oldSpd);
                                p.UpdateBike(b);
                                p.SetEffect(PlayerEffect.None);
                            });
                        }
                    }
                    break;
                default:
                    return;
            }

            player.SetCurrentPowerUpType(PowerUpType.Nothing);
            _playerPowerUps.Remove(player.GetId());
        }

        public List<PowerUpDto> GetDtoList()
        {
            var powerUpsDto = new List<PowerUpDto>();
            foreach (var p in _availablePowerUps)
            {
                var powerUpDto = p.GetDto();
                powerUpsDto.Add(powerUpDto);
            }
            return powerUpsDto;
        }

        public List<PowerUp> GetList()
        {
            return _availablePowerUps.ToList();
        }
    }
}
