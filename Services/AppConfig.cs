using System.IO;
using Newtonsoft.Json.Linq;

namespace CbcRoastersErp.Services
{
    public static class AppConfig
    {
        public static string GetConnectionString()
        {
            try
            {
                string json = File.ReadAllText("appsettings.json");
                var jObject = JObject.Parse(json);
                return jObject["ConnectionStrings"]["DefaultConnection"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading database connection string: " + ex.Message);
            }
        }
    }
}
