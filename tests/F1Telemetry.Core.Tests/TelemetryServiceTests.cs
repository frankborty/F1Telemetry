using F1Telemetry.Core.Models;
using F1Telemetry.Core.Services;

namespace F1Telemetry.Core.Tests;

public sealed class TelemetryServiceTests
{
    [Fact]
    public async Task WriteAsync_StoresLatestSnapshotByRaceNumber()
    {
        var service = new TelemetryService();
        var snapshot = new TelemetryData { RaceNumber = 44, Speed = 321, Timestamp = DateTimeOffset.UtcNow };

        await service.WriteAsync(snapshot);

        Assert.True(service.TryGetLatest(44, out var latest));
        Assert.Same(snapshot, latest);
    }

    [Fact]
    public async Task ReadAsync_ReturnsWrittenSnapshot()
    {
        var service = new TelemetryService();
        var snapshot = new TelemetryData { RaceNumber = 16 };
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        await service.WriteAsync(snapshot, cancellation.Token);
        await foreach (var value in service.GetTelemetryAsync(cancellation.Token))
        {
            Assert.Same(snapshot, value);
            return;
        }

        Assert.Fail("No snapshot was returned.");
    }
}
