using UnityEngine;

[CreateAssetMenu(fileName = "W", menuName = "World/Instance")]
public class WorldContext : ScriptableObject
{
	public WorldContextType Type;

	public GameObject[] Objects;
}