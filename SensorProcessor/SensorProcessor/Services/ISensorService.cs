using SensorProcessor.DTOs;
using SensorProcessor.Models;

namespace SensorProcessor.Services;

public interface ISensorService
{
    Task<SensorDataResponseDto> AddSensorDataAsync(SensorDataDto dataDto);
    Task<IEnumerable<SensorDataResponseDto>> GetSensorDataAsync(DateTime start, DateTime end);
    Task<IEnumerable<SensorSummary>> GetSensorsSummaryAsync(DateTime start, DateTime end);
    Task<XmlUploadResponseDto> ProcessXmlDataAsync(Stream xmlStream);
}