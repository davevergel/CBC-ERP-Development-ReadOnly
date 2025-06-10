using Microsoft.Extensions.Configuration;

namespace CbcRoastersErp.Helpers
{
    public static class ConfigHelper
    {
        public static IConfiguration Configuration { get; private set; }

        public static void Initialize(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}

