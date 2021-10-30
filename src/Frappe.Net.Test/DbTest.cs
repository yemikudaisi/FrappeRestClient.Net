using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Frappe.Net.Test
{
    [TestClass]
    public class DbTest
    {
        IConfiguration config;

        public DbTest()
        {
            config =
                new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false)
                    .Build();
        }

        [TestMethod]
        public async Task TestIfGetListAsync()
        {
            var frappe = new Frappe(config["baseUrl"]);
            await frappe.UseTokenAsync(config["apiKey"], config["apiSecret"]);
            var userList = await frappe.Db.GetListAsync("ToDo");
            Assert.IsTrue(((int)userList.Count > 1 ? true : false));
        }

        [TestMethod]
        public async Task TestGetCountAsync()
        {
            var frappe = new Frappe(config["baseUrl"], true);
            await frappe.UseTokenAsync(config["apiKey"], config["apiSecret"]);
            var userList = await frappe.Db.GetListAsync("ToDo");
            var count = await frappe.Db.GetCountAsync("ToDo");
            Assert.AreEqual(userList.Count, count);
        }

        [TestMethod]
        public async Task TestGetAysnc() {
            
            var frappe = new Frappe(config["baseUrl"]);
            await frappe.UsePasswordAsync(config["adminUser"], config["adminPassword"]);
            var doc = await frappe.Db.GetAsync("ToDo", "c31b510a68");
            Assert.AreEqual(doc.description.ToObject<String>(), "test doctype");

        }

        [TestMethod]
        public async Task TestGetSingleValueAsync()
        {
            var frappe = new Frappe(config["baseUrl"]);
            var value = await frappe.UsePasswordAsync(config["adminUser"], config["adminPassword"]).Result
                .Db.GetSingleValueAysnc("Website Settings", "website_theme");
            Assert.AreEqual(value.ToObject<String>(), "Standard");
        }

        [TestMethod]
        public async Task TestInsertAsync()
        {
            var desc = "more inserted dictionary";
            var frappe = new Frappe(config["baseUrl"], true);
            var doc = await frappe.UsePasswordAsync(config["adminUser"], config["adminPassword"]).Result
                .Db.InsertAsync(new Dictionary<string, object> {
                    { "doctype", "ToDo"},
                    { "description",desc}
                });
            Assert.AreEqual(doc.description.ToObject<String>(), desc);
        }

        [TestMethod]
        public async Task TestInsertManyAsync()
        {
            var frappe = new Frappe(config["baseUrl"], true);
            Dictionary<string, object>[] manyDocs = {
                new Dictionary<string, object> {
                    { "doctype", "ToDo"},
                    { "description","First insert"}
                },
                new Dictionary<string, object> {
                    { "doctype", "ToDo"},
                    { "description","second insert"}
                }
            };
            var docs = await frappe.UsePasswordAsync(config["adminUser"], config["adminPassword"]).Result
                .Db.InsertManyAsync(manyDocs);
            Assert.AreEqual((int)docs.Count, 2);
        }

        [TestMethod]
        public async Task TestSaveAsync()
        {
            var frappe = new Frappe(config["baseUrl"], true);
            string[] fields = { "name", "description" };

            var data = Guid.NewGuid().ToString();
            var list = await frappe.UsePasswordAsync(config["adminUser"], config["adminPassword"]).Result
                .Db.GetListAsync(
                "ToDo",
                fields:fields,
                limitPageLength: 1);
            var doc = list[0];
            doc.description = data;
            var name = doc.name;
            doc.doctype = "ToDo";
            await frappe.Db.SaveAsync(doc);
            var updateDoc = await frappe.Db.GetAsync("ToDo", name.ToObject<String>());

            Assert.AreEqual(data, updateDoc.description.ToObject<String>());
        }
    }
}
