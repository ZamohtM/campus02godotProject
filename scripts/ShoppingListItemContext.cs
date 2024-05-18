using Godot;
using System;

public partial class ShoppingListItemContext : Control
{
	public override void _Ready()
	{
	}

	private void _on_edit_button_pressed()
	{
		GD.Print(this.GetMeta("document_id"));
	}
}
