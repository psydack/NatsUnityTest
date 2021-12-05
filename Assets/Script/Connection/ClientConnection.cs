public class ClientConnection : NetConnection
{
	protected override void OnCreateConnection()
	{
		_subscriberMap.Add(
			Command.Print,
			_connection.SubscribeSync(Command.Print.ToString()));

		_subscriberMap.Add(
			Command.Move,
			_connection.SubscribeSync(Command.Move.ToString()));
	}
}
