using UnityEngine;

[CreateAssetMenu(fileName = "W", menuName = "World/Instance")]
public class WorldContext : ScriptableObject
{
	public ContextType Type;

	public GameObject[] Objects;
}