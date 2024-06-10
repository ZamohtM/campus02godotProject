using Godot;
using Newtonsoft.Json.Linq;
using System;

public partial class NewShoppingListContext : Control
{
	private ConnectorContext connector
    {
        get => GlobalData.Instance.connector;
        set => GlobalData.Instance.connector = value;
    }
	
	
	private PackedScene itemScene;
	public override void _Ready()
	{
		//AddChild(connector as Node);	
		itemScene = (PackedScene)ResourceLoader.Load("res://scenes/ShoppingListItemContext.tscn");
	}

	public async void _on_create_button_pressed()
	{
		string name = GetNode<LineEdit>("NameTextField").Text;
		string[] sharedWith = GetNode<LineEdit>("SharedWithTextField").Text.Split(';');

		string response = await connector._send_new_shoppinglist_request(sharedWith,name,GetNode<HttpRequest>("HTTPNewShoppingList"));
		GD.Print(response);
		//200 reicht nicht weil es eine http response ist
		if (response.Equals("200") || response.Equals("OK"))
		{
			GD.Print("Success");
			QueueFree();
			GetTree().ChangeSceneToFile("res://scenes/MainContext.tscn");
		} else {
			GD.Print("Error");
		}
	}

	private void _on_close_button_pressed()
	{
		GetNode<MainContext>("/root/MainContext").EnableAllButtons(GetTree().Root);
		QueueFree();
	}
}
