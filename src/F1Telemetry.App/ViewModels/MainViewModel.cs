using F1Telemetry.App.Commands;
using F1Telemetry.Core.Models;
using System.Windows.Input;
using System.Windows.Threading;

namespace F1Telemetry.App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly DispatcherTimer _uiTimer;
        private TelemetryData? _telemetry;
        private TelemetryData? _latestTelemetry;
        public ICommand ResetCommand { get; }

        public MainViewModel()
        {
            _uiTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };

            _uiTimer.Tick += OnUiTimerTick;
            _uiTimer.Start();
            ResetCommand = new RelayCommand(Reset);
        }

        public TelemetryData? Telemetry
        {
            get => _telemetry;
            private set => SetProperty(ref _telemetry, value);
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
            _latestTelemetry = telemetryData;
        }

        private void Reset()
        {
            _latestTelemetry = null;
            Telemetry = null;
        }
    }
}
