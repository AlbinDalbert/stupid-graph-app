using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace kiss_graph_api.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CreativeWorkType
    {
        [EnumMember(Value = "movie")]
        Movie,

        [EnumMember(Value = "book")]
        Book,

        [EnumMember(Value = "game")]
        Game,

        [EnumMember(Value = "other")]
        Other,
    }
}