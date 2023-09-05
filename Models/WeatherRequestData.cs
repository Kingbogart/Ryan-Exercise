using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Coding_Exercise.Models
{
        public class WeatherRequestData
        {
            [JsonRequired]
            [JsonProperty("date")]
            public DateTime Date { get; set; }

[JsonRequired]
            [JsonProperty("temperature")]   
            public int Temperature { get; set; }

[JsonRequired]
            [JsonProperty("humidity")]
            public int Humidity { get; set; }
            
            [JsonRequired]
            [JsonProperty("pressure")]
            public int Pressure { get; set; }
            public int Wind { get; set; }
[JsonRequired]
            [JsonProperty("windDirection")]
            public int WindDirection { get; set; }
            [JsonRequired]
            [JsonProperty("rain")]
            public int Rain { get; set; }
        }
}
