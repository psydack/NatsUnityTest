using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class SimulateInputClientSystem : SystemBase
{
	private NetClientSystem _clientSystem;

	protected override void OnCreate()
	{
		base.OnCreate();

		_clientSystem = World.GetExistingSystem<NetClientSystem>();

		RequireSingletonForUpdate<NetClientContext>();
		RequireSingletonForUpdate<NetStarted>();
	}

	protected override void OnUpdate()
	{
		var horizontal = UnityEngine.Input.GetAxis("Horizontal");
		var vertical = UnityEngine.Input.GetAxis("Vertical");
		var movement = new float3(horizontal, 0, vertical) * 5;

		if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftShift))
		{
			movement *= 2f;
		}

		var data = new byte[UnsafeUtility.SizeOf<float3>()];

		unsafe
		{
			fixed (byte* ptr = data)
			{
				UnsafeUtility.CopyStructureToPtr(ref movement, ptr);
			}
		}

		_clientSystem.Connection.Publish(Command.RequestMove.ToString(), data);
	}
}
