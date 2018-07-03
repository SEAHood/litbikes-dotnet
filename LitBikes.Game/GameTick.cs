//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace LitBikes.Game
//{
//    class GameTick implements Runnable
//    {
		
//		long tim = System.currentTimeMillis();


//        public void run()
//        {
//            try
//            {
//                //Increment tick count first
//                gameTick++;

//                if (roundInProgress)
//                {

//                    List<Player> activePlayers = players.stream()
//                            .filter(b->b.isAlive())
//                            .collect(Collectors.toList());
//                    List<Thread> threads = new ArrayList<>();
//                    for (Player player : activePlayers)
//                    {
//                        Thread t = new Thread(()-> {
			    			
//			    			    // Faster father from center - disabled temporarily
//			    			    /* 
//			    			    Point2D center = new Point2D.Double(gameSize/2, gameSize/2);
//			    			    Point2D bikePos = new Point2D.Double(bike.getPos().x, bike.getPos().y);
//			    			    double distance = bikePos.distance(center);
//			    			    double oldMin = 0;
//			    			    double oldMax = gameSize/2;
//			    			    double newMin = 0;
//			    			    double newMax = 0.5;
//			    			    double oldRange = oldMax - oldMin;
//			    			    double newRange = newMax - newMin;
//	    					    double spdModifier = ((distance - oldMin) * newRange / oldRange) + newMin; // Trust me
	    							
//	    					    bike.setSpd(BASE_BIKE_SPEED + spdModifier);*/
			    			
//				    		    player.update();

//                        Bike playerBike = player.getBike();
//                        boolean collided = false;
//                        ICollidable collidedWith = null;
//                        PowerUp powerUpCollected = null;


//                        for (Player p : activePlayers)
//                        {
//                            boolean isSelf = p.getId() == player.getId();
//                            collided = collided || playerBike.collides(p.getBike().getTrailSegmentList(!isSelf), 1);
//                            if (collided)
//                            {
//                                collidedWith = p;
//                                break;
//                            }

//                            for (PowerUp powerUp : powerUps)
//                            {
//                                Vector pos = p.getBike().getPos();
//                                double aheadX = pos.x + (2 * p.getBike().getDir().x);
//                                double aheadY = pos.y + (2 * p.getBike().getDir().y);
//                                Line2D line = new Line2D.Double(pos.x, pos.y, aheadX, aheadY);
//                                if (powerUp.collides(line))
//                                {
//                                    powerUpCollected = powerUp;
//                                    p.setCurrentPowerUpType(powerUp.getType());
//                                    break;
//                                }
//                            }
//                        }

//                        if (!collided && arena.checkCollision(playerBike, 1))
//                        {
//                            collided = true;
//                            collidedWith = new Wall();
//                        }

//                        if (collided)
//                        {
//                            // Player crashed
//                            player.crashed(collidedWith);
//                            eventListener.playerCrashed(player);

//                            // Update scores
//                            if (!(collidedWith instanceof Wall) && collidedWith.getId() != player.getId()) {
//                        score.grantScore(collidedWith.getId(), collidedWith.getName(), 1);
//                    } else {
//                        // Decrement player score if they crashed into themselves or the wall
//                        score.grantScore(player.getId(), player.getName(), -1);
//                    }
//                    eventListener.scoreUpdated(score.getScores());
//                }

//                if (powerUpCollected != null)
//                {
//                    powerUpCollected.setCollected(true);
//                    final PowerUp powerUp = powerUpCollected;
//                    Timer timer = new Timer();
//                    timer.schedule(new TimerTask() {
//                                        @Override

//                                        public void run()
//                    {
//                        try
//                        {
//                            powerUps.remove(powerUp);
//                        }
//                        catch (Exception e)
//                        {
//                            e.printStackTrace();
//                        }
//                    }
//                }, 4000);
//            }

//                            });
			    		
//			    		    threads.add(t);
//			    		    t.start();
//			    	    }
			    	
//			    	    for (int i = 0; i<threads.size(); i++ )
//			    		    threads.get(i).join();
		    		
//		    	    }
		    	
//			    } catch (Exception e) {
//				    e.printStackTrace();
//				    LOG.info(e.getMessage());
//			    }
//	        }
//	    }
//    }
//}
