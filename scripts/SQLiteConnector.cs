using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Newtonsoft.Json.Linq;



public partial class SQLiteConnector : Node, ConnectorContext
{

    //private Node dbOperations;
    private GodotObject database;
    static GDScript databaseScript;

    public override void _Ready()
    {
        databaseScript = GD.Load<GDScript>("res://addons/SQLiteConnectorAdapter.gd");
        database = (GodotObject)databaseScript.New();
        if (database == null)
        {
            GD.PrintErr("Failed to create database instance.");
            return;
        }

        var databaseNode = new Node();
        databaseNode.SetScript(databaseScript);
        AddChild(databaseNode);

        //TESTING
        // await _send_new_shoppinglist_request(new string[]{"Test"}, "name", null);
        // GD.Print("_get_shoppinglists_owner_request");
        // await _get_shoppinglists_owner_request("name", null);
        // GD.Print("_get_shoppinglists_shared_request");
        // await _get_shoppinglists_shared_request("name", null);
        // GD.Print("_get_shoppinglist_request");
        // await _get_shoppinglist_request("name", null);
        // GD.Print("_get_shoppinglist_request");
        // await _get_shoppinglist_request("1", null);
        // GD.Print("_delete_shoppinglist_request");
        // await _delete_shoppinglist_request("2", null);
        // GD.Print("_patch_shoppinglist_request");
        // JObject body = new JObject
        // {
        //     { "name", "Groceries" },
        //     { "owner", "john.doe@example.com" }
        // };
        // await _patch_shoppinglist_request("3", body, null);
        // GD.Print("_send_auth_request");
        // await _send_auth_request(true, "email", "password", null);
    }

    public async Task<string> _send_new_shoppinglist_request(string[] sharedWith, string name, HttpRequest httpRequest)
    {
        GD.Print("_send_new_shoppinglist_request");
        // Call the GDScript function
        var result = (string)database.Call("_send_new_shoppinglist_request", sharedWith, name, GlobalData.Instance.Email);
        return await Task.FromResult(result);
    }


    public async Task<JArray> _get_shoppinglists_owner_request(string email, HttpRequest httpRequest)
    {
        GD.Print("_get_shoppinglists_owner_request");

        var result = (Godot.Collections.Array)database.Call("_get_shoppinglists_owner_request", email);

        if (result is Godot.Collections.Array resultArray)
        {
            var formattedJArray = ConvertShoppingListsToJArray(resultArray);
            return await Task.FromResult(formattedJArray);
        }

        return await Task.FromResult(new JArray());
    }

