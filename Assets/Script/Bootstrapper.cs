using Unity.Entities;

public abstract class Bootstrapper : ICustomBootstrap
{
	public abstract bool Initialize(string defaultWorldName);
}
