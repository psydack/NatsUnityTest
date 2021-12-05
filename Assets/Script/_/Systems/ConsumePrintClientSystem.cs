﻿using System.Text;
using Unity.Entities;

public class ConsumePrintClientSystem : SystemBase
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