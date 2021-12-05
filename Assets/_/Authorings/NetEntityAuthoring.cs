using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class NetEntityAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	public string EntityType;

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		dstManager.AddComponentData(entity, new EntityTypeData
		{
			Value = (EntityType)EntityType
		});
	}
}
