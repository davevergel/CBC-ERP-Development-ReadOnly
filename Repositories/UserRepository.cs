using System.Data;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models;
using Dapper;

namespace CbcRoastersErp.Repositories
{
    public class UserRepository
    {
        private readonly IDbConnection _db;

        public UserRepository()
        {
            _db = DatabaseHelper.GetConnection();
        }

        public UserModel GetUserByUsername(string username)
        {
            try
            {
                return _db.Query<UserModel>("SELECT * FROM UserAccounts WHERE Username = @Username", new { Username = username }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                // Log the exception using ApplicationLogger
                ApplicationLogger.Log(ex, nameof(GetUserByUsername), nameof(UserRepository), username);
                return null;
            }
        }

        public bool ValidateUser(string username, string password)
        {
            try
            {
                var user = GetUserByUsername(username);
                return user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            }
            catch (Exception ex)
            {
                // Log the exception using ApplicationLogger
                ApplicationLogger.Log(ex, nameof(ValidateUser), nameof(UserRepository), username);
                return false;
            }
        }

        public void RegisterUser(UserModel user)
        {
            try
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                _db.Execute("INSERT INTO UserAccounts (Username, Password, RoleId) VALUES (@Username, @PasswordHash, @RoleId)", user);
            }
            catch (Exception ex)
            {
                // Log the exception using ApplicationLogger
                ApplicationLogger.Log(ex, nameof(RegisterUser), nameof(UserRepository), user.Username);
                throw; // Re-throw to handle it in the calling code if needed
            }
        }

        // EmployeeProfiles CRUD Operations
        public IEnumerable<EmployeeProfiles> GetAllEmployees()
        {
            try
            {
                const string sql = "SELECT * FROM EmployeeProfiles";

                return _db.Query(sql).Select(row => new EmployeeProfiles
                {
                    EmployeeID = row.EmployeeID,
                    FullName = row.FullName,
                    Position = row.Position,
                    Email = row.Email,
                    PhoneNumber = row.PhoneNumber,
                    DateHired = row.DateHired is MySqlConnector.MySqlDateTime dt && dt.IsValidDateTime ? dt.GetDateTime() : null
                });
            }
            catch (Exception ex)
            {
                // Log the exception using ApplicationLogger
                ApplicationLogger.Log(ex, nameof(GetAllEmployees), nameof(UserRepository));
                return Enumerable.Empty<EmployeeProfiles>();
            }
        }

        public void AddEmployee(EmployeeProfiles emp)
        {
            try
            {
                string sql = "INSERT INTO EmployeeProfiles (FullName, Position, Email, PhoneNumber, DateHired) VALUES (@FullName, @Position, @Email, @PhoneNumber, @DateHired)";
                _db.Execute(sql, emp);
            }
            catch (Exception ex)
            {
                // Log the exception using ApplicationLogger
                ApplicationLogger.Log(ex, nameof(AddEmployee), nameof(UserRepository), emp.FullName);
                throw; // Re-throw to handle it in the calling code if needed
            }
        }

        public void UpdateEmployee(EmployeeProfiles emp)
        {
            try
            {
            string sql = "UPDATE EmployeeProfiles SET FullName=@FullName, Position=@Position, Email=@Email, PhoneNumber=@PhoneNumber, DateHired=@DateHired WHERE EmployeeID=@EmployeeID";
            _db.Execute(sql, emp);
            }
            catch (Exception ex)
            {
                // Log the exception using ApplicationLogger
                ApplicationLogger.Log(ex, nameof(UpdateEmployee), nameof(UserRepository), emp.FullName);
                throw; // Re-throw to handle it in the calling code if needed
            }
        }

        public void DeleteEmployee(int employeeId)
        {
            _db.Execute("DELETE FROM EmployeeProfiles WHERE EmployeeID = @EmployeeID", new { EmployeeID = employeeId });
        }

        // WorkSchedule CRUD Operations

        public IEnumerable<WorkSchedules> GetAllSchedules()
        {
            const string sql = @"
        SELECT s.*, e.FullName AS EmployeeName
        FROM WorkSchedules s
        LEFT JOIN EmployeeProfiles e ON s.EmployeeID = e.EmployeeID";

            return _db.Query(sql).Select(row => new WorkSchedules
            {
                ScheduleID = row.ScheduleID,
                EmployeeID = row.EmployeeID,
                TaskDescription = row.TaskDescription,
                TotalHours = row.TotalHours,
                BreakDuration = row.BreakDuration,
                Status = row.Status,
                StartDate = row.StartDate is MySqlConnector.MySqlDateTime sd && sd.IsValidDateTime ? sd.GetDateTime() : null,
                StartTime = row.StartTime,
                EndDate = row.EndDate is MySqlConnector.MySqlDateTime ed && ed.IsValidDateTime ? ed.GetDateTime() : null,
                EndTime = row.EndTime,
                EmployeeFullName = row.EmployeeName
            });

        }

