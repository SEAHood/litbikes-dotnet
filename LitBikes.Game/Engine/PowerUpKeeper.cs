using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using LitBikes.Model;
using Nine.Geometry;

namespace LitBikes.Game.Engine
{
    class PowerUpKeeper
    {
        private bool _isSpawning;
        private readonly Dictionary<Guid, PowerUp> _playerPowerUps;
        private readonly List<PowerUp> _availablePowerUps;

        public PowerUpKeeper()
        {
            _availablePowerUps = new List<PowerUp>();
            _playerPowerUps = new Dictionary<Guid, PowerUp>();
        }

        public void StartSpawner()
        {
            if (!_isSpawning)
            {
                _isSpawning = true;
            }
        }

        public void StopSpawner()
        {
            if (_isSpawning)
            {
                _isSpawning = false;
            }
        }

        public void PlayerCollectedPowerUp(Player player, PowerUp powerUp)
        {
            var availablePowerUp = _availablePowerUps.FirstOrDefault(p => p.GetId() == powerUp.GetId());
            if (availablePowerUp == null || !availablePowerUp.Equals(powerUp))
                throw new Exception("PowerUp does not exist");

            _playerPowerUps.Add(player.GetId(), availablePowerUp);
            _availablePowerUps.Remove(availablePowerUp);
        }

        public void PlayerRequestsUse(Player player, List<Player> playerList, List<TrailSegment> trails, int gameSize)
        {
            if (_playerPowerUps.TryGetValue(player.GetId(), out var powerUp))
            {
                switch (powerUp.GetPowerUpType())
                {
                    case PowerUpType.ROCKET:
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
                            //LOG.warn("Trail exists without a player - this is a bug");
                            return;
                        }

                        trailOwner.BreakTrailSegment(impactPoint, 10);
                        //debug.addImpact(impactPoint);
                        break;
                    case PowerUpType.SLOW:
                        foreach (var p in playerList)
                        {
                            if (p.GetId() != player.GetId())
                            {
                                var b = p.GetBike();
                                var oldSpd = b.GetSpd();
                                b.SetSpd(0.5f);
                                p.UpdateBike(b);
                                p.SetEffect(PlayerEffect.Slowed);

                                var timer = new Timer(3000);
                                timer.Elapsed += (sender, e) =>
                                {
                                    b.SetSpd(oldSpd);
                                    p.UpdateBike(b);
                                    p.SetEffect(PlayerEffect.None);
                                };
                                timer.Start();
                            }
                        }
                        break;
                    default:
                        return;
                }
                player.SetCurrentPowerUpType(PowerUpType.NOTHING);
                _playerPowerUps.Remove(player.GetId());
            }
        }
    }
}
