using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pushbullet
{
    public class DeviceList
    {
        [JsonProperty("devices")]
        public List<Device> Devices { get; set; }
    }
}
