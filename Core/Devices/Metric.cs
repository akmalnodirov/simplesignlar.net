using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Devices;

public class Metric
{
    public long Id { get; set; }    
    public string Name { get; set; }
    public int Value { get; set; }

    public long DeviceId { get; set; }
}
