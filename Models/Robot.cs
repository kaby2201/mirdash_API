using backend.Models.Robots;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Robot
    {
        public int Id { get; set; }

        public string GuId { get; set; }

        public string BasePath { get; set; }

        [JsonPropertyName("robot_name")] 
        public string Hostname { get; set; }

        public string Token { get; set; }

        public bool IsOnline { get; set; }

        // Extenden info
        // [JsonIgnore] 
        // public string AllowedMethods { get; set; }

        [JsonPropertyName("battery_percentage")]
        public float BatteryPercentage { get; set; }

        [JsonPropertyName("battery_time_remaining")]
        public int BatteryTimeRemaining { get; set; }

        [JsonPropertyName("distance_to_next_target")]
        public float DistanceToNextTarget { get; set; }

        public string Footprint { get; set; }

        [JsonPropertyName("mission_queue_id")] public int MissionQueueId { get; set; }

        [JsonPropertyName("mission_queue_url")]
        public string MissionQueueUrl { get; set; }

        [JsonPropertyName("mission_text")] public string MissionText { get; set; }

        [JsonPropertyName("mode_id")] public int ModeId { get; set; }

        [JsonPropertyName("mode_text")] public string ModeText { get; set; }

        public float Moved { get; set; }

        [JsonIgnore] public int PositionId { get; set; }
        public Position Position { get; set; }

        [JsonPropertyName("robot_model")] public string RobotModel { get; set; }

        [JsonPropertyName("session_id")] public string SessionId { get; set; }

        [JsonPropertyName("serial_number")] public string SerialNumber { get; set; }

        [JsonPropertyName("state_id")] public int StateId { get; set; }

        [JsonPropertyName("state_text")] public string StateText { get; set; }

        [JsonPropertyName("unloaded_map_changes")]
        public bool UnloadedMapChanges { get; set; }

        public int Uptime { get; set; }

        // [JsonIgnore, NotMapped]
        // public int UserPromptId { get; set; }
        [JsonIgnore] public int VelocityId { get; set; }
        [JsonPropertyName("velocity")] public Velocity Velocity { get; set; }
    }
}