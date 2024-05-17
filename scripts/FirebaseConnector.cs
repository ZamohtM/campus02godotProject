using Godot;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


public partial class FirebaseConnector : Node
{
	private static readonly string API_KEY = "AIzaSyBVjrtLGD1NkLkzGgor1AU4YpdE5GMS82M";
	private static readonly string PROJECT_ID = "gmc-c02";
	private static readonly string REGISTER_URL = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=";
	private static readonly string LOGIN_URL = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=";


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


}
