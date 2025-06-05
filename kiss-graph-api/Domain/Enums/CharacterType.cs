using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace kiss_graph_api.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CharacterType
    {
        [EnumMember(Value = "main")]
        Main,

        [EnumMember(Value = "antagonist")]
        Antagonist,

        [EnumMember(Value = "other")]
        Other,
    }
}