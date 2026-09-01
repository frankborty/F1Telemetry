using F1Telemetry.Core.Models;

namespace F1Telemetry.Core.Interfaces
{
    public interface ITelemetrySource
    {
        IAsyncEnumerable<TelemetryData> GetTelemetryAsync(CancellationToken cancellationToken = default);
    }
}
