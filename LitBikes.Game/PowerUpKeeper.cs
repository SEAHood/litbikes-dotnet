using LitBikes.Model;
using Nine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace LitBikes.Game
{
    class PowerUpKeeper
    {
        private bool _isSpawning;
        private Dictionary<int, PowerUp> _playerPowerUps;
        private List<PowerUp> _availablePowerUps;
        private IEngine _engine;

        public PowerUpKeeper(IEngine engine)
        {
            _engine = engine;
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

        public void PlayerRequestsUse(Player player, List<TrailSegment> trails, int gameSize)
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

                        var trailOwner = _engine
                            .GetPlayerList()
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
                        var players = _engine.GetPlayerList();
                        foreach (var p in players)
                        {
                            if (p.GetId() != player.GetId())
                            {
                                var b = p.GetBike();
                                var oldSpd = b.GetSpd();
                                b.SetSpd(0.5f);
                                p.UpdateBike(b);
                                p.SetEffect(PlayerEffect.SLOWED);

                                var timer = new Timer(3000);
                                timer.Elapsed += (sender, e) =>
                                {
                                    b.SetSpd(oldSpd);
                                    p.UpdateBike(b);
                                    p.SetEffect(PlayerEffect.NONE);
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
