using SensorProcessor.Models;

namespace SensorProcessor.Repositories
{
    public interface ISensorRepository
    {
        Task<SensorData> AddAsync(SensorData data);
        Task<IEnumerable<SensorData>> GetByTimeRangeAsync(DateTime start, DateTime end);
        Task<IEnumerable<SensorSummary>> GetSummaryAsync(DateTime start, DateTime end);
        Task<bool> SaveXmlDataAsync(string xmlContent);
    }
}
