using F1Telemetry.Core.Interfaces;
using Microsoft.Extensions.Hosting;

namespace F1Telemetry.Core.Services
{
    public class TelemetryProducer : BackgroundService
    {
        private readonly ITelemetryService _telemetryService;
        private readonly ITelemetrySource _telemetrySource;

        public TelemetryProducer(ITelemetryService telemetryService, ITelemetrySource telemetrySource)
        {
            _telemetryService = telemetryService;
            _telemetrySource = telemetrySource;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await _telemetryService.WriteAsync(
                        _telemetrySource.GetTelemetryData(),
                        cancellationToken);

                    await Task.Delay(100, cancellationToken);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
        }
    }
}
