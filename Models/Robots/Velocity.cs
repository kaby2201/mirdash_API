using System.Text.Json.Serialization;

namespace backend.Models.Robots
{
    public class Velocity
    {
        public int Id { get; set; }
        
        public float Angular { get; set; }
        public float Linear { get; set; }
    }
}