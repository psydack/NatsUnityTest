using UnityEditor;

public static class EditorUtilities
{
	public static WorldContextSettings LoadWorlds(string worldContextAsset)
	{
		return AssetDatabase.LoadAssetAtPath<WorldContextSettings>($"Assets/Settings/{worldContextAsset}.asset");
	}
}