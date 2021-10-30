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

            try {
                var frappe = new Frappe(baseUrl);
                await frappe.UseTokenAsync(apiKey, apiSecret);
                string[] fields = { 
                    "name", 
                    "description",
                    "status"
                };
                
                string[,] filters = { 
                    { 
                        "status", "=", "Open" 
                    } 
                };

                var count = await frappe.Db.GetCountAsync("ToDo");
                console.WriteLine($"{count}");

                var todos = await frappe.Db.GetListAsync(
                    "ToDo", 
                    fields:fields, 
                    filters:filters,
                    orderBy: "modified desc"
                );

                foreach ( var t in todos) {
                    console.WriteLine($"{t.name} -> {t.description} : {t.status}");
                }
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
