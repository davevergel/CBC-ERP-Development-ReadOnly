using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using CbcRoastersErp.Models.Finance;
using CbcRoastersErp.Repositories.Finance;
using CbcRoastersErp.Helpers;
using Dapper;

namespace CbcRoastersErp.Services.Finance
{
    public class DriposSalesImporterService
    {
        private readonly IJournalEntryRepository _journalRepo;
        private readonly IAccountRepository _accountRepo;
        private object _dailyRepo;

        public DriposSalesImporterService(IJournalEntryRepository journalRepo, IAccountRepository accountRepo)
        {
            _journalRepo = journalRepo;
            _accountRepo = accountRepo;
        }

        public async Task<int> ImportDailySalesCsvAsync(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length < 6) return 0;

                var header = lines[1].Split(',').ElementAtOrDefault(1)?.Trim();
                int headerYear = DateTime.Today.Year;

                var dateRangeMatch = System.Text.RegularExpressions.Regex.Match(header ?? "", @"(\d{1,2}/\d{1,2}/(\d{2,4}))");
                if (dateRangeMatch.Success && DateTime.TryParseExact(dateRangeMatch.Groups[1].Value, "M/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedStart))
                {
                    headerYear = parsedStart.Year;
                }

                var dateRow = lines[3].Split(',').Skip(1).ToArray();    // Row 4: Dates
                var salesRow = lines[5].Split(',').Skip(1).ToArray();   // Row 6: Product Sales

                int importedCount = 0;
                for (int i = 0; i < dateRow.Length && i < salesRow.Length; i++)
                {
                    var rawDate = dateRow[i].Trim('"').Trim();
                    var rawAmount = salesRow[i].Trim('"').Trim();

                    if (!DateTime.TryParseExact(rawDate, "M/d", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                        continue;

                    if (!decimal.TryParse(rawAmount.Replace("$", "").Replace(",", ""), out var amount))
                        continue;

                    if (amount == 0) continue;

                    var revenueAccount = await GetOrCreateAccountAsync("Sales Revenue", "Revenue");
                    var cashAccount = await GetOrCreateAccountAsync("Dripos Clearing", "Asset");

                    var entry = new JournalEntry
                    {
                        EntryDate = new DateTime(headerYear, date.Month, date.Day),
                        Description = "Dripos Daily Sales Import",
                        CreatedAt = DateTime.Now,
                        Lines = new ObservableCollection<JournalEntryLine>
                        {
                            new JournalEntryLine { AccountID = cashAccount.AccountID, Amount = amount, IsDebit = true },
                            new JournalEntryLine { AccountID = revenueAccount.AccountID, Amount = amount, IsDebit = false }
                        }
                    };

                    await _journalRepo.AddAsync(entry, entry.Lines);
                    importedCount++;
                }

                return importedCount;
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                return 0;
            }
        }

        public async Task<List<DriposSaleRow>> PreviewSalesRowsAsync(string filePath)
        {
            var result = new List<DriposSaleRow>();

            try
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length < 6) return result;

                var header = lines[1].Split(',').ElementAtOrDefault(1)?.Trim();
                int headerYear = DateTime.Today.Year;

                var dateRangeMatch = System.Text.RegularExpressions.Regex.Match(header ?? "", @"(\d{1,2}/\d{1,2}/(\d{2,4}))");
                if (dateRangeMatch.Success && DateTime.TryParseExact(dateRangeMatch.Groups[1].Value, "M/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedStart))
                {
                    headerYear = parsedStart.Year;
                }

                var dateRow = lines[3].Split(',').Skip(1).ToArray();
                var salesRow = lines[5].Split(',').Skip(1).ToArray();

                for (int i = 0; i < dateRow.Length && i < salesRow.Length; i++)
                {
                    var rawDate = dateRow[i].Trim('"').Trim();
                    var rawAmount = salesRow[i].Trim('"').Trim();

                    if (!DateTime.TryParseExact(rawDate, "M/d", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                        continue;

                    if (!decimal.TryParse(rawAmount.Replace("$", "").Replace(",", ""), out var amount))
                        continue;

                    if (amount == 0) continue;

                    result.Add(new DriposSaleRow
                    {
                        Date = new DateTime(headerYear, date.Month, date.Day),
                        Amount = amount
                    });
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
            }

            return result;
        }

        private async Task<Account> GetOrCreateAccountAsync(string name, string type)
        {
            var accounts = await _accountRepo.GetAllAsync();
            var account = accounts.FirstOrDefault(a => a.AccountName == name);
            if (account != null) return account;

            account = new Account { AccountName = name, AccountType = type, IsActive = true };
            await _accountRepo.AddAsync(account);
            return account;
        }

        public async Task SaveToDatabaseAsync(IEnumerable<DriposSaleRow> rows)
        {
            foreach (var row in rows)
            {
                var entry = new DriposDailySale
                {
                    SaleDate = row.Date,
                    Amount = row.Amount,
                    Source = "Dripos"
                };

                try
                {
                    using var conn = DatabaseHelper.GetOpenConnection();
                    await conn.ExecuteAsync(
                        "INSERT INTO dripos_daily_sales (sale_date, amount, source) VALUES (@SaleDate, @Amount, @Source)",
                        entry);
                }
                catch (Exception ex)
                {
                    ApplicationLogger.Log(ex, "System");
                }
            }
        }


    }
}
