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

        startMatchmakingButton.clicked += StartMatchmaking;
    }

    private void StartMatchmaking()
    {
        statusLabel.text = "Matchmaking...";
        startMatchmakingButton.SetEnabled(false);
        webSocketClient.Send("matchmaking_request", null).ConfigureAwait(false);
    }

    private void HandleMessageReceived(string eventName, Dictionary<string, object> data)
    {
        switch (eventName)
        {
            case "lobby_ready":
                OnLobbyReady(data);
                break;
        }
    }
    private void OnLobbyReady(Dictionary<string, object> data)
    {
        string lobbyID = data["lobby_id"] != null ? data["lobby_id"].ToString() : "Unknown";
        statusLabel.text = $"Lobby trovata: {lobbyID}";
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameSceneDemo");
    }
}