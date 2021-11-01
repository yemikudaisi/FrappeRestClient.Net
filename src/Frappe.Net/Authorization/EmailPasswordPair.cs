using System;
using System.Collections.Generic;
using System.Text;

namespace Frappe.Net.Authorization
{
    /// <summary>
    /// Helper class for holding Frappe based email password pair
    /// </summary>
    public class EmailPasswordPair
    {
        /// <summary>
        /// The username or email
        /// </summary>
        public string usr { get; set; } 

        /// <summary>
        /// The user password
        /// </summary>
        public string pwd { get; set; } 
    }
}
