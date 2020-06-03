using System.Text.Json.Serialization;

namespace backend.Models
{
    public class MissionQueueRequest
    {
        [JsonIgnore]
        public int Id { get; set; }
        
        [JsonPropertyName("mission_id")]
        public int MissionId { get; set; }
        
        [JsonIgnore]
        public Mission Mission { get; set; }
        
        [JsonIgnore]
        public int RobotId { get; set; }
        [JsonIgnore]
        public Robot Robot { get; set; }
        public string Name { get; set; }
        public string Guid { get; set; }
        public string Description { get; set; }
    }
}