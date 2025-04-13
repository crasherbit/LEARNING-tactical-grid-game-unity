using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public UIDocument uiDocument;
    
    private GameManager gameManager;
    private AbilityManager abilityManager;
    private PlayerController player;
    private AbilityTooltip abilityTooltip;

    // Elementi UI
    private Button endTurnButton;
    private Label playerHealthLabel;
    private Label playerApLabel;
    private Label turnText;
    private VisualElement turnIndicator;
    private VisualElement abilitiesContainer;
    
    // Tenere traccia del pulsante abilità selezionato
    private Button selectedAbilityButton = null;

    // Dizionario per i pulsanti delle abilità
    private Dictionary<string, Button> abilityButtons = new Dictionary<string, Button>();

    void Start()
    {
        // Ottieni i riferimenti
        gameManager = FindObjectOfType<GameManager>();
        abilityManager = FindObjectOfType<AbilityManager>();
        player = FindObjectOfType<PlayerController>();
        abilityTooltip = GetComponent<AbilityTooltip>();

        if (abilityTooltip == null)
        {
            abilityTooltip = gameObject.AddComponent<AbilityTooltip>();
            abilityTooltip.uiDocument = uiDocument;
        }

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

        // Aggiorna i pulsanti delle abilità in base ai cooldown
        UpdateAbilityButtons();
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

            // Crea un elemento per il cooldown (inizialmente nascosto)
            Label cooldownLabel = new Label();
            cooldownLabel.AddToClassList("ability-button-cooldown");
            cooldownLabel.text = "";
            cooldownLabel.style.display = DisplayStyle.None;
            abilityButton.Add(cooldownLabel);

            // Aggiungi gli handler di eventi
            abilityButton.RegisterCallback<MouseEnterEvent>(evt =>
            {
                var abilityObj = abilityManager.GetAbilityById(ability.id);
                if (abilityObj != null)
                {
                    abilityTooltip.ShowTooltip(abilityObj, new Vector2(evt.position.x, evt.position.y));
                }
            });

            abilityButton.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                abilityTooltip.HideTooltip();
            });

            abilityButton.clicked += () => {
                // Deseleziona il pulsante precedentemente selezionato
                if (selectedAbilityButton != null)
                {
                    selectedAbilityButton.RemoveFromClassList("ability-button-selected");
                }

                // Verifica se l'abilità è disponibile
                var abilityObj = abilityManager.GetAbilityById(ability.id);
                if (abilityObj != null && abilityObj.IsAvailable())
                {
                    // Seleziona questo pulsante
                    abilityButton.AddToClassList("ability-button-selected");
                    selectedAbilityButton = abilityButton;

                    // Notifica il GameManager
                    gameManager.SelectAbility(ability.id);
                }
                else
                {
                    Debug.Log("Questa abilità è in cooldown!");
                }
            });

            // Aggiungi il pulsante al contenitore
            abilitiesContainer.Add(abilityButton);

            // Memorizza il riferimento al pulsante
            abilityButtons[ability.id] = abilityButton;

            Debug.Log($"Aggiunto bottone per abilità: {ability.name}");
        }
    }

    // Aggiorna lo stato visivo dei pulsanti delle abilità
    private void UpdateAbilityButtons()
    {
        foreach (var ability in abilityManager.abilities)
        {
            if (abilityButtons.TryGetValue(ability.id, out Button button))
            {
                Label cooldownLabel = button.Q<Label>(null, "ability-button-cooldown");

                // Se l'abilità è in cooldown, mostra il contatore
                if (ability.currentCooldown > 0)
                {
                    cooldownLabel.text = ability.currentCooldown.ToString();
                    cooldownLabel.style.display = DisplayStyle.Flex;
                    button.AddToClassList("ability-button-disabled");
                }
                else
                {
                    cooldownLabel.style.display = DisplayStyle.None;
                    button.RemoveFromClassList("ability-button-disabled");
                }

                // Disabilita il pulsante se non ci sono abbastanza punti azione
                if (player != null && player.actionPoints < ability.actionPointCost)
                {
                    button.SetEnabled(false);
                    button.tooltip = "Non hai abbastanza punti azione!";
                }
                else
                {
                    button.SetEnabled(ability.currentCooldown <= 0);
                    button.tooltip = ability.description;
                }
            }
        }
    }

    // Chiamato quando un'abilità viene usata con successo
    public void OnAbilityUsed(string abilityId)
    {
        // Deseleziona il pulsante
        if (selectedAbilityButton != null)
        {
            selectedAbilityButton.RemoveFromClassList("ability-button-selected");
            selectedAbilityButton = null;
        }

        // Aggiorna immediatamente l'UI delle abilità
        UpdateAbilityButtons();
    }
}