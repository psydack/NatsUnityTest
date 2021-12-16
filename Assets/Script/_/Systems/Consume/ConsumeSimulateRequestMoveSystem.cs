using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class ConsumeSimulateRequestMoveSystem : SystemBase
{
	private EntityQuery _query;
	private NetServerSystem _serverSystem;

	protected override void OnCreate()
	{
		base.OnCreate();

		_serverSystem = World.GetExistingSystem<NetServerSystem>();

		RequireForUpdate(_query);
		RequireSingletonForUpdate<NetServerContext>();
		RequireSingletonForUpdate<NetStarted>();
	}

	protected override void OnUpdate()
	{
		// TODO: Map entity;
		var connection = _serverSystem.Connection;
		var map = connection.SubscriberMap;
		var deltaTime = Time.DeltaTime;

		Entities
			.WithStoreEntityQueryInField(ref _query)
			.ForEach((Entity entity, ref Translation translation, in Rotation rotation) =>
			{
				if (map.TryGetValue(Command.RequestMove, out var subscriber))
				{
					while (subscriber.PendingMessages > 0)
					{
						var message = subscriber.NextMessage();

						var data = message.Data;

						unsafe
						{
							fixed (byte* ptr = data)
							{
								// consume
								UnsafeUtility.CopyPtrToStructure(ptr, out float3 movement);
								translation.Value += math.mul(rotation.Value, movement) * deltaTime;

								// send back
								UnsafeUtility.CopyStructureToPtr(ref translation.Value, ptr);
								connection.Publish(Command.Position.ToString(), data);
							}
						}
					}
				}
			})
			.WithoutBurst()
			.Run();
	}
}
