using F1Telemetry.App.Commands;
using F1Telemetry.Core.Models;
using F1Telemetry.Core.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace F1Telemetry.App.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private bool _disposed;
        private readonly Func<Action<TelemetryDashboardViewModel>, TelemetryDashboardViewModel> _factory;
        private readonly TelemetryConnectionState _connectionState;
        private readonly DispatcherTimer _connectionTimer;

        public ObservableCollection<TelemetryDashboardViewModel> TelemetryDashboardVMList { get; } = new();

        public ICommand AddDashboardCommand { get; }
        public TelemetryConnectionStatus ConnectionStatus => _connectionState.Status;
        public string ConnectionMessage => _connectionState.Message;
        public DateTimeOffset? LastPacketAt => _connectionState.LastPacketAt;

        public MainWindowViewModel(
            Func<Action<TelemetryDashboardViewModel>, TelemetryDashboardViewModel> factory,
            TelemetryConnectionState connectionState)
        {
            _factory = factory;
            _connectionState = connectionState;
            _connectionState.Changed += OnConnectionChanged;
            _connectionTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _connectionTimer.Tick += OnConnectionTimerTick;
            _connectionTimer.Start();
            AddDashboardCommand = new RelayCommand(AddDashboard);
            AddDashboard();
        }

        private void OnConnectionChanged()
        {
            if (!System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(OnConnectionChanged);
                return;
            }
            OnPropertyChanged(nameof(ConnectionStatus));
            OnPropertyChanged(nameof(ConnectionMessage));
            OnPropertyChanged(nameof(LastPacketAt));
        }

        private void OnConnectionTimerTick(object? sender, EventArgs e)
        {
            var lastPacket = _connectionState.LastPacketAt;
            if (lastPacket.HasValue && DateTimeOffset.UtcNow - lastPacket.Value > TimeSpan.FromSeconds(2))
            {
                _connectionState.Set(TelemetryConnectionStatus.Disconnected, "Nessun pacchetto ricevuto da oltre 2 secondi");
            }
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
            _connectionTimer.Stop();
            _connectionTimer.Tick -= OnConnectionTimerTick;

            foreach (var dashboard in TelemetryDashboardVMList)
            {
                dashboard.Dispose();
            }

            TelemetryDashboardVMList.Clear();
            _connectionState.Changed -= OnConnectionChanged;
        }
    }
}
