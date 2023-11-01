namespace Core.SignalR;

public class ClientData
{
    public long DeviceId { get; set; }
    public List<long> MetricsId { get; set; } = new List<long>();
}
