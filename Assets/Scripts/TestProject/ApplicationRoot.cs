using System;
using UnityEngine;

public static class ApplicationRoot
{
    public static ApplicationContext Context { get; private set; }

    static ApplicationRoot ()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged += (x) =>
        {
            if (x == UnityEditor.PlayModeStateChange.ExitingPlayMode)
                DisposeContext();
        };
#endif
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static async void BeforeSceneLoad ()
    {
        Context?.Dispose();
        Context = new ApplicationContext();
        await Context.Initialize();
    }

    static void DisposeContext ()
    {
        Context?.Dispose();
    }
}
