using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public UIDocument uiDocument;
    
    private GameManager gameManager;
    private AbilityManager abilityManager;
    private PlayerController player;
    
    // Elementi UI
    private Button endTurnButton;
    private Label playerHealthLabel;
    private Label playerApLabel;
    private Label turnText;
    private VisualElement turnIndicator;
    private VisualElement abilitiesContainer;
    
    // Tenere traccia del pulsante abilità selezionato
    private Button selectedAbilityButton = null;
    
    void Start()
    {
        // Ottieni i riferimenti
        gameManager = FindObjectOfType<GameManager>();
        abilityManager = FindObjectOfType<AbilityManager>();
        player = FindObjectOfType<PlayerController>();
        
        if (gameManager == null || abilityManager == null)
        {
            Debug.LogError("UIManager: Riferimenti mancanti!");
            return;
        }
        
        // Inizializza l'interfaccia utente
        InitializeUI();
    }
    
    void Update()
    {
        // Aggiorna l'interfaccia utente solo se abbiamo tutti i riferimenti necessari
        if (playerHealthLabel != null && playerApLabel != null && player != null)
        {
            UpdatePlayerInfo();
        }
    }
    
    void InitializeUI()
    {
        if (uiDocument == null || uiDocument.rootVisualElement == null)
        {
            Debug.LogError("UIDocument non configurato correttamente");
            return;
        }
        
        // Ottieni i riferimenti agli elementi UI
        var root = uiDocument.rootVisualElement;
        
        endTurnButton = root.Q<Button>("end-turn-button");
        playerHealthLabel = root.Q<Label>("health-label");
        playerApLabel = root.Q<Label>("ap-label");
        turnText = root.Q<Label>("turn-indicator-label");
        turnIndicator = root.Q<VisualElement>("turn-container");
        abilitiesContainer = root.Q<VisualElement>("abilities-container");
        
        // Verifica che tutti gli elementi UI siano stati trovati
        if (playerHealthLabel == null || playerApLabel == null || turnText == null || 
            turnIndicator == null || abilitiesContainer == null)
        {
            Debug.LogWarning("UIManager: Alcuni elementi UI non sono stati trovati. Controlla i nomi nel file UXML.");
        }
        
        // Configura i listener degli eventi
        if (endTurnButton != null)
        {
            endTurnButton.clicked += () => gameManager.OnEndTurnButtonPressed();
        }
        
        // Crea i pulsanti delle abilità
        CreateAbilityButtons();
        
        Debug.Log("UIManager inizializzato con successo!");
    }
    
    private void UpdatePlayerInfo()
    {
        // Controllo di sicurezza per evitare NullReferenceException
        if (player != null && playerHealthLabel != null && playerApLabel != null)
        {
            playerHealthLabel.text = $"Health: {player.health}/{player.maxHealth}";
            playerApLabel.text = $"Action Points: {player.actionPoints}";
        }
        
        if (gameManager != null && turnText != null && turnIndicator != null)
        {
            bool isPlayerTurn = gameManager.currentPhase == GameManager.GamePhase.PlayerTurn;
            turnIndicator.style.backgroundColor = isPlayerTurn ? 
                new StyleColor(new Color(0, 0.4f, 0, 0.5f)) : 
                new StyleColor(new Color(0.4f, 0, 0, 0.5f));
            turnText.text = isPlayerTurn ? "Your Turn" : "Enemy Turn";
        }
    }
    private void CreateAbilityButtons()
    {
        Debug.Log($"Creo {abilityManager.abilities.Count} pulsanti per le abilità");
        
        foreach (var ability in abilityManager.abilities)
        {
            // Crea un nuovo pulsante
            Button abilityButton = new Button();
            abilityButton.AddToClassList("ability-button");
            abilityButton.name = $"ability-{ability.id}";
            
            // Aggiungi il testo del pulsante
            abilityButton.text = ability.name;
            
            // Aggiungi il tooltip
            abilityButton.tooltip = $"{ability.name}\n{ability.description}\nCosto: {ability.actionPointCost} AP";
            
            // Aggiungi l'handler di click
            abilityButton.clicked += () => {
                // Deseleziona il pulsante precedentemente selezionato
                if (selectedAbilityButton != null)
                {
                    selectedAbilityButton.RemoveFromClassList("ability-button-selected");
                }
                
                // Seleziona questo pulsante
                abilityButton.AddToClassList("ability-button-selected");
                selectedAbilityButton = abilityButton;
                
                // Notifica il GameManager
                gameManager.SelectAbility(ability.id);
            };
            
            // Aggiungi il pulsante al contenitore
            abilitiesContainer.Add(abilityButton);
            
            Debug.Log($"Aggiunto bottone per abilità: {ability.name}");
        }
    }
}