using F1Telemetry.App.ViewModels;
using F1Telemetry.Core.Interfaces;
using F1Telemetry.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace F1Telemetry.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(ConfigureServices)
                .Build();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITelemetryService, TelemetryService>();
            services.AddSingleton<ITelemetrySource, FakeTelemetrySource>();

            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();

            services.AddHostedService<TelemetryProducer>();
            services.AddSingleton<TelemetryConsumer>();
            services.AddHostedService(sp =>
                sp.GetRequiredService<TelemetryConsumer>());
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            await _host.StartAsync();
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();

            base.OnExit(e);
        }
    }

}
