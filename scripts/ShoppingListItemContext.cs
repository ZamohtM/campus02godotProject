using Godot;
using System;

public partial class ShoppingListItemContext : Control
{
	public override void _Ready()
	{
	}

	public void _on_edit_button_pressed()
	{
		GlobalData.Instance.setOpenedList(this.GetMeta("document_id").ToString(),this.GetMeta("name").ToString());
		GetTree().ChangeSceneToFile("res://scenes/ShoppingListContext.tscn");
	}
}
