using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class NetClientSystem : SystemBase
{
	private struct NetStarted : IComponentData { }
	private EntityCommandBufferSystem _commandBufferSystem;

	public ClientConnection Connection { get; private set; }

	protected override void OnCreate()
	{
		Connection = new ClientConnection();
		_commandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
	}

	protected override void OnUpdate()
	{
		var commandBuffer = _commandBufferSystem.CreateCommandBuffer();

		Entities
			.WithNone<NetStarted>()
			.WithAll<ClientContext>()
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