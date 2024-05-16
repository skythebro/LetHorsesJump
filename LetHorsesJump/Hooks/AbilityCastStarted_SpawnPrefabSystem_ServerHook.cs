using BepInEx.Logging;
using Bloodstone.API;
using HarmonyLib;
using LetHorsesJump.Utils;
using ProjectM;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;


namespace LetHorsesJump.Hooks;

[HarmonyPatch]
public static class AbilityCastStarted_SpawnPrefabSystem_ServerHook
{
    private static ManualLogSource _log => Plugin.LogInstance;


    [HarmonyPatch(typeof(AbilityCastStarted_SpawnPrefabSystem_Server),
        nameof(AbilityCastStarted_SpawnPrefabSystem_Server.OnUpdate))]
    [HarmonyPrefix]
    private static void OnUpdate(AbilityCastStarted_SpawnPrefabSystem_Server __instance)
    {
        if (!VWorld.IsServer || __instance.__query_577032082_0.IsEmpty) return;
        //var prefabCollectionSystem =
        //    __instance.EntityManager.World.GetExistingSystemManaged<PrefabCollectionSystem>();
        var entities = __instance.__query_577032082_0.ToEntityArray(Allocator.Temp);
        foreach (var entity in entities)
        {
            var abEvent =
                __instance.EntityManager.GetComponentDataAOT<AbilityCastStartedEvent>(entity);
            var ability = abEvent.Ability;
            var character = abEvent.Character;
            if (__instance.EntityManager.TryGetComponentData<PrefabGUID>(ability, out var prefabGUID))
            {
                
                if (prefabGUID != new PrefabGUID(-372722894))
                {
                    if (prefabGUID.GuidHash == 0)
                    {
                        continue;
                    }
                    //_log.LogDebug($"[PrefabGUIDName that is not the one im looking for][{prefabCollectionSystem.PrefabGuidToNameDictionary[prefabGUID]}]");
                    continue;
                    // //_log.LogDebug("Ability is AB_Gallop_Cast");
                }
            }
            
            //_log.LogDebug($"[PrefabGUIDName][{prefabCollectionSystem.PrefabGuidToNameDictionary[prefabGUID]}]");
            
            
            
            __instance.EntityManager.TryGetComponentData<PrefabGUID>(ability, out var abilityguid);
            //_log.LogDebug($"[Ability][{prefabCollectionSystem.PrefabGuidToNameDictionary[abilityguid]}]");
            __instance.EntityManager.TryGetComponentData<PrefabGUID>(character, out var characterguid);
            //_log.LogDebug($"[Character][{prefabCollectionSystem.PrefabGuidToNameDictionary[characterguid]}]");

            var horseEntity = HorseUtil.GetClosestHorse(character);
            if (horseEntity == Entity.Null)
            {
                //_log.LogDebug($"No horse found");
                return;
            }

            Entity horse = horseEntity.Value;

            var hasPrefabGuid = __instance.EntityManager.HasComponent<PrefabGUID>(horse);
            //_log.LogDebug($"hasPrefabGuid = {hasPrefabGuid}");
            if (hasPrefabGuid)
            {
                __instance.EntityManager.TryGetComponentData<PrefabGUID>(horse,
                    out var horsePrefabGUID);
                //_log.LogDebug(
                   // $"[HorseName][{prefabCollectionSystem.PrefabGuidToNameDictionary[horsePrefabGUID]}]");
                //_log.LogDebug($"Horse has prefabGUID: {horsePrefabGUID}");
                if (horsePrefabGUID == new PrefabGUID(-1502865710))
                {
                    //_log.LogDebug($"Horse with prefabGUID {horsePrefabGUID} is vampire horse");
                    // if the horse is a vampire horse dont change anything
                    continue;
                }

                if (horsePrefabGUID == new PrefabGUID(1149585723))
                {
                    //_log.LogDebug($"Horse with prefabGUID {horsePrefabGUID} is normal horse");
                }

                if (horsePrefabGUID == new PrefabGUID(2022889449))
                {
                    //_log.LogDebug($"Horse with prefabGUID {horsePrefabGUID} is spectral horse");
                }

                if (horsePrefabGUID == new PrefabGUID(1213710323))
                {
                    //_log.LogDebug($"Horse with prefabGUID {horsePrefabGUID} is gloomrot horse");
                }

                var hasMountable = __instance.EntityManager.HasComponent<Mountable>(horse);
                if (!hasMountable)
                {
                    //_log.LogDebug($"Adding Mountable to horse");
                    __instance.EntityManager.AddComponentData(horse, new Mountable());
                }


                horse.WithComponentDataH((ref Mountable mb) => { mb.MountBuff = new PrefabGUID(854656674); });

                var hasSaddleBearer = __instance.EntityManager.HasComponent<SaddleBearer>(horse);
                if (!hasSaddleBearer)
                {
                    //_log.LogDebug($"Adding SaddleBearer to horse");
                    __instance.EntityManager.AddComponentData(horse, new SaddleBearer());
                }

                var hasImmortal = __instance.EntityManager.HasComponent<Immortal>(horse);
                if (!hasImmortal)
                {
                    //_log.LogDebug($"Adding Immortal to horse");
                    __instance.EntityManager.AddComponentData(horse, new Immortal()
                    {
                        IsImmortal = true
                    });
                }
                //_log.LogDebug($"Finished adding and changing components from horse");
                return;
            }
            //_log.LogDebug($"Horse doesn't have prefabGUID");
            
        }
    }
}