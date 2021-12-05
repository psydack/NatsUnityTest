using NATS.Client;
using System.Text;
using Unity.Entities;
using UnityEngine;

public abstract class NetConnection
{
	//protected ENet.Host _host;
	private IConnection _connection;

	public virtual void Start()
	{
		//_host = new ENet.Host();
		var connectionOptions = ConnectionFactory.GetDefaultOptions();
		connectionOptions.Url = "localhost:4222";
		connectionOptions.Secure = false;

		var connectionFactory = new ConnectionFactory();
		_connection = connectionFactory.CreateConnection(connectionOptions);

		var subscriber = _connection.SubscribeAsync("PinBallGroup", (sender, args) =>
		{
			Debug.Log(args.Message);
		});

		_connection.Publish("PinBallGroup", Encoding.UTF8.GetBytes("Let us bring pinball back"));
	}

	public virtual void Stop()
	{
		//_host.Dispose();
	}
}


public class ClientConnection : NetConnection
{

}

public class ServerConnection : NetConnection
{

}

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
