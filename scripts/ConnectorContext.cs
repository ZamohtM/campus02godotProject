using System.Threading.Tasks;
using Godot;
using Newtonsoft.Json.Linq;

public interface ConnectorContext
{
	Task<string> _send_new_shoppinglist_request(string[] sharedWith, string name, Godot.HttpRequest httpRequest);
	Task<JArray> _get_shoppinglists_owner_request(string email, HttpRequest httpRequest);
	Task<JArray> _get_shoppinglists_shared_request(string email, HttpRequest httpRequest);
	Task<string> _get_shoppinglist_request(string shoppingListId, HttpRequest httpRequest);
	Task<string> _delete_shoppinglist_request(string shoppingListId, HttpRequest httpRequest);
	Task<string> _patch_shoppinglist_request(string shoppingListId, JObject body, HttpRequest httpRequest);

	Task<JObject> _send_auth_request(bool function, string email, string password, HttpRequest httpRequest);
}
