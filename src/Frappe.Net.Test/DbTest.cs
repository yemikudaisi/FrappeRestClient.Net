using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            var name = "340a5acab3";
            var doc = await Frappe.Db.GetAsync("ToDo", name);
            Assert.AreEqual(doc.name.ToObject<String>(), "340a5acab3");
        }

        [TestMethod]
        public async Task TestGetThrowsExceptionWhenInvalidNameAysnc()
        {
            Type actualType = null;
            try
            {
                var doc = await Frappe.Db.GetAsync("ToDo", "xxx");
            }
            catch (Exception e)
            {
                actualType = e.GetType();
            }
            Assert.AreEqual(typeof(KeyNotFoundException), actualType);
        }

        [TestMethod]
        public async Task TestGetValueAsync()
        {
            string[,] filter = { { "name", "=", "bafc4c81fe" } };
            var value = await Frappe.Db.GetValueAsync("ToDo", "description",filter);
            Assert.AreEqual("The J07 of G50X is 8OFTOCZ", value.description.ToString());
        }

        [TestMethod]
        public async Task TestGetSingleValueAsync()
        {
            var value = await Frappe.Db.GetSingleValueAsync("Website Settings", "website_theme");
            Assert.AreEqual(value.ToObject<String>(), "Standard");
        }

        [TestMethod]
        public async Task TestSetValueAsync()
        {
            string[] fields = { "name", "description" };

            var data = $"The {GenerateRandom(3)} of {GenerateRandom(4)} is {GenerateRandom(7)}";
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
            string[] fields = { "name", "description" };

            var list = await Frappe.Db.GetListAsync(
                "ToDo",
                fields:fields,
                limitPageLength: 1);
            var doc = list[0];
            var description = $"The {GenerateRandom(3)} of {GenerateRandom(4)} is {GenerateRandom(7)}";
            doc.description = description;
            var name = doc.name.ToObject<String>();
            doc.doctype = "ToDo";
            await Frappe.Db.SaveAsync(doc);
            var updateDoc = await Frappe.Db.GetAsync("ToDo", name.ToString());

            Assert.AreEqual(description, updateDoc.description.ToObject<String>());
        }

        [TestMethod]
        public async Task TestRenameDocAsync()
        {

            string oldName = "testb@email.com";
            string newName = "testa@email.com";

            var doc = await Frappe.Db.GetAsync("User", oldName);
            var renamedDoc = await Frappe.Db.renameDoc(
                "User", 
                oldName, 
                newName
            );
            Assert.AreEqual(newName, renamedDoc);
        }

        [TestMethod]
        public async Task TestDeleteDocAsync()
        {
            string[] fields = { "name", "description" };

            var newDoc = await Frappe.Db.InsertAsync(new Dictionary<string, object> {
                    { "doctype", "ToDo"},
                    { "description", $"{GenerateRandom(5)} of {GenerateRandom(3)}"}
                });
            await Frappe.Db.DeleteAsync("ToDo", newDoc.name.ToObject<String>());

            Type actualType = null;
            try
            {
                var doc = await Frappe.Db.GetAsync("ToDo", newDoc.name.ToObject<String>());
            }
            catch (Exception e)
            {
                actualType = e.GetType();
            }
            Assert.AreEqual(typeof(KeyNotFoundException), actualType);
        }

        [TestMethod]
        public async Task TestAttachFileAsync() {
            var fileName = $"filename-{GenerateRandom(7)}.txt";
            var newFile = await Frappe.Db.AttachFileAsync(
                fileName, 
                Encoding.ASCII.GetBytes(fileName),
                "User",
                "testa@email.com");
            Assert.AreEqual(newFile.file_url.ToString(), $"/files/{fileName}");

        }
    }
}
