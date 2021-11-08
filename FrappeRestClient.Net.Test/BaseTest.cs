using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace Frappe.Net.Test
{
    public class BaseTest
    {
        protected IConfiguration config;
        private FrappeRestClient.Net.FrappeRestClient frappe;
        public BaseTest()
        {
            config =
                new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false)
                    .Build();
            frappe = new FrappeRestClient.Net.FrappeRestClient(config["baseUrl"], true)
                .SetToken(config["apiKey"], config["apiSecret"]);
        }

        public FrappeRestClient.Net.FrappeRestClient Frappe
        {
            get { return frappe; }
        }

        private static Random random = new Random();
        public string GenerateRandom(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
