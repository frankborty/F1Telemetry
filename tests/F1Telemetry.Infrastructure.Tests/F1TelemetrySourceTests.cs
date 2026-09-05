using F1Telemetry.Infrastructure.Services;

namespace F1Telemetry.Infrastructure.Tests;

public sealed class F1TelemetrySourceTests
{
    [Fact]
    public async Task Cancellation_StopsWaitingForUdpPacket()
    {
        using var source = new F1TelemetrySource(0);
        using var cancellation = new CancellationTokenSource();
        await using var enumerator = source.GetTelemetryAsync(cancellation.Token).GetAsyncEnumerator(cancellation.Token);

        var pendingRead = enumerator.MoveNextAsync().AsTask();
        await Task.Delay(50);
        cancellation.Cancel();

        var completed = await Task.WhenAny(pendingRead, Task.Delay(TimeSpan.FromSeconds(2)));
        Assert.Same(pendingRead, completed);
        Assert.False(await pendingRead);
    }
}
