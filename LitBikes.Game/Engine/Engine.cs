using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using LitBikes.Game.Controller;
using LitBikes.Model;
using LitBikes.Model.Dtos;
using LitBikes.Model.Dtos.FromClient;
using Nine.Geometry;

namespace LitBikes.Game.Engine
{
    public class GameEngine
    {
        private const float BaseBikeSpeed = 1.5f;
        private const int GameTickMs = 25;

        private readonly ConcurrentDictionary<Guid, Player> _players;
        private readonly Arena _arena;
        private readonly RoundKeeper _roundKeeper;
        private readonly int _gameSize;
        private readonly Debug _debug;
        private readonly ScoreKeeper _score;
        private readonly PowerUpKeeper _powerUpKeeper;
        private readonly GameEventController _gameEventController;

        private long _gameTick;
        private Thread _tickThread;
        private DateTime _nextTickTime;

        public GameEngine(GameEventController gameEventController, GameSettings settings)
        {
            _gameEventController = gameEventController;
            _arena = new Arena(settings.ArenaSize);
            _players = new ConcurrentDictionary<Guid, Player>();
            _score = new ScoreKeeper();
            _debug = new Debug();
            _roundKeeper = new RoundKeeper(settings.RoundDuration, settings.RoundCountdownDuration, _gameEventController);
            _powerUpKeeper = new PowerUpKeeper(settings.ArenaSize);
            _gameSize = settings.ArenaSize;
        }
        
        public void Start()
        {
            _nextTickTime = DateTime.UtcNow;
            _tickThread = new Thread(delegate ()
            {
                while (true)
                {
                    if (DateTime.UtcNow <= _nextTickTime) continue;
                    GameTick();
                    _nextTickTime = DateTime.UtcNow.AddMilliseconds(GameTickMs);
                }
            });
            _tickThread.Start();

            Console.WriteLine("Starting game at " + GameTickMs + "ms per game tick");
        }

        private void GameTick()
        {
            _gameTick++;
            if (!_players.Any() || !_roundKeeper.IsRoundInProgress()) return;
            UpdatePlayerPositions(out var activePlayers, out var trails);
            CheckForEvents(activePlayers, trails);
        }

        public void NewRound()
        {
            foreach (var p in _players.Values)
            {
                p.SetSpectating(true);
            }
            _roundKeeper.StartRound();
            _powerUpKeeper.StartSpawner();
        }

        public void RoundStarted()
        {
            foreach (var p in _players.Values)
            {
                p.Respawn(FindSpawn());
            }
            _score.Reset();
        }

        public void RoundEnded()
        {
            _powerUpKeeper.StopSpawner();
        }

