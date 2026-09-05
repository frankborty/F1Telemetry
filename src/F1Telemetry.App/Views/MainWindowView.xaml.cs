using F1Telemetry.App.ViewModels;
using F1Telemetry.Core.Models;
using F1Telemetry.Core.Services;
using System.Windows;

namespace F1Telemetry.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}