using System.Text.Json.Serialization;

namespace backend.Models
{
    public class Mission
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Guid { get; set; }

        public string Url { get; set; }
        
        public string Name { get; set; }
        
        public string Actions { get; set; }
        [JsonPropertyName("Created_by")]
        
        public string CreatedBy { get; set; }
        
        public string CreatedById { get; set; }
        public string Definition { get; set; }
        public string Description { get; set; }
        
        public bool HasUserParameters { get; set; }
        public bool Hidden { get; set; }
        
        
        
        public string SessionId { get; set; }
        public bool Valid { get; set; }
        [JsonIgnore]
        public int RobotId { get; set; }
        public Robot Robot { get; set; }
    }
}