    private JArray ConvertShoppingListsToJArray(Godot.Collections.Array shoppingLists)
    {
        var formattedList = new JArray();

        foreach (Godot.Collections.Dictionary item in shoppingLists)
        {
            var formattedItem = new JObject
            {
                ["document"] = new JObject
                {
                    ["name"] = $"projects/gmc-c02/databases/(default)/documents/shoppingLists/{item["id"]}",
                    ["fields"] = new JObject
                    {
                        ["name"] = new JObject { ["stringValue"] = item["name"].ToString() }
                    },
                    ["createTime"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"),
                    ["updateTime"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ")
                },
                ["readTime"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ")
            };
            formattedList.Add(formattedItem);
        }

        return formattedList;
    }

    private JObject ConvertShoppingListWithItems(string shoppingList)
    {
        {
            var shoppingListObj = JObject.Parse(shoppingList);

            var itemsArray = new JArray();
            foreach (var item in shoppingListObj["items"])
            {
                var mapValue = new JObject
                {
                    ["mapValue"] = new JObject
                    {
                        ["fields"] = new JObject
                        {
                            ["quantity"] = new JObject { ["stringValue"] = item["quantity"].ToString() },
                            ["name"] = new JObject { ["stringValue"] = item["name"].ToString() }
                        }
                    }
                };
                itemsArray.Add(mapValue);
            }

            var formattedItem = new JObject
            {
                ["name"] = $"projects/gmc-c02/databases/(default)/documents/shoppingLists/{shoppingListObj["id"]}",
                ["fields"] = new JObject
                {
                    ["name"] = new JObject { ["stringValue"] = shoppingListObj["name"].ToString() },
                    ["items"] = new JObject
                    {
                        ["arrayValue"] = new JObject
                        {
                            ["values"] = itemsArray
                        }
                    },
                    ["owner"] = new JObject { ["stringValue"] = shoppingListObj["owner"].ToString() },
                    ["sharedWith"] = new JObject
                    {
                        ["arrayValue"] = new JObject
                        {
                            ["values"] = new JArray(new JObject { ["stringValue"] = "offline" }) // Adjust this as needed
                        }
                    }
                },
                ["createTime"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"),
                ["updateTime"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ")
            };

            return formattedItem;
        }
    }


    public async Task<JArray> _get_shoppinglists_shared_request(string email, HttpRequest httpRequest)
    {
        GD.Print("_get_shoppinglists_shared_request");

        var result = (Godot.Collections.Array)database.Call("_get_shoppinglists_owner_request", email);

        if (result is Godot.Collections.Array resultArray)
        {
            var formattedJArray = ConvertShoppingListsToJArray(resultArray);
            return await Task.FromResult(formattedJArray);
        }

        return await Task.FromResult(new JArray());
    }

    public async Task<string> _get_shoppinglist_request(string shoppingListId, HttpRequest httpRequest)
    {
        GD.Print("_get_shoppinglist_request");

        if (!int.TryParse(shoppingListId, out int shoppingListIdInt))
        {
            GD.PrintErr("Invalid shopping list ID");
            return "NOT FOUND";
        }

        var result = (string)database.Call("_get_shoppinglist_request", shoppingListIdInt);
        //GD.Print(result);
        JObject formattedItem = ConvertShoppingListWithItems(result);
        //GD.Print("Formatted item:");
        //GD.Print(formattedItem);



        GlobalData.Instance.opendShoppingList = formattedItem;
        return "200";
    }



    // public async Task<string> _get_shoppinglist_request2(string shoppingListId, HttpRequest httpRequest)
    // {
    //     GD.Print("_get_shoppinglist_request");
    //     var result = database.Call("_get_shoppinglist_request", shoppingListId);

    //     //save JObjekt items to GlobalData
    //     //GlobalData.Instance.opendShoppingList = JObject.Parse(System.Text.Encoding.UTF8.GetString(responseBody));

    //     return await Task.FromResult(result.ToString());
    // }

    public async Task<string> _delete_shoppinglist_request(string shoppingListId, HttpRequest httpRequest)
    {
        GD.Print("_delete_shoppinglist_request");
        var result = database.Call("_delete_shoppinglist_request", shoppingListId);
        return await Task.FromResult(result.ToString());
    }

public async Task<string> _patch_shoppinglist_request(string shoppingListId, JObject body, HttpRequest httpRequest)
{
    // Step 1: Fetch the existing shopping list
    //GD.Print("Step 1");
    //string existingListResult = await _get_shoppinglist_request(shoppingListId, httpRequest);
    //JObject existingList = JObject.Parse(existingListResult);

    GD.Print("Step 2");
    // Step 2: Get new items from body
    JObject futureList = body;

    GD.Print("Step 3");
    // Step 3: Delete every item from the existing list with id
    database.Call("_delete_items", shoppingListId);

    GD.Print("Step 4");
    // Step 4: Add new items in body to the existing list
    foreach (JObject item in futureList["fields"]["items"]["arrayValue"]["values"])
    {
        //foreach (JObject item in items)
        //{
            GD.Print(item);
            /* item:
            {
            "mapValue": {
              "fields": {
                "quantity": {
                  "stringValue": "1STK"
                },
                "name": {
                  "stringValue": "Apfel"
                }
              }
            }
          }
            */
            string name = item["mapValue"]["fields"]["name"]["stringValue"].ToString();
            string quantity = item["mapValue"]["fields"]["quantity"]["stringValue"].ToString();
            // Add item to database (this is a placeholder, you need to implement this method in your GDScript
            database.Call("_add_item", shoppingListId, name, quantity);
        }
    //}

    return "Shopping list updated successfully";
}




private Godot.Collections.Dictionary ConvertJObjectToDictionary(JObject jobject)
{
    var dict = new Godot.Collections.Dictionary();
    foreach (var property in jobject.Properties())
    {
        if (property.Value.Type == JTokenType.Object)
        {
            dict[property.Name] = ConvertJObjectToDictionary((JObject)property.Value);
        }
        else if (property.Value.Type == JTokenType.Array)
        {
            dict[property.Name] = ConvertJArrayToArray((JArray)property.Value);
        }
        else
        {
            dict[property.Name] = property.Value.ToString();
        }
    }
    return dict;
}

private Godot.Collections.Array ConvertJArrayToArray(JArray jarray)
{
    var array = new Godot.Collections.Array();
    foreach (var item in jarray)
    {
        if (item.Type == JTokenType.Object)
        {
            array.Add(ConvertJObjectToDictionary((JObject)item));
        }
        else if (item.Type == JTokenType.Array)
        {
            array.Add(ConvertJArrayToArray((JArray)item));
        }
        else
        {
            array.Add(item.ToString());
        }
    }
    return array;
}


    public async Task<JObject> _send_auth_request(bool function, string email, string password, HttpRequest httpRequest)
    {
        GD.Print("_send_auth_request");
        var result = database.Call("_send_auth_request", function, email, password);
        return await Task.FromResult(JObject.FromObject(result));
    }
}

