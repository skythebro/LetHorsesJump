using BepInEx.Logging;
using Bloodstone.API;
using HarmonyLib;
using ProjectM;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;

namespace LetHorsesJump.Hooks;

[HarmonyPatch]
public static class BuffSystemHook
{
    private static ManualLogSource _log => Plugin.LogInstance;


    [HarmonyPatch(typeof(BuffSystem_Spawn_Server), nameof(BuffSystem_Spawn_Server.OnUpdate))]
    [HarmonyPrefix]
    private static void OnUpdate(BuffSystem_Spawn_Server __instance)
    {
        if (!VWorld.IsServer || __instance.__query_401358634_0.IsEmpty) return;
        var entities = __instance.__query_401358634_0.ToEntityArray(Allocator.Temp);
        foreach (var entity in entities)
        {
            // Check if the buff entity has a PrefabGUID component
            if (__instance.EntityManager.HasComponent<PrefabGUID>(entity))
            {
                __instance.EntityManager.TryGetComponentData<PrefabGUID>(entity, out var prefabGUID);
                // is it a normal or vampire horse buff?
                if (prefabGUID == new PrefabGUID(854656674) || prefabGUID == new PrefabGUID(-978792376))
                {
                    var hasAttachComponent = __instance.EntityManager.HasComponent<Attach>(entity);

                    //_log.LogDebug($"hasAttachComponent = {hasAttachComponent}");
                    if (hasAttachComponent)
                    {
                        __instance.EntityManager.TryGetComponentData<EntityOwner>(entity, out var owner);
                        var player = owner.Owner;


                        var horseEntity = HorseUtil.GetClosestHorse(player);
                        if (horseEntity == Entity.Null)
                        {
                            //_log.LogDebug($"No horse found");
                            return;
                        }

                        Entity horse = horseEntity.Value;

                        //print name of prefabGUID
                        //var prefabCollectionSystem = __instance.EntityManager.World
                        //    .GetExistingSystemManaged<PrefabCollectionSystem>();
                        var hasPrefabGuid = __instance.EntityManager.HasComponent<PrefabGUID>(horse);
                        //_log.LogDebug($"hasPrefabGuid = {hasPrefabGuid}");
                        if (hasPrefabGuid)
                        {
                            __instance.EntityManager.TryGetComponentData<PrefabGUID>(horse,
                                out var horsePrefabGUID);
                            //_log.LogDebug(
                            //   $"[HorseName][{prefabCollectionSystem.PrefabGuidToNameDictionary[horsePrefabGUID]}]");
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
                        }
                    }
                }

                var hasbuffs =
                    __instance.EntityManager.TryGetBuffer<ReplaceAbilityOnSlotBuff>(entity, out var buffs);
                if (!hasbuffs) return;
                //_log.LogDebug($"Found buff with GUID: {prefabGUID}");
                //_log.LogDebug($"Buff count: {buffs.Length}");
                //_log.LogDebug($"Buff at slot 1: {buffs[1].NewGroupId} = 0?");
                // Check if the buffer has enough elements
                if (buffs.Length > 2 && buffs[1].NewGroupId == new PrefabGUID(0))
                {
                    // Get the buff at slot 2
                    ReplaceAbilityOnSlotBuff buff = buffs[1];

                    // Modify the properties of buff
                    buff.NewGroupId = new PrefabGUID(436038744); // AB_Horse_Vampire_Leap_Travel_AbilityGroup
                    buff.CastBlockType = GroupSlotModificationCastBlockType.PreCast;
                    buff.CopyCooldown = true;

                    // Write the modified buff back to the buffer
                    buffs[1] = buff;
                    //_log.LogDebug($"Modified buff at slot 1");
                }
            }
        }
    }
}