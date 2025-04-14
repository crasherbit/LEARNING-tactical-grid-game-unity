using UnityEngine;
using UnityEngine.UIElements;
public class TurnManager : MonoBehaviour
{
    //     private Label turnLabel;
    //     private Button endTurnButton;
    //     private VisualElement background;
    //     private bool isMyTurn = false;
    //     private string playerID = "player_1"; // Questo ID dovrebbe essere assegnato dal server

    //     [SerializeField] private UIDocument gameUI;
    //     private WebSocketClient webSocketClient;

    //     void Start()
    //     {
    //         webSocketClient = FindObjectOfType<WebSocketClient>();

    //         var root = gameUI.rootVisualElement;

    //         turnLabel = root.Q<Label>("turn-label");
    //         endTurnButton = root.Q<Button>("end-turn");
    //         background = root.Q<VisualElement>("background");

    //         UpdateTurnDisplay();

    //         endTurnButton.clicked += EndTurn;
    //     }

    //     private void UpdateTurnDisplay()
    //     {
    //         if (isMyTurn)
    //         {
    //             turnLabel.text = "Your Turn";
    //             background.style.backgroundColor = Color.green;
    //             endTurnButton.SetEnabled(true);
    //         }
    //         else
    //         {
    //             turnLabel.text = "Opponent's Turn";
    //             background.style.backgroundColor = Color.red;
    //             endTurnButton.SetEnabled(false);
    //         }
    //     }

    //     private void EndTurn()
    //     {
    //         isMyTurn = false;
    //         UpdateTurnDisplay();

    //         // Invia il messaggio di fine turno al server
    //         webSocketClient.SendMessageToServer("{\"type\":\"end_turn\"}");
    //     }

    //     public void OnTurnChange(string currentTurn)
    //     {
    //         isMyTurn = currentTurn == playerID;
    //         UpdateTurnDisplay();
    //     }
}