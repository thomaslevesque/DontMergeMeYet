using Newtonsoft.Json;

namespace DontMergeMeYet.Models.Github
{
    public class Installation
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}