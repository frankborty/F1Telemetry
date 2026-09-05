using F1Telemetry.App.ViewModels;
using F1Telemetry.App.Views;
using F1Telemetry.Core.Interfaces;
using F1Telemetry.Core.Services;
using F1Telemetry.Infrastructure;
using F1Telemetry.Infrastructure.Services;
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

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            string? source = context.Configuration["Telemetry:Source"];
            int udpPort = int.TryParse(
                context.Configuration["Telemetry:UdpPort"],
                out int configuredPort)
                    ? configuredPort
                    : 20777;

            services.AddSingleton<ITelemetryService, TelemetryService>();
            services.AddSingleton<ITelemetrySource>(_ =>
                string.Equals(source, "F1", StringComparison.OrdinalIgnoreCase)
                    ? new F1TelemetrySource(udpPort)
                    : new FakeTelemetrySource());


            /********************************************************/

            services.AddSingleton<MainWindowView>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<TelemetryDashboardViewModel>();

            /********************************************************/

            services.AddHostedService<TelemetryProducer>();
            services.AddSingleton<TelemetryConsumer>();
            services.AddHostedService(sp =>
                sp.GetRequiredService<TelemetryConsumer>());

            services.AddTransient<TelemetryViewModel>(_ => new TelemetryViewModel(0));
            services.AddTransient<TelemetryView>();

        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            await _host.StartAsync();
            var mainView = _host.Services.GetRequiredService<MainWindowView>();
            mainView.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();

            base.OnExit(e);
        }
    }

}
