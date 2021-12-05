using UnityEngine;

[CreateAssetMenu(fileName = "W", menuName = "World/Settings")]
public class WorldContextSettings : ScriptableObject
{
	public WorldContext[] Worlds;
}
