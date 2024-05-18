using Godot;

public partial class GlobalData : Node
{
    public static GlobalData Instance { get; private set; }

    public string Email = "";
    public string IdToken = "";
    public string LocalId = "";
    public string refreshToken = "";

    public override void _Ready()
    {
        Instance = this;
    }
}
