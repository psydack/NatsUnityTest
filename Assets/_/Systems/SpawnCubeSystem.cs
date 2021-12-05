using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class SpawnCubeSystem : SystemBase
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