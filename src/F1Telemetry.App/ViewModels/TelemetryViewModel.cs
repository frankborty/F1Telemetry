using F1Telemetry.Core.Models;
using System.Windows.Threading;

namespace F1Telemetry.App.ViewModels
{
    public class TelemetryViewModel : ViewModelBase, IDisposable
    {
        private int _raceNumber;
        public int RaceNumber
        {
            get => _raceNumber;
            set
            {
                if (!SetProperty(ref _raceNumber, value))
                {
                    return;
                }

                _latestTelemetry = null;
                Telemetry = null;
                _history.Clear();
                OnPropertyChanged(nameof(History));
            }
        }

        private readonly DispatcherTimer _uiTimer;
        private TelemetryData? _latestTelemetry;
        private TelemetryData? _telemetry;
        private readonly Queue<(DateTimeOffset Timestamp, double Speed, double Rpm)> _history = new();
        public IReadOnlyCollection<(DateTimeOffset Timestamp, double Speed, double Rpm)> History => _history;

        public TelemetryData? Telemetry
        {
            get => _telemetry;
            private set => SetProperty(ref _telemetry, value);
        }

        public TelemetryViewModel(int driverId)
        {
            RaceNumber = driverId;
            _uiTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };

            _uiTimer.Tick += OnUiTimerTick;
            _uiTimer.Start();
        }

        private void OnUiTimerTick(object? sender, EventArgs e)
        {
            var latestTelemetry = Volatile.Read(ref _latestTelemetry);
            if (latestTelemetry is null)
            {
                return;
            }

            Telemetry = latestTelemetry;
            AddHistory(latestTelemetry);
        }

        private void AddHistory(TelemetryData telemetry)
        {
            _history.Enqueue((telemetry.Timestamp, telemetry.Speed, telemetry.Rpm));
            var cutoff = telemetry.Timestamp - TimeSpan.FromSeconds(60);
            while (_history.Count > 600 || (_history.Count > 0 && _history.Peek().Timestamp < cutoff))
            {
                _history.Dequeue();
            }

            OnPropertyChanged(nameof(History));
        }

        public void UpdateTelemetry(TelemetryData telemetryData)
        {
            if (telemetryData.RaceNumber != RaceNumber)
                return;
            Interlocked.Exchange(ref _latestTelemetry, telemetryData);
        }

        public void Dispose()
        {
            _uiTimer?.Stop();
            if (_uiTimer != null)
            {
                _uiTimer.Tick -= OnUiTimerTick;
            }
        }
    }
}
