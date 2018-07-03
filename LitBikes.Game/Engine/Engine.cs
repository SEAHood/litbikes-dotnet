using LitBikes.Model;
using LitBikes.Model.Dtos;
using Nine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Timers;

namespace LitBikes.Game
{
    public class GameEngine
    {
        public static readonly double BASE_BIKE_SPEED = 1.5;

        //private static Logger LOG = Log.getLogger(GameEngine.class);
	    private static readonly int GAME_TICK_MS = 25;
        private static readonly int PU_SPAWN_DELAY_MIN = 4;
        private static readonly int PU_SPAWN_DELAY_MAX = 7;
        private static readonly int PU_DURATION_MIN = 10;
        private static readonly int PU_DURATION_MAX = 20;

        //private GameEventListener eventListener;
        private long gameTick = 0;

        private List<Player> players;
        private List<PowerUp> powerUps;
        private Arena arena;
        private readonly ScoreKeeper score;
        private readonly PowerUpKeeper powerUpKeeper;
        private RoundKeeper roundKeeper;
        private int gameSize;
        private Debug debug;

        private Timer powerUpSpawnTimer = new Timer();

        public GameEngine(int gameSize)
        {
            arena = new Arena(gameSize);
            players = new List<Player>();
            powerUps = new List<PowerUp>();
            score = new ScoreKeeper();
            debug = new Debug();
            roundKeeper = new RoundKeeper(300, 15);
            powerUpKeeper = new PowerUpKeeper();
            this.gameSize = gameSize;
        }

        private bool _started;
        private DateTime _startedAt;
        private DateTime _lastTick;

        public void Start()
        {
            if (_started) return;

            _startedAt = DateTime.Now;

            while (true)
            {
                var tickRequired = _lastTick == null || DateTime.Now.Subtract(_lastTick).Milliseconds > GAME_TICK_MS;

                if (tickRequired)
                {
                    // Last tick from start or end? not sure yet
                    _lastTick = DateTime.Now;
                    GameTick();
                    //
                }

            }

            //Runnable gameLoop = new GameTick();
            //ScheduledExecutorService executor = Executors.newScheduledThreadPool(1);
            //executor.scheduleAtFixedRate(gameLoop, 0, GAME_TICK_MS, TimeUnit.MILLISECONDS);

            //PowerUpSpawner powerUpSpawner = new PowerUpSpawner();
            //powerUpSpawner.run();

            //LOG.info("Starting game at " + GAME_TICK_MS + "ms per game tick");
            //eventListener.gameStarted();
        }
        
        private void GameTick()
        {
            gameTick++;

            if (roundKeeper.IsRoundInProgress())
            {
                UpdatePlayerPositions(out var activePlayers, out var trails);
                CheckForEvents(activePlayers, trails);
            }
        }

        public void StartRound()
        {
            roundKeeper.StartRound();
        }

        private void CheckForEvents(List<Player> activePlayers, List<TrailSegment> trails)
        {
            foreach (Player player in activePlayers)
            {
                var bike = player.GetBike();
                var crashed = bike.Collides(trails, 1, out var collidedWith) || arena.CheckCollision(bike, 1);

                if (crashed)
                {
                    var crashedInto = activePlayers.FirstOrDefault(p => p.GetId() == collidedWith);
                    PlayerCrashed(player, crashedInto);
                }

                foreach (var powerUp in powerUps)
                {
                    var pos = bike.GetPos();
                    var dir = bike.GetDir();
                    var aheadX = pos.X + (2 * dir.X);
                    var aheadY = pos.X + (2 * dir.Y);
                    LineSegment2D line = new LineSegment2D(new Vector2(pos.X, pos.Y), new Vector2(aheadX, aheadY));
                    if (powerUp.Collides(line))
                    {
                        // power-up keeper . PlayerCollectedPowerUp(powerUp)
                        powerUpKeeper.PlayerCollectedPowerUp(player, powerUp);
                        //powerUpCollected = powerUp;
                        //p.setCurrentPowerUpType(powerUp.getType());
                        //break;
                    }
                }
            }
        }

        private ICollidable PlayerCrashed(Player player, ICollidable crashedInto)
        {
            if (crashedInto == null)
                crashedInto = new Wall();
            player.Crashed(crashedInto);

            // Update scores
            if (!(crashedInto.GetType() == typeof(Wall)) && crashedInto.GetId() != player.GetId())
            {
                score.GrantScore(crashedInto.GetId(), crashedInto.GetName(), 1);
            }
            else
            {
                // Decrement player score if they crashed into themselves or the wall
                score.GrantScore(player.GetId(), player.GetName(), -1);
            }
            eventListener.scoreUpdated(score.GetScores());
            return crashedInto;
        }

