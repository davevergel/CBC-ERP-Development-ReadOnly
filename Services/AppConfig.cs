using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace CbcRoastersErp.Services
{
    public static class AppConfig
    {
        private static readonly IConfigurationRoot _config;

        static AppConfig()
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }

        public static string GetConnectionString()
        {
            try
            {
                return _config.GetConnectionString("DefaultConnection");
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading database connection string: " + ex.Message);
            }
        }

        public static string GetArtisanPath()
        {
            return _config["Artisan:ExecutablePath"];
        }

        public static (string Operator, string Origin, string Weight, string TemplatePath) GetArtisanDefaults()
        {
            return (
                _config["Artisan:Operator"],
                _config["Artisan:DefaultOrigin"],
                _config["Artisan:DefaultWeight"],
                _config["Artisan:TemplatePath"]
            );
        }
    }
}
