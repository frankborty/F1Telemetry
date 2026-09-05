using F1Telemetry.App.Commands;
using F1Telemetry.Core.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace F1Telemetry.App.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        public ICommand AddDashboardCommand { get; }

        public ObservableCollection<TelemetryDashboardViewModel> TelemetryDashboardVMList { get; } = new();

        private readonly Func<TelemetryDashboardViewModel> factory;

        public MainWindowViewModel(Func<TelemetryDashboardViewModel> factory)
        {
            this.factory = factory;
            AddDashboardCommand = new RelayCommand(AddDashboard);
            AddDashboard();
        }

        private void AddDashboard()
        {
            TelemetryDashboardVMList.Add(factory.Invoke());
        }
    }
}
