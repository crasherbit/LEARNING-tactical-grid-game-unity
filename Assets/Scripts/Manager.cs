using UnityEngine;
using UnityEngine.UIElements;

public abstract class Manager<T> : MonoBehaviour where T : Manager<T>
{
    public static T Instance { get; private set; }
    protected WebSocketClient webSocketClient;
    protected void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be two " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        if (Instance == null)
        {
            Instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }
    }
    protected void Start()
    {
        webSocketClient = WebSocketClient.Instance;
    }
}