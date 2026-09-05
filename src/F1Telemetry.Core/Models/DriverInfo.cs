namespace F1Telemetry.Core.Models
{
    public class DriverInfo
    {
        public int CarIndex { get; init; }
        public string Driver { get; init; } = string.Empty;
        public string Team { get; init; } = string.Empty;
        public int RaceNumber { get; init; }
    }
}
