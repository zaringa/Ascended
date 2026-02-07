using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Ensures only one EventSystem exists across all scenes.
/// This manager uses [RuntimeInitializeOnLoadMethod] to run before any scene loads.
/// NO NEED TO ATTACH THIS SCRIPT TO ANY GAMEOBJECT - it runs automatically.
/// </summary>
public class EventSystemManager
{
    private static EventSystem mainEventSystem;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // Subscribe to scene loaded event
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        CleanupDuplicateEventSystems();
    }

    private static void CleanupDuplicateEventSystems()
    {
        EventSystem[] eventSystems = Object.FindObjectsOfType<EventSystem>();

        if (eventSystems.Length == 0)
        {
            // No EventSystem found, create one
            GameObject eventSystemGO = new GameObject("EventSystem");
            mainEventSystem = eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
            Object.DontDestroyOnLoad(eventSystemGO);
            Debug.Log("EventSystemManager: Created new EventSystem.");
            return;
        }

        // If we already have a main EventSystem
        if (mainEventSystem != null)
        {
            // Destroy all EventSystems except the main one
            foreach (EventSystem es in eventSystems)
            {
                if (es != mainEventSystem)
                {
                    //Debug.Log($"EventSystemManager: Destroying duplicate EventSystem '{es.gameObject.name}'");
                    Object.DestroyImmediate(es.gameObject);
                }
            }
        }
        else
        {
            mainEventSystem = eventSystems[0];
            Object.DontDestroyOnLoad(mainEventSystem.gameObject);
            //Debug.Log($"EventSystemManager: Keeping EventSystem '{mainEventSystem.gameObject.name}' as main.");

            for (int i = 1; i < eventSystems.Length; i++)
            {
                //Debug.Log($"EventSystemManager: Destroying duplicate EventSystem '{eventSystems[i].gameObject.name}'");
                Object.DestroyImmediate(eventSystems[i].gameObject);
            }
        }
    }
}