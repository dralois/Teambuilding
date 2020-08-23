using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HttpClient : MonoBehaviour
{
	public static string serverAdress = "http://212.83.56.221:8080/";

	public static IEnumerator CallMethod(string method, Headers data, System.Action<Headers> resultCallback)
	{
		string uri = serverAdress + UnityWebRequest.EscapeURL(method);
		using (var request = UnityWebRequest.Post(uri, ""))
		{
			foreach(var kvp in data.Fields)
			{
				request.SetRequestHeader(kvp.Key, kvp.Value);
			}

			yield return request.SendWebRequest();

			if (request.isDone)
			{
				if(request.responseCode == 200)
				{
					var headers = request.GetResponseHeaders();
					if(headers != null)
					{
						resultCallback(new Headers(headers));
					}
					else
					{
						resultCallback(new Headers());
					}
				}
				else
				{
					Debug.LogError($"Response error: {request.responseCode}");
				}
			}
			else
			{
				Debug.LogError($"Request error: {request.error}");
			}
		}
	}
}
