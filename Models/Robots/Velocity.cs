using System.Text.Json.Serialization;

namespace backend.Models.Robots
{
    public class Velocity
    {
        [JsonIgnore]
        public int Id { get; set; }
        
        [JsonPropertyName("angular")]
        public int Angular { get; set; }
        public float Linear { get; set; }
    }
}