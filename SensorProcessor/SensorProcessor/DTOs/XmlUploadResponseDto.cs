namespace SensorProcessor.DTOs;

public class XmlUploadResponseDto
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}