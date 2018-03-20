using Newtonsoft.Json;

namespace DontMergeMeYet.Models.Github.Webhooks
{
    public class InstallationEventPayload : EventPayload
    {
        [JsonProperty("action")]
        public string Action { get; set; }
        
        [JsonProperty("installation")]
        public Installation Installation { get; set; }

        [JsonProperty("repositories")]
        public Repository[] Repositories { get; set; }
    }
}