        private void CheckForEvents(List<Player> activePlayers, List<TrailSegment> trails)
        {
            foreach (var player in activePlayers)
            {
                var bike = player.GetBike();
                var crashed = bike.Collides(trails, 1, out var collidedWith) || _arena.CheckCollision(bike, 1);

                if (crashed)
                {
                    var crashedInto = activePlayers.FirstOrDefault(p => p.GetId() == collidedWith);
                    PlayerCrashed(player, crashedInto);
                }

                foreach (var powerUp in _powerUpKeeper.GetList().Where(p => !p.IsCollected()))
                {
                    var pos = bike.GetPos();
                    var dir = bike.GetDir();
                    var aheadX = pos.X + (2 * dir.X);
                    var aheadY = pos.X + (2 * dir.Y);
                    var line = new LineSegment2D(new Vector2(pos.X, pos.Y), new Vector2(aheadX, aheadY));
                    if (powerUp.Collides(line))
                        _powerUpKeeper.PlayerCollectedPowerUp(player, powerUp);
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
                _score.GrantScore(crashedInto.GetId(), crashedInto.GetName(), 1);
            }
            else
            {
                // Decrement player score if they crashed into themselves or the wall
                _score.GrantScore(player.GetId(), player.GetName(), -1);
            }
            _gameEventController.PlayerCrashed(player);
            _gameEventController.ScoreUpdated(_score.GetScores());
            return crashedInto;
        }

        private void UpdatePlayerPositions(out List<Player> activePlayers, out List<TrailSegment> trails)
        {
            activePlayers = _players.Values.Where(p => p.IsAlive()).ToList();
            trails = new List<TrailSegment>();
            foreach (var player in activePlayers)
            {
                player.UpdatePosition();
                trails.AddRange(player.GetBike().GetTrailSegmentList(true));
            }
        }

        public Player PlayerJoin(Guid pid, string name, bool isHuman)
        {
            Console.WriteLine("Creating new player with pid " + pid);

            var player = new Player(pid, isHuman);
            player.SetName(name);

            var bike = new Bike(pid);
            bike.Init(FindSpawn(), true);
            player.SetBike(bike);

            _players.TryAdd(pid, player);
            _score.GrantScore(pid, name, 0);
            return player;
        }

        public void DropPlayer(Guid pid)
        {
            _players.TryRemove(pid, out _);
            _score.RemoveScore(pid);
            Console.WriteLine("Dropped player " + pid);
        }

        public bool HandleClientUpdate(ClientUpdateDto data)
        {
            if (!data.IsValid()) return false;

            var player = _players.Values.FirstOrDefault(p => p.GetId() == data.PlayerId);
            if (player == null)
                return false;

            var bike = player.GetBike();
            bike.SetDir(new Vector2(data.XDir.Value, data.YDir.Value));
            player.UpdateBike(bike);
            return true;
        }

        public ServerWorldDto GetWorldDto()
        {
            var startTime = DateTime.Now;
            var worldDto = new ServerWorldDto();
            var playersDto = new List<PlayerDto>();
            foreach (var player in _players.Values)
            {
                var playerDto = player.GetDto();
                playerDto.Score = _score.GetScore(playerDto.PlayerId);
                playersDto.Add(playerDto);
            }

            var powerUpsDto = new List<PowerUpDto>();
            foreach (var p in _powerUpKeeper.GetList())
            {
                var powerUpDto = p.GetDto();
                powerUpsDto.Add(powerUpDto);
            }

            worldDto.Timestamp = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
            worldDto.RoundInProgress = _roundKeeper.IsRoundInProgress();
            worldDto.RoundTimeLeft = (int)_roundKeeper.GetTimeUntilRoundEnd().TotalSeconds;
            worldDto.TimeUntilNextRound = (int)_roundKeeper.GetTimeUntilCountdownEnd().TotalSeconds;
            worldDto.CurrentWinner = _score.GetCurrentWinner();
            worldDto.GameTick = _gameTick;
            worldDto.Arena = _arena.GetDto();
            worldDto.Players = playersDto;
            worldDto.PowerUps = powerUpsDto;
            worldDto.Debug = _debug.GetDto();
            var endTime = DateTime.Now;
            var diff = endTime - startTime;
            if (diff.TotalMilliseconds > 50)
                Console.WriteLine($"Generated world dto in {diff.TotalMilliseconds}ms");

            return worldDto;
        }

        public void RequestRespawn(Player player)
        {
            if (player == null || !player.IsCrashed()) return;

            player.Respawn(FindSpawn());
            _gameEventController.PlayerSpawned();
        }

        public void RequestUsePowerUp(Player player)
        {
            _powerUpKeeper.PlayerRequestsUse(player, _players.Values.ToList(), GetTrails(), _gameSize);
        }

        public Spawn FindSpawn()
        {
            var spawn = new Spawn(_gameSize, BaseBikeSpeed);
            var i = 0;
            while (!SpawnIsAcceptable(spawn) && i++ < 10)
            {
                spawn = new Spawn(_gameSize, BaseBikeSpeed);
            }
            return spawn;
        }

        public bool SpawnIsAcceptable(Spawn spawn)
        {
            const int limit = 100; // Distance to nearest Trail
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
            foreach (var p in _players.Values)
            {
                trails.AddRange(p.GetBike().GetTrailSegmentList(true));
            }
            return trails;
        }

        public List<ScoreDto> GetScores()
        {
            return _score.GetScores();
        }

        public int GetGameTickMs()
        {
            return GameTickMs;
        }

        public Arena GetArena()
        {
            return _arena;
        }

        public List<Player> GetPlayers()
        {
            return _players.Values.ToList();
        }

        public Player GetPlayer(Guid pid)
        {
            return _players.Values.FirstOrDefault(p => p.GetId() == pid);
        }
    }	
}