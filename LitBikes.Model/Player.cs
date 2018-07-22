using System;
using System.Collections.Generic;
using System.Text;
using LitBikes.Model.Dtos;

namespace LitBikes.Model
{
    public enum PlayerEffect
    {
        None,
        Slowed
    }

    public class Player : ICollidable
    {
        private Guid _pid;
        private Bike _bike;
        private string name;
        private bool _crashed = false;
        private bool _spectating = false;
        private ICollidable _crashedInto = null;
        private readonly bool _isHuman;
        private PowerUpType _currentPowerUpType = PowerUpType.Nothing;
        private PlayerEffect _effect = PlayerEffect.None;

        public Player() { }

        public Player(Guid pid, bool isHuman)
        {
            _pid = pid;
            _isHuman = isHuman;
        }

        public Guid GetId()
        {
            return _pid;
        }

        public string GetName()
        {
            return name;
        }

        public Bike GetBike()
        {
            return _bike;
        }

        public void SetBike(Bike bike)
        {
            _bike = bike;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public bool IsAlive()
        {
            return !_crashed && !_spectating;
        }

        public bool IsHuman()
        {
            return _isHuman;
        }

        public PowerUpType GetCurrentPowerUpType()
        {
            return _currentPowerUpType;
        }

        public void SetCurrentPowerUpType(PowerUpType type)
        {
            _currentPowerUpType = type;
        }

        public PlayerEffect GetEffect()
        {
            return _effect;
        }

        public void SetEffect(PlayerEffect effect)
        {
            _effect = effect;
        }

        public PlayerDto GetDto()
        {
            return new PlayerDto
            {
                PlayerId = _pid,
                Name = name,
                Bike = _bike.GetDto(),
                Crashed = _crashed,
                CrashedInto = _crashedInto?.GetId(),
                CrashedIntoName = _crashedInto?.GetName(),
                Spectating = _spectating,
                CurrentPowerUp = _currentPowerUpType,
                Effect = _effect
            };
        }

        public void UpdatePosition()
        {
            if (IsAlive())
            {
                _bike.UpdatePosition();
            }
        }

        public void UpdateBike(Bike _bike)
        {
            // this is terrible
            _bike.SetPos(_bike.GetPos());
            _bike.SetDir(_bike.GetDir());
        }

        public bool IsCrashedIntoSelf()
        {
            if (_crashedInto == null)
                return false;
            return _crashedInto.GetId() == _pid;
        }

        public bool IsSpectating()
        {
            return _spectating;
        }

        public void SetSpectating(bool spectating)
        {
            _spectating = spectating;
        }

        public ICollidable GetCrashedInto()
        {
            return _crashedInto;
        }

        public void SetCrashedInto(ICollidable crashedInto)
        {
            _crashedInto = crashedInto;
        }

        public bool IsCrashed()
        {
            return _crashed;
        }

        public void SetCrashed(bool crashed)
        {
            _crashed = crashed;
        }

        public void Crashed(ICollidable collidedWith)
        {
            _bike.Crash();
            SetCrashed(true);
            SetCrashedInto(collidedWith);
            SetSpectating(true);
        }

        public void Respawn(Spawn spawn)
        {
            _bike.Init(spawn, false);
            SetCrashed(false);
            SetCrashedInto(null);
            SetSpectating(false);
            _currentPowerUpType = PowerUpType.Nothing;
            _effect = PlayerEffect.None;
        }

        public void BreakTrailSegment(ImpactPoint impactPoint, double radius)
        {
            _bike.BreakTrailSegment(impactPoint, radius);
        }
    }
}
