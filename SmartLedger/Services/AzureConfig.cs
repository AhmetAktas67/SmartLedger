using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedger.Services
{
    public static class AzureConfig
    {
        private static IConfiguration _config;

        static AzureConfig()
        {
            _config = new ConfigurationBuilder()
                .AddUserSecrets<SmartLedgerDbContext>() 
                .Build();
        }

        public static string DocIntelEndpoint => _config["AzureDocIntel:Endpoint"];
        public static string DocIntelKey => _config["AzureDocIntel:Key"];
    }
}
