# FrappeRestClient.Net
[![NuGet](https://img.shields.io/nuget/v/FrappeRestClient.Net.svg?label=NuGet)](https://www.nuget.org/packages/FrappeRestClient.Net/)
[![Build status](https://ci.appveyor.com/api/projects/status/9w5vjt7yq2cpbo1u/branch/main?svg=true)](https://ci.appveyor.com/project/yemikudaisi/frapperestclient-net/branch/main)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

.Net REST client for [Frappe Framework](https://frappeframework.com/)

## Basic Usage

### Create the Client

```cs
using Frappe.Net

var frappe = new Frappe("https://base-url.com/");
```

### Test Connection to Frappe Site

To ping a Frappe Site use the ```PingAsync``` method.

```cs
...
var res = await frappe.PingAsync()
Console.WriteLine(res) // pong
```

### Debug

When the debug mode is on, Frappe.Net logs all HTTP requests in debug console

```cs
var frappe = new Frappe("https://base-url.com/", true);
```

### Authentication

Frappe.Net supports all [Frappe Framework](https://frappeframework.com/) [REST](https://frappeframework.com/docs/user/en/api/rest) authentication methods. It attempts to validate credentials once supplied, as such all athentication functions are asynchronous. All authentication methods support fluent style coding.

#### 1. Token Based Authentication

```cs
frappe.UseTokenAync("api-key", "api-secret");
```

#### 2. Password Based 


```cs
frappe.UseTokenAync("email-or-username", "password");
```

#### 3. Access Token 

```cs
frappe.UseAccessTokenAync("oauth-access-token");
```

### Get Logged In User

```cs
var user = await frappe.UseTokenAync("api-key", "api-secret")
	.GetLoggedUserAsync()

Console.WriteLine(user); // administrator
```

### DB Funcitons

The methods implemented corellates to RESTful requests that are mapped to the `/api/resource` in Frappe. Also, some other ```frappe.client``` APImethods are implemented here.

#### Listing Documents

To get a list of records of a DocType use ```Frappe.Db.GetListAsync()```

```cs
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

var todos = await frappe.Db.GetListAsync(
    "ToDo", 
    fields:fields, 
    filters:filters,
    orderBy: "modified desc",
    limitStart: 10,
    limitPageLenght: 30,
);

foreach ( var t in todos) {
    console.WriteLine($"{t.name} -> {t.description} : {t.status}");
}
```

By default Frappe will return 20 records and will only fetch the name of the records unless fields  supplied. 

#### Get Count

```cs
string[,] filters = { 
    { 
        "status", "=", "Closed" 
    } 
};
int count = frappe.Db.GetCount("ToDo"); // count all close ToDo
```

#### Get Single Document

To get a document with a document name use the ```GetAsync``` method.
```cs
...
var doc = await frappe.Db.GetAsync("ToDo", "340a5acab3");
Console.WriteLine(doc.name); // 340a5acab3
```

This method will throw a ```KeyNotFoundException``` if the document for the suplied name is not found.

#### Get a Value from Document

```cs
...
string[,] filter = { { "name", "=", "bafc4c81fe" } };
var value = await frappe.Db.GetValueAsync("ToDo", "description", filter);
Console.WriteLine(value) // Some ToDo description
```

#### Get Single Value from Single-Type Document

```cs
...
var value = await frappe.Db.GetSingleValueAsync("Website Settings", "website_theme");
Console.WriteLine(doc.name) // Standard
```

#### Set a Single Value in a Document

```cs
...
// returns the updated document as a ```dynamic``` object
await frappe.Db.SetValueAsync("ToDo", doc.name.ToObject<string>(), "description", data);
```
 
#### Insert Document

```cs
...
var doc = await frappe.Db.InsertAsync(
    new Dictionary<string, object> {
        { "doctype", "ToDo"},
        { "description", desc}
    }
);
Console.WriteLine(doc.description.ToString()); // desc
```

#### Insert Many Document

```cs
...
Dictionary<string, object>[] manyDocs = {
    new Dictionary<string, object> {
        { "doctype", "ToDo"},
        { "description","Description 1"}
    },
    new Dictionary<string, object> {
        { "doctype", "ToDo"},
        { "description", "Description 2"}
    }
};
var docs = await frappe.Db.InsertManyAsync(manyDocs);
Console.WriteLine((int)docs.Count); // 2
```

#### Update (save) an existing Document

```cs
var doc = await Frappe.Db.GetAsync("ToDo", "xxxxxx");
doc.description = "new description";
// Note that the document received from Get will not contain a
// ```doctype``` property for  hence the need to add it before save
doc.doctype = "ToDo"; 
await Frappe.Db.SaveAsync(doc);
var updateDoc = await Frappe.Db.GetAsync("ToDo", doc.name.ToString());
```
