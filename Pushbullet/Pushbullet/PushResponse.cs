using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pushbullet
{
    public class PushResponse
    {
        [JsonProperty("iden")]
        public string Identifier { get; set; }

        [JsonProperty("device_iden")]
        public string Device { get; set; }

        [JsonProperty("data")]
        public PushResponseData Data { get; set; }

        [JsonProperty("created")]
        public long Timestamp { get; set; }
    }
}
