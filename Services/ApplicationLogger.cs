using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using CbcRoastersErp.Models;
using Dapper;
using MySqlConnector;

namespace CbcRoastersErp.Helpers
{
    public static class ApplicationLogger
    {
        private const string LogFilePath = "ApplicationLog.txt";

        public static void Log(Exception ex, string userName = "Unknown",
                               string severity = "Error",
                               [CallerMemberName] string method = "",
                               [CallerFilePath] string source = "")
        {
            try
            {
                using var connection = DatabaseHelper.GetOpenConnection();
                string query = @"
                    INSERT INTO ApplicationErrorLogs 
                    (Timestamp, Source, Method, Message, StackTrace, UserName, Severity) 
                    VALUES (@Timestamp, @Source, @Method, @Message, @StackTrace, @UserName, @Severity);";

                connection.Execute(query, new
                {
                    Timestamp = DateTime.Now,
                    Source = Path.GetFileName(source),
                    Method = method,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    UserName = userName,
                    Severity = severity
                });
            }
            catch (Exception fallbackEx)
            {
                LogToFile($"EXCEPTION: {ex.Message},STACK: {ex.StackTrace}", userName, severity, method, source, fallbackEx);
            }
        }

        public static void LogInfo(string message, string userName = "System",
                                   string severity = "Info",
                                   [CallerMemberName] string method = "",
                                   [CallerFilePath] string source = "")
        {
            try
            {
                using var connection = DatabaseHelper.GetOpenConnection();
                string query = @"
                    INSERT INTO ApplicationErrorLogs 
                    (Timestamp, Source, Method, Message, StackTrace, UserName, Severity) 
                    VALUES (@Timestamp, @Source, @Method, @Message, '', @UserName, @Severity);";

                connection.Execute(query, new
                {
                    Timestamp = DateTime.Now,
                    Source = Path.GetFileName(source),
                    Method = method,
                    Message = message,
                    UserName = userName,
                    Severity = severity
                });
            }
            catch (Exception fallbackEx)
            {
                LogToFile($"INFO: {message}", userName, severity, method, source, fallbackEx);
            }
        }

        public static void LogWarning(string message, string userName = "System",
                           string severity = "Warning",
                           [CallerMemberName] string method = "",
                           [CallerFilePath] string source = "")
        {
            try
            {
                using var connection = DatabaseHelper.GetOpenConnection();
                string query = @"
                    INSERT INTO ApplicationErrorLogs 
                    (Timestamp, Source, Method, Message, StackTrace, UserName, Severity) 
                    VALUES (@Timestamp, @Source, @Method, @Message, '', @UserName, @Severity);";

                connection.Execute(query, new
                {
                    Timestamp = DateTime.Now,
                    Source = Path.GetFileName(source),
                    Method = method,
                    Message = message,
                    UserName = userName,
                    Severity = severity
                });
            }
            catch (Exception fallbackEx)
            {
                LogToFile($"Warning: {message}", userName, severity, method, source, fallbackEx);
            }
        }

        private static void LogToFile(string message, string userName, string severity, string method, string source, Exception dbFailException)
        {
            try
            {
                var logText = $@"
[{DateTime.Now}]
Severity: {severity}
User: {userName}
Source: {Path.GetFileName(source)}
Method: {method}
Message: {message}
DB Logging Failure: {dbFailException.Message}
--------------------------
";
                File.AppendAllText(LogFilePath, logText);
            }
            catch
            {
                // Last-resort failure – do nothing
            }
        }
    }
}
