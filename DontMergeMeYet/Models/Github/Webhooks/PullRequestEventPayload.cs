using Newtonsoft.Json;

namespace DontMergeMeYet.Models.Github.Webhooks
{
    public class PullRequestEventPayload : EventPayload
    {
        [JsonProperty("action")]
        public string Action { get; set; }
        [JsonProperty("number")]
        public int Number { get; set; }
        [JsonProperty("pull_request")]
        public PullRequest PullRequest { get; set; }
        [JsonProperty("repository")]
        public Repository Repository { get; set; }
    }
}