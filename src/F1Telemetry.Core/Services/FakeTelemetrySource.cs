using F1Telemetry.Core.Interfaces;
using F1Telemetry.Core.Models;

namespace F1Telemetry.Core.Services
{
    public class FakeTelemetrySource : ITelemetrySource
    {
        private static readonly int[] RaceNumbers =
            [22, 23, 18, 10, 87, 33, 44, 63, 81, 6, 12, 7, 5, 14, 55, 31, 30, 27, 16];

        public async IAsyncEnumerable<TelemetryData> GetTelemetryAsync(
            [System.Runtime.CompilerServices.EnumeratorCancellation]
            CancellationToken cancellationToken = default)
        {
            var sessionId = (ulong)Random.Shared.NextInt64(1, long.MaxValue);
            var startedAt = DateTimeOffset.UtcNow;
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100, cancellationToken);
                var elapsed = (DateTimeOffset.UtcNow - startedAt).TotalSeconds;
                for (int driverId = 1; driverId <= 19; driverId++)
                {
                    var phase = elapsed * 0.8 + driverId * 0.35;
                    var throttle = Math.Clamp((Math.Sin(phase) + 1) / 2, 0, 1);
                    var brake = Math.Clamp((Math.Sin(phase + Math.PI) - 0.35) * 1.5, 0, 1);
                    var speed = Math.Clamp(80 + throttle * 260 - brake * 180, 0, 350);
                    var gear = Math.Clamp((int)(speed / 45) + 1, 1, 8);
                    var rpm = Math.Clamp(2500 + (int)(speed / 350 * 11500) + gear * 250, 800, 15000);
                    var lap = (int)(elapsed / 90) + 1;
                    var sector = (int)(elapsed / 30) % 3 + 1;
                    yield return new TelemetryData
                    {
                        Timestamp = DateTimeOffset.UtcNow,
                        SessionId = sessionId,
                        IsComplete = true,
                        DriverId = driverId,
                        RaceNumber = RaceNumbers[driverId - 1],

                        // Car Telemetry
                        Speed = speed,
                        Throttle = throttle * 100,
                        Brake = brake * 100,
                        SteeringAngle = Math.Sin(phase * 1.7) * 25,
                        Gear = gear,
                        Rpm = rpm,
                        Clutch = 0,

                        DrsActive = speed > 250 && brake == 0,
                        RevLightsPercent = Math.Clamp((rpm - 8000) * 100 / 7000, 0, 100),
                        RevLightsBitValue = 0,

                        // Brakes
                        FrontLeftBrakeTemperature = 350 + (int)(brake * 650),
                        FrontRightBrakeTemperature = 350 + (int)(brake * 650),
                        RearLeftBrakeTemperature = 300 + (int)(brake * 500),
                        RearRightBrakeTemperature = 300 + (int)(brake * 500),

                        // Tyres - surface temperature
                        FrontLeftTyreSurfaceTemperature = 85 + (int)(throttle * 25),
                        FrontRightTyreSurfaceTemperature = 85 + (int)(throttle * 25),
                        RearLeftTyreSurfaceTemperature = 82 + (int)(throttle * 22),
                        RearRightTyreSurfaceTemperature = 82 + (int)(throttle * 22),

                        // Tyres - inner temperature
                        FrontLeftTyreInnerTemperature = 80 + (int)(throttle * 20),
                        FrontRightTyreInnerTemperature = 80 + (int)(throttle * 20),
                        RearLeftTyreInnerTemperature = 78 + (int)(throttle * 18),
                        RearRightTyreInnerTemperature = 78 + (int)(throttle * 18),

                        // Tyres - pressure
                        FrontLeftTyrePressure = 21.2 + throttle * 0.5,
                        FrontRightTyrePressure = 21.2 + throttle * 0.5,
                        RearLeftTyrePressure = 20.4 + throttle * 0.5,
                        RearRightTyrePressure = 20.4 + throttle * 0.5,

                        // Tyres - surface type
                        FrontLeftSurfaceType = 1,
                        FrontRightSurfaceType = 1,
                        RearLeftSurfaceType = 1,
                        RearRightSurfaceType = 1,

                        // Engine
                        EngineTemperature = 90 + (int)(throttle * 25),

                        // ERS / Battery
                        BatteryLevel = 45 + Math.Sin(elapsed / 12) * 35,
                        ErsDeployment = throttle * 100,
                        ErsRecovery = brake * 35,

                        // Fuel
                        FuelRemaining = Math.Max(1, 100 - elapsed / 8),
                        FuelConsumption = 1.5 + throttle * 2,

                        // Lap
                        Lap = lap,
                        Sector = sector,
                        LapTime = TimeSpan.FromSeconds(90 + Math.Sin(elapsed / 15) * 3)
                    };
                }
            }
        }
    }
}
