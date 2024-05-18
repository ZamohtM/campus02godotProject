using Godot;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


public partial class FirebaseConnector : Node
{
	private static readonly string API_KEY = "AIzaSyBVjrtLGD1NkLkzGgor1AU4YpdE5GMS82M";
	private static readonly string PROJECT_ID = "gmc-c02";
	private static readonly string REGISTER_URL = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=";
	private static readonly string LOGIN_URL = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=";
    private static readonly string REST_URL = "https://firestore.googleapis.com/v1/projects/gmc-c02/databases/(default)/documents";
    private static readonly string QUERY_URL = "https://firestore.googleapis.com/v1/projects/gmc-c02/databases/(default)/documents:runQuery";


	public async Task<JObject> _send_auth_request(bool function, string email, string password, HttpRequest httpRequest)
    {
		string url = function ? LOGIN_URL+API_KEY : REGISTER_URL + API_KEY;

        var body = new JObject
        {
            { "email", email },
            { "password", password },
            { "returnSecureToken", true }
        };

        var error = httpRequest.Request(url, new string[] {"Content-Type: application/json"}, HttpClient.Method.Post, body.ToString());
        if (error != Error.Ok)
        {
			return new JObject(
				new JProperty("error",error.ToString())
			);
        }

        var signal = await ToSignal(httpRequest, "request_completed");

        long result = (long)signal[0];
        long responseCode = (long)signal[1];
        string[] responseHeaders = (string[])signal[2];
        byte[] responseBody = (byte[])signal[3];

		return JObject.Parse(System.Text.Encoding.UTF8.GetString(responseBody));
    }

    public async Task<string> _send_new_shoppinglist_request(string[] sharedWith, string name, HttpRequest httpRequest)
    {
		string url = REST_URL + "/shoppingLists/";

        JArray sharedWithUsers = new JArray();
		foreach (string user in sharedWith)
		{
    		sharedWithUsers.Add(new JObject(new JProperty("stringValue", user)));
		}

		var body = new JObject(
    		new JProperty("fields",
        		new JObject(
            		new JProperty("name", new JObject(new JProperty("stringValue", name))),
            		new JProperty("owner", new JObject(new JProperty("stringValue", GlobalData.Instance.Email))),
            		new JProperty("sharedWith", new JObject(
                		new JProperty("arrayValue", 
                   			new JObject(new JProperty("values", sharedWithUsers))
                		)
            		))
        		)
    		)
		);

        var error = httpRequest.Request(url, new string[] {"Content-Type: application/json","Auth: Bearer "+GlobalData.Instance.IdToken}, HttpClient.Method.Post, body.ToString());
        if (error != Error.Ok)
        {
			return error.ToString();
        }

        var signal = await ToSignal(httpRequest, "request_completed");

        long result = (long)signal[0];
        long responseCode = (long)signal[1];
        string[] responseHeaders = (string[])signal[2];
        byte[] responseBody = (byte[])signal[3];

		return responseCode.ToString();
    } 

    public async Task<JArray> _get_shoppinglists_owner_request(string email, HttpRequest httpRequest)
    {
		string url = QUERY_URL;

        JObject body = new JObject
        {
            ["structuredQuery"] = new JObject
            {
                ["select"] = new JObject
                {
                    ["fields"] = new JArray(new JObject { ["fieldPath"] = "name" })
                },
                ["from"] = new JArray(new JObject { ["collectionId"] = "shoppingLists" }),
                ["where"] = new JObject
                {
                    ["fieldFilter"] = new JObject
                    {
                        ["field"] = new JObject { ["fieldPath"] = "owner" },
                        ["op"] = "EQUAL",
                        ["value"] = new JObject { ["stringValue"] = email }
                    }
                }
            }
        };

        var error = httpRequest.Request(url, new string[] {"Content-Type: application/json","Auth: Bearer "+GlobalData.Instance.IdToken}, HttpClient.Method.Post, body.ToString());
        if (error != Error.Ok)
        {
			return new JArray("error",error);
        }

        var signal = await ToSignal(httpRequest, "request_completed");

        long result = (long)signal[0];
        long responseCode = (long)signal[1];
        string[] responseHeaders = (string[])signal[2];
        byte[] responseBody = (byte[])signal[3];

		return JArray.Parse(System.Text.Encoding.UTF8.GetString(responseBody));
    } 

    public async Task<JArray> _get_shoppinglists_shared_request(string email, HttpRequest httpRequest)
    {
		string url = QUERY_URL;

        JObject body = new JObject
        {
            ["structuredQuery"] = new JObject
            {
                ["select"] = new JObject
                {
                    ["fields"] = new JArray(new JObject { ["fieldPath"] = "name" })
                },
                ["from"] = new JArray(new JObject { ["collectionId"] = "shoppingLists" }),
                ["where"] = new JObject
                {
                    ["fieldFilter"] = new JObject
                    {
                        ["field"] = new JObject { ["fieldPath"] = "sharedWith" },
                        ["op"] = "ARRAY_CONTAINS",
                        ["value"] = new JObject { ["stringValue"] = email }
                    }
                }
            }
        };

        var error = httpRequest.Request(url, new string[] {"Content-Type: application/json","Auth: Bearer "+GlobalData.Instance.IdToken}, HttpClient.Method.Post, body.ToString());
        if (error != Error.Ok)
        {
			return new JArray("error",error);
        }

        var signal = await ToSignal(httpRequest, "request_completed");

        long result = (long)signal[0];
        long responseCode = (long)signal[1];
        string[] responseHeaders = (string[])signal[2];
        byte[] responseBody = (byte[])signal[3];

		return JArray.Parse(System.Text.Encoding.UTF8.GetString(responseBody));
    }
}
