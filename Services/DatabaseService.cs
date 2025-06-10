using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CbcRoastersErp.Models;

namespace CbcRoastersErp.Services
{
    public class DatabaseService
    {
        private static ApplicationDbContext _context;

        public static void Initialize(IConfiguration config)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(config.GetConnectionString("DefaultConnection"),
                          ServerVersion.AutoDetect(config.GetConnectionString("DefaultConnection")))
                .Options;

            _context = new ApplicationDbContext(options);
        }

        public static ApplicationDbContext GetDbContext() => _context;
    }
}
