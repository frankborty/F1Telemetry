using F1Telemetry.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace F1Telemetry.Core.Services
{
    internal class TelemetryConsumer
    {
        private readonly ITelemetryReader _telemetryReader;

        public TelemetryProducer(ITelemetryReader telemetryReader)
        {
            _telemetryReader = telemetryReader;
        }

        public async Task ConsumeAsync(CancellationToken cancellationToken = default)
        {
            await foreach (var telemetryData in _telemetryReader.GetTelemetryAsync(cancellationToken))
            {
                Console.WriteLine($"Received telemetry data: {telemetryData}");
            }
        }
    }
}
