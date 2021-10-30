using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
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
            var userList = await frappe.Db.GetListAsync("ToDo");
            Assert.IsTrue(((int)userList.Count > 1 ? true : false));
        }

        [TestMethod]
        public async Task TestGetCountAsync()
        {
            var frappe = new Frappe(BASE_URL);
            await frappe.UseTokenAsync("ff38e60e1ef4c35", "346923835a2f4e0");
            var userList = await frappe.Db.GetListAsync("ToDo");
            var count = await frappe.Db.GetCountAsync("ToDo");
            Assert.AreEqual(userList.Count, count);
        }

        [TestMethod]
        public async Task TestGetAysnc() {
            
            var frappe = new Frappe(BASE_URL);
            await frappe.UsePasswordAsync("administrator", "cyberm");
            var doc = await frappe.Db.GetAsync("ToDo", "c31b510a68");
            Assert.AreEqual(doc.description.ToObject<String>(), "test doctype");

        }

        [TestMethod]
        public async Task TestGetSingleValueAsync()
        {
            var frappe = new Frappe(BASE_URL);
            var value = await frappe.UsePasswordAsync("administrator", "cyberm").Result
                .Db.GetSingleValueAysnc("Website Settings", "website_theme");
            Assert.AreEqual(value.ToObject<String>(), "Standard");
        }

        [TestMethod]
        public async Task TestInsertAsync()
        {
            var frappe = new Frappe(BASE_URL, true);
            var doc = await frappe.UsePasswordAsync("administrator", "cyberm").Result
                .Db.InsertAsync(new Dictionary<string, object> {
                    { "doctype", "ToDo"},
                    { "description","inserted dictionary"}
                });
            Assert.AreEqual(doc.description.ToObject<String>(), "inserted dictionary");
        }
    }
}
