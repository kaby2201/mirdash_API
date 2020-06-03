using System.Text.Json.Serialization;

namespace backend.Models.Robots
{
    public class UserPrompt
    {
        [JsonIgnore]
        public int Id { get; set; }
        
        public string Guid { get; set; }
        public string Options { get; set; }
        public string Question { get; set; }
        public float Timeout { get; set; }
        
        [JsonPropertyName("user_group")]
        public string UserGroup { get; set; }
    }
}