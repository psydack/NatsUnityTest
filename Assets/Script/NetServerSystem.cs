using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class NetServerSystem : SystemBase
{
	private struct NetStarted : IComponentData { }
	private EntityCommandBufferSystem _commandBufferSystem;

	private ServerConnection Connection;

	protected override void OnCreate()
	{
		Connection = new ServerConnection();
		_commandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
	}

	protected override void OnUpdate()
	{
		var commandBuffer = _commandBufferSystem.CreateCommandBuffer();

		Entities
			.WithNone<NetStarted>()
			.WithAll<ServerContext>()
			.ForEach((Entity entity) =>
			{
				commandBuffer.AddComponent<NetStarted>(entity);
				Connection.Start();
			})
			.WithoutBurst()
			.Run();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		Connection.Stop();
	}
}
