using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace kiss_graph_api.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ActingRoleType
    {
        [EnumMember(Value = "lead")]
        Lead,
    }
}