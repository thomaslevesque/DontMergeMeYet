using Newtonsoft.Json;

namespace DontMergeMeYet.Models.Github
{
    public class Repository
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("full_name")]
        public string FullName { get; set; }
    }
}