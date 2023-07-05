using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmplitudeFeatureLibrary.Models
{
    internal class FeatureFlagValue
    {
        [JsonProperty("key")]
        public string Value { get; set; }
    }
}
