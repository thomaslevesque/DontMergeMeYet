using Newtonsoft.Json;

namespace DontMergeMeYet.Models.Github
{
    public class CommitStatus
    {
        [JsonProperty("state")]
        public CommitStatusState State { get; set; }
        [JsonProperty("target_url")]
        public string TargetUrl { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("context")]
        public string Context { get; set; }
    }
}