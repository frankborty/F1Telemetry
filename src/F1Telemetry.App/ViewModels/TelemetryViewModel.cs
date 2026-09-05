using F1Telemetry.Core.Models;
using F1Telemetry.Infrastructure.Data;
using System.Windows.Threading;

namespace F1Telemetry.App.ViewModels
{
    public class TelemetryViewModel : ViewModelBase
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

                // Evita di mostrare ancora i dati del pilota precedente.
                _latestTelemetry = null;
                Telemetry = null;
            }
        }
        private DriverInfo? _selectedDriver;
        public DriverInfo? SelectedDriver
        {
            get => _selectedDriver;
            set
            {
                if (!SetProperty(ref _selectedDriver, value) || value is null)
                {
                    return;
                }

                RaceNumber = value.RaceNumber;
            }
        }

        public IReadOnlyList<DriverInfo> Drivers { get; } = DriverData.Drivers;

        private readonly DispatcherTimer _uiTimer;
        private TelemetryData? _latestTelemetry;
        private TelemetryData? _telemetry;

        public TelemetryData? Telemetry
        {
            get => _telemetry;
            private set => SetProperty(ref _telemetry, value);
        }

        public TelemetryViewModel()
        {
            RaceNumber = Drivers[0].RaceNumber;
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
    }
}
