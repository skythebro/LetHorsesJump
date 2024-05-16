using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using LetHorsesJump.Utils;
using System;
using Bloodstone.API;
using Il2CppInterop.Runtime;
using ProjectM;
using ProjectM.Hybrid;
using ProjectM.Scripting;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using VampireCommandFramework;

namespace LetHorsesJump;

public static partial class Commands
{
    private static ManualLogSource _log => Plugin.LogInstance;

    static Commands()
    {
        Enabled = IL2CPPChainloader.Instance.Plugins.TryGetValue("gg.deca.VampireCommandFramework", out var info);
        if (Enabled) _log.LogWarning($"VCF Version: {info.Metadata.Version}");
    }

    public static bool Enabled { get; private set; }

    public static void Register() => CommandRegistry.RegisterAll();
    public static void Unregister() => CommandRegistry.UnregisterAssembly();

    private static System.Random _random = new();

    public record Horse(Entity Entity);

    public class NamedHorseConverter : CommandArgumentConverter<Horse, ChatCommandContext>
    {
        const float radius = 25f;

        public override Horse Parse(ChatCommandContext ctx, string input)
        {
            var horses = HorseUtil.ClosestHorses(ctx.Event.SenderCharacterEntity, radius);

            foreach (var horse in horses)
            {
                var name = VWorld.Server.EntityManager.GetComponentData<NameableInteractable>(horse);
                if (name.Name.ToString().Contains(input, StringComparison.OrdinalIgnoreCase))
                {
                    return new Horse(horse);
                }
            }

            throw ctx.Error($"Could not find a horse within {radius:F1} units named like \"{input}\"");
        }

        [CommandGroup("horsejump", "hj")]
        public class HorseCommands
        {
            private Horse GetRequiredClosestHorse(ChatCommandContext ctx)
            {
                var closest = HorseUtil.GetClosestHorse(ctx.Event.SenderCharacterEntity);
                if (closest == null)
                {
                    throw ctx.Error("No closest horse and expected to find one based on usage.");
                }

                return new Horse(closest.Value);
            }

            [Command("spawn", adminOnly: true)]
            public void HorseMe(ChatCommandContext ctx, int num = 1, string horseType = "Regular")
            {
                switch (horseType)
                {
                    case "Regular":
                    //case "Gloomrot":
                    case "Spectral":
                    //case "Vampire":
                        float3 localPos = VWorld.Server.EntityManager
                            .GetComponentData<Translation>(ctx.Event.SenderUserEntity).Value;
                        HorseUtil.SpawnHorse(num, localPos, horseType);
                        ctx.Reply($"Spawned {num} horse{(num > 1 ? "s" : "")} of type: {horseType} near you.");
                        break;
                 default:   
                     ctx.Reply($"Horse of type: {horseType} not found.");
                     break;
                }
            }

            [Command("whistle", shortHand: "w", adminOnly: true)]
            public void Whistle(ChatCommandContext ctx, Horse horse = null)
            {
                horse ??= GetRequiredClosestHorse(ctx);
                float3 userPos = VWorld.Server.EntityManager
                    .GetComponentData<Translation>(ctx.Event.SenderUserEntity).Value;
                float3 horsePos = VWorld.Server.EntityManager.GetComponentData<Translation>(horse.Entity).Value;

                horse.Entity.WithComponentDataH((ref Translation t) => { t.Value = userPos; });
                ctx.Reply("Horse moved to you.");
            }

            // [Command("convert", adminOnly: true)]
            // public void Convert(ChatCommandContext ctx, float radius = 5f)
            // {
            //     var horses = HorseUtil.ClosestHorses(ctx.Event.SenderCharacterEntity, true, radius);
            //     var count = horses.Length;
            //     HorseUtil.addCustomComponents(horses, VWorld.Server.EntityManager);
            //
            //     ctx.Reply($"Edited {count} horses.");
            // }
            //
            // [Command("reset", adminOnly: true)]
            // public void Unconvert(ChatCommandContext ctx, float radius = 5f)
            // {
            //     var horses = HorseUtil.ClosestHorses(ctx.Event.SenderCharacterEntity, true, radius);
            //     var count = horses.Length;
            //     HorseUtil.RemoveCustomComponents(horses, VWorld.Server.EntityManager);
            //
            //     ctx.Reply($"unedited {count} horses.");
            // }

            [Command("kill", adminOnly: true)]
            public void Kill(ChatCommandContext ctx, Horse horse = null)
            {
                horse ??= GetRequiredClosestHorse(ctx);
                horse.Entity.WithComponentDataH((ref Health t) => { t.IsDead = true; });
                VWorld.Server.EntityManager.AddComponent(horse.Entity, Il2CppType.Of<Dead>());
                ctx.Reply($"Horse removed.");
            }

            [Command("cull", adminOnly: true)]
            public void Kill(ChatCommandContext ctx, float radius = 5f, float percentage = 1f)
            {
                var horses = HorseUtil.ClosestHorses(ctx.Event.SenderCharacterEntity, radius);
                var count = horses.Count;
                var toRemove = Math.Clamp((int)(count * percentage), 0, count);
                var remaining = toRemove;
                foreach (var horse in horses)
                {
                    if (remaining == 0) break;
                    horse.WithComponentDataH((ref Health t) => { t.IsDead = true; });
                    VWorld.Server.EntityManager.AddComponent(horse, Il2CppType.Of<Dead>());
                    remaining--;
                }

                ctx.Reply($"Removed {toRemove} horses.");
            }
        }
    }
}