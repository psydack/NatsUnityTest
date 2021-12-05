using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveSystem : SystemBase
{
	protected override void OnUpdate()
	{
		var deltaTime = Time.DeltaTime;

		Entities.ForEach((ref Translation translation, in Rotation rotation) =>
		{
			translation.Value += math.mul(rotation.Value, new float3(0, 0, 10)) * deltaTime;
		}).Schedule();
	}
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class TestSystem : SystemBase
{
	private EntityQuery _query;

	private DatabaseSystem _databaseSystem;
	private EntityCommandBufferSystem _commandBufferSystem;

	protected override void OnCreate()
	{
		base.OnCreate();

		_databaseSystem = World.GetExistingSystem<DatabaseSystem>();
		_commandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();

		var entity = EntityManager.CreateEntity();
		EntityManager.AddComponentData(entity, new SpawnEvent
		{
			EntityType = new EntityTypeData("Cube")
		});

		RequireForUpdate(_query);
		RequireSingletonForUpdate<Ready>();
	}

	protected override void OnUpdate()
	{
		var map = _databaseSystem.Map;
		var commandBuffer = _commandBufferSystem.CreateCommandBuffer();

		JobHandle.CombineDependencies(Dependency, _databaseSystem.GetOutputDependency());

		Dependency = Entities
			.WithStoreEntityQueryInField(ref _query)
			.ForEach((Entity spawnEntity, ref SpawnEvent @event) =>
			{
				if (map.TryGetValue(@event.EntityType.Value, out var prefab))
				{
					commandBuffer.Instantiate(prefab);
				}

				commandBuffer.DestroyEntity(spawnEntity);
			})
			.Schedule(Dependency);

		_commandBufferSystem.AddJobHandleForProducer(Dependency);
	}
}