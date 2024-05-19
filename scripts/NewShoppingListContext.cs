using Godot;
using Newtonsoft.Json.Linq;
using System;

public partial class NewShoppingListContext : Control
{
	FirebaseConnector fbConnector = new FirebaseConnector();
	private PackedScene itemScene;
	public override void _Ready()
	{
		AddChild(fbConnector);	
		itemScene = (PackedScene)ResourceLoader.Load("res://scenes/ShoppingListItemContext.tscn");
	}

	public async void _on_create_button_pressed()
	{
		string name = GetNode<LineEdit>("NameTextField").Text;
		string[] sharedWith = GetNode<LineEdit>("SharedWithTextField").Text.Split(';');

		string response = await fbConnector._send_new_shoppinglist_request(sharedWith,name,GetNode<HttpRequest>("HTTPNewShoppingList"));
		if (response.Equals("200"))
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
