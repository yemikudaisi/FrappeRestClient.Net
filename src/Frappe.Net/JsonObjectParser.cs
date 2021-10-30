using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frappe.Net
{
    public abstract class JsonObjectParser
    {
        protected dynamic ToObject(string json)
        {
            return JObject.Parse(json);
        }
    }
}
