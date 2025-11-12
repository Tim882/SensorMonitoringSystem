namespace SensorProcessor.DTOs;

public class SensorDataResponseDto
{
    public int Id { get; set; }
    public int SensorId { get; set; }
    public double Value { get; set; }
    public DateTime Timestamp { get; set; }
}