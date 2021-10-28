using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Frappe.Net.Test
{
    [TestClass]
    public class DbTest
    {
        const string BASE_URL = "http://172.18.245.101:8000";

        [TestMethod]
        public async Task TestIfGetListAsync()
        {
            var frappe = new Frappe(BASE_URL);
            await frappe.UseTokenAsync("ff38e60e1ef4c35", "346923835a2f4e0");
            await frappe.Db.GetListAsync("ToDo");
            Assert.IsTrue(false);
        }
    }
}
