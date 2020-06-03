using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class Robot
    {
        public int Id { get; set; }

        public string GuId { get; set; }
        public string BasePath { get; set; }
        public string Hostname { get; set; }
        public string Token { get; set; }
        public bool IsOnline { get; set; }
        
        [JsonPropertyName("state_text")]
        public string StateText { get; set; }
    }
}
