using F1Telemetry.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace F1Telemetry.Core.Services
{
    internal class TelemetryProducer
    {
        private readonly ITelemetryWriter _telemetryWriter;

        public TelemetryProducer(ITelemetryWriter telemetryWriter)
        {
            _telemetryWriter = telemetryWriter;
        }

        public async Task ProduceAsync(CancellationToken cancellationToken = default)
        {
            await _telemetryWriter.WriteAsync(
                CreateTelemetryData(), 
                cancellationToken);
        }

        private TelemetryData CreateTelemetryData()
        {
            return new TelemetryData
            {
                Timestamp = DateTimeOffset.UtcNow,
                Speed = Random.Shared.NextDouble() * 350,
                Rpm = Random.Shared.Next(8000, 15000),
                Gear = Random.Shared.Next(1, 9),
                Throttle = Random.Shared.NextDouble(),
                Brake = Random.Shared.NextDouble(),
                DrsActive = Random.Shared.Next(0, 2) == 1,
                SteeringAngle = Random.Shared.NextDouble() * 60 - 30,
                BatteryLevel = Random.Shared.NextDouble() * 100,
                ErsDeployment = Random.Shared.NextDouble() * 100,
                ErsRecovery = Random.Shared.NextDouble() * 100,
                EngineTemperature = Random.Shared.NextDouble() * 40 + 90,
                FuelRemaining = Random.Shared.NextDouble() * 100,
                FuelConsumption = Random.Shared.NextDouble() * 5,
                Lap = Random.Shared.Next(1, 60),
                Sector = Random.Shared.Next(1, 4),
                LapTime = TimeSpan.FromSeconds(Random.Shared.NextDouble() * 60 + 60)
            };
        }
    }
}
