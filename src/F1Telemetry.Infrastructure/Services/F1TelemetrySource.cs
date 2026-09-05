using F1Game.UDP;
using F1Game.UDP.Data;
using F1Game.UDP.Enums;
using F1Game.UDP.Packets;
using F1Telemetry.Core.Interfaces;
using F1Telemetry.Core.Models;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace F1Telemetry.Infrastructure.Services
{
    public sealed class F1TelemetrySource : ITelemetrySource, IDisposable
    {
        private const int DefaultPort = 20777;

        private readonly UdpClient _udpClient;
        private bool _disposed;

        public F1TelemetrySource(int port = DefaultPort)
        {
            _udpClient = new UdpClient(port);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _udpClient.Dispose();
        }

        public async IAsyncEnumerable<TelemetryData> GetTelemetryAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            CarStatusData? status = null;
            LapData? lap = null;
            ParticipantData? participant = null;

            while (!cancellationToken.IsCancellationRequested)
            {
                UdpReceiveResult result = await _udpClient.ReceiveAsync(cancellationToken);
                UnionPacket packet = result.Buffer.ToPacket();

                switch (packet.PacketType)
                {
                    case PacketType.CarStatus
                        when packet.TryGetCarStatusDataPacket(out var statusPacket):
                        status = statusPacket.CarStatusData[statusPacket.Header.PlayerCarIndex];
                        break;

                    case PacketType.LapData
                        when packet.TryGetLapDataPacket(out var lapPacket):
                        lap = lapPacket.LapData[lapPacket.Header.PlayerCarIndex];
                        break;

                    case PacketType.CarTelemetry
                        when packet.TryGetCarTelemetryDataPacket(out var telemetryPacket):
                        var player = telemetryPacket.CarTelemetryData[telemetryPacket.Header.PlayerCarIndex];
                        if (participant is not null)
                        {
                            yield return Map(player, status, lap);
                        }

                        break;
                }
            }
        }

        private static TelemetryData Map(CarTelemetryData telemetry, CarStatusData? status, LapData? lap)
        {
            return new TelemetryData
            {
                // Car Telemetry
                Speed = telemetry.Speed,
                Throttle = telemetry.Throttle,
                Brake = telemetry.Brake,
                SteeringAngle = telemetry.Steer,
                Gear = telemetry.Gear,
                Rpm = telemetry.EngineRPM,
                Clutch = telemetry.Clutch,

                DrsActive = telemetry.IsDrsOn,

                RevLightsPercent = telemetry.RevLightsPercent,
                RevLightsBitValue = telemetry.RevLightsBitValue,

                // Brakes
                FrontLeftBrakeTemperature = telemetry.BrakesTemperature.FrontLeft,
                FrontRightBrakeTemperature = telemetry.BrakesTemperature.FrontRight,
                RearLeftBrakeTemperature = telemetry.BrakesTemperature.RearLeft,
                RearRightBrakeTemperature = telemetry.BrakesTemperature.RearRight,

                // Tyres - surface temperature
                FrontLeftTyreSurfaceTemperature = telemetry.TyresSurfaceTemperature.FrontLeft,
                FrontRightTyreSurfaceTemperature = telemetry.TyresSurfaceTemperature.FrontRight,
                RearLeftTyreSurfaceTemperature = telemetry.TyresSurfaceTemperature.RearLeft,
                RearRightTyreSurfaceTemperature = telemetry.TyresSurfaceTemperature.RearRight,

                // Tyres - inner temperature
                FrontLeftTyreInnerTemperature = telemetry.TyresInnerTemperature.FrontLeft,
                FrontRightTyreInnerTemperature = telemetry.TyresInnerTemperature.FrontRight,
                RearLeftTyreInnerTemperature = telemetry.TyresInnerTemperature.RearLeft,
                RearRightTyreInnerTemperature = telemetry.TyresInnerTemperature.RearRight,

                // Tyres - pressure
                FrontLeftTyrePressure = telemetry.TyresPressure.FrontLeft,
                FrontRightTyrePressure = telemetry.TyresPressure.FrontRight,
                RearLeftTyrePressure = telemetry.TyresPressure.RearLeft,
                RearRightTyrePressure = telemetry.TyresPressure.RearRight,

                // Tyres - surface type
                FrontLeftSurfaceType = (int)telemetry.SurfaceType.FrontLeft,
                FrontRightSurfaceType = (int)telemetry.SurfaceType.FrontRight,
                RearLeftSurfaceType = (int)telemetry.SurfaceType.RearLeft,
                RearRightSurfaceType = (int)telemetry.SurfaceType.RearRight,

                // Engine
                EngineTemperature = telemetry.EngineTemperature,

                // ERS / Battery
                BatteryLevel = status?.ErsStoreEnergy ?? 0,
                ErsDeployment = status?.ErsDeployedThisLap ?? 0,
                ErsRecovery = status?.ErsHarvestedThisLapMGUK ?? 0,

                // Fuel
                FuelRemaining = status?.FuelInTank ?? 0,

                // Lap
                Lap = lap?.CurrentLapNum ?? 0,
                Sector = lap is null
                    ? 0
                    : (int)lap.Value.Sector + 1,

                LapTime = lap is null
                    ? TimeSpan.Zero
                    : TimeSpan.FromMilliseconds(
                        lap.Value.CurrentLapTimeInMS)
            };
        }
    }
}
