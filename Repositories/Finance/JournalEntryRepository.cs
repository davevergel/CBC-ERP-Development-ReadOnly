using System.Data;
using CbcRoastersErp.Models.Finance;
using CbcRoastersErp.Helpers;
using Dapper;
using System.Collections.ObjectModel;

namespace CbcRoastersErp.Repositories.Finance
{
    public class JournalEntryRepository : IJournalEntryRepository
    {
        private readonly IDbConnection _db;

        public JournalEntryRepository()
        {
            _db = DatabaseHelper.GetConnection();
        }

        public async Task<IEnumerable<JournalEntry>> GetAllAsync() =>
            await _db.QueryAsync<JournalEntry>("SELECT * FROM JournalEntries ORDER BY EntryDate DESC");

        public async Task<JournalEntry> GetByIdAsync(int id)
        {
            try
            {
                const string sql = @"SELECT * FROM JournalEntries WHERE JournalEntryID = @id;
                                 SELECT * FROM JournalEntryLines WHERE JournalEntryID = @id;";
                using var multi = await _db.QueryMultipleAsync(sql, new { id });

                var entry = await multi.ReadFirstOrDefaultAsync<JournalEntry>();
                if (entry != null)
                    entry.Lines = new ObservableCollection<JournalEntryLine>(await multi.ReadAsync<JournalEntryLine>());
                return entry;
            }

            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                ApplicationLogger.Log(ex, "Error retrieving journal entry by ID");
                return null;
            }
        }

        public async Task AddAsync(JournalEntry entry, IEnumerable<JournalEntryLine> lines)
        {
            const string insertHeader = @"INSERT INTO JournalEntries (EntryDate, Description)
                                          VALUES (@EntryDate, @Description);
                                          SELECT LAST_INSERT_ID();";
            var entryId = await _db.ExecuteScalarAsync<int>(insertHeader, entry);

            foreach (var line in lines)
            {
                line.JournalEntryID = entryId;
            }

            const string insertLines = @"INSERT INTO JournalEntryLines 
                                         (JournalEntryID, AccountID, Amount, IsDebit)
                                         VALUES (@JournalEntryID, @AccountID, @Amount, @IsDebit)";
            await _db.ExecuteAsync(insertLines, lines);
        }

        public async Task DeleteAsync(int id)
        {
            await _db.ExecuteAsync("DELETE FROM JournalEntryLines WHERE JournalEntryID = @id", new { id });
            await _db.ExecuteAsync("DELETE FROM JournalEntries WHERE JournalEntryID = @id", new { id });
        }
    }
}
