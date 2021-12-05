using System;
using Unity.Entities;
using UnityEngine.LowLevel;
using Object = UnityEngine.Object;

public class EditorBootstrapper : Bootstrapper
{
	public override bool Initialize(string defaultWorldName)
	{
		var contexts = Utilities.LoadWorlds("WorldSettings");
		foreach (var context in contexts?.Worlds)
		{
			var world = new World($"{context.Type}_{Guid.NewGuid()}");

			World.DefaultGameObjectInjectionWorld = world;
			var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
			DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, systems);
			var playerLoop = PlayerLoop.GetCurrentPlayerLoop();

			ScriptBehaviourUpdateOrder.AddWorldToPlayerLoop(world, ref playerLoop);
			PlayerLoop.SetPlayerLoop(playerLoop);

			foreach (var asset in context.Objects)
			{
				_ = Object.Instantiate(asset);
			}

			var entityManager = world.EntityManager;
			var entity = entityManager.CreateEntity();
			entityManager.AddComponent<NetContext>(entity);

			switch (context.Type)
			{
				case WorldContextType.Server:
					entityManager.AddComponent<NetServerContext>(entity);
					break;
				case WorldContextType.Client:
					entityManager.AddComponent<NetClientContext>(entity);
					break;
			}
		}

		return true;
	}
}
