using UnityEngine;

public static class ApplicationRoot
{
    static ApplicationContext context;

    static ApplicationRoot ()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged += (x) =>
        {
            if (x == UnityEditor.PlayModeStateChange.ExitingPlayMode)
                Dispose();
        };
#endif
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void BeforeSceneLoad ()
    {
        context?.Dispose();
        context = new ApplicationContext();
        context.Initialize();
    }

    static void Dispose ()
    {
        context?.Dispose();
    }
}
