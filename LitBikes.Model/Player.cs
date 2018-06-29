using System;
using System.Collections.Generic;
using System.Text;

namespace LitBikes.Model
{
    public class PlayerDto
    {
        public int pid;
        public String name;
        public BikeDto bike;
        public bool spectating;
        public bool crashed;
        public int? crashedInto;
        public String crashedIntoName;
        public int score;
        public PowerUpType currentPowerUp;
        public PlayerEffect effect;
    }

    public enum PlayerEffect
    {
        NONE,
        SLOWED
    }

    public class Player : ICollidable
    {
        protected int pid;
        protected Bike bike;
        private String name;
        private bool crashed = false;
        private bool spectating = false;
        private ICollidable crashedInto = null;
        private readonly bool isHuman;
        private PowerUpType currentPowerUpType = PowerUpType.NOTHING;
        private PlayerEffect effect = PlayerEffect.NONE;

        public Player(int _pid, bool _isHuman)
        {
            pid = _pid;
            isHuman = _isHuman;
        }

        public int GetId()
        {
            return pid;
        }

        public String GetName()
        {
            return name;
        }

        public Bike GetBike()
        {
            return bike;
        }

        public void SetBike(Bike bike)
        {
            this.bike = bike;
        }

        public void SetName(String name)
        {
            this.name = name;
        }

        public bool IsAlive()
        {
            return !crashed && !spectating;
        }

        public bool IsHuman()
        {
            return isHuman;
        }

        public PowerUpType GetCurrentPowerUpType()
        {
            return currentPowerUpType;
        }

        public void SetCurrentPowerUpType(PowerUpType type)
        {
            currentPowerUpType = type;
        }

        public PlayerEffect GetEffect()
        {
            return effect;
        }

        public void SetEffect(PlayerEffect effect)
        {
            this.effect = effect;
        }

        public PlayerDto GetDto()
        {
            return new PlayerDto
            {
                pid = pid,
                name = name,
                bike = bike.GetDto(),
                crashed = crashed,
                crashedInto = crashedInto?.GetId(),
                crashedIntoName = crashedInto?.GetName(),
                spectating = spectating,
                currentPowerUp = currentPowerUpType,
                effect = effect
            };
        }

        public void Update()
        {
            if (IsAlive())
            {
                bike.UpdatePosition();
            }
        }

        public void UpdateBike(Bike _bike)
        {
            // this is terrible
            bike.SetPos(_bike.GetPos());
            bike.SetDir(_bike.GetDir());
        }

        public bool CrashedIntoSelf()
        {
            if (crashedInto == null)
                return false;
            return crashedInto.GetId() == pid;
        }

        public bool IsSpectating()
        {
            return spectating;
        }

        public void SetSpectating(bool spectating)
        {
            this.spectating = spectating;
        }

        public ICollidable GetCrashedInto()
        {
            return crashedInto;
        }

        public void SetCrashedInto(ICollidable crashedInto)
        {
            this.crashedInto = crashedInto;
        }

        public bool IsCrashed()
        {
            return crashed;
        }

        public void SetCrashed(bool crashed)
        {
            this.crashed = crashed;
        }

        public void Crashed(ICollidable collidedWith)
        {
            bike.Crash();
            SetCrashed(true);
            SetCrashedInto(collidedWith);
            SetSpectating(true);
        }

        public void Respawn(Spawn spawn)
        {
            bike.Init(spawn, false);
            SetCrashed(false);
            SetCrashedInto(null);
            SetSpectating(false);
            currentPowerUpType = PowerUpType.NOTHING;
            effect = PlayerEffect.NONE;
        }

        public void BreakTrailSegment(ImpactPoint impactPoint, double radius)
        {
            bike.BreakTrailSegment(impactPoint, radius);
        }
    }
}
