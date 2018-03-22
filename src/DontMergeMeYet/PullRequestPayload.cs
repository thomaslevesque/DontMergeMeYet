using Newtonsoft.Json;
using Octokit;

namespace DontMergeMeYet
{
    public class PullRequestPayload : PullRequestEventPayload
    {
        [JsonProperty("installation")]
        public Installation Installation { get; set; }
    }
}