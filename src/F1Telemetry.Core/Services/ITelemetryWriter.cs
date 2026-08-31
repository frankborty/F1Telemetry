using F1Telemetry.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace F1Telemetry.Core.Services
{
    public interface ITelemetryWriter
    {
        public Task WriteAsync(TelemetryData telemetryData, CancellationToken cancellationToken = default);
    }
}
