using Godot;
using System;

public partial class NewProductContext : Control
{

	private PackedScene productScene;
	public override void _Ready()
	{
		productScene = (PackedScene)ResourceLoader.Load("res://scenes/ProductItemContext.tscn");
	}

	private void _on_close_button_pressed()
	{
		GetNode<ShoppingListContext>("/root/ShoppingListContext").EnableAllButtons(GetTree().Root);
		QueueFree();
	}

	private void _on_create_button_pressed()
	{
		string name = GetNode<LineEdit>("NameTextField").Text;
		string quantity = GetNode<LineEdit>("QuantityTextField").Text;

		Node productInstance = productScene.Instantiate();
		productInstance.GetNode<Label>("Panel/ProductLabel").Text = name;
		productInstance.GetNode<Label>("Panel/QuantityLabel").Text = quantity;
		GetNode<ShoppingListContext>("/root/ShoppingListContext").GetNode<VBoxContainer>("Panel/ScrollContainerProducts/VBoxContainerProducts").AddChild(productInstance);
		GetNode<ShoppingListContext>("/root/ShoppingListContext").EnableAllButtons(GetTree().Root);
		QueueFree();
	}
}
