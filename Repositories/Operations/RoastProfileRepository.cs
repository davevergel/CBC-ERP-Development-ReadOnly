using Dapper;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models.Production;
using CbcRoastersErp.Repositories;

public class RoastProfileRepository
{
    public int InsertRoastProfile(RoastProfile profile)
    {
        try
        {
            using var conn = DatabaseHelper.GetOpenConnection();
            string sql = @"INSERT INTO roast_profiles (roast_date, bean_type, profile_file_path, notes)
                       VALUES (@RoastDate, @BeanType, @ProfileFilePath, @Notes);
                       SELECT LAST_INSERT_ID();";
            return conn.ExecuteScalar<int>(sql, profile);
        }
        catch (Exception ex)
        {
            ApplicationLogger.Log(ex, nameof(InsertRoastProfile), nameof(RoastProfileRepository), Environment.UserName);
            return -1; // Indicate failure
        }
    }

    public void InsertRoastDataPoints(IEnumerable<RoastDataPoint> points)
    {
        try
        {
            using var conn = DatabaseHelper.GetOpenConnection();
            string sql = @"INSERT INTO roast_data_points (roast_profile_id, time_seconds, bean_temp, environment_temp, ror)
                       VALUES (@RoastProfileId, @TimeSeconds, @BeanTemp, @EnvironmentTemp, @ROR);";
            conn.Execute(sql, points);
        }
        catch (Exception ex)
        {
            ApplicationLogger.Log(ex, nameof(InsertRoastDataPoints), nameof(RoastProfileRepository), Environment.UserName); 
        }
    }

    public List<RoastProfile> GetAllProfiles()
    {
        try
        {
            using var conn = DatabaseHelper.GetOpenConnection();
            return conn.Query<RoastProfile>("SELECT id, roast_date as RoastDate, bean_type as BeanType, profile_file_path as ProfileFilePath, notes as Notes FROM roast_profiles ORDER BY roast_date DESC").ToList();
        }
        catch (Exception ex)
        {
            ApplicationLogger.Log(ex, nameof(GetAllProfiles), nameof(RoastProfileRepository), Environment.UserName);
            return new List<RoastProfile>();
        }
    }

    public List<RoastDataPoint> GetDataPointsByProfileId(int profileId)
    {
        try
        {
            using var conn = DatabaseHelper.GetOpenConnection();
            return conn.Query<RoastDataPoint>("SELECT id, roast_profile_id as RoastProfileId, time_seconds as TimeSeconds, bean_temp as BeanTemp, environment_temp as EnvironmentTemp, ror as ROR FROM roast_data_points WHERE roast_profile_id = @Id", new { Id = profileId }).ToList();
        }
        catch (Exception ex)
        {
            ApplicationLogger.Log(ex, nameof(GetDataPointsByProfileId), nameof(RoastProfileRepository), Environment.UserName);
            return new List<RoastDataPoint>();
        }
    }

    public void DeleteRoastProfile(int profileId)
    {
        try
        {
            using var conn = DatabaseHelper.GetOpenConnection();
            conn.Execute("DELETE FROM roast_data_points WHERE roast_profile_id = @Id", new { Id = profileId });
            conn.Execute("DELETE FROM roast_profiles WHERE id = @Id", new { Id = profileId });
        }
        catch (Exception ex)
        {
            ApplicationLogger.Log(ex, nameof(DeleteRoastProfile), nameof(RoastProfileRepository), Environment.UserName);
        }
    }
}

