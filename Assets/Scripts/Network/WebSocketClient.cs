

using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public class WebSocketClient : MonoBehaviour
{
    public static WebSocketClient Instance;
    [SerializeField] private const string WebSocketUrl = "ws://localhost:8080/ws"; // Cambia l'URL in base al tuo server WebSocket
    private ClientWebSocket webSocket;
    private CancellationTokenSource cancellationTokenSource;
    private const int BufferSize = 1024;

    public event Action<string, Dictionary<string, object>> OnMessageReceived;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        // Inizializza la connessione WebSocket
        Connect(WebSocketUrl).ConfigureAwait(false);
    }
    public async Task Connect(string url)
    {
        webSocket = new ClientWebSocket();
        cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await webSocket.ConnectAsync(new Uri(url), cancellationTokenSource.Token);
            Debug.Log("WebSocket connesso al server.");

            // Avvia il loop per ricevere i messaggi
            _ = ReceiveLoop();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Errore durante la connessione WebSocket: {ex.Message}");
        }
    }

    public async Task Send(string eventName, Dictionary<string, object> data)
    {
        if (webSocket == null || webSocket.State != WebSocketState.Open)
        {
            Debug.LogError("WebSocket non connesso.");
            return;
        }

        var message = new Message { EventName = eventName, Data = data };
        string json = JsonConvert.SerializeObject(message);
        byte[] buffer = Encoding.UTF8.GetBytes(json);

        try
        {
            Debug.Log($"Messaggio inviato: {json}");
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Errore durante l'invio del messaggio: {ex.Message} stack: {ex.StackTrace}");
        }
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[BufferSize];

        while (webSocket.State == WebSocketState.Open)
        {
            try
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationTokenSource.Token);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationTokenSource.Token);
                }
                else
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Debug.Log($"Messaggio pre parser : {message}");
                    var parsedMessage = JsonConvert.DeserializeObject<Message>(message);
                    Debug.Log($"Messaggio parsed : {parsedMessage.EventName}");

                    OnMessageReceived?.Invoke(parsedMessage.EventName, parsedMessage.Data);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Errore durante la ricezione del messaggio: {ex.Message}");
                break;
            }
        }
    }

    private async void OnDestroy()
    {
        if (webSocket != null)
        {
            cancellationTokenSource.Cancel();
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Chiusura client", CancellationToken.None);
            webSocket.Dispose();
        }
    }

    [Serializable]
    private class Message
    {
        public string EventName { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}