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
                //var respone = await client.GetRequest("frappe.auth.get_logged_user")
                //    .ExecuteAsStringAsync();
                var frappe = new Frappe(BASE_URL);
                await frappe.UsePasswordAsync("wrong", "details");
            }
            catch (AuthenticationException e)
            {
                exceptionThrowed = true;
            }
            Assert.IsTrue(exceptionThrowed, $"An {nameof(AuthenticationException)} must be thrown");
        }

        [TestMethod]
        public async Task TestIfCorrectLoginAsync()
        {
            var frappe = new Frappe(BASE_URL);
            await frappe.UsePasswordAsync("user@domail.com", "user@domail");
            var user = await frappe.GetLoggedUserAsync();
            Assert.AreEqual("user@domail.com", user);
        }
    }
}
