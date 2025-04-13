using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WebSocketClient : MonoBehaviour
{
    public static WebSocketClient Instance { get; private set; }
    [SerializeField] private string serverUrl = "ws://localhost:8080/ws";

    private ClientWebSocket webSocket;
    private CancellationTokenSource cancellationTokenSource;
    private bool isConnected = false;

    // Event handlers
    public delegate void MessageEventHandler(string eventName, ResponseData data);
    public event MessageEventHandler OnMessageReceived;

    // Specific game events
    public event Action<string> OnError;
    public event Action OnConnected;
    public event Action OnDisconnected;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be two WebSocketClient." + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        // Connect on start
        ConnectAsync();
    }

    private void OnDestroy()
    {
        CloseConnectionAsync().ConfigureAwait(false);
    }

    public async Task ConnectAsync()
    {
        if (isConnected)
            return;

        try
        {
            cancellationTokenSource = new CancellationTokenSource();
            webSocket = new ClientWebSocket();

            Debug.Log($"Connecting to {serverUrl}...");
            await webSocket.ConnectAsync(new Uri(serverUrl), cancellationTokenSource.Token);

            isConnected = true;
            Debug.Log("WebSocket connected successfully");
            OnConnected?.Invoke();

            // Start listening for messages
            StartCoroutine(ReceiveLoop());
        }
        catch (Exception e)
        {
            Debug.LogError($"WebSocket connection error: {e.Message}");
            OnError?.Invoke($"Connection error: {e.Message}");
        }
    }

    private IEnumerator ReceiveLoop()
    {
        Task.Run(async () =>
        {
            byte[] buffer = new byte[4096];

            while (isConnected && webSocket.State == WebSocketState.Open)
            {
                try
                {
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), cancellationTokenSource.Token);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        isConnected = false;
                        Debug.Log("Server closed connection");

                        UnityMainThreadDispatcher.Instance.Enqueue(() =>
                        {
                            OnDisconnected?.Invoke();
                        });
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Debug.Log($"Message received: {message}");

                    ProcessMessage(message);
                }
                catch (Exception e)
                {
                    if (!cancellationTokenSource.IsCancellationRequested)
                    {
                        Debug.LogError($"Error receiving message: {e.Message}");
                        UnityMainThreadDispatcher.Instance.Enqueue(() =>
                        {
                            OnError?.Invoke($"Receive error: {e.Message}");
                        });
                    }

                    isConnected = false;
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        OnDisconnected?.Invoke();
                    });
                    break;
                }
            }
        });

        yield break;
    }

    private void ProcessMessage(string message)
    {
        try
        {
            // Assuming JSON format: {"eventName": "event_name", "data": {...}} Dictionary<string, object>
            ResponseJsonMessage jsonMessage = JsonUtility.FromJson<ResponseJsonMessage>(message);
            Debug.Log($"Processing message: {jsonMessage.eventName} with data: {jsonMessage.data.is_my_turn}");
            UnityMainThreadDispatcher.Instance.Enqueue(() => OnMessageReceived?.Invoke(jsonMessage.eventName, jsonMessage.data));
        }
        catch (Exception e)
        {
            Debug.LogError($"Error processing message: {e.Message}");
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                OnError?.Invoke($"Message processing error: {e.Message}");
            });
        }
    }

    public async Task SendMessageAsync(string eventName, string data)
    {
        if (!isConnected || webSocket.State != WebSocketState.Open)
        {
            Debug.LogError("Cannot send message: WebSocket is not connected");
            OnError?.Invoke("Cannot send message: WebSocket is not connected");
            return;
        }

        try
        {
            string message = JsonUtility.ToJson(new JsonMessage { eventName = eventName, data = data });
            byte[] bytes = Encoding.UTF8.GetBytes(message);

            await webSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                cancellationTokenSource.Token);

            Debug.Log($"Sent message: {message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error sending message: {e.Message}");
            OnError?.Invoke($"Send error: {e.Message}");
        }
    }

    public async Task CloseConnectionAsync()
    {
        if (!isConnected || webSocket == null)
            return;

        try
        {
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Client closing connection",
                    CancellationToken.None);
            }

            isConnected = false;
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            webSocket.Dispose();

            Debug.Log("WebSocket connection closed");
            OnDisconnected?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error closing connection: {e.Message}");
        }
    }

    [Serializable]
    private class JsonMessage
    {
        public string eventName;
        public string data;
    }

    // Helper class for JSON serialization/deserialization
    [Serializable]
    private class ResponseJsonMessage
    {
        public string eventName;
        public ResponseData data;
    }
    [Serializable]
    public class ResponseData
    {
        public string lobby_id;
        public string current_turn;
        public bool is_my_turn;

    }
}