using F1Telemetry.App.ViewModels;
using F1Telemetry.Core.Models;
using F1Telemetry.Core.Services;
using System.Windows;

namespace F1Telemetry.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly TelemetryConsumer _telemetryConsumer;
        public MainWindow(TelemetryConsumer telemetryConsumer, MainViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            _viewModel = viewModel;
            _telemetryConsumer = telemetryConsumer;

            telemetryConsumer.TelemetryReceived += OnTelemetryReceived;
        }

        private async void OnTelemetryReceived(TelemetryData telemetryData)
        {
            _viewModel.UpdateTelemetry(telemetryData);
        }
        protected override void OnClosed(EventArgs e)
        {
            _telemetryConsumer.TelemetryReceived -= OnTelemetryReceived;
            base.OnClosed(e);
        }
    }
}