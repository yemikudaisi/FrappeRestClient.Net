using log4net;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tiny.RestClient;

namespace Frappe.Net
{
    public class Db : JsonObjectParser
    {
        private const string RESOURCE_PATH = "resource/";
        private static readonly ILog log = LogManager.GetLogger(typeof(Db));
        TinyRestClient client;

        public Db(TinyRestClient client)
        {
            this.client = client;
        }

        public async Task<dynamic> GetListAsync(string doctype, string[] fields = null, string[,] filters = null, string orderBy= null, int limit_start= 0, int limitPageLength = 20, string parent = null, bool debug= false, bool asDict= true)
        {
            //var text = JsonConvert.SerializeObject(filters);
            var request = client.GetRequest(getUri(doctype));

            if (fields != null)
                request.AddQueryParameter("fields", JsonConvert.SerializeObject(fields));
                
            var response = await request.ExecuteAsStringAsync();
            return ToObject(response).data;
        }



        private string getUri(string resourceName)
        {
            return RESOURCE_PATH + resourceName;
        }
    }
}
