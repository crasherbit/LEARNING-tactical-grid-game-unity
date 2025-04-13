using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    // lobby UI elements
    [SerializeField] private UIDocument lobbyDocument;
    private Button startMatchmakingButton;
    private Label statusLabel;
    private WebSocketClient webSocketClient;
    private void Start()
    {
        webSocketClient = WebSocketClient.Instance;
        webSocketClient.OnMessageReceived += HandleMessageReceived;
    }
    void OnEnable()
    {
        var root = lobbyDocument.rootVisualElement;

        // Collega il pulsante e l'etichetta di stato
        startMatchmakingButton = root.Q<Button>("start-matchmaking");
        statusLabel = root.Q<Label>("status-label");
        statusLabel.text = "Clicca per iniziare il matchmaking.";

        Debug.Log("Lobby UI elements initialized." + startMatchmakingButton);
        startMatchmakingButton.clicked += StartMatchmaking;
    }

    private void StartMatchmaking()
    {
        Debug.Log("Richiesta di matchmaking inviata al server.");
        statusLabel.text = "Matchmaking...";
        startMatchmakingButton.SetEnabled(false);
        webSocketClient.SendMessageAsync("matchmaking_request", null).ConfigureAwait(false);
    }

    private void HandleMessageReceived(string eventName, WebSocketClient.ResponseData data)
    {
        switch (eventName)
        {
            case "lobby_ready":
                OnLobbyReady(data);
                break;
            default:
                Debug.Log($"Evento sconosciuto ricevuto: {eventName} con dati: {data}");
                break;
        }
    }
    private void OnLobbyReady(WebSocketClient.ResponseData data)
    {
        // Supponiamo che data contenga un oggetto con un campo "lobbyID"
        string lobbyID = data.lobby_id;
        Debug.Log($"Lobby trovata con ID: {lobbyID}");
        statusLabel.text = $"Lobby trovata: {lobbyID}";
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameSceneDemo");
    }
}