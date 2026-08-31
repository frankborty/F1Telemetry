using F1Telemetry.Core.Interfaces;
using F1Telemetry.Core.Models;
using System.Diagnostics;
using System.Threading.Channels;

namespace F1Telemetry.Core.Services
{
    public sealed class TelemetryService : ITelemetryService
    {
        private readonly Channel<TelemetryData> _channel;

        public TelemetryService()
        {
            _channel = Channel.CreateBounded<TelemetryData>(100);
            Debug.WriteLine($"TelemetryService: {GetHashCode()}");
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

        public async Task WriteAsync(TelemetryData telemetryData, CancellationToken cancellationToken = default)
        {
            await _channel.Writer.WriteAsync(telemetryData, cancellationToken);
        }
    }
}
