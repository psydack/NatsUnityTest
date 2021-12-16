#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class BuildEditor : MonoBehaviour
{
#if UNITY_EDITOR
	[MenuItem("Build/Server")]
	public static void BuildServer()
	{
		var options = new BuildPlayerOptions();

		var result = BuildPipeline.BuildPlayer(options);
		if (result.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
		{
			Debug.Log("AEEE");
			return;
		}

		Debug.Log($"<color=red>AAAHHHH =(</color> {result.summary.totalErrors}.");
	}


#endif
}
