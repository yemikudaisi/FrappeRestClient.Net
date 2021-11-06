using FrappeRestClient.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Frappe.Net.Test
{
    [TestClass]
    public class FrappeTest
    {
        IConfiguration config;

        public FrappeTest()
        {
            config =
                new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false)
                    .Build();
        }

        [TestMethod]
        public async Task TestIfWrongLoginReturnsExceptionAsync()
        {
            bool exceptionThrowed = false;
            try
            {
                var frappe = new FrappeRestClient(config["baseUrl"]);
                await frappe.UsePasswordAsync("wrong", "details");
            }
            catch (AuthenticationException)
            {
                exceptionThrowed = true;
            }
            Assert.IsTrue(exceptionThrowed, $"An {nameof(AuthenticationException)} must be thrown when wrong login credentials are supplied");
        }

        [TestMethod]
        public async Task TestIfCorrectLoginAsync()
        {
            var frappe = new FrappeRestClient(config["baseUrl"]);
            await frappe.UsePasswordAsync(config["regularUser"], config["regularPassword"]);
            var user = await frappe.GetLoggedUserAsync();
            Assert.AreEqual(config["regularUser"], user);
        }

        [TestMethod]
        public async Task TestIfCorrectLoginCookiesAsync()
        {
            var frappe = new FrappeRestClient(config["baseUrl"]);
            var cookies = await frappe.UsePasswordAsync(config["regularUser"], config["regularPassword"]);
            Assert.AreEqual(config["regularUser"], cookies[Cookies.FieldNames.UserId]);
        }

        [TestMethod]
        public async Task TestIfWrongTokenThrowsExceptionAsync()
        {
            bool exceptionThrowed = false;
            try
            {
                var frappe = new FrappeRestClient(config["baseUrl"]);
                await frappe.UseTokenAsync("secret", "key");
            }
            catch (AuthenticationException)
            {
                exceptionThrowed = true;
            }
            Assert.IsTrue(exceptionThrowed, $"An {nameof(AuthenticationException)} must be thrown");
        }

        [TestMethod]
        public async Task TestIfCorrectTokenAsync()
        {
            var frappe = new FrappeRestClient(config["baseUrl"]);
            await frappe.UseTokenAsync(config["apiKey"], config["apiSecret"]);
            var user = await frappe.GetLoggedUserAsync();
            Assert.AreEqual("Administrator", user);
        }

        [TestMethod]
        public async Task TestIsLoggedOutAsync()
        {
            var frappe = new FrappeRestClient(config["baseUrl"]);
            await frappe.UseTokenAsync(config["apiKey"], config["apiSecret"]);
            frappe.Logout();

            bool exceptionThrown = false;
            try
            {
                var user = await frappe.GetLoggedUserAsync();

            }
            catch (Tiny.RestClient.HttpException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, $"An {nameof(Tiny.RestClient.HttpException)} must be thrown when {nameof(FrappeRestClient)} is logged out");
        }

        [TestMethod]
        public async Task TestPing()
        {
            var frappe = new FrappeRestClient(config["baseUrl"]);
            var response = await frappe.PingAsync();
            Assert.AreEqual("pong", response);
        }
    }
}
