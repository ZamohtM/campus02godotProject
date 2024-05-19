using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json.Linq;

public partial class MainContext : Control
{
	private FirebaseConnector fbConnector;
	private PackedScene newShoppingListScene;
	private Node newShoppingListInstance;
	private PackedScene itemScene;

	public override async void _Ready()
	{
		newShoppingListScene = (PackedScene)ResourceLoader.Load("res://scenes/NewShoppingListContext.tscn");
		itemScene = (PackedScene)ResourceLoader.Load("res://scenes/ShoppingListItemContext.tscn");
		fbConnector = new FirebaseConnector();

		JArray responseOwner = await fbConnector._get_shoppinglists_owner_request(GlobalData.Instance.Email,GetNode<HttpRequest>("HTTPOwner"));
		JArray responseShared = await fbConnector._get_shoppinglists_shared_request(GlobalData.Instance.Email,GetNode<HttpRequest>("HTTPShared"));

		
		if(responseOwner.Any(item => item["document"] != null))
		{
			foreach (JObject document in responseOwner)
			{
				string name = document["document"]["fields"]["name"]["stringValue"].ToString();
				string docuId = document["document"]["name"].ToString().Split('/').Last();

				Node itemInstance = itemScene.Instantiate();
				Label label = itemInstance.GetNode<Label>("Panel/NameLabel");
				label.Text = name;
				itemInstance.SetMeta("document_id", docuId);
				itemInstance.SetMeta("name", name);
				GetNode<VBoxContainer>("ScrollContainerOwner/VBoxContainerOwner").AddChild(itemInstance);
			}
		}
		
		if(responseShared.Any(item => item["document"] != null))
		{
			foreach (JObject document in responseShared)
			{
				string name = document["document"]["fields"]["name"]["stringValue"].ToString();
				string docuId = document["document"]["name"].ToString().Split('/').Last();

				Node itemInstance = itemScene.Instantiate();
				Label label = itemInstance.GetNode<Label>("Panel/NameLabel");
				label.Text = name;
				itemInstance.SetMeta("document_id", docuId);
				itemInstance.SetMeta("name", name);
				GetNode<VBoxContainer>("ScrollContainerShared/VBoxContainerShared").AddChild(itemInstance);
			}
		}
	}
		

	public void _on_add_button_pressed()
	{
		DisableAllButtons(GetTree().Root);

		Node newShoppingListInstance = newShoppingListScene.Instantiate();

		AddChild(newShoppingListInstance);
	}

	public void _on_log_out_button_pressed()
	{
		GlobalData.Instance.logOut();
		GetTree().ChangeSceneToFile("res://scenes/AuthContext.tscn");
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
