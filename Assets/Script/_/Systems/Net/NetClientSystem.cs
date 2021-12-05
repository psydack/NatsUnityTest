using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class NetClientSystem : NetSystem<NetClientContext>
{
	protected override void OnCreate()
	{
		base.OnCreate();

		Connection = new NetClientConnection();
	}

	protected override void OnUpdate()
	{
		Connection.Start();

		var entity = GetSingletonEntity<NetClientContext>();
		EntityManager.AddComponent<NetStarted>(entity);
	}
}