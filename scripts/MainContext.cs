using Godot;

public partial class MainContext : Control
{
	public override void _Ready()
	{
		GetNode<RichTextLabel>("EmailLabel").Text =  GlobalData.Instance.Email;
	}

}
