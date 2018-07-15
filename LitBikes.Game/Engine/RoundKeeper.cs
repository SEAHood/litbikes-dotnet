using System;
using System.Timers;
using LitBikes.Game.Controller;

namespace LitBikes.Game.Engine
{
    public class RoundKeeper
    {
        private readonly int _roundDuration; // Seconds
        private readonly int _countdownDuration; // Seconds

        private bool _roundInProgress;
        private bool _roundCountdownInProgress;
        
        private DateTime _roundCountdownStartedAt;
        private DateTime _roundStartedAt;

        private readonly Timer _roundTimer;
        private readonly Timer _countdownTimer;

        private readonly GameEventController _eventController;

        public RoundKeeper(int roundDuration, int countdownDuration, GameEventController eventController)
        {
            _roundDuration = roundDuration;
            _countdownDuration = countdownDuration;
            _eventController = eventController;

            _countdownTimer = new Timer(_countdownDuration * 1000);
            _countdownTimer.Elapsed += CountdownTimerEndHandler;

            _roundTimer = new Timer(_roundDuration * 1000);
            _roundTimer.Elapsed += RoundTimerEndHandler;
        }
                
        public void StartRound()
        {
            if (_roundInProgress) return;

            _countdownTimer.Start();
            _roundCountdownStartedAt = DateTime.Now;
            _roundCountdownInProgress = true;
        }

        private void RoundStarted()
        {
            _roundStartedAt = DateTime.Now;
            _roundInProgress = true;
            _roundCountdownInProgress = false;
            _eventController.RoundStarted();
        }

        private void RoundEnded()
        {
            _roundInProgress = false;
            _eventController.RoundEnded();
        }

        public bool IsRoundInProgress()
        {
            return _roundInProgress;
        }

        public TimeSpan GetTimeUntilCountdownEnd()
        {
            var countdownEndsAt = _roundCountdownStartedAt.AddSeconds(_countdownDuration);
            return _roundCountdownInProgress ? countdownEndsAt - DateTime.Now : new TimeSpan(0);
        }

        public TimeSpan GetTimeUntilRoundEnd()
        {
            var roundEndsAt = _roundStartedAt.AddSeconds(_roundDuration);
            return _roundInProgress ? roundEndsAt - DateTime.Now : new TimeSpan(0);
        }

        private void RoundTimerEndHandler(object sender, ElapsedEventArgs e)
        {
            _roundTimer.Stop();
            RoundEnded();
        }

        private void CountdownTimerEndHandler(object sender, ElapsedEventArgs e)
        {
            _countdownTimer.Stop();
            _roundTimer.Start();
            RoundStarted();
        }
    }
}
