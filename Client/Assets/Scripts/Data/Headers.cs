using System.Collections.Generic;
using System.ComponentModel;

public class Headers
{
	private Dictionary<string, string> vals;

	public Dictionary<string, string> Fields { get => vals; }

	public Headers AddHeader(string key, string val)
	{
		vals.Add(key, val);
		return this;
	}

	public T GetHeader<T>(string key)
	{
		if(vals.TryGetValue(key, out string val))
		{
			return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(val);
		}
		else
		{
			return default(T);
		}
	}

	public Headers()
	{
		vals = new Dictionary<string, string>();
	}

	public Headers(Dictionary<string, string> preset)
	{
		vals = preset;
	}
}
