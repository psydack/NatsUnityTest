using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class NetServerSystem : NetSystem<NetServerContext>
{
	protected override void OnCreate()
	{
		base.OnCreate();
		Connection = new NetServerConnection();
	}

	protected override void OnUpdate()
	{
		Connection.Start();

		var entity = GetSingletonEntity<NetServerContext>();
		EntityManager.AddComponent<NetStarted>(entity);
	}
}