        public IEnumerable<WorkSchedules> GetAllSchedulesWithEmployeeNames()
        {
            return GetAllSchedules();
        }

        public void AddSchedule(WorkSchedules sched)
        {
            string sql = @"INSERT INTO WorkSchedules (EmployeeID, TaskDescription, TotalHours, BreakDuration, Status, StartDate, StartTime, EndDate, EndTime)
                   VALUES (@EmployeeID, @TaskDescription, @TotalHours, @BreakDuration, @Status, @StartDate, @StartTime, @EndDate, @EndTime)";
            _db.Execute(sql, sched);
        }

        public void UpdateSchedule(WorkSchedules sched)
        {
            string sql = @"UPDATE WorkSchedules SET TaskDescription=@TaskDescription, TotalHours=@TotalHours, BreakDuration=@BreakDuration,
                   Status=@Status, StartDate=@StartDate, StartTime=@StartTime, EndDate=@EndDate, EndTime=@EndTime
                   WHERE ScheduleID=@ScheduleID";
            _db.Execute(sql, sched);
        }

        public void DeleteWorkSchedulesh(int scheduleID)
        {
            _db.Execute("DELETE FROM WorkSchedules WHERE ScheduleID = @ScheduleID", new { ScheduleID = scheduleID });
        }

        public void DeleteSchedule(int scheduleId)
        {
            _db.Execute("DELETE FROM WorkSchedules WHERE ScheduleID = @ScheduleID", new { ScheduleID = scheduleId });
        }

        // UserAccounts CRUD Operations
        public IEnumerable<UserAccount> GetAllUsers()
        {
            const string sql = @"SELECT users.*
                                , roles.RoleName
                                 FROM UserAccounts as users
                                 lEFT JOIN Roles roles on users.Roleid = roles.RoleID";

            return _db.Query(sql).Select(row => new UserAccount
            {
                UserId = row.UserId,
                Username = row.Username,
                PasswordHash = row.PasswordHash,
                RoleId = row.RoleId,
                EmployeeId = row.EmployeeId,
                RoleName = row.RoleName, // Assuming RoleName is included in the query
                IsActive = row.IsActive ?? "Yes", // Default to "Yes" if null
                DateLastLogin = row.DateLastLogin is MySqlConnector.MySqlDateTime dt && dt.IsValidDateTime ? dt.GetDateTime() : null,
                DateCreated = row.DateCreated is MySqlConnector.MySqlDateTime dtc && dtc.IsValidDateTime ? dtc.GetDateTime() : null
            });
        }

        public void AddUser(UserAccount user, string plainPassword)
        {
            if (string.IsNullOrEmpty(plainPassword))
                throw new ArgumentNullException(nameof(plainPassword), "Password cannot be null or empty.");

            string hash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            string sql = @"INSERT INTO UserAccounts (Username, PasswordHash, RoleId, EmployeeId, DateCreated)
                   VALUES (@Username, @PasswordHash, @RoleId, @EmployeeId, CURDATE())";
            _db.Execute(sql, new { user.Username, PasswordHash = hash, user.RoleId, user.EmployeeId });
        }

        public void UpdateUser(UserAccount user)
        {
            string sql = @"UPDATE UserAccounts SET Username=@Username, RoleId=@RoleId, EmployeeId=@EmployeeId
                   WHERE UserId=@UserId";
            _db.Execute(sql, user);
        }

        public void UpdateLastLoginDate(string username)
        {
            // using var connection = DatabaseHelper.CreateConnection();
            string query = "UPDATE UserAccounts SET DateLastLogin = @DateLastLogin WHERE Username = @Username";

            _db.Execute(query, new
            {
                DateLastLogin = DateTime.Now,
                Username = username
            });

        }


        public void DeleteUser(int userId)
        {
            _db.Execute("DELETE FROM UserAccounts WHERE UserId = @UserId", new { UserId = userId });
        }

        // Roles CRUD Operations
        public IEnumerable<Role> GetAllRoles()
        {
            const string sql = @"SELECT * FROM Roles";
            return _db.Query<Role>(sql);
        }

        // Permissions CRUD Operations
        public UserModel GetUserWithPermissions(string username)
        {
            try
            {
                var user = GetUserByUsername(username);

                if (user != null)
                {
                    var permissions = _db.Query<string>(
                        @"SELECT p.PermissionName 
              FROM Permissions p
              JOIN RolePermissions rp ON p.PermissionId = rp.PermissionId
              WHERE rp.RoleId = @RoleId", new { RoleId = user.RoleId }).ToList();

                    user.Permissions = permissions;
                }

                return user;
            }
            catch (Exception ex)
            {
                // Log the exception using ApplicationLogger
                ApplicationLogger.Log(ex, nameof(GetUserWithPermissions), nameof(UserRepository), username);
                return null;
            }
        }
    }
}
