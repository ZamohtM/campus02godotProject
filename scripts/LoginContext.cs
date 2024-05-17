using Godot;
using Newtonsoft.Json.Linq;

public partial class LoginContext : Control
{
	FirebaseConnector fbConnector = new FirebaseConnector();

	public override void _Ready()
	{
		AddChild(fbConnector);		
	}

	public async void _on_login_button_pressed()
	{
		string email = GetNode<LineEdit>("EmailTextField").Text;
		string password = GetNode<LineEdit>("PasswordTextField").Text;

		if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
		{
			GD.Print("Email or Password missing!");
			return;
		}

		JObject response = await fbConnector._send_auth_request(true,email,password,GetNode<HttpRequest>("HTTPRegister"));
		if(response.ContainsKey("error"))
		{
			GD.Print("Error occured!");	
		}else {
			GlobalData.Instance.Email = response["email"].ToString();
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

		JObject response = await fbConnector._send_auth_request(false,email,password,GetNode<HttpRequest>("HTTPRegister"));
		if(response.ContainsKey("error"))
		{
			GD.Print("Error occured!");	
		}else {
			GD.Print(response);
		}
	}

}
