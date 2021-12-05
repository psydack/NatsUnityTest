using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class MoveSystem : SystemBase
{
	protected override void OnUpdate()
	{
		var deltaTime = Time.DeltaTime;

		Entities
			.ForEach((ref Translation translation, in Rotation rotation) =>
			{
				//translation.Value += math.mul(rotation.Value, new float3(0, 0, 10)) * deltaTime;
			})
			.Schedule();
	}
}
