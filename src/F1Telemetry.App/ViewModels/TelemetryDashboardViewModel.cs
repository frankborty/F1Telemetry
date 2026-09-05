using F1Telemetry.App.Commands;
using F1Telemetry.Core.Models;
using F1Telemetry.Core.Services;
using F1Telemetry.Infrastructure.Data;
using System.Windows.Input;

namespace F1Telemetry.App.ViewModels
{
    public class TelemetryDashboardViewModel : ViewModelBase, IDisposable
    {
        private bool _disposed;
        private readonly TelemetryConsumer _telemetryConsumer;
        private readonly Action<TelemetryDashboardViewModel> _removeSelf;
        public TelemetryViewModel TelemetryVM { get; }

        public ICommand RemoveCommand { get; }

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

                TelemetryVM.RaceNumber = value.RaceNumber;
            }
        }

        public IReadOnlyList<DriverInfo> Drivers { get; } = DriverData.Drivers;

        public TelemetryDashboardViewModel(
            TelemetryConsumer telemetryConsumer,
            TelemetryViewModel telemetryViewModel,
            Action<TelemetryDashboardViewModel> removeSelf)
        {
            _telemetryConsumer = telemetryConsumer;
            TelemetryVM = telemetryViewModel;
            _removeSelf = removeSelf;
            RemoveCommand = new RelayCommand(RemoveItself);
            _telemetryConsumer.TelemetryReceived += OnTelemetryReceived;
            SelectedDriver = Drivers[0];
        }

        private void RemoveItself()
        {
            Dispose();
            _removeSelf(this);
        }

        private void OnTelemetryReceived(TelemetryData telemetryData)
        {
            TelemetryVM.UpdateTelemetry(telemetryData);
        }

        public void Dispose()
        {

            if (_disposed)
            {
                return;
            }

            _disposed = true;

            _telemetryConsumer.TelemetryReceived -= OnTelemetryReceived;
            TelemetryVM.Dispose();
        }
    }
}
