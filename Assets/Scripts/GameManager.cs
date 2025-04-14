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
    private VisualElement turnContainerElement;

    private void OnEnable()
    {
        var root = lobbyDocument.rootVisualElement;

        // Collega il pulsante e l'etichetta di stato
        endTurnButton = root.Q<Button>("end-turn-button");
        turnIndicatorLabel = root.Q<Label>("turn-indicator-label");
        turnContainerElement = root.Q<VisualElement>("turn-container");

        endTurnButton.clicked += EndTurn;
    }
    private void Start()
    {
        webSocketClient = WebSocketClient.Instance;
        webSocketClient.OnMessageReceived += HandleMessageReceived;
        webSocketClient.SendMessageAsync("init_data_request", null).ConfigureAwait(false);
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
            turnContainerElement.style.backgroundColor = new StyleColor(new Color(0.2f, 0.8f, 0.2f)); // Verde
            turnIndicatorLabel.text = "Your Turn";
            endTurnButton.SetEnabled(true);
        }
        else
        {
            turnContainerElement.style.backgroundColor = new StyleColor(new Color(0.8f, 0.2f, 0.2f)); // Rosso
            turnIndicatorLabel.text = "Opponent's Turn";
            endTurnButton.SetEnabled(false);
        }
    }
    public void OnTurnChange(WebSocketClient.ResponseData data)
    {
        isMyTurn = data.is_my_turn;
        UpdateTurnDisplay();
    }

    private void OnInitDataResponse(WebSocketClient.ResponseData data)
    {
        // Supponiamo che data contenga un oggetto con un campo "is_my_turn"
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
            case "init_data_response":
                OnInitDataResponse(data);
                break;
        }
    }


}