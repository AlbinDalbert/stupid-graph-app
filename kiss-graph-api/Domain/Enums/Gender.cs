using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace kiss_graph_api.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        [EnumMember(Value = "male")]
        Male,

        [EnumMember(Value = "female")]
        Female,

        [EnumMember(Value = "non-binary")]
        NonBinary,
    }
}
