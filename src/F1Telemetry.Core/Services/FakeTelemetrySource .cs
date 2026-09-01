using F1Telemetry.Core.Interfaces;
using F1Telemetry.Core.Models;

namespace F1Telemetry.Core.Services
{
    public class FakeTelemetrySource : ITelemetrySource
    {
        public async IAsyncEnumerable<TelemetryData> GetTelemetryAsync(
            [System.Runtime.CompilerServices.EnumeratorCancellation]
            CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100, cancellationToken);
                yield return new TelemetryData
                {
                    Timestamp = DateTimeOffset.UtcNow,

                    // Car Telemetry
                    Speed = Random.Shared.NextDouble() * 350,
                    Throttle = Random.Shared.NextDouble(),
                    Brake = Random.Shared.NextDouble(),
                    SteeringAngle = Random.Shared.NextDouble() * 60 - 30,
                    Gear = Random.Shared.Next(1, 9),
                    Rpm = Random.Shared.Next(8000, 15000),
                    Clutch = Random.Shared.Next(0, 101),

                    DrsActive = Random.Shared.Next(0, 2) == 1,
                    RevLightsPercent = Random.Shared.Next(0, 101),
                    RevLightsBitValue = Random.Shared.Next(),

                    // Brakes
                    FrontLeftBrakeTemperature = Random.Shared.Next(300, 1100),
                    FrontRightBrakeTemperature = Random.Shared.Next(300, 1100),
                    RearLeftBrakeTemperature = Random.Shared.Next(300, 900),
                    RearRightBrakeTemperature = Random.Shared.Next(300, 900),

                    // Tyres - surface temperature
                    FrontLeftTyreSurfaceTemperature = Random.Shared.Next(70, 130),
                    FrontRightTyreSurfaceTemperature = Random.Shared.Next(70, 130),
                    RearLeftTyreSurfaceTemperature = Random.Shared.Next(70, 130),
                    RearRightTyreSurfaceTemperature = Random.Shared.Next(70, 130),

                    // Tyres - inner temperature
                    FrontLeftTyreInnerTemperature = Random.Shared.Next(70, 110),
                    FrontRightTyreInnerTemperature = Random.Shared.Next(70, 110),
                    RearLeftTyreInnerTemperature = Random.Shared.Next(70, 110),
                    RearRightTyreInnerTemperature = Random.Shared.Next(70, 110),

                    // Tyres - pressure
                    FrontLeftTyrePressure = Random.Shared.NextDouble() * 2 + 20,
                    FrontRightTyrePressure = Random.Shared.NextDouble() * 2 + 20,
                    RearLeftTyrePressure = Random.Shared.NextDouble() * 2 + 19,
                    RearRightTyrePressure = Random.Shared.NextDouble() * 2 + 19,

                    // Tyres - surface type
                    FrontLeftSurfaceType = Random.Shared.Next(0, 6),
                    FrontRightSurfaceType = Random.Shared.Next(0, 6),
                    RearLeftSurfaceType = Random.Shared.Next(0, 6),
                    RearRightSurfaceType = Random.Shared.Next(0, 6),

                    // Engine
                    EngineTemperature = Random.Shared.Next(90, 120),

                    // ERS / Battery
                    BatteryLevel = Random.Shared.NextDouble() * 100,
                    ErsDeployment = Random.Shared.NextDouble() * 100,
                    ErsRecovery = Random.Shared.NextDouble() * 100,

                    // Fuel
                    FuelRemaining = Random.Shared.NextDouble() * 100,
                    FuelConsumption = Random.Shared.NextDouble() * 5,

                    // Lap
                    Lap = Random.Shared.Next(1, 60),
                    Sector = Random.Shared.Next(1, 4),
                    LapTime = TimeSpan.FromSeconds(Random.Shared.NextDouble() * 60 + 60)
                };
            }
        }
    }
}
