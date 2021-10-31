using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Frappe.Net.Test
{
    [TestClass]
    public class DbTest : BaseTest
    {
        public DbTest()
            : base()
        {
        }

        [TestMethod]
        public async Task TestIfGetListAsync()
        {
            var userList = await Frappe.Db.GetListAsync("ToDo");
            Assert.IsTrue(((int)userList.Count > 1 ? true : false));
        }

        [TestMethod]
        public async Task TestGetCountAsync()
        {
            var userList = await Frappe.Db.GetListAsync("ToDo");
            var count = await Frappe.Db.GetCountAsync("ToDo");
            Assert.AreEqual(userList.Count, count);
        }

        [TestMethod]
        public async Task TestGetAysnc() 
        {
            var doc = await Frappe.Db.GetAsync("ToDo", "c31b510a68");
            Assert.AreEqual(doc.description.ToObject<String>(), "test doctype");
        }

        [TestMethod]
        public async Task TestGetSingleValueAsync()
        {
            var value = await Frappe.Db.GetSingleValueAysnc("Website Settings", "website_theme");
            Assert.AreEqual(value.ToObject<String>(), "Standard");
        }

        [TestMethod]
        public async Task TestSetValueAsync()
        {
            string[] fields = { "name", "description" };

            var data = Guid.NewGuid().ToString();
            var list = await Frappe.Db.GetListAsync(
                "ToDo",
                fields: fields,
                limitPageLength: 1);
            var doc = list[0];
            await Frappe.Db.SetValueAsync("ToDo",doc.name.ToObject<String>(), "description", data);
            var updateDoc = await Frappe.Db.GetAsync("ToDo", doc.name.ToObject<String>());

            Assert.AreEqual(data, updateDoc.description.ToObject<String>());
        }

        [TestMethod]
        public async Task TestInsertAsync()
        {
            var desc = $"{GenerateRandom(6)} {GenerateRandom(9)}";
            var doc = await Frappe.Db.InsertAsync(new Dictionary<string, object> {
                    { "doctype", "ToDo"},
                    { "description", desc}
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
                    { "description",$"{GenerateRandom(5)} {GenerateRandom(10)}"}
                },
                new Dictionary<string, object> {
                    { "doctype", "ToDo"},
                    { "description",$"{GenerateRandom(5)} {GenerateRandom(10)}"}
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

        [TestMethod]
        public async Task TestRenameDocAsync()
        {
            string[] fields = { "name", "description" };

            var doc = await Frappe.Db.GetAsync("User", config["testEmail"]);
            var renamedDoc = await Frappe.Db.renameDoc("ToDo", doc.name.ToObject<String>(), config["altTestEmail"]);
            Assert.AreEqual(config["altTestEmail"], renamedDoc.name.ToObject<String>());
        }
    }
}
