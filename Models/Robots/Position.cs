using System.Text.Json.Serialization;

namespace backend.Models.Robots
{
    public class Position
    {
        public int Id { get; set; }
        
        public float Orientation { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
}