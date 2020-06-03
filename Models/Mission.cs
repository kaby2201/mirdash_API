namespace backend.Models
{
    public class Mission
    {
        public int Id { get; set; }
        
        public string Guid { get; set; }
        
        public string Name { get; set; }
        
        public string Url { get; set; }
        
        public int RobotId { get; set; }
        
        public Robot Robot { get; set; }
    }
}