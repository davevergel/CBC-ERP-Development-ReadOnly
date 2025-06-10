using System;
using System.Collections.ObjectModel;

namespace CbcRoastersErp.Models.Finance
{
    public class JournalEntry
    {
        public int JournalEntryID { get; set; }
        public DateTime EntryDate { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public ObservableCollection<JournalEntryLine> Lines { get; set; } = new();
    }
}
