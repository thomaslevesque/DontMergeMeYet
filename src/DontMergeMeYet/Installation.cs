using Newtonsoft.Json;

namespace DontMergeMeYet
{
    public class Installation
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}