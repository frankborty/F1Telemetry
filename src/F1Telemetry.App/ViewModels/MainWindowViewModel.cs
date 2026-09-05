namespace F1Telemetry.App.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public TelemetryDashboardViewModel TelemetryDashboardVM { get; }

        public MainWindowViewModel(TelemetryDashboardViewModel telemetryDashboardVM)
        {
            TelemetryDashboardVM = telemetryDashboardVM;
        }
    }
}
