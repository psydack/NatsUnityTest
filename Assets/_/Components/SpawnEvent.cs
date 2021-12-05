using Unity.Entities;

public struct SpawnEvent : IComponentData
{
	public EntityTypeData EntityType;
}

public struct Ready : IComponentData { }