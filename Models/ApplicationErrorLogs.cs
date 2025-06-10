using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models
{
    public class ApplicationErrorLogs
    {
        public int LogId { get; set; } // Primary Key
        public DateTime Timestamp { get; set; } // When the error occurred
        public string Source { get; set; } // The source of the error (e.g., method name, class name)
        public string Method { get; set; } // The method where the error occurred
        public string Message { get; set; } // Error message
        public string StackTrace { get; set; } // Stack trace of the error
        public string UserName { get; set; } // The username of the user who encountered the error
        public enum Severity
        {
            info,
            Warning,
            Error,
            Critical
        }

    }
}
