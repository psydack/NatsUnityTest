using NATS.Client;
using System.Collections.Generic;

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
		//_connection?.Drain(100);
		_connection?.Close();
	}

	public void Publish(string subject, in byte[] data)
	{
		_connection.Publish(subject, data);
	}

	protected abstract void OnCreateConnection();
}
