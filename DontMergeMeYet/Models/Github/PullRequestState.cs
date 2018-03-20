using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DontMergeMeYet.Models.Github
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PullRequestState
    {
        [EnumMember(Value = "open")]
        Open,
        [EnumMember(Value = "closed")]
        Closed
    }
}