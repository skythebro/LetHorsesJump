using Bloodstone.API;
using Il2CppInterop.Runtime;
using Il2CppSystem;
using Unity.Entities;

namespace LetHorsesJump.Utils;

public static class Il2cppHelper
{
    public static void WithComponentDataH<T>(this Entity entity, ActionRefVmod<T> action) where T : struct
    {
        VWorld.Game.EntityManager.TryGetComponentData<T>(entity, out var componentData);
        action(ref componentData);
        VWorld.Game.EntityManager.SetComponentData<T>(entity, componentData);
    }

    public static void WithComponentDataHAOT<T>(this Entity entity, ActionRefVmod<T> action) where T : unmanaged
    {
        var componentData = VWorld.Game.EntityManager.GetComponentDataAOT<T>(entity);
        action(ref componentData);
        VWorld.Game.EntityManager.SetComponentData<T>(entity, componentData);
    }

    private static Type GetType<T>() => Il2CppType.Of<T>();

    public static unsafe T GetComponentDataAOT<T>(this EntityManager entityManager, Entity entity) where T : unmanaged
    {
        var type = TypeManager.GetTypeIndex(GetType<T>());
        var result = (T*)entityManager.GetComponentDataRawRW(entity, type);
        return *result;
    }

    public delegate void ActionRefVmod<T>(ref T item);
}