using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tiny.RestClient;
using console = System.Console;

namespace Frappe.Net.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            console.WriteLine("Hello World!");
            var baseUrl = "http://172.18.245.101:8000";
            var apiKey = "ff38e60e1ef4c35";
            var apiSecret = "346923835a2f4e0";
            //var client = new TinyRestClient(new HttpClient(), $"{baseUrl}/api/method");
            //client.Settings.DefaultHeaders.AddBearer($"{apiKey}:{apiSecret}");
            //client.Settings.DefaultHeaders.Add("Authorization", $"token {apiKey}:{apiSecret}");

            try {
                //var respone = await client.GetRequest("frappe.auth.get_logged_user")
                //    .ExecuteAsStringAsync();
                var frappe = new Frappe(baseUrl);
                //await frappe.UsePasswordAsync("user@domail.com", "user@domail");
                await frappe.UsePasswordAsync(apiKey, apiSecret);
                var user = await frappe.GetLoggedUserAsync();
                console.WriteLine(user);
            }
            catch (HttpException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    console.WriteLine("Not permitted");
                }
            }
            catch (Exception e)
            {
                console.WriteLine(e);
            }

            console.ReadLine();
        }            
    }
}
