using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coding_Exercise.Configuration
{
    public class KafkaConfig
    {
        public string Topic  { get; set; } = "";
        public string GroupID { get; set; } = "";
        public string BootstrapServers { get; set; } = "";

        public string ForwardEndpoint { get; set; } = "";
    }
}
