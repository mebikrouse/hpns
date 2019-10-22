using System.Collections.Generic;
using CitizenFX.Core;

namespace Hubs.Data
{
    public class HubConfiguration
    {
        public IEnumerable<SpawnConfiguration> SpawnConfigurations { get; set; }

        public Vector3 HubCenter { get; set; }

        public float HubRadius { get; set; }

        public float Timeout { get; set; }
    }
}