using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class MissionQueuesResponse
    {
        [Key, Column(Order=0)]
        public int Id { get; set; }
        
        public string State { get; set; }
        
        public string Url { get; set; }
        
        [JsonIgnore]
        [Key, Column(Order=1)]
        public int RobotId { get; set; }
        [JsonIgnore]
        public Robot Robot { get; set; }
    }
}