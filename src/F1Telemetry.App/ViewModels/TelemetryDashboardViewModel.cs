using F1Telemetry.Core.Models;
using F1Telemetry.Core.Services;
using F1Telemetry.Infrastructure.Data;

namespace F1Telemetry.App.ViewModels
{
    public class TelemetryDashboardViewModel : ViewModelBase
    {
        private readonly TelemetryConsumer _telemetryConsumer;
        public TelemetryViewModel TelemetryVM { get; }

        public TelemetryDashboardViewModel(TelemetryConsumer telemetryConsumer, TelemetryViewModel telemetryViewModel)
        {
            _telemetryConsumer = telemetryConsumer;
            TelemetryVM = telemetryViewModel;
            _telemetryConsumer.TelemetryReceived += OnTelemetryReceived;
        }

        private void OnTelemetryReceived(TelemetryData telemetryData)
        {
            TelemetryVM.UpdateTelemetry(telemetryData);
        }

    }
}
