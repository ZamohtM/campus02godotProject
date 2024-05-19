using Godot;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ShoppingListContext : Control
{
    private PackedScene newProductScene;
	private PackedScene productScene;
    private FirebaseConnector fbConnector;

    public override async void _Ready()
    {
		fbConnector = new FirebaseConnector();

        newProductScene = (PackedScene)ResourceLoader.Load("res://scenes/NewProductContext.tscn");
		productScene = (PackedScene)ResourceLoader.Load("res://scenes/ProductItemContext.tscn");

        GetNode<Label>("Panel/ShoppingListLabel").Text = GlobalData.Instance.openedShoppingListName;

        string response = await fbConnector._get_shoppinglist_request(GlobalData.Instance.openedShoppingListId, GetNode<HttpRequest>("HTTPShoppingList"));
        
        JToken itemsToken = GlobalData.Instance.opendShoppingList["fields"]?["items"]?["arrayValue"]?["values"];
        bool containsMapValue = itemsToken != null && itemsToken.Any(item => item["mapValue"] != null);

        if (response.Equals("200") && containsMapValue)
        {
            foreach (JObject map in itemsToken)
            {
                Node productInstance = productScene.Instantiate();
                string name = map["mapValue"]["fields"]["name"]["stringValue"].ToString();
                string quantity = map["mapValue"]["fields"]["quantity"]["stringValue"].ToString();

                productInstance.GetNode<Label>("Panel/ProductLabel").Text = name;
                productInstance.GetNode<Label>("Panel/QuantityLabel").Text = quantity;

                GetNode<VBoxContainer>("Panel/ScrollContainerProducts/VBoxContainerProducts").AddChild(productInstance);
            }

            EnableAllButtons(GetTree().Root);
        }
    }
    

    private void _on_add_button_pressed()
    {
        DisableAllButtons(this);
        Node newProductInstance = newProductScene.Instantiate();
        AddChild(newProductInstance);
    }

	private async void _on_save_button_pressed()
	{
		var products = GetNode<VBoxContainer>("Panel/ScrollContainerProducts/VBoxContainerProducts").GetChildren();
		GlobalData.Instance.opendShoppingList["fields"]["items"]["arrayValue"]["values"] = new JArray();
		JArray itemsArray = (JArray)GlobalData.Instance.opendShoppingList["fields"]["items"]["arrayValue"]["values"];


		foreach(var product in products)
		{
			JObject mapValueItem = new JObject(
                new JProperty("mapValue", new JObject(
                    new JProperty("fields", new JObject(
                        new JProperty("name", new JObject(
                            new JProperty("stringValue", product.GetNode<Label>("Panel/ProductLabel").Text)
                        )),
                        new JProperty("quantity", new JObject(
                            new JProperty("stringValue", product.GetNode<Label>("Panel/QuantityLabel").Text)
                        ))
                    ))
                ))
            );
			itemsArray.Add(mapValueItem);
		}

		string response = await fbConnector._patch_shoppinglist_request(GlobalData.Instance.openedShoppingListId,GlobalData.Instance.opendShoppingList,GetNode<HttpRequest>("HTTPShoppingListProducts"));
		if(response.Equals("200"))
		{
			GetTree().ChangeSceneToFile("res://scenes/MainContext.tscn");
		}
	}

	private async void _on_delete_button_pressed()
	{
		string response = await fbConnector._delete_shoppinglist_request(GlobalData.Instance.openedShoppingListId,GetNode<HttpRequest>("HTTPShoppingListDelete"));
		if(response.Equals("200"))
		{
			GetTree().ChangeSceneToFile("res://scenes/MainContext.tscn");
		}
	}

    private void _on_back_button_pressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/MainContext.tscn");
    }

    public void DisableAllButtons(Node node)
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is Button button)
            {
                button.Disabled = true;
                button.MouseFilter = Control.MouseFilterEnum.Ignore;
            }
            DisableAllButtons(child);
        }
    }

    public void EnableAllButtons(Node node)
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is Button button)
            {
                button.Disabled = false;
                button.MouseFilter = Control.MouseFilterEnum.Stop;
            }
            EnableAllButtons(child);
        }
    }
}
