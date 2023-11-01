using Core;
using Core.Devices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRApp;
using System.Text.Json;

namespace ProtoApp.Controllers;

[ApiController]
[Route("[controller]")]
public class HubController : ControllerBase
{
    /// <summary>
    /// The hub context
    /// </summary>
    private readonly IHubContext<MainHub> _hubContext;

    /// <summary>
    /// Gets or sets the devices.
    /// </summary>
    /// <value>
    /// The devices.
    /// </value>
    public List<Device> Devices { get; set; } = new List<Device>();

    /// <summary>
    /// Initializes a new instance of the <see cref="HubController"/> class.
    /// </summary>
    /// <param name="hubContext">The hub context.</param>
    public HubController(IHubContext<MainHub> hubContext)
    {
        FillMetrics();
        _hubContext = hubContext;
    }

    /// <summary>
    /// Receivers the check asynchronous.
    /// </summary>
    /// <param name="receiverCheckRequest">The receiver check request.</param>
    /// <returns></returns>
    [HttpPost("device-change")]
    public async Task<IActionResult> ReceiverCheckAsync(List<Device> request)
    {

        var metricsIds = new List<long>();
        var changedMetrics = new List<Metric>();
        foreach (var device in request)
        {
            var storedDevice = Devices.FirstOrDefault(d => d.Id == device.Id);

            if(storedDevice != null)
            {
                foreach (var item in device.Metrics)
                {
                    var storedMetric = storedDevice.Metrics.FirstOrDefault(m => m.Id == item.Id);
                    if (storedMetric != null)
                    {
                        storedMetric.Value = item.Value;
                        changedMetrics.Add(storedMetric);
                        metricsIds.Add(storedMetric.Id);
                    }
                }
            }
        }

        var clientIds = ClientsService.ConnectedClients
        .Where(kvp => kvp.Value.MetricsId.Any(metricsId => metricsIds.Contains(metricsId)))
        .Select(kvp => kvp.Key)
        .ToList();

        await _hubContext.Clients.Clients(clientIds).SendAsync("refresh", JsonSerializer.Serialize(changedMetrics));

        return Ok();
    }

    /// <summary>
    /// Deviceses the list.
    /// </summary>
    /// <returns></returns>
    [HttpGet("devices")]
    public async Task<IActionResult> DevicesList()
    {
        return Ok(Devices);
    }

    private void FillMetrics()
    {
        Random rand = new Random();
        var devices = new string[5] { "Thermometer", "Multimeter", "Barometer", "Hygrometer", "Speedometer" };
        var dict = new Dictionary<int, string[]>();
        dict[0] = new string[3] { "Temperature", "Temperature2", "Temperature3" };
        dict[1] = new string[3] { "Voltage", "Current", "Resistance" };
        dict[2] = new string[3] { "Atmospheric Pressure", "Atmospheric Pressure2", "Atmospheric Pressure3" };
        dict[3] = new string[3] { "Humidity1", "Humidity2", "Humidity3" };
        dict[4] = new string[2] { "Speed", "Speed2" };

        for (int i = 0; i < devices.Length; i++)
        {
            var device = new Device
            {
                Name = devices[i],
                Id = i + 1,
            };

            var metrics = new List<Metric>();

            var counter = 1;
            foreach (var item in dict[i])
            {
                metrics.Add(new Metric
                {
                    Name = item,
                    Value = rand.Next(10, 101),
                    Id = counter,
                    DeviceId = device.Id
                });

                counter++;
            }

            device.Metrics = metrics;

            Devices.Add(device);
        }
    }
}
