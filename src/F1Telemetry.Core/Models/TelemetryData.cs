namespace F1Telemetry.Core.Models
{
    public sealed class TelemetryData
    {
        public DateTimeOffset Timestamp { get; init; }

        // =========================
        // Driver Information
        // =========================

        public int DriverId { get; init; }
        public string DriverName { get; init; } = string.Empty;
        public int TeamId { get; init; }
        public int RaceNumber { get; init; }
        public int NationalityId { get; init; }

        // =========================
        // Car Telemetry
        // =========================

        public double Speed { get; init; }
        public double Throttle { get; init; }
        public double Brake { get; init; }
        public double SteeringAngle { get; init; }
        public int Gear { get; init; }
        public int Rpm { get; init; }
        public int Clutch { get; init; }

        public bool DrsActive { get; init; }

        public int RevLightsPercent { get; init; }
        public int RevLightsBitValue { get; init; }

        // =========================
        // Brakes
        // =========================

        public int FrontLeftBrakeTemperature { get; init; }
        public int FrontRightBrakeTemperature { get; init; }
        public int RearLeftBrakeTemperature { get; init; }
        public int RearRightBrakeTemperature { get; init; }

        // =========================
        // Tyres
        // =========================

        public int FrontLeftTyreSurfaceTemperature { get; init; }
        public int FrontRightTyreSurfaceTemperature { get; init; }
        public int RearLeftTyreSurfaceTemperature { get; init; }
        public int RearRightTyreSurfaceTemperature { get; init; }

        public int FrontLeftTyreInnerTemperature { get; init; }
        public int FrontRightTyreInnerTemperature { get; init; }
        public int RearLeftTyreInnerTemperature { get; init; }
        public int RearRightTyreInnerTemperature { get; init; }

        public double FrontLeftTyrePressure { get; init; }
        public double FrontRightTyrePressure { get; init; }
        public double RearLeftTyrePressure { get; init; }
        public double RearRightTyrePressure { get; init; }

        public int FrontLeftSurfaceType { get; init; }
        public int FrontRightSurfaceType { get; init; }
        public int RearLeftSurfaceType { get; init; }
        public int RearRightSurfaceType { get; init; }

        // =========================
        // Engine
        // =========================

        public int EngineTemperature { get; init; }

        // =========================
        // ERS / Battery
        // =========================

        public double BatteryLevel { get; init; }
        public double ErsDeployment { get; init; }
        public double ErsRecovery { get; init; }

        // =========================
        // Fuel
        // =========================

        public double FuelRemaining { get; init; }
        public double FuelConsumption { get; init; }

        // =========================
        // Lap
        // =========================

        public int Lap { get; init; }
        public int Sector { get; init; }
        public TimeSpan LapTime { get; init; }

    }
}
