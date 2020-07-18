using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class HttpClient : MonoBehaviour
{
	public static string serverAdress = "http://212.83.56.221:8080/";

	public static IEnumerator CallMethod(string method, Dictionary<string, string> data)
	{
		string uri = serverAdress + UnityWebRequest.EscapeURL(method);
		using (var request = UnityWebRequest.Post(uri, data))
		{
			Debug.Log($"Sending {uri}");
			yield return request.SendWebRequest();
			if (request.isDone)
			{
				if(request.responseCode == 200)
				{
					var headers = request.GetResponseHeaders();
					Debug.Log($"Recieved {request.responseCode}");
					foreach(var header in headers)
					{
						Debug.Log($"{header.Key} : {header.Value}");
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
