public class NetClientConnection : NetConnection
{
	protected override void OnCreateConnection()
	{
		_subscriberMap.Add(
			Command.Print,
			_connection.SubscribeSync(Command.Print.ToString()));

		_subscriberMap.Add(
			Command.Position,
			_connection.SubscribeSync(Command.Position.ToString()));
	}
}
