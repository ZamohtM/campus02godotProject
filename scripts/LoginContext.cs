/*




DEPRECATED




*/

using System.Threading.Tasks;
using Godot;
using Newtonsoft.Json.Linq;


public partial class LoginContext : Control
{
	 
	ConnectorContext connector;

	public override void _Ready()
	{
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

		JObject response = await connector._send_auth_request(false,email,password,GetNode<HttpRequest>("HTTPRegister"));
		if(response.ContainsKey("error"))
		{
			GD.Print("Error occured!");	
		}else {
			GD.Print(response);
		}
	}

}
