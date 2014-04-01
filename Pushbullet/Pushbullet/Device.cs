using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pushbullet
{
    public class Device
    {
        [JsonProperty("iden")]
        public string Identifier { get; set; }

        [JsonProperty("extras")]
        public DeviceDetails Details { get; set; }
    }
}
