using Unity.Assertions;
using UnityEditor;

[InitializeOnLoad]
public static class EditorEnetInitialization
{
	static EditorEnetInitialization()
	{
		EditorApplication.playModeStateChanged += OnQuitting;
	}

	private static void OnQuitting(PlayModeStateChange playModeStateChange)
	{
		if (playModeStateChange == PlayModeStateChange.EnteredPlayMode)
		{
			Assert.IsTrue(ENet.Library.Initialize());
		}
		if (playModeStateChange == PlayModeStateChange.ExitingPlayMode)
		{
			ENet.Library.Deinitialize();
		}
	}
}