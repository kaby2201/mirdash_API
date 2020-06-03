using backend.Models.Robots;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Status
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int RobotId { get; set; }
        [JsonIgnore]
        public Robot Robot { get; set; }
        
        [JsonPropertyName("battery_percentage")]
        public float BatteryPercentage { get; set; }
        
        [JsonPropertyName("battery_time_remaining")]
        public float BatteryTimeRemaining { get; set; }
        
        [JsonPropertyName("distance_to_next_target")]
        public float DistanceToNextTarget { get; set; }
        
        [NotMapped]
        public List<Error> Errors { get; set; }
        
        [JsonIgnore]
        public string Footprint { get; set; }
        
        [JsonPropertyName("map_id")]
        public string MapId { get; set; }
        
        [JsonPropertyName("mission_queue_id")]
        public int MissionQueueId { get; set; }
        
        [JsonPropertyName("mission_queue_url")]
        public string MissionQueueUrl { get; set; }
        
        [JsonPropertyName("mission_text")]
        public string MissionText { get; set; }

        [JsonPropertyName("mode_id")]
        public int ModeId { get; set; }
        
        [JsonPropertyName("mode_text")]
        public string ModeText { get; set; }
        
        public float Moved { get; set; }
        
        public Position Position { get; set; }
        
        [JsonPropertyName("robot_model")]
        public string RobotModel { get; set; }

        [JsonPropertyName("robot_name")]
        public string RobotName { get; set; }
        
        [JsonPropertyName("serial_number")]
        public string SerialNumber { get; set; }
        
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
        
        [JsonPropertyName("state_id")]
        public int StateId { get; set; }
        
        [JsonPropertyName("state_text")]
        public string StateText { get; set; }
        
        [JsonPropertyName("unloaded_map_changes")]
        public bool UnloadedMapChanges { get; set; }
        
        public int Uptime { get; set; }
        
        [JsonPropertyName("user_prompt"), JsonIgnore]
        public UserPrompt UserPrompt { get; set; }
        
        public Velocity Velocity { get; set; }
    }
}

