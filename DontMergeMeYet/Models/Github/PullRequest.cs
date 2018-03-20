using Newtonsoft.Json;

namespace DontMergeMeYet.Models.Github
{
    public class PullRequest
    {
        [JsonProperty("number")]
        public int Number { get; set; }
        [JsonProperty("state")]
        public PullRequestState State { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("commits_url")]
        public string CommitsUrl { get; set; }
        [JsonProperty("head")]
        public Commit Head { get; set; }
    }
}