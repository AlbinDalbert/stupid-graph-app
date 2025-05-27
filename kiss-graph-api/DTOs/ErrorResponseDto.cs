using System.Text.Json.Serialization;

namespace kiss_graph_api.DTOs
{
    public class ErrorResponseDto
    {
        public required int StatusCode { get; set; }
        public required string Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Detail { get; set; }

    }
}