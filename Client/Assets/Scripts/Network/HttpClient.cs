using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class HttpClient : MonoBehaviour
{
	public static string serverAdress = "http://212.83.56.221:8080/";

	public static IEnumerator CallMethod(string method, Headers data, System.Action<Headers, bool> resultCallback)
	{
		string uri = serverAdress + UnityWebRequest.EscapeURL(method);
		using (var request = UnityWebRequest.Post(uri, ""))
		{
			foreach(var kvp in data.Fields)
			{
				request.SetRequestHeader(kvp.Key, kvp.Value);
			}

			yield return request.SendWebRequest();

			if (request.isDone && request.downloadHandler.isDone)
			{
				if(!(request.isHttpError || request.isNetworkError))
				{
					var response = request.downloadHandler.text;
					var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

					if (values != null)
					{
						resultCallback(new Headers(values), true);
					}
					else
					{
						resultCallback(new Headers(), false);
					}
				}
				else
				{
					resultCallback(new Headers(), false);
				}
			}
			else
			{
				resultCallback(new Headers(), false);
			}
		}
	}
}
