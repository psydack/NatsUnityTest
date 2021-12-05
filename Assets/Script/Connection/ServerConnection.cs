using System.Text;
using System.Timers;

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
		Publish(Command.Print.ToString(), Encoding.UTF8.GetBytes($"The Elapsed event was raised at {e.SignalTime}"));
	}
}
