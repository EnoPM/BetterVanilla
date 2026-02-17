using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class DestroyableSingletonExtensions
{
    extension<T>(T singleton) where T : DestroyableSingleton<T>
    {
        public void BaseAwake()
        {
            if (DestroyableSingleton<T>._instance == null)
            {
                DestroyableSingleton<T>._instance = singleton;
                if (!singleton.DontDestroy) return;
                Object.DontDestroyOnLoad(singleton.gameObject);
            }
            else
            {
                if (DestroyableSingleton<T>._instance != singleton) return;
                Object.Destroy(singleton.gameObject);
            }
        }

        public void BaseOnDestroy()
        {
            if (singleton.DontDestroy || DestroyableSingleton<T>._instance != singleton) return;
            DestroyableSingleton<T>._instance = null!;
        }
    }
}