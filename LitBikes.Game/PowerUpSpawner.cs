using System;
using System.Collections.Generic;
using System.Text;

namespace LitBikes.Game
{
    class PowerUpSpawner extends TimerTask
    {
        @Override

        public void run()
            {
                try
                {
                    PowerUpType type = PowerUpType.SLOW;
                    int rand = new Random().nextInt(2);
                    if (rand == 1)
                        type = PowerUpType.ROCKET;
                    else
                        type = PowerUpType.SLOW;

                    PowerUp powerUp = new PowerUp(Vector.random(gameSize, gameSize), type);
                    powerUps.add(powerUp);
                    int delay = (PU_SPAWN_DELAY_MIN + new Random().nextInt(PU_SPAWN_DELAY_MAX)) * 1000;
                    powerUpSpawnTimer.schedule(new PowerUpSpawner(), delay);

                    // Schedule despawn of powerup
                    int duration = (PU_DURATION_MIN + new Random().nextInt(PU_DURATION_MAX)) * 1000;
                    powerUpSpawnTimer.schedule(new TimerTask() {
                            @Override

                            public void run()
                    {
                        if (!powerUp.isCollected())
                        {
                            powerUps.remove(powerUp);
                        }
                    }
                }, duration);
            } catch (Exception ex) {
				LOG.warn("Exception thrown in power up manager", ex);
			}
		}
	}
}
