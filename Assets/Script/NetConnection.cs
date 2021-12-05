using NATS.Client;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Unity.Entities;

public enum Command
{
	Move,
	Print
}

public abstract class NetConnection
{
	protected IConnection _connection;

	public Dictionary<Command, ISyncSubscription> SubscriberMap => _messageQueue;
	protected Dictionary<Command, ISyncSubscription> _messageQueue;

	public virtual void Start()
	{
		var connectionOptions = ConnectionFactory.GetDefaultOptions();
		connectionOptions.Url = "localhost:4222";
		connectionOptions.Secure = false;

		var connectionFactory = new ConnectionFactory();
		_connection = connectionFactory.CreateConnection(connectionOptions);
		_messageQueue = new Dictionary<Command, ISyncSubscription>();

		OnCreateConnection();
	}

	public virtual void Stop()
	{
		_connection?.Drain();
		_connection?.Close();
	}

	protected abstract void OnCreateConnection();
}


public class ClientConnection : NetConnection
{
	protected override void OnCreateConnection()
	{
		_messageQueue.Add(
			Command.Print,
			_connection.SubscribeSync(Command.Print.ToString()));

		if (_messageQueue.TryGetValue(Command.Print, out var subscriber))
		{
			while (subscriber.PendingMessages > 0)
			{
				var message = subscriber.NextMessage();
				if (message != null)
				{
					UnityEngine.Debug.Log(Encoding.UTF8.GetString(message.Data));
				}
			}
		}

	}
}

public class ServerConnection : NetConnection
{
	private Timer _timer;
	protected override void OnCreateConnection()
	{
		_timer = new Timer();
		_timer.Interval = 1000;
		_timer.Elapsed += PublishMessageTimed;
		_timer.AutoReset = true;
		_timer.Enabled = true;
	}

	private void PublishMessageTimed(object sender, ElapsedEventArgs e)
	{
		_connection.Publish(Command.Print.ToString(), Encoding.UTF8.GetBytes($"The Elapsed event was raised at {e.SignalTime}"));
	}
}


public class ConsumePrintClient : SystemBase
{
	private NetClientSystem _clientSystem;

	protected override void OnCreate()
	{
		base.OnCreate();

		_clientSystem = World.GetExistingSystem<NetClientSystem>();
	}

	protected override void OnUpdate()
	{
		var map = _clientSystem.Connection.SubscriberMap;

		Entities
			.WithAll<ClientContext>()
			.ForEach((Entity entity) =>
			{
				if (map.TryGetValue(Command.Print, out var subscriber))
				{
					while (subscriber.PendingMessages > 0)
					{
						var message = subscriber.NextMessage();
						if (message != null)
						{
							UnityEngine.Debug.Log(Encoding.UTF8.GetString(message.Data));
						}
					}
				}
			})
			.WithoutBurst()
			.Run();
	}
}