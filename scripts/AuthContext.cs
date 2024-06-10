using Godot;
using Newtonsoft.Json.Linq;

public partial class AuthContext : Control
{
	//ConnectorContext connector;
	private ConnectorContext connector
    {
        get => GlobalData.Instance.connector;
        set => GlobalData.Instance.connector = value;
    }

	public override void _Ready()
	{	
		if(Utilities.IsInternetAvailable())
		{
			connector = new FirebaseConnector();
		}
		else
		{
			connector = new SQLiteConnector();
		}
		AddChild(connector as Node);
	}

	public async void _on_login_button_pressed()
	{
		if(Utilities.IsInternetAvailable())
		{
			connector = new FirebaseConnector();
		}
		else
		{
			connector = new SQLiteConnector();
		}

		string email = GetNode<LineEdit>("EmailTextField").Text;
		string password = GetNode<LineEdit>("PasswordTextField").Text;

		if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
		{
			GD.Print("Email or Password missing!");
			return;
		}

		JObject response = await connector._send_auth_request(true,email,password,GetNode<HttpRequest>("HTTPRegister"));
		if(response.ContainsKey("error"))
		{
			GD.Print("Error occured!");	
		}else {
			GlobalData.Instance.logIn(response["email"].ToString(),response["idToken"].ToString(),response["localId"].ToString(),response["refreshToken"].ToString());
			GlobalData.Instance.connector = connector;
			GetTree().ChangeSceneToFile("res://scenes/MainContext.tscn");
		}
	}

	public async void _on_register_button_pressed()
	{
		string email = GetNode<LineEdit>("EmailTextField").Text;
		string password = GetNode<LineEdit>("PasswordTextField").Text;

		if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
		{
			GD.Print("Email or Password missing!");
			return;
		}

		JObject response = await connector._send_auth_request(false,email,password,GetNode<HttpRequest>("HTTPRegister"));
		if(response.ContainsKey("error"))
		{
			GD.Print("Error occured!");	
		}else {
			GD.Print(response);
		}
	}

	public void _on_offline_button_pressed()
	{
		GD.Print("Offline");
		connector = new SQLiteConnector();
		AddChild(connector as Node);
		GlobalData.Instance.Email = "offline";
		GlobalData.Instance.connector = connector;
		GetTree().ChangeSceneToFile("res://scenes/MainContext.tscn");
	}


}
