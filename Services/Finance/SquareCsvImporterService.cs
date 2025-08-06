
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CbcRoastersErp.Models.Finance;
using CbcRoastersErp.Repositories.Finance;

namespace CbcRoastersErp.Services.Finance
{
    public class SquareImportService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly IJournalEntryRepository _journalRepo;

        public SquareImportService(IAccountRepository accountRepo, IJournalEntryRepository journalRepo)
        {
            _accountRepo = accountRepo;
            _journalRepo = journalRepo;
        }

        public async Task<int> ImportFromCsvAsync(string filePath)
        {
            var lines = File.ReadAllLines(filePath).Skip(1); // Skip header
            int importedCount = 0;

            foreach (var line in lines)
            {
                var columns = line.Split(',');

                if (columns.Length < 8 || !columns[6].Equals("Business", StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    var date = DateTime.Parse(columns[0]);
                    var description = columns[1];
                    var amount = ParseAmount(columns[2]);
                    var category = columns[7];

                    var expenseAccount = await GetOrCreateAccountAsync(category, "Expense");
                    var bankAccount = await GetOrCreateAccountAsync("Square Business Checking", "Asset");

                    var entry = new JournalEntry
                    {
                        EntryDate = date,
                        Description = $"Square Import - {description}",
                        CreatedAt = DateTime.Now,
                        Lines = new ObservableCollection<JournalEntryLine>
                        {
                            new JournalEntryLine { AccountID = expenseAccount.AccountID, Amount = amount, IsDebit = true },
                            new JournalEntryLine { AccountID = bankAccount.AccountID, Amount = amount, IsDebit = false }
                        }
                    };

                    await _journalRepo.AddAsync(entry, entry.Lines);
                    importedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing line: {ex.Message}");
                }
            }

            return importedCount;
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

        private decimal ParseAmount(string raw)
        {
            raw = raw.Replace("(", "-").Replace(")", "").Replace("$", "").Trim();
            return decimal.Parse(raw, CultureInfo.InvariantCulture);
        }
    }
}
