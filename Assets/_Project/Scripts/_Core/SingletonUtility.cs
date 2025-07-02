using UnityEngine;

public static class SingletonUtility
{
    /// <summary>
    /// Turns a MonoBehaviour class into the Singleton pattern.
    /// </summary>
    /// <typeparam name="T">The type of the MonoBehaviour class implementing the Singleton.</typeparam>
    /// <param name="instance">A reference to the static instance of the class.</param>
    /// <param name="self">The current instance of the MonoBehaviour calling the method.</param>
    /// <param name="dontDestroyOnLoad">Indicates whether the instance should persist across scene loads (optional, defaults to true).</param>
    public static void MakeSingleton<T>(ref T instance, T self, bool dontDestroyOnLoad = true) where T : MonoBehaviour
    {
        if (instance == null)
        {
            instance = self;

            if (dontDestroyOnLoad)
            {
                var gameObject = self.gameObject;

                // Detach from parent to ensure it is a root GameObject
                if (gameObject.transform.parent != null)
                {
                    gameObject.transform.SetParent(null);
                }

                MonoBehaviour.DontDestroyOnLoad(gameObject);
            }
        }
        else if (instance != self)
        {
            MonoBehaviour.Destroy(self.gameObject);
        }
    }
}