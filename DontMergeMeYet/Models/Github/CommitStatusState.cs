using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DontMergeMeYet.Models.Github
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CommitStatusState
    {
        [EnumMember(Value = "pending")]
        Pending,
        [EnumMember(Value = "error")]
        Error,
        [EnumMember(Value = "failure")]
        Failure,
        [EnumMember(Value = "success")]
        Success
    }
}