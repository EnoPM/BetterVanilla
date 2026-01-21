using System;
using System.Diagnostics.CodeAnalysis;
using Il2CppInterop.Runtime.InteropTypes;

namespace BetterVanilla.Extensions;

public static class Il2CppObjectBaseExtensions
{
    extension(Il2CppObjectBase baseObject)
    {
        public bool Is<T>([NotNullWhen(true)] out T? castedObject)
            where T : Il2CppObjectBase
        {
            castedObject = baseObject.TryCast<T>();
            return castedObject != null;
        }

        public bool Is<T>() where T : Il2CppObjectBase => baseObject.Is<T>(out _);

        public T As<T>() where T : Il2CppObjectBase
        {
            if (!baseObject.Is<T>(out var castedObject))
            {
                throw new InvalidOperationException($"Unable to cast {baseObject.GetType().Name} to {typeof(T).Name}");
            }
            return castedObject;
        }
    }
}