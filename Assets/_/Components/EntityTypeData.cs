using System;
using Unity.Collections;
using Unity.Entities;

public struct EntityTypeData : IComponentData
{
	public EntityType Value;

	public EntityTypeData(string value)
	{
		Value = (EntityType)value;
	}
}

public struct EntityType : IComparable<EntityType>, IEquatable<EntityType>
{
	public FixedString64 Value;

	public int CompareTo(EntityType other)
	{
		return other.CompareTo(other);
	}

	public bool Equals(EntityType other)
	{
		return Value.Equals(other.Value);
	}

	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}

	public static explicit operator EntityType(string type)
	{
		return new EntityType
		{
			Value = new FixedString64(type)
		};
	}
}
