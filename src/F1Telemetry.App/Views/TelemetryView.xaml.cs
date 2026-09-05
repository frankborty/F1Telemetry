using F1Telemetry.App.ViewModels;
using F1Telemetry.Core.Models;
using F1Telemetry.Core.Services;
using System.Windows;
using System.Windows.Controls;

namespace F1Telemetry.App.Views
{
    /// <summary>
    /// Interaction logic for TelemetryView.xaml
    /// </summary>
    public partial class TelemetryView : UserControl
    {
        private readonly TelemetryConsumer _telemetryConsumer;
        private readonly TelemetryViewModel _viewModel;

        public TelemetryView(
            TelemetryConsumer telemetryConsumer,
            TelemetryViewModel viewModel)
        {
            InitializeComponent();

            _telemetryConsumer = telemetryConsumer;
            _viewModel = viewModel;

            DataContext = viewModel;

            _telemetryConsumer.TelemetryReceived += OnTelemetryReceived;
        }

        private void OnTelemetryReceived(TelemetryData telemetryData)
        {
            _viewModel.UpdateTelemetry(telemetryData);
        }
    }
}
