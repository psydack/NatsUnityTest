using UnityEditor;

public static class Utilities
{
	public static WorldContextSettings LoadWorlds(string worldContextAsset)
	{
		return AssetDatabase.LoadAssetAtPath<WorldContextSettings>($"Assets/Settings/{worldContextAsset}.asset");
	}
}