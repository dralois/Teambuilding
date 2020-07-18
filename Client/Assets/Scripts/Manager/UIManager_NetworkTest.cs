using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.UIElements.Runtime;

public class UIManager_NetworkTest : MonoBehaviour
{
	public PanelRenderer testUI;

	private void OnEnable()
	{
		testUI.postUxmlReload = X_BindTestUI;
	}

	private IEnumerable<UnityEngine.Object> X_BindTestUI()
	{
		var root = testUI.visualTree;
		var sendButton = root.Q<Button>("send-request");
		sendButton.clickable.clicked += () =>
		{
			StartCoroutine(HttpClient.CallMethod("TestMethod", new Dictionary<string, string>() { { "TestKey", "TestVal" } }));
		};
		return null;
	}
}
