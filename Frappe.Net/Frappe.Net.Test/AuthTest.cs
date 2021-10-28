using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Frappe.Net.Test
{
    [TestClass]
    public class AuthTest
    {
        const string BASE_URL = "http://172.18.245.101:8000";
        [TestMethod]
        public async Task TestIfWrongLoginReturnsExceptionAsync()
        {
            bool exceptionThrowed = false;
            try
            {
                var frappe = new Frappe(BASE_URL);
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
            var frappe = new Frappe(BASE_URL);
            await frappe.UsePasswordAsync("user@domail.com", "user@domail");
            var user = await frappe.GetLoggedUserAsync();
            Assert.AreEqual("user@domail.com", user);
        }

        [TestMethod]
        public async Task TestIfWrongTokenThrowsExceptionAsync()
        {
            bool exceptionThrowed = false;
            try
            {
                var frappe = new Frappe(BASE_URL);
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
            var frappe = new Frappe(BASE_URL);
            await frappe.UseTokenAsync("ff38e60e1ef4c35", "346923835a2f4e0");
            var user = await frappe.GetLoggedUserAsync();
            Assert.AreEqual("yemikudaisi@gmail.com", user);
        }

        [TestMethod]
        public void TestIfWrongAccessTokenThrowsExceptionAsync()
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void TestIfCorrectAccessTokenAsync()
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public async Task TestIsLoggedOutAsync() {
            var frappe = new Frappe(BASE_URL);
            await frappe.UseTokenAsync("ff38e60e1ef4c35", "346923835a2f4e0");
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
            Assert.IsTrue(exceptionThrown, $"An {nameof(Tiny.RestClient.HttpException)} must be thrown when {nameof(Frappe)} is logged out");
        }
    }
}
