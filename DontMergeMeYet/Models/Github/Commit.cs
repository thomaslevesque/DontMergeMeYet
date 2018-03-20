using Newtonsoft.Json;

namespace DontMergeMeYet.Models.Github
{
    public class Commit
    {
        [JsonProperty("sha")]
        public string Sha { get; set; }

        [JsonProperty("commit")]
        public CommitDetails Details { get; set; }
    }

    public class CommitDetails
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}