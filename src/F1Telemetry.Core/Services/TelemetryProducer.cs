using F1Telemetry.Core.Interfaces;
using F1Telemetry.Core.Models;
using Microsoft.Extensions.Hosting;

namespace F1Telemetry.Core.Services
{
    public class TelemetryProducer : BackgroundService
    {
        private readonly ITelemetryService _telemetryService;
        private readonly ITelemetrySource _telemetrySource;
        private readonly TelemetryConnectionState _connectionState;

        public TelemetryProducer(
            ITelemetryService telemetryService,
            ITelemetrySource telemetrySource,
            TelemetryConnectionState connectionState)
        {
            _telemetryService = telemetryService;
            _telemetrySource = telemetrySource;
            _connectionState = connectionState;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var sourceKind = _telemetrySource is FakeTelemetrySource
                ? TelemetryConnectionStatus.Fake
                : TelemetryConnectionStatus.Waiting;
            _connectionState.Set(sourceKind, sourceKind == TelemetryConnectionStatus.Fake
                ? "Sorgente simulata attiva"
                : "In attesa di pacchetti UDP...");
            try
            {
                await foreach (var telemetry in
                    _telemetrySource.GetTelemetryAsync(cancellationToken))
                {
                    await _telemetryService.WriteAsync(telemetry, cancellationToken);
                    _connectionState.Set(
                        sourceKind == TelemetryConnectionStatus.Fake
                            ? TelemetryConnectionStatus.Fake
                            : TelemetryConnectionStatus.Connected,
                        sourceKind == TelemetryConnectionStatus.Fake
                            ? "Sorgente simulata attiva"
                            : "Telemetria UDP ricevuta",
                        telemetry.Timestamp);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
            catch (Exception exception)
            {
                _connectionState.Set(TelemetryConnectionStatus.Error, exception.Message);
            }
        }
    }
}
