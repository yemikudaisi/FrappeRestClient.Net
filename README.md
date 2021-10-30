# Frappe.Net

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

.Net REST client for [Frappe Framework](https://frappeframework.com/)

## Basic Usage

### Create the Client

```cs
using Frappe.Net

var frappe = new Frappe("https://base-url.com/");
```

### Debug

When the debug mode is on, Frappe.Net logs all requests in debug console

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

The methods implemented corellates to RESTful requests that are mapped to the `/api/resource` in Frappe. Also, some ```frappe.client``` functions are implemented here.

#### Listing Documents

To get a list of records of a DocType us ```Frappe.Db.GetListAsync()```

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
    orderBy: "modified desc"
);

foreach ( var t in todos) {
    console.WriteLine($"{t.name} -> {t.description} : {t.status}");
}
```

By default Frappe will return 20 records and will only fetch the name of the records unless fields  supplied. 

#### Get Count

#### Get Single Document

#### Get Single Value from Document

#### Get Single Value from Single-Type Document

#### Set a Single Value in a Document

#### Insert Document

#### Insert Many Document

#### Update (save) an existing Document
