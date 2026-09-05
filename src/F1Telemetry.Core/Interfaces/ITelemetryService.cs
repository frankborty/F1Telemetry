using F1Telemetry.Core.Models;

namespace F1Telemetry.Core.Interfaces
{
    public interface ITelemetryService
    {
        public IAsyncEnumerable<TelemetryData> GetTelemetryAsync(CancellationToken cancellationToken = default);
        public ValueTask WriteAsync(TelemetryData telemetryData, CancellationToken cancellationToken = default);
    }
}
