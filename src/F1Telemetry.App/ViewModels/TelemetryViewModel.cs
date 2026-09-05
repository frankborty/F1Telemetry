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
            }
        }

        private readonly DispatcherTimer _uiTimer;
        private TelemetryData? _latestTelemetry;
        private TelemetryData? _telemetry;

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
            if (_latestTelemetry is null)
            {
                return;
            }

            Telemetry = _latestTelemetry;
        }

        public void UpdateTelemetry(TelemetryData telemetryData)
        {
            if (telemetryData.RaceNumber != RaceNumber)
                return;
            _latestTelemetry = telemetryData;
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
