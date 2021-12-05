using NATS.Client;
using System.Collections.Generic;
using Unity.Entities;

public abstract class NetConnection
{
	protected IConnection _connection;

	public Dictionary<Command, ISyncSubscription> SubscriberMap => _subscriberMap;
	protected Dictionary<Command, ISyncSubscription> _subscriberMap;

	public virtual void Start()
	{
		var connectionOptions = ConnectionFactory.GetDefaultOptions();
		connectionOptions.Url = "localhost:4222";
		connectionOptions.Secure = false;

		var connectionFactory = new ConnectionFactory();
		_connection = connectionFactory.CreateConnection(connectionOptions);
		_subscriberMap = new Dictionary<Command, ISyncSubscription>();

		OnCreateConnection();
	}

	public virtual void Stop()
	{
		_connection?.Drain();
		_connection?.Close();
	}

	public void Publish(string subject, byte[] data)
	{
		_connection.Publish(subject, data);
	}

	protected abstract void OnCreateConnection();
}

public class SimulateInputClientSystem : SystemBase
{
	private NetClientSystem _clientSystem;

	protected override void OnCreate()
	{
		base.OnCreate();

		_clientSystem = World.GetExistingSystem<NetClientSystem>();
	}

	protected override void OnUpdate()
	{
		var connection = _clientSystem.Connection;

		Entities
			.WithAll<ClientContext>()
			.ForEach((Entity entity) =>
			{
				//connection.Publish(Command.RequestMove, )
			})
			.Schedule();
	}
}
