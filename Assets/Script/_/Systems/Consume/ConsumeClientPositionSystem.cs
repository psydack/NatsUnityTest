using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class ConsumeClientPositionSystem : SystemBase
{
	private EntityQuery _query;
	private NetClientSystem _clientSystem;

	protected override void OnCreate()
	{
		base.OnCreate();

		_clientSystem = World.GetExistingSystem<NetClientSystem>();

		RequireForUpdate(_query);
		RequireSingletonForUpdate<NetClientContext>();
		RequireSingletonForUpdate<NetStarted>();
	}

	protected override void OnUpdate()
	{
		var map = _clientSystem.Connection.SubscriberMap;
		var deltaTime = Time.DeltaTime;
		var maxMessage = 30;

		Entities
			.WithStoreEntityQueryInField(ref _query)
			.ForEach((Entity entity, ref Translation translation, in Rotation rotation) =>
			{
				if (map.TryGetValue(Command.Position, out var subscriber))
				{
					while (subscriber.PendingMessages > 0 && --maxMessage > 0)
					{
						var message = subscriber.NextMessage();

						var data = message.Data;

						unsafe
						{
							fixed (byte* ptr = data)
							{
								UnsafeUtility.CopyPtrToStructure(ptr, out float3 position);
								translation.Value = position;
							}
						}
					}
				}
			})
			.WithoutBurst()
			.Run();
	}
}