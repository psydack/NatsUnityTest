using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class DatabaseSystem : SystemBase
{
	private struct AddedToPrefabMap : IComponentData { }

	private NativeHashMap<EntityType, Entity> _entitiesMap;
	private EntityQuery _query;

	public NativeHashMap<EntityType, Entity> Map => _entitiesMap;

	public JobHandle GetOutputDependency()
	{
		return Dependency;
	}

	protected override void OnCreate()
	{
		base.OnCreate();

		_entitiesMap = new NativeHashMap<EntityType, Entity>(0xF, Allocator.Persistent);
	}

	protected override void OnUpdate()
	{
		var mapWriter = _entitiesMap.AsParallelWriter();

		Dependency = Entities
			.WithAll<Prefab>()
			.WithNone<AddedToPrefabMap>()
			.WithChangeFilter<EntityTypeData>()
			.WithStoreEntityQueryInField(ref _query)
			.ForEach((Entity entity, in EntityTypeData type) =>
			{
				Assert.IsTrue(mapWriter.TryAdd(type.Value, entity));
			})
			.ScheduleParallel(Dependency);

		Dependency.Complete();

		EntityManager.AddComponent<AddedToPrefabMap>(_query);

		// SINGLETON
		var entity = EntityManager.CreateEntity();
		EntityManager.AddComponent<Ready>(entity);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		_entitiesMap.Dispose();
	}
}