        private void UpdatePlayerPositions(out List<Player> activePlayers, out List<TrailSegment> trails)
        {
            activePlayers = players.Where(p => p.IsAlive()).ToList();
            trails = new List<TrailSegment>();
            foreach (Player player in activePlayers)
            {
                player.UpdatePosition();
                trails.AddRange(player.GetBike().GetTrailSegmentList(true));
            }
        }

        public Player PlayerJoin(int pid, String name, bool isHuman)
        {
            //LOG.info("Creating new player with pid " + pid);

            Player player = new Player(pid, isHuman);
            player.SetName(name);

            Bike bike = new Bike(pid);
            bike.Init(FindSpawn(), true);
            player.SetBike(bike);

            players.Add(player);
            score.GrantScore(pid, name, 0);
            return player;
        }

        public void DropPlayer(int pid)
        {
            var player = players.SingleOrDefault(p => p.GetId() == pid);

            if (player == null)
                return;

            players.Remove(player);
            score.RemoveScore(pid);
            //LOG.info("Dropped player " + pid);
        }

        public bool HandleClientUpdate(ClientUpdateDto data)
        {
            if (data.IsValid())
            {
                var player = players.FirstOrDefault(p => p.GetId() == data.pid);
                if (player == null)
                    return false;

                var bike = player.GetBike();
                bike.SetDir(new Vector2(data.xDir.Value, data.yDir.Value));
                player.UpdateBike(bike);
                return true;
            }
            else
                return false;
        }

        public ServerWorldDto GetWorldDto()
        {
            var worldDto = new ServerWorldDto();
            var playersDto = new List<PlayerDto>();
            foreach (var player in players)
            {
                var playerDto = player.GetDto();
                playerDto.score = score.GetScore(playerDto.pid);
                playersDto.Add(playerDto);
            }

            var powerUpsDto = new List<PowerUpDto>();
            foreach (var p in powerUps)
            {
                PowerUpDto powerUpDto = p.GetDto();
                powerUpsDto.Add(powerUpDto);
            }

            worldDto.timestamp = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
            worldDto.roundInProgress = roundKeeper.IsRoundInProgress();
            worldDto.roundTimeLeft = roundKeeper.GetTimeUntilRoundEnd().Seconds;
            worldDto.timeUntilNextRound = roundKeeper.GetTimeUntilCountdownEnd().Seconds;
            worldDto.currentWinner = score.GetCurrentWinner();
            worldDto.gameTick = gameTick;
            worldDto.arena = arena.GetDto();
            worldDto.players = playersDto;
            worldDto.powerUps = powerUpsDto;
            worldDto.debug = debug.GetDto();

            return worldDto;
        }

        //public void attachListener(GameEventListener listener)
        //{
        //    eventListener = listener;
        //}

        public void RequestRespawn(Player player)
        {
            if (player != null && player.IsCrashed())
            {
                player.Respawn(FindSpawn());
                eventListener.playerSpawned(player.getId());
            }
        }

        public void RequestUsePowerUp(Player player)
        {
            powerUpKeeper.PlayerRequestsUse(player, players, GetTrails(), gameSize);
        }

        public Spawn FindSpawn()
        {
            Spawn spawn = new Spawn(gameSize);
            int i = 0;
            while (!SpawnIsAcceptable(spawn) && i++ < 10)
            {
                spawn = new Spawn(gameSize);
            }
            return spawn;
        }

        public bool SpawnIsAcceptable(Spawn spawn)
        {
            var limit = 100; // Distance to nearest trail
            List<TrailSegment> trails = GetTrails();

            var aheadX = spawn.GetPos().X + (limit * spawn.GetDir().X);
            var aheadY = spawn.GetPos().Y + (limit * spawn.GetDir().Y);

            var line = new LineSegment2D(spawn.GetPos(), new Vector2(aheadX, aheadY));

            foreach (var segment in trails)
            {
                if (line.Intersects(segment.GetLine()))
                    return false;
            }

            return true;
        }

        private List<TrailSegment> GetTrails()
        {
            List<TrailSegment> trails = new List<TrailSegment>();
            foreach (var p in players)
            {
                trails.AddRange(p.GetBike().GetTrailSegmentList(true));
            }
            return trails;
        }

        public List<ScoreDto> GetScores()
        {
            return score.GetScores();
        }

        public int GetGameTickMs()
        {
            return GAME_TICK_MS;
        }

        public Arena GetArena()
        {
            return arena;
        }

        public List<Player> GetPlayers()
        {
            return players;
        }

        public Player GetPlayer(int pid)
        {
            return players.FirstOrDefault(p => p.GetId() == pid);
        }

        public TimeSpan GetRoundTimeLeft()
        {
            return roundKeeper.GetTimeUntilRoundEnd();
        }

        public int GetGameSize()
        {
            throw new NotImplementedException();
        }

        public List<Player> GetPlayerList()
        {
            throw new NotImplementedException();
        }
    }	
	
}
