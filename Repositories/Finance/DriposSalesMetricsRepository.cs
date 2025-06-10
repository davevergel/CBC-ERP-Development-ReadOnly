using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models.Finance;
using Dapper;

namespace CbcRoastersErp.Repositories.Finance
{
    public class DriposSalesMetricsRepository : IDriposSalesMetricsRepository
    {
        public async Task<IEnumerable<DriposSalesMetric>> GetAllAsync()
        {
            try
            {
                using var conn = DatabaseHelper.GetOpenConnection();
                return await conn.QueryAsync<DriposSalesMetric>("SELECT * FROM dripos_sales_metrics ORDER BY MetricDate");
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System");
                return new List<DriposSalesMetric>();
            }
        }

        public async Task<IEnumerable<DriposSalesMetric>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            try
            {
                using var conn = DatabaseHelper.GetOpenConnection();
                const string sql = "SELECT * FROM dripos_sales_metrics WHERE MetricDate BETWEEN @start AND @end ORDER BY MetricDate";
                var results = await conn.QueryAsync<DriposSalesMetric>(sql, new { start, end });
                ApplicationLogger.LogInfo($"Loaded {results?.Count()} records from dripos_sales_metrics", "Finance");
                return results;

            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System");
                return new List<DriposSalesMetric>();
            }
        }

        public async Task InsertAsync(IEnumerable<DriposSalesMetric> metrics)
        {
            const string sql = "INSERT INTO dripos_sales_metrics (MetricDate, MetricName, Amount, Source, CreatedAt) " +
                               "VALUES (@MetricDate, @MetricName, @Amount, @Source, @CreatedAt)";

            try
            {
                using var conn = DatabaseHelper.GetOpenConnection();
                await conn.ExecuteAsync(sql, metrics);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System");
            }
        }

        public async Task InsertAsync(DriposSalesMetric metric)
        {
            await InsertAsync(new List<DriposSalesMetric> { metric });
        }
    }
}
