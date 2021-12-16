using UnityEngine;

public static class Utilities
{
	public static WorldContext LoadWorldContext(string worldContextFilename)
	{
		return Resources.Load<WorldContext>($"{worldContextFilename}");
	}
}
