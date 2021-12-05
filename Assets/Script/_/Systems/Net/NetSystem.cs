using Unity.Entities;

public struct NetStarted : IComponentData { }

public abstract class NetSystem<T> : SystemBase
{
	public NetConnection Connection { get; protected set; }

	protected override void OnCreate()
	{
		base.OnCreate();

		RequireForUpdate(GetEntityQuery(
			ComponentType.Exclude<NetStarted>(),
			ComponentType.ReadOnly<NetContext>(),
			ComponentType.ReadOnly(typeof(T))));
	}

	protected override void OnUpdate() { }

	protected override void OnDestroy()
	{
		base.OnDestroy();

		Connection?.Stop();
	}
}
