using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private bool isMyTurn = false;
    private WebSocketClient webSocketClient;

    // lobby UI elements
    [SerializeField] private UIDocument lobbyDocument;
    private Button endTurnButton;
    private Label turnIndicatorLabel;
    private void Start()
    {
        webSocketClient = WebSocketClient.Instance;
        webSocketClient.OnMessageReceived += HandleMessageReceived;
    }
    void OnEnable()
    {
        var root = lobbyDocument.rootVisualElement;

        // Collega il pulsante e l'etichetta di stato
        endTurnButton = root.Q<Button>("end-turn-button");
        turnIndicatorLabel = root.Q<Label>("turn-indicator-label");

        endTurnButton.clicked += EndTurn;
    }

    private void EndTurn()
    {
        Debug.Log("Turno terminato");
        // Invia il messaggio di fine turno al server
        webSocketClient.SendMessageAsync("end_turn", null).ConfigureAwait(false);
    }
    public void UpdateTurnDisplay()
    {
        if (isMyTurn)
        {
            turnIndicatorLabel.text = "Your Turn";
            endTurnButton.SetEnabled(true);
        }
        else
        {
            turnIndicatorLabel.text = "Opponent's Turn";
            endTurnButton.SetEnabled(false);
        }
    }
    public void OnTurnChange(WebSocketClient.ResponseData data)
    {
        isMyTurn = data.is_my_turn;

        UpdateTurnDisplay();
    }
    private void HandleMessageReceived(string eventName, WebSocketClient.ResponseData data)
    {
        switch (eventName)
        {
            case "turn_changed":
                OnTurnChange(data);
                break;
            default:
                Debug.Log($"Evento sconosciuto ricevuto: {eventName} con dati: {data}");
                break;
        }
    }


}