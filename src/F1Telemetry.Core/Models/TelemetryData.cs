using System;
using System.Collections.Generic;
using System.Text;

namespace F1Telemetry.Core.Models
{
    public sealed class TelemetryData
    {
        public DateTimeOffset Timestamp { get; init; }
        public double Speed { get; init; }
        public int Rpm { get; init; }
        public int Gear { get; init; }
        public double Throttle { get; init; }
        public double Brake { get; init; }
        public bool DrsActive { get; init; }
        public double SteeringAngle { get; init; }
        public double BatteryLevel { get; init; }
        public double ErsDeployment { get; init; }
        public double ErsRecovery { get; init; }
        public double EngineTemperature { get; init; }
        public double FuelRemaining { get; init; }
        public double FuelConsumption { get; init; }
        public int Lap { get; init; }
        public int Sector { get; init; }
        public TimeSpan LapTime { get; init; }
    }
}
