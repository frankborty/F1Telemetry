using F1Telemetry.Core.Models;

namespace F1Telemetry.Core.Interfaces
{
    public interface ITelemetrySource
    {
        TelemetryData GetTelemetryData();
    }
}
