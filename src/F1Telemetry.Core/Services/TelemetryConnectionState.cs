using F1Telemetry.Core.Models;

namespace F1Telemetry.Core.Services;

public sealed class TelemetryConnectionState
{
    private readonly object _sync = new();
    private TelemetryConnectionStatus _status = TelemetryConnectionStatus.Starting;
    private string _message = "Avvio della sorgente telemetria...";
    private DateTimeOffset? _lastPacketAt;

    public event Action? Changed;

    public TelemetryConnectionStatus Status { get { lock (_sync) return _status; } }
    public string Message { get { lock (_sync) return _message; } }
    public DateTimeOffset? LastPacketAt { get { lock (_sync) return _lastPacketAt; } }

    public void Set(TelemetryConnectionStatus status, string message, DateTimeOffset? lastPacketAt = null)
    {
        lock (_sync)
        {
            _status = status;
            _message = message;
            if (lastPacketAt.HasValue)
            {
                _lastPacketAt = lastPacketAt;
            }
        }

        Changed?.Invoke();
    }
}
