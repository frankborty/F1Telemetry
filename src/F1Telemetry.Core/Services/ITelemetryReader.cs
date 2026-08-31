using F1Telemetry.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace F1Telemetry.Core.Services
{
    public interface ITelemetryReader
    {
        IAsyncEnumerable<TelemetryData> GetTelemetryAsync(CancellationToken cancellationToken = default);
    }
}
