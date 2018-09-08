using System;
using System.Collections.Concurrent;
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
    internal class PowerUpKeeper
    {
        private const int SpawnDelayMin = 4000;
        private const int SpawnDelayMax = 7000;
        private const int DurationMin = 15000;
        private const int DurationMax = 20000;

        private readonly Dictionary<Guid, PowerUp> _playerPowerUps;
        private readonly ConcurrentDictionary<Guid, PowerUp> _availablePowerUps;
        private readonly int _gameSize;

        private bool _isSpawning;
        private Thread _spawnThread;

        public PowerUpKeeper(int gameSize)
        {
            _gameSize = gameSize;
            _availablePowerUps = new ConcurrentDictionary<Guid, PowerUp>();
            _playerPowerUps = new Dictionary<Guid, PowerUp>();
        }

        public void StartSpawner()
        {
            if (_isSpawning) return;

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

        public void StopSpawner()
        {
            if (_isSpawning)
                _isSpawning = false;
        }

        public void SpawnPowerUp()
        {
            PowerUpType type;
            if (NumberUtil.RandInt(0, 2) == 0)
                type = PowerUpType.Rocket;
            else
                type = PowerUpType.Slow;

            var powerUpPos = new Vector2(NumberUtil.RandInt(0, _gameSize), NumberUtil.RandInt(0, _gameSize));
            var powerUp = new PowerUp(powerUpPos, type);
            _availablePowerUps.TryAdd(powerUp.GetId(), powerUp);

            var duration = NumberUtil.RandInt(DurationMin, DurationMax);
            Console.WriteLine($"Spawned {type} powerup at {powerUpPos.X}, {powerUpPos.Y}, despawning in {duration/1000}s");
            
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(duration);
                if (!powerUp.IsCollected())
                    _availablePowerUps.TryRemove(powerUp.GetId(), out _);
            });
        }

        public void PlayerCollectedPowerUp(Player player, PowerUp powerUp)
        {
            var availablePowerUp = _availablePowerUps.Values.FirstOrDefault(p => p.GetId() == powerUp.GetId());
            if (availablePowerUp == null || !availablePowerUp.Equals(powerUp))
                throw new Exception("PowerUp does not exist");

            _playerPowerUps.Remove(player.GetId());
            _playerPowerUps.Add(player.GetId(), availablePowerUp);

            _availablePowerUps.TryRemove(availablePowerUp.GetId(), out _);
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
                    switch (dir.X)
                    {
                        // moving on y axis
                        case 0 when dir.Y > 0:
                            wallAhead = new Point(pos.X, gameSize);
                            break;
                        case 0:
                            if (dir.Y < 0)
                                wallAhead = new Point(pos.X, 0);
                            break;
                        default:
                        {
                            switch (dir.Y)
                            {
                                // moving on x axis
                                case 0 when dir.X > 0:
                                    wallAhead = new Point(gameSize, pos.Y);
                                    break;
                                case 0:
                                    if (dir.X < 0)
                                        wallAhead = new Point(0, pos.Y);
                                    break;
                            }
                            break;
                        }
                    }


                    var ray = new LineSegment2D(pos.ToVector2(), wallAhead.ToVector2());
                    var impactPoint = Physics.FindClosestImpactPoint(pos, ray, trails);
                    if (impactPoint?.GetTrailSegment() == null)
                    {
                        break;
                    }

                    var trailOwner = playerList
                        .FirstOrDefault(p => p.GetId() == impactPoint.GetTrailSegment()
                                                 .GetOwnerPid());

                    if (trailOwner == null)
                    {
                        Console.WriteLine("Trail exists without a player - this is a bug");
                        return;
                    }

                    trailOwner.BreakTrailSegment(impactPoint, 10);
                    break;
                case PowerUpType.Slow:
                    foreach (var p in playerList)
                    {
                        if (p.GetId() == player.GetId()) continue;

                        var b = p.GetBike();
                        var oldSpd = b.GetSpd();
                        b.SetSpd(0.5f);
                        p.UpdateBike(b);
                        p.SetEffect(PlayerEffect.Slowed);

                        Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(3000);
                            b.SetSpd(oldSpd);
                            p.UpdateBike(b);
                            p.SetEffect(PlayerEffect.None);
                        });
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
            foreach (var p in _availablePowerUps.Values)
            {
                var powerUpDto = p.GetDto();
                powerUpsDto.Add(powerUpDto);
            }
            return powerUpsDto;
        }

        public List<PowerUp> GetList()
        {
            return _availablePowerUps.Values.ToList();
        }
    }
}
