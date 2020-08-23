using UnityEngine;

public static class JSONArray
{
	public static T[] FromJson<T>(string json)
	{
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
		return wrapper.items;
	}

	[System.Serializable]
	private class Wrapper<T>
	{
		public T[] items;
	}
}

[System.Serializable]
public class Player
{
	public int identifier;
	public string name;
	public bool ready;
}
