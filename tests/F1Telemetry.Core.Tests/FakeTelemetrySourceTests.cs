using F1Telemetry.Core.Services;

namespace F1Telemetry.Core.Tests;

public sealed class FakeTelemetrySourceTests
{
    [Fact]
    public async Task Source_ProducesCoherentSnapshots()
    {
        var source = new FakeTelemetrySource();
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        await using var enumerator = source.GetTelemetryAsync(cancellation.Token).GetAsyncEnumerator(cancellation.Token);

        Assert.True(await enumerator.MoveNextAsync());
        var snapshot = enumerator.Current;

        Assert.InRange(snapshot.Speed, 0, 350);
        Assert.InRange(snapshot.Throttle, 0, 100);
        Assert.InRange(snapshot.Brake, 0, 100);
        Assert.InRange(snapshot.BatteryLevel, 0, 100);
        Assert.True(snapshot.Timestamp > DateTimeOffset.MinValue);
        Assert.NotEqual(0UL, snapshot.SessionId);
        Assert.True(snapshot.IsComplete);
    }
}
