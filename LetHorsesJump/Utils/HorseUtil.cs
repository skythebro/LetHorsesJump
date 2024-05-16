using BepInEx.Logging;
using Il2CppSystem;
using LetHorsesJump.Utils;
using ProjectM.Gameplay.Scripting;
using ProjectM.Scripting;
using ProjectM.Shared;
using Stunlock.Core;

namespace LetHorsesJump;

using System.Collections.Generic;
using System.Linq;
using Bloodstone.API;
using ProjectM;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

internal static class HorseUtil
{
	private static Entity empty_entity = new Entity();
    
    private static ManualLogSource _log => Plugin.LogInstance;

	private static Dictionary<string, PrefabGUID> HorseGuids = new()
	{
			{ "Regular", new(1149585723) },
			//{ "Gloomrot", new(1213710323) }, //HARD CRASH 
			{ "Spectral", new(2022889449) },
            //{ "Vampire", new(-1502865710) }, //HARD CRASH // CHAR_Mount_Horse
	};


	private static System.Random _r = new System.Random();
	internal static void SpawnHorse(int countlocal, float3 localPos, string horsetype = "Regular")
	{
		var horse = HorseGuids[horsetype];
		VWorld.Server.GetExistingSystemManaged<UnitSpawnerUpdateSystem>().SpawnUnit(empty_entity, horse, localPos, countlocal, 1, 2, -1);
	}

	internal static NativeArray<Entity> GetHorses()
	{
		var horseQuery = VWorld.Server.EntityManager.CreateEntityQuery(new EntityQueryDesc()
		{
			All = new[] { ComponentType.ReadWrite<FeedableInventory>(),
					ComponentType.ReadWrite<NameableInteractable>(),
					ComponentType.ReadWrite<Mountable>(),
					ComponentType.ReadOnly<LocalToWorld>(),
					ComponentType.ReadOnly<Team>()
				},
			None = new[] { ComponentType.ReadOnly<Dead>(), ComponentType.ReadOnly<DestroyTag>() }
		});

		return horseQuery.ToEntityArray(Allocator.Temp);
	}

	internal static Entity? GetClosestHorse(Entity e)
	{
		var horseEntityQuery = GetHorses();

		var origin = VWorld.Server.EntityManager.GetComponentData<LocalToWorld>(e).Position;
		var closest = float.MaxValue;

		Entity? closestHorse = null;
		foreach (var horse in horseEntityQuery)
		{
			var position = VWorld.Server.EntityManager.GetComponentData<LocalToWorld>(horse).Position;
			var distance = UnityEngine.Vector3.Distance(origin, position); // wait really?
			if (distance < closest)
			{
				closest = distance;
				closestHorse = horse;
			}
		}

		return closestHorse;
	}

	internal static List<Entity> ClosestHorses(Entity e, float radius = 5f)
	{
		var horses = GetHorses();
		var results = new List<Entity>();
		var origin = VWorld.Server.EntityManager.GetComponentData<LocalToWorld>(e).Position;

		foreach (var horse in horses)
		{
			var position = VWorld.Server.EntityManager.GetComponentData<LocalToWorld>(horse).Position;
			var distance = UnityEngine.Vector3.Distance(origin, position); // wait really?
			if (distance < radius)
			{
				results.Add(horse);
			}
		}

		return results;
	}
	
	// internal static NativeArray<Entity> ClosestHorses(Entity e, bool nativearray, float radius = 5f)
	// {
	// 	var horses = GetHorses();
	// 	var results = new List<Entity>();
	// 	var origin = VWorld.Server.EntityManager.GetComponentData<LocalToWorld>(e).Position;
	//
	// 	foreach (var horse in horses)
	// 	{
	// 		var position = VWorld.Server.EntityManager.GetComponentData<LocalToWorld>(horse).Position;
	// 		var distance = UnityEngine.Vector3.Distance(origin, position);
	// 		if (distance < radius)
	// 		{
	// 			results.Add(horse);
	// 		}
	// 	}
	//
	// 	// Convert the list to an array
	// 	Entity[] resultsArray = results.ToArray();
	//
	// 	// Create a NativeArray from the array
	// 	NativeArray<Entity> resultsNativeArray = new NativeArray<Entity>(resultsArray.Length, Allocator.Temp);
	//
	// 	// Copy the array to the NativeArray
	// 	for (int i = 0; i < resultsArray.Length; i++)
	// 	{
	// 		resultsNativeArray[i] = resultsArray[i];
	// 	}
	//
	// 	return resultsNativeArray;
	// }
	
