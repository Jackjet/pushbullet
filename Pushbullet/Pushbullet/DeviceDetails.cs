using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pushbullet
{
    public class DeviceDetails
    {
        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("android_version")]
        public string AndroidVersion { get; set; }

        [JsonProperty("sdk_version")]
        public string SDKVersion { get; set; }

        [JsonProperty("app_version")]
        public string AppVersion { get; set; }
        
        [JsonProperty("nickname")]
        public string Nickname { get; set; }
    }
}
