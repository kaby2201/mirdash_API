using System.Text.Json.Serialization;

namespace backend.Models
{
    public class PutStatus
    {
        [JsonPropertyName("state_id")]
        public int StateId { get; set; }
    }
}