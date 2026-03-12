using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            // Check if the instance is null.
            if (_instance == null)
            {
                // First try to find the object already in the scene.
                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    // Couldn't find the singleton in the scene, so make it.
                    GameObject singleton = new GameObject(typeof(T).Name);
                    _instance = singleton.AddComponent<T>();
                }
            }

            return _instance;
        }
    }

    public bool DestroyOnLoad;

    protected virtual void Awake()
    {
#if UNITY_EDITOR
        // Debug.Log($"<color=blue>Singleton awake:</color> {typeof(T)}");
#endif
        if (_instance == null)
        {
            _instance = this as T;
            if (!transform.parent && !DestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
