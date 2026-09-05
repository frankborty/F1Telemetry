using F1Telemetry.Core.Models;
using System.Windows.Threading;

namespace F1Telemetry.App.ViewModels
{
    public class TelemetryViewModel : ViewModelBase
    {
        private readonly int _driverId;
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
            _driverId = driverId;
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
            if (telemetryData.DriverId != _driverId)
                return;
            _latestTelemetry = telemetryData;
        }
    }
}
