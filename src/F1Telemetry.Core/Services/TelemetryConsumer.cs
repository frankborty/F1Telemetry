using F1Telemetry.Core.Interfaces;
using F1Telemetry.Core.Models;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace F1Telemetry.Core.Services
{
    public class TelemetryConsumer : BackgroundService
    {
        public event Action<TelemetryData>? TelemetryReceived;
        private readonly ITelemetryService _telemetryService;

        public TelemetryConsumer(ITelemetryService telemetryService)
        {
            _telemetryService = telemetryService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                await foreach (var telemetryData in
                    _telemetryService.GetTelemetryAsync(cancellationToken))
                {
                    TelemetryReceived?.Invoke(telemetryData);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested){ }
        }
    }
}
