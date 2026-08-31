using F1Telemetry.Core.Interfaces;
using F1Telemetry.Core.Models;

namespace F1Telemetry.Core.Services
{
    public class FakeTelemetrySource : ITelemetrySource
    {
        public TelemetryData GetTelemetryData()
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
                LapTime = TimeSpan.FromSeconds(
                    Random.Shared.NextDouble() * 60 + 60)
            };
        }
    }
}
