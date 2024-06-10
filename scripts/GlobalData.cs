using Godot;
using Newtonsoft.Json.Linq;

public partial class GlobalData : Node
{
	public static GlobalData Instance { get; private set; }

	public string Email = "";
	public string IdToken = "";
	public string LocalId = "";
	public string refreshToken = "";
	public string openedShoppingListId = "";
	public string openedShoppingListName = "";
	public JObject opendShoppingList = new JObject();

	public ConnectorContext connector;




	public override void _Ready()
	{
		Instance = this;
	}

	public void logIn(string email, string idtoken, string localid, string refreshtoken)
	{
		this.Email = email;
		this.IdToken = idtoken;
		this.LocalId = localid;
		this.refreshToken = refreshtoken;
	}

	public void logOut()
	{
		this.Email = "";
		this.IdToken = "";
		this.LocalId = "";
		this.refreshToken = "";
	}

	public void setOpenedList(string docuId, string name)
	{
		this.openedShoppingListId = docuId;
		this.openedShoppingListName = name;
	}
}
