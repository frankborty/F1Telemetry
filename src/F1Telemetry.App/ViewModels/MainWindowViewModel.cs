using F1Telemetry.App.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace F1Telemetry.App.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private bool _disposed;
        private readonly Func<Action<TelemetryDashboardViewModel>, TelemetryDashboardViewModel> _factory;

        public ObservableCollection<TelemetryDashboardViewModel> TelemetryDashboardVMList { get; } = new();

        public ICommand AddDashboardCommand { get; }

        public MainWindowViewModel(Func<Action<TelemetryDashboardViewModel>, TelemetryDashboardViewModel> factory)
        {
            _factory = factory;
            AddDashboardCommand = new RelayCommand(AddDashboard);
            AddDashboard();
        }

        private void AddDashboard()
        {
            var vm = _factory(vm => TelemetryDashboardVMList.Remove(vm));
            TelemetryDashboardVMList.Add(vm);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            foreach (var dashboard in TelemetryDashboardVMList)
            {
                dashboard.Dispose();
            }

            TelemetryDashboardVMList.Clear();
        }
    }
}
