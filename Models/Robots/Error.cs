using System.Text.Json.Serialization;

namespace backend.Models.Robots
{
    public class Error
    {
        [JsonIgnore]
        public int Id { get; set; }
        
        public int Code { get; set; }
        public string Description { get; set; }
        public string Module { get; set; }
    }
}