	private static bool IsHorseWeChange(Entity horse, EntityManager instance)
    {
        if (horse == Entity.Null)
        {
            _log?.LogDebug("Horse is null.");
            return false;
        }
    
        if (instance.World == null)
        {
            _log?.LogDebug("Instance.World is null.");
            return false;
        }
    
        EntityManager em = instance;
        _log?.LogDebug($"Checking if horse <{horse.Index}> is a horse.");
        if (em.HasComponent<Team>(horse))
        {
            // _log?.LogDebug($"Horse <{horse.Index}> has Team component.");
            //var teamhorse = getTeam[horse];
            // _log?.LogDebug($"Team of horse <{horse.Index}> is {teamhorse}");
            // var isUnit = Team.IsInUnitTeam(teamhorse);
            // _log?.LogDebug($"Is horse <{horse.Index}> a unit? {isUnit}");
            var isUnit = true;
            // Wild horses are Units, appear to no longer be units after you ride them.
            // return true if it is a horse false will be a claimed horse and hopefully we dont need to change them?
            return isUnit;
        }
    
        // Handle the case when the horse entity does not have the Team component.
        _log?.LogDebug($"Horse <{horse.Index}> does not have Team component. {horse}");
        return false;
    }
    
    // public static void addCustomComponents(NativeArray<Entity> horses, EntityManager __instance)
    // {
    //     _log?.LogDebug($"FeedableInventorySystem_Spawn_Patch Prefix - Horses Found");
    //     foreach (var horseEntity in horses)
    //     {
    //         if (!IsHorseWeChange(horseEntity, __instance)) continue;
    //         _log?.LogDebug($"Horse <{horseEntity.Index}> Found.");
    //
    //
    //         var hasMountable = __instance.HasComponent<Mountable>(horseEntity);
    //         _log?.LogDebug($"Horse <{horseEntity.Index}> has Mountable: {hasMountable}");
    //         if (!hasMountable)
    //         {
    //             var addedSpawnBuffElement = __instance.AddComponent<Mountable>(horseEntity);
    //             _log?.LogDebug($"trying to add Mountable to horse <{horseEntity.Index}>");
    //             if (!addedSpawnBuffElement)
    //             {
    //                 _log?.LogDebug($"Failed to add Mountable to horse <{horseEntity.Index}>");
    //                 continue;
    //             }
    //         }
    //         
    //         //instead of adding the vampire mount buff, we'll add the abilities to the original mount buff
    //         //horseEntity.WithComponentDataH((ref Mountable mb) => { mb.MountBuff = new PrefabGUID(-978792376); });
    //         _log?.LogDebug($"Added Mountable to horse <{horseEntity.Index}>");
    //
    //
    //         var hasSpawnBuffElement = __instance.HasComponent<SpawnBuffElement>(horseEntity);
    //         _log?.LogDebug($"Horse <{horseEntity.Index}> has SpawnBuffElement: {hasSpawnBuffElement}");
    //         if (!hasSpawnBuffElement)
    //         {
    //             var addedSpawnBuffElement = __instance.AddComponent<SpawnBuffElement>(horseEntity);
    //             _log?.LogDebug($"trying to add SpawnBuffElement to horse <{horseEntity.Index}>");
    //             if (!addedSpawnBuffElement)
    //             {
    //                 _log?.LogDebug($"Failed to add SpawnBuffElement to horse <{horseEntity.Index}>");
    //                 continue;
    //             }
    //         }
    //
    //         horseEntity.WithComponentDataH((ref SpawnBuffElement sbe) =>
    //         {
    //             sbe.Kind = SpawnBuffKind.Default;
    //             sbe.Buff = new PrefabGUID(507944752);
    //             sbe.OriginPositionFactor = 0;
    //             sbe.Weight = 1;
    //         });
    //         _log?.LogDebug($"Added SpawnBuffElement to horse <{horseEntity.Index}>");
    //
    //
    //         var hasScriptApplyBuffUnderHealthThresholdDataServer =
    //             __instance.HasComponent<Script_ApplyBuffUnderHealthThreshold_DataServer>(horseEntity);
    //         _log?.LogDebug(
    //             $"Horse <{horseEntity.Index}> has Script_ApplyBuffUnderHealthThreshold_DataServer: {hasScriptApplyBuffUnderHealthThresholdDataServer}");
    //         if (!hasScriptApplyBuffUnderHealthThresholdDataServer)
    //         {
    //             var addedScriptApplyBuffUnderHealthThresholdDataServer =
    //                 __instance.AddComponent<Script_ApplyBuffUnderHealthThreshold_DataServer>(horseEntity);
    //             _log?.LogDebug(
    //                 $"trying to add Script_ApplyBuffUnderHealthThreshold_DataServer to horse <{horseEntity.Index}>");
    //             if (!addedScriptApplyBuffUnderHealthThresholdDataServer)
    //             {
    //                 _log?.LogDebug(
    //                     $"Failed to add Script_ApplyBuffUnderHealthThreshold_DataServer to horse <{horseEntity.Index}>");
    //                 continue;
    //             }
    //         }
    //
    //         horseEntity.WithComponentDataH((ref Script_ApplyBuffUnderHealthThreshold_DataServer sabuhtds) =>
    //         {
    //             sabuhtds.HealthFactor = 0;
    //             sabuhtds.NewBuffEntity = new PrefabGUID(525019977);
    //             sabuhtds.TriggerSequence = SequenceGUID.Empty;
    //             sabuhtds.OnDamageTakenListener = new ListenerId();
    //             sabuhtds.ThresholdMet = false;
    //             sabuhtds.DontTriggerOnDots = false;
    //             sabuhtds.DontTriggerInFlight = false;
    //         });
    //         _log?.LogDebug($"Added Script_ApplyBuffUnderHealthThreshold_DataServer to horse <{horseEntity.Index}>");
    //
    //         var hasGetTranslationOnSpawn = __instance.HasComponent<GetTranslationOnSpawn>(horseEntity);
    //         _log?.LogDebug($"Horse <{horseEntity.Index}> has GetTranslationOnSpawn: {hasGetTranslationOnSpawn}");
    //         if (!hasGetTranslationOnSpawn)
    //         {
    //             var addedGetTranslationOnSpawn =
    //                 __instance.AddComponent<GetTranslationOnSpawn>(horseEntity);
    //             _log?.LogDebug($"trying to add GetTranslationOnSpawn to horse <{horseEntity.Index}>");
    //             if (!addedGetTranslationOnSpawn)
    //             {
    //                 _log?.LogDebug($"Failed to add GetTranslationOnSpawn to horse <{horseEntity.Index}>");
    //                 continue;
    //             }
    //         }
    //
    //         horseEntity.WithComponentDataH((ref GetTranslationOnSpawn gtos) =>
    //         {
    //             gtos.TranslationSource = GetTranslationSource.Creator;
    //             gtos.SnapToGround = false;
    //         });
    //         _log?.LogDebug($"Added GetTranslationOnSpawn to horse <{horseEntity.Index}>");
    //
    //         var hasSaddleBearer = __instance.HasComponent<SaddleBearer>(horseEntity);
    //         _log?.LogDebug($"Horse <{horseEntity.Index}> has SaddleBearer: {hasSaddleBearer}");
    //         if (!hasSaddleBearer)
    //         {
    //             var addedSaddleBearer = __instance.AddComponent<SaddleBearer>(horseEntity);
    //             _log?.LogDebug($"trying to add SaddleBearer to horse <{horseEntity.Index}>");
    //             if (!addedSaddleBearer)
    //             {
    //                 _log?.LogDebug($"Failed to add SaddleBearer to horse <{horseEntity.Index}>");
    //                 continue;
    //             }
    //         }
    //
    //         horseEntity.WithComponentDataH((ref SaddleBearer sb) => { sb.SaddleId = PrefabGUID.Empty; });
    //         _log?.LogDebug($"Added SaddleBearer to horse <{horseEntity.Index}>");
    //
    //
    //         var hasImmortal = __instance.HasComponent<Immortal>(horseEntity);
    //         _log?.LogDebug($"Horse <{horseEntity.Index}> has Immortal: {hasImmortal}");
    //         if (!hasImmortal)
    //         {
    //             var addedImmortal = __instance.AddComponent<Immortal>(horseEntity);
    //             _log?.LogDebug($"trying to add Immortal to horse <{horseEntity.Index}>");
    //             if (!addedImmortal)
    //             {
    //                 _log?.LogDebug($"Failed to add Immortal to horse <{horseEntity.Index}>");
    //                 continue;
    //             }
    //         }
    //
    //         horseEntity.WithComponentDataH((ref Immortal imm) => { imm.IsImmortal = true; });
    //         _log?.LogDebug($"Added Immortal to horse <{horseEntity.Index}>");
    //
    //
    //         var hasApplyBuffOnGameplayEvent =
    //             __instance.HasComponent<ApplyBuffOnGameplayEvent>(horseEntity);
    //         _log?.LogDebug($"Horse <{horseEntity.Index}> has ApplyBuffOnGameplayEvent: {hasApplyBuffOnGameplayEvent}");
    //         if (!hasApplyBuffOnGameplayEvent)
    //         {
    //             var addApplyBuffOnGameplayEvent =
    //                 __instance.AddComponent<ApplyBuffOnGameplayEvent>(horseEntity);
    //             _log?.LogDebug($"trying to add ApplyBuffOnGameplayEvent to horse <{horseEntity.Index}>");
    //             if (!addApplyBuffOnGameplayEvent)
    //             {
    //                 _log?.LogDebug($"Failed to add ApplyBuffOnGameplayEvent to horse <{horseEntity.Index}>");
    //                 continue;
    //             }
    //         }
    //
    //         horseEntity.WithComponentDataH((ref ApplyBuffOnGameplayEvent aboge) =>
    //         {
    //             aboge.BuffTarget = ApplyBuffTarget.Self;
    //             aboge.SpellTarget = SetSpellTarget.Self;
    //             aboge.EntityOwner = SetEntityOwner.EventTarget;
    //             aboge.OverrideDuration = new Nullable_Unboxed<float>();
    //             aboge.Stacks = 1;
    //             aboge.Buff0 = new PrefabGUID(685381065);
    //             aboge.Buff1 = new PrefabGUID(1600351239);
    //             aboge.Buff2 = new PrefabGUID(307775055);
    //             aboge.Buff3 = PrefabGUID.Empty;
    //             aboge.EventOnConsume = new GameplayEventId
    //             {
    //                 GameplayEventType = GameplayEventType.Local,
    //                 EventId = 0
    //             };
    //             aboge.ConsumeIfAlreadyExists = false;
    //             aboge.ConsumeConditional = BlobAssetReference<ConditionBlob>.Null;
    //             aboge.CustomAbilitySpellModsSource = PrefabGUID.Empty;
    //         });
    //         _log?.LogDebug($"Added ApplyBuffOnGameplayEvent to horse <{horseEntity.Index}>");
    //
    //         var hasCreateGameplayEventsOnSpawn =
    //             __instance.HasComponent<CreateGameplayEventsOnSpawn>(horseEntity);
    //         _log?.LogDebug(
    //             $"Horse <{horseEntity.Index}> has CreateGameplayEventsOnSpawn: {hasCreateGameplayEventsOnSpawn}");
    //         if (!hasCreateGameplayEventsOnSpawn)
    //         {
    //             var addCreateGameplayEventsOnSpawn =
    //                 __instance.AddComponent<CreateGameplayEventsOnSpawn>(horseEntity);
    //             _log?.LogDebug($"trying to add CreateGameplayEventsOnSpawn to horse <{horseEntity.Index}>");
    //             if (!addCreateGameplayEventsOnSpawn)
    //             {
    //                 _log?.LogDebug($"Failed to add CreateGameplayEventsOnSpawn to horse <{horseEntity.Index}>");
    //                 continue;
    //             }
    //         }
    //
    //         horseEntity.WithComponentDataH((ref CreateGameplayEventsOnSpawn cgeos) =>
    //         {
    //             cgeos.EventId = new GameplayEventId
    //             {
    //                 GameplayEventType = GameplayEventType.Local,
    //                 EventId = 373290447
    //             };
    //             cgeos.Target = GameplayEventTarget.Owner;
    //         });
    //         _log?.LogDebug($"Added components to horse <{horseEntity.Index}>");
    //     }
    // }
    //
    // public static void RemoveCustomComponents(NativeArray<Entity> horses, EntityManager __instance)
    // {
    //     _log?.LogDebug($"FeedableInventorySystem_Spawn_Patch Prefix - Horses Found");
    //     foreach (var horseEntity in horses)
    //     {
    //         if (!IsHorseWeChange(horseEntity, __instance)) continue;
    //         _log?.LogDebug($"Horse <{horseEntity.Index}> Found.");
    //
    //
    //         var hasMountable = __instance.HasComponent<Mountable>(horseEntity);
    //         _log?.LogDebug($"Horse <{horseEntity.Index}> has Mountable: {hasMountable}");
    //         if (hasMountable)
    //         {
    //             horseEntity.WithComponentDataH((ref Mountable mb) => { mb.MountBuff = new PrefabGUID(854656674); });
    //             _log?.LogDebug($"Undid Mountable from horse <{horseEntity.Index}>");
    //         }
    //
    //         var hasSpawnBuffElement = __instance.HasComponent<SpawnBuffElement>(horseEntity);
    //         _log?.LogDebug($"Horse <{horseEntity.Index}> has SpawnBuffElement: {hasSpawnBuffElement}");
    //         if (hasSpawnBuffElement)
    //         {
    //             horseEntity.WithComponentDataH((ref SpawnBuffElement sbe) =>
    //             {
    //                 sbe.Kind = SpawnBuffKind.Default;
    //                 sbe.Buff = new PrefabGUID(396339796);
    //                 sbe.OriginPositionFactor = 0;
    //                 sbe.Weight = 1;
    //             });
    //             _log?.LogDebug($"Undid SpawnBuffElement from horse <{horseEntity.Index}>");
    //         }
    //         
    //         var hasSaddleBearer = __instance.HasComponent<SaddleBearer>(horseEntity);
    //         _log?.LogDebug($"Horse <{horseEntity.Index}> has SaddleBearer: {hasSaddleBearer}");
    //         if (hasSaddleBearer)
    //         {
    //             var addedSaddleBearer = __instance.RemoveComponent<SaddleBearer>(horseEntity);
    //             _log?.LogDebug($"trying to remove SaddleBearer from horse <{horseEntity.Index}>");
    //             if (addedSaddleBearer)
    //             {
    //                 _log?.LogDebug($"Removed SaddleBearer from horse <{horseEntity.Index}>");
    //             }
    //             else
    //             {
    //                 _log?.LogDebug($"Failed to remove SaddleBearer from horse <{horseEntity.Index}>");
    //             }
    //         }
    //
    //         var hasImmortal = __instance.HasComponent<Immortal>(horseEntity);
    //         _log?.LogDebug($"Horse <{horseEntity.Index}> has Immortal: {hasImmortal}");
    //         if (hasImmortal)
    //         {
    //             var addedImmortal = __instance.RemoveComponent<Immortal>(horseEntity);
    //             _log?.LogDebug($"trying to remove Immortal from horse <{horseEntity.Index}>");
    //             if (!addedImmortal)
    //             {
    //                 _log?.LogDebug($"Failed to remove Immortal from horse <{horseEntity.Index}>");
    //             }
    //             else
    //             {
    //                 _log?.LogDebug($"Removed Immortal from horse <{horseEntity.Index}>");
    //             }
    //         }
    //         
    //         _log?.LogDebug($"Removed components from horse <{horseEntity.Index}>");
    //     }
    // }
}
