using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace LitBikes.Game
{
    public class RoundKeeper
    {
        private readonly int _roundDuration; // Seconds
        private readonly int _countdownDuration; // Seconds

        private bool _roundInProgress;
        private bool _roundCountdownInProgress;
        
        private DateTime _roundCountdownStartedAt;
        private DateTime _roundStartedAt;

        private Timer _roundTimer;
        private Timer _countdownTimer;

        public RoundKeeper(int roundDuration, int countdownDuration)
        {
            _roundDuration = roundDuration;
            _countdownDuration = countdownDuration;
        }
                
        public void StartRound()
        {
            if (!_roundInProgress)
            {
                _countdownTimer = new Timer(_countdownDuration * 1000);
                _countdownTimer.Elapsed += new ElapsedEventHandler(CountdownTimerEndHandler);
                _countdownTimer.Start();

                _roundCountdownStartedAt = DateTime.Now;
                _roundCountdownInProgress = true;
            }
        }

        private void RoundStarted()
        {
            _roundTimer = new Timer(_roundDuration * 1000);
            _roundTimer.Elapsed += new ElapsedEventHandler(RoundTimerEndHandler);
            _roundTimer.Start();

            _roundStartedAt = DateTime.Now;
            _roundInProgress = true;
            _roundCountdownInProgress = false;
        }

        private void RoundEnded()
        {
            _roundInProgress = false;
        }

        public bool IsRoundInProgress()
        {
            return _roundInProgress;
        }

        public TimeSpan GetTimeUntilCountdownEnd()
        {
            return _roundCountdownInProgress ? DateTime.Now - _roundCountdownStartedAt : new TimeSpan(0);
        }

        public TimeSpan GetTimeUntilRoundEnd()
        {
            return _roundInProgress ? DateTime.Now - _roundStartedAt : new TimeSpan(0);
        }

        private void RoundTimerEndHandler(object sender, ElapsedEventArgs e)
        {
            RoundEnded();
        }

        private void CountdownTimerEndHandler(object sender, ElapsedEventArgs e)
        {
            RoundStarted();
        }
    }
}
