using System.Text.Json.Serialization;

namespace backend.Models
{
    public class MissionQueue
    {
        public int Id { get; set; }

        public string Actions { get; set; }
        
        [JsonPropertyName("created_by_id")]
        public string CreatedById { get; set; }
        
        [JsonPropertyName("created_by_name")]
        public string CreatedByName { get; set; }
        public string Description { get; set; }
        public string Finished { get; set; }
        
        [JsonPropertyName("id")]
        public int MissionQueueId { get; set; }

        public string Message { get; set; }
        public string Mission { get; set; }

        [JsonPropertyName("mission_id")]
        public string MissionId { get; set; }
        
        public string Ordered { get; set; }
        public int Priority { get; set; }
        public string State { get; set; }
    }
}