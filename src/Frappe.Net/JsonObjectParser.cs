using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frappe.Net
{
    /// <summary>
    /// Base class for classed that can convert JSOn sting to object
    /// </summary>
    public abstract class JsonObjectParser
    {
        /// <summary>
        /// Convets JSON string to object
        /// </summary>
        /// <param name="json">The string to be parsed to object</param>
        /// <returns>A dynamic object parse from JSON</returns>
        protected dynamic ToObject(string json)
        {
            return JObject.Parse(json);
        }
    }
}
