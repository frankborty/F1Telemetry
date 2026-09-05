using F1Telemetry.Core.Interfaces;
using F1Telemetry.Core.Models;
using System.Diagnostics;
using System.Threading.Channels;

namespace F1Telemetry.Core.Services
{
    public sealed class TelemetryService : ITelemetryService
    {
        private readonly Channel<TelemetryData> _channel;
        private readonly object _latestLock = new();
        private readonly Dictionary<int, TelemetryData> _latestByRaceNumber = new();

        public TelemetryService()
        {
            _channel = Channel.CreateBounded<TelemetryData>(
           new BoundedChannelOptions(256)
           {
               SingleWriter = true,
               SingleReader = true,
               FullMode = BoundedChannelFullMode.DropOldest,
               AllowSynchronousContinuations = false
           });
        }

        public async IAsyncEnumerable<TelemetryData> GetTelemetryAsync(
            [System.Runtime.CompilerServices.EnumeratorCancellation]
            CancellationToken cancellationToken = default)
        {
            await foreach (var telemetry in _channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return telemetry;
            }
        }

        public ValueTask WriteAsync(TelemetryData telemetryData, CancellationToken cancellationToken = default)
        {
            lock (_latestLock)
            {
                if (telemetryData.RaceNumber > 0)
                {
                    _latestByRaceNumber[telemetryData.RaceNumber] = telemetryData;
                }
            }

            return _channel.Writer.WriteAsync(telemetryData, cancellationToken);
        }

        public bool TryGetLatest(int raceNumber, out TelemetryData? telemetryData)
        {
            lock (_latestLock)
            {
                return _latestByRaceNumber.TryGetValue(raceNumber, out telemetryData);
            }
        }
    }
}
