namespace Core.Devices;

public class Device
{
    public long Id { get; set; }
    public string Name { get; set; }
    public List<Metric> Metrics { get; set; }
}
