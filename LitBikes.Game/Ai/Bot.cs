using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LitBikes.Model;
using LitBikes.Model.Dtos.FromClient;

namespace LitBikes.Ai
{
    public class Bot
    {
        private readonly Guid _botId;
        private bool _crashed;
        private Bike _botBike;

        //private static Logger LOG = Log.getLogger(Bot.class);
	    private static int AiTickMs = 50;
        private static int AI_RESPAWN_MS = 3000;
        //private final BotIOClient ioClient;
	    private List<Player> _players;
        private Arena _arena;
        long lastPredictionTime;
        long predictionCooldown = 100;
        private BotController _controller;
        private Random _rand = new Random();

        private Thread _tickThread;
        private DateTime _lastTick;

        public Guid GetId() => _botId;

        public Bot(Guid botId)
        {
            _botId = botId;
            _players = new List<Player>();
        }

        public void SetCrashed(bool crashed)
        {
            _crashed = crashed;
        }

        public void AttachController(BotController controller)
        {
            _controller = controller;
        }

        public void UpdateWorld(List<Player> players, Arena arena)
        {
            _arena = arena;
            _players = players;
            _botBike = players.FirstOrDefault(p => p.GetId() == _botId)?.GetBike();
        }

        public void Start()
        {
            Console.WriteLine($"Starting bot {_botId}");
            _tickThread = new Thread(delegate ()
            {
                while (true)
                {
                    var tickRequired = DateTime.Now.Subtract(_lastTick).Milliseconds > AiTickMs;
                    if (!tickRequired) continue;
                    if (_botBike == null) continue; // No data to work with

                    _lastTick = DateTime.Now;
                    if (!_crashed)
                        PredictCollision();
                    else
                    {
                        Thread.Sleep(AI_RESPAWN_MS);
                        _controller.SendRespawnRequest(_botId);
                    }
                }
            });
            _tickThread.Start();
        }

        private void PredictCollision()
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

            if (!incCollision) return;

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

            _controller.SendUpdate(_botId, updateDto);
        }

    }
}
