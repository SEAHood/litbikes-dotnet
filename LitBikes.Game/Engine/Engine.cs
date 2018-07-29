using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Timers;
using LitBikes.Game.Controller;
using LitBikes.Model;
using LitBikes.Model.Dtos;
using LitBikes.Model.Dtos.FromClient;
using Nine.Geometry;

namespace LitBikes.Game.Engine
{
    public class GameEngine
    {

        public static readonly float BaseBikeSpeed = 1.5f;
        //private static Logger LOG = Log.getLogger(GameEngine.class);
        private static readonly int GameTickMs = 25;
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
        private Thread _tickThread;

        private System.Timers.Timer powerUpSpawnTimer = new System.Timers.Timer();

        private readonly GameEventController _gameEventController;

        public GameEngine(GameEventController gameEventController, GameSettings settings)
        {
            _gameEventController = gameEventController;
            arena = new Arena(settings.ArenaSize);
            players = new List<Player>();
            powerUps = new List<PowerUp>();
            score = new ScoreKeeper();
            debug = new Debug();
            roundKeeper = new RoundKeeper(settings.RoundDuration, settings.RoundCountdownDuration, _gameEventController);
            powerUpKeeper = new PowerUpKeeper();
            gameSize = settings.ArenaSize;
        }

        private bool _started;
        private DateTime _startedAt;
        private DateTime _lastTick;

        public void Start()
        {
            if (_started) return;

            _startedAt = DateTime.Now;

            _tickThread = new Thread(delegate()
            {
                while (true)
                {
                    //if (players.Count == 0)
                        //Thread.Sleep(1000);
                    var tickRequired = DateTime.Now.Subtract(_lastTick).Milliseconds > GameTickMs;
                    if (tickRequired)
                    {
                        // Last tick from start or end? not sure yet
                        _lastTick = DateTime.Now;
                        GameTick();
                        //
                    }
                }
            });
            _tickThread.Start();

            //PowerUpSpawner powerUpSpawner = new PowerUpSpawner();
            //powerUpSpawner.run();

            //LOG.info("Starting game at " + GameTickMs + "ms per game tick");
            //eventListener.gameStarted();
        }
        
        private void GameTick()
        {
            gameTick++;
            if (!players.Any() || !roundKeeper.IsRoundInProgress()) return;
            UpdatePlayerPositions(out var activePlayers, out var trails);
            CheckForEvents(activePlayers, trails);
        }

        public void NewRound()
        {
            foreach (var p in players)
            {
                p.SetSpectating(true);
            }
            roundKeeper.StartRound();
        }

        public void RoundStarted()
        {
            foreach (var p in players)
            {
                p.Respawn(FindSpawn());
            }
            score.Reset();
        }

        private void CheckForEvents(List<Player> activePlayers, List<TrailSegment> trails)
        {
            foreach (var player in activePlayers)
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
                    var line = new LineSegment2D(new Vector2(pos.X, pos.Y), new Vector2(aheadX, aheadY));
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
            _gameEventController.PlayerCrashed(player);
            _gameEventController.ScoreUpdated(score.GetScores());
            return crashedInto;
        }

        private void UpdatePlayerPositions(out List<Player> activePlayers, out List<TrailSegment> trails)
        {
            activePlayers = players.Where(p => p.IsAlive()).ToList();
            trails = new List<TrailSegment>();
            foreach (var player in activePlayers)
            {
                player.UpdatePosition();
                trails.AddRange(player.GetBike().GetTrailSegmentList(true));
            }
        }

        public Player PlayerJoin(Guid pid, string name, bool isHuman)
        {
            //LOG.info("Creating new player with pid " + pid);

            var player = new Player(pid, isHuman);
            player.SetName(name);

            var bike = new Bike(pid);
            bike.Init(FindSpawn(), true);
            player.SetBike(bike);

            players.Add(player);
            score.GrantScore(pid, name, 0);
            return player;
        }

        public void DropPlayer(Guid pid)
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
                var player = players.FirstOrDefault(p => p.GetId() == data.PlayerId);
                if (player == null)
                    return false;

                var bike = player.GetBike();
                bike.SetDir(new Vector2(data.XDir.Value, data.YDir.Value));
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
            foreach (var player in players.ToList())
            {
                var playerDto = player.GetDto();
                playerDto.Score = score.GetScore(playerDto.PlayerId);
                playersDto.Add(playerDto);
            }

            var powerUpsDto = new List<PowerUpDto>();
            foreach (var p in powerUps)
            {
                var powerUpDto = p.GetDto();
                powerUpsDto.Add(powerUpDto);
            }

            worldDto.Timestamp = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
            worldDto.RoundInProgress = roundKeeper.IsRoundInProgress();
            worldDto.RoundTimeLeft = (int)roundKeeper.GetTimeUntilRoundEnd().TotalSeconds;
            worldDto.TimeUntilNextRound = (int)roundKeeper.GetTimeUntilCountdownEnd().TotalSeconds;
            worldDto.CurrentWinner = score.GetCurrentWinner();
            worldDto.GameTick = gameTick;
            worldDto.Arena = arena.GetDto();
            worldDto.Players = playersDto;
            worldDto.PowerUps = powerUpsDto;
            worldDto.Debug = debug.GetDto();

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
                _gameEventController.PlayerSpawned();
            }
        }

        public void RequestUsePowerUp(Player player)
        {
            powerUpKeeper.PlayerRequestsUse(player, players, GetTrails(), gameSize);
        }

        public Spawn FindSpawn()
        {
            var spawn = new Spawn(gameSize, BaseBikeSpeed);
            var i = 0;
            while (!SpawnIsAcceptable(spawn) && i++ < 10)
            {
                spawn = new Spawn(gameSize, BaseBikeSpeed);
            }
            return spawn;
        }

        public bool SpawnIsAcceptable(Spawn spawn)
        {
            var limit = 100; // Distance to nearest Trail
            var trails = GetTrails();

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
            var trails = new List<TrailSegment>();
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
            return GameTickMs;
        }

        public Arena GetArena()
        {
            return arena;
        }

        public List<Player> GetPlayers()
        {
            return players;
        }

        public Player GetPlayer(Guid pid)
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
