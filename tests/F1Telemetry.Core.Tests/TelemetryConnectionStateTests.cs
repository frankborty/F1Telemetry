using F1Telemetry.Core.Models;
using F1Telemetry.Core.Services;

namespace F1Telemetry.Core.Tests;

public sealed class TelemetryConnectionStateTests
{
    [Fact]
    public void Set_ExposesLatestStatusAndNotifiesSubscribers()
    {
        var state = new TelemetryConnectionState();
        var notifications = 0;
        state.Changed += () => notifications++;
        var timestamp = DateTimeOffset.UtcNow;

        state.Set(TelemetryConnectionStatus.Connected, "OK", timestamp);

        Assert.Equal(TelemetryConnectionStatus.Connected, state.Status);
        Assert.Equal("OK", state.Message);
        Assert.Equal(timestamp, state.LastPacketAt);
        Assert.Equal(1, notifications);
    }
}
