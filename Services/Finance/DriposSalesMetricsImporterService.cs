using System.Globalization;
using System.IO;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models.Finance;
using Dapper;

namespace CbcRoastersErp.Services.Finance
{
    public class DriposSalesMetricsImporterService
    {
        public async Task<List<SalesMetricRow>> LoadDetailedMetricsAsync(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length < 6) return new List<SalesMetricRow>();

                // Detect year from header (line 1, column 1)
                var header = lines[1].Split(',').ElementAtOrDefault(1)?.Trim();
                int headerYear = DateTime.Today.Year;

                var dateRangeMatch = System.Text.RegularExpressions.Regex.Match(header ?? "", @"(\d{1,2}/\d{1,2}/(\d{2,4}))");
                if (dateRangeMatch.Success && DateTime.TryParseExact(dateRangeMatch.Groups[1].Value, "M/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedStart))
                {
                    headerYear = parsedStart.Year;
                }

                var dates = lines[3].Split(',').Skip(1).ToArray();  // date headers
                var result = new List<SalesMetricRow>();

                for (int row = 4; row < lines.Length; row++)
                {
                    var columns = lines[row].Split(',');
                    if (columns.Length < 2 || string.IsNullOrWhiteSpace(columns[0])) continue;

                    var metricName = columns[0].Trim('"').Trim();
                    for (int i = 1; i < columns.Length && i <= dates.Length; i++)
                    {
                        var rawDate = dates[i - 1].Trim('"').Trim();
                        var rawAmount = columns[i].Trim('"').Trim();

                        if (!DateTime.TryParseExact(rawDate, "M/d", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
                        {
                            ApplicationLogger.LogInfo($"Invalid date format: {rawDate}", "DriposMetricsDebug");
                            continue;
                        }

                        var fullDate = new DateTime(headerYear, dateOnly.Month, dateOnly.Day);

                        if (!decimal.TryParse(rawAmount.Replace("$", "").Replace(",", ""), out var amount))
                        {
                            ApplicationLogger.LogInfo($"Invalid amount format: '{rawAmount}' for {metricName} on {fullDate.ToShortDateString()}", "DriposMetricsDebug");
                            continue;
                        }

                        if (amount == 0) continue;

                        ApplicationLogger.LogInfo($"Parsed metric '{metricName}' on {fullDate.ToShortDateString()} as {amount}", "DriposMetricsDebug");

                        result.Add(new SalesMetricRow
                        {
                            Date = fullDate,
                            Metric = metricName,
                            Amount = amount
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                return new List<SalesMetricRow>();
            }
        }

        public async Task SaveToDatabaseAsync(IEnumerable<SalesMetricRow> rows)
        {
            foreach (var row in rows)
            {
                var metric = new DriposSalesMetric
                {
                    MetricDate = row.Date,
                    MetricName = row.Metric,
                    Amount = row.Amount,
                    Source = "Dripos",
                    CreatedAt = DateTime.Now
                };

                try
                {
                    using var conn = DatabaseHelper.GetOpenConnection();
                    await conn.ExecuteAsync(
                        "INSERT INTO dripos_sales_metrics (MetricDate, MetricName, Amount, Source, CreatedAt) VALUES (@MetricDate, @MetricName, @Amount, @Source, @CreatedAt)",
                        metric);
                }
                catch (Exception ex)
                {
                    ApplicationLogger.Log(ex, "System");
                }
            }
        }

    }
}
