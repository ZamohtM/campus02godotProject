using Godot;
using System;

public partial class ProductItemContext : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	private void _on_button_pressed()
	{
		QueueFree();
	}
}
