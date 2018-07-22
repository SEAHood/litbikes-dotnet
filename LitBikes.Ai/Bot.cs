using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LitBikes.Model;
using LitBikes.Model.Dtos.FromClient;

namespace LitBikes.Ai
{
    public class Bot : Player
    {
        //private static Logger LOG = Log.getLogger(Bot.class);
	    private static int AiTickMs = 50;
        private static int AI_RESPAWN_MS = 3000;
        //private final BotIOClient ioClient;
	    private List<Player> players;
        private Arena arena;
        long lastPredictionTime;
        long predictionCooldown = 100;
        private BotController controller;
        private Random _rand = new Random();

        private Thread _tickThread;
        private DateTime _lastTick;


        public void AttachController(BotController _controller)
        {
            controller = _controller;
        }

        public void UpdateWorld(List<Player> _players, Arena _arena)
        {
            arena = _arena;
            players = _players;
        }

        public void Start()
        {
            _tickThread = new Thread(delegate ()
            {
                while (true)
                {
                    var tickRequired = DateTime.Now.Subtract(_lastTick).Milliseconds > AiTickMs;
                    if (!tickRequired) continue;

                    _lastTick = DateTime.Now;
                    if (!IsCrashed())
                        PredictCollision();
                    else
                    {
                        Thread.Sleep(AI_RESPAWN_MS);
                        controller.SendRespawnRequest(GetId());
                    }
                }
            });
            _tickThread.Start();
        }


        private void PredictCollision()
        {
            var dDist = 20;
            var activePlayers = players.Where(p => p.IsAlive()).ToList();

            var allTrails = new List<TrailSegment>();
            foreach (var p in activePlayers)
            {
                var isSelf = p.GetId() == GetId();
                allTrails.AddRange(p.GetBike().GetTrailSegmentList(!isSelf));
            }

            var incCollision = GetBike().Collides(allTrails, dDist, out _) || arena.CheckCollision(GetBike(), dDist);

            if (!incCollision) return;

            var newVal = _rand.Next(0, 2) == 0 ? -1 : 1;
            var updateDto = new ClientUpdateDto {PlayerId = GetId()};

            if (GetBike().GetDir().X != 0)
            {
                updateDto.XDir = 0;
                updateDto.YDir = newVal;
            }
            else if (GetBike().GetDir().Y != 0)
            {
                updateDto.XDir = newVal;
                updateDto.YDir = 0;
            }

            controller.SendUpdate(GetId(), updateDto);
        }
    }
}
