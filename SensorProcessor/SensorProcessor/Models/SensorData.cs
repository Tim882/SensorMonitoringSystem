namespace SensorProcessor.Models
{
    public class SensorData
    {
        public int Id { get; set; }
        public int SensorId { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
