using UnityEngine;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    // Riferimenti
    private GridManager gridManager;
    private AbilityManager abilityManager;
    private NetworkManager networkManager;
    public GameObject playerPrefab;
    private PlayerController playerInstance;

    // Identificatori di gioco
    public string gameId;
    public string playerId;

    // Stati di gioco - Rinominato da GameState a GamePhase per evitare conflitti
    public enum GamePhase
    {
        WaitingForPlayers,
        PlayerTurn,
        OpponentTurn,
        GameOver
    }

    public GamePhase currentPhase = GamePhase.WaitingForPlayers;

    // Stato di selezione
    private enum SelectionMode
    {
        None,
        Moving,
        TargetingAbility
    }

    private SelectionMode currentSelectionMode = SelectionMode.None;
    private string selectedAbilityId = null;

    // Polling dello stato (temporaneo, in futuro useremo WebSockets)
    private float pollInterval = 2.0f;
    private float pollTimer = 0f;

    void Start()
    {
        Debug.Log("GameManager: Inizializzazione");
        gridManager = FindObjectOfType<GridManager>();
        abilityManager = FindObjectOfType<AbilityManager>();
        networkManager = NetworkManager.Instance; // Usa l'istanza singleton invece di FindObjectOfType

        if (gridManager == null || abilityManager == null || networkManager == null)
        {
            Debug.LogError("GameManager: Riferimenti mancanti!");
            return;
        }

        // Registra il callback per la ricezione dello stato del gioco
        networkManager.OnGameStateReceived += OnGameStateReceived;

        // Ottieni informazioni partita dai PlayerPrefs (imposti dalla LobbyUI)
        gameId = PlayerPrefs.GetString("GameID", "");
        playerId = PlayerPrefs.GetString("PlayerID", "player1");

        if (string.IsNullOrEmpty(gameId))
        {
            Debug.LogError("ID partita non trovato! Avvia il gioco dalla lobby.");
            return;
        }

        Debug.Log($"Connessione alla partita: {gameId} come player: {playerId}");

        // Crea il player
        SpawnPlayer();

        // Inizia a recuperare lo stato del gioco
        networkManager.GetGameState(gameId, OnInitialStateReceived);
    }

    void Update()
    {
        // Se siamo in una partita, effettua polling dello stato
        if (!string.IsNullOrEmpty(gameId))
        {
            pollTimer -= Time.deltaTime;
            if (pollTimer <= 0f)
            {
                networkManager.GetGameState(gameId, null);
                pollTimer = pollInterval;
            }
        }
    }

    private void OnInitialStateReceived(GameState state)
    {
        if (state == null)
        {
            Debug.LogError("Impossibile recuperare lo stato iniziale della partita.");
            return;
        }

        Debug.Log($"Stato iniziale ricevuto: {state.players.Count} giocatori, Turno: {state.currentTurn}");

        // Imposta la fase di gioco corretta
        if (state.currentPlayerId == playerId)
        {
            currentPhase = GamePhase.PlayerTurn;
            if (playerInstance != null)
            {
                playerInstance.StartTurn();
            }
        }
        else
        {
            currentPhase = GamePhase.OpponentTurn;
        }

        // Trova lo stato del nostro player
        foreach (var playerState in state.players)
        {
            if (playerState.playerId == playerId && playerInstance != null)
            {
                // Aggiorna stato e posizione
                playerInstance.health = playerState.health;
                playerInstance.maxHealth = playerState.maxHealth;
                playerInstance.actionPoints = playerState.actionPoints;
                playerInstance.gridX = playerState.position.x;
                playerInstance.gridY = playerState.position.y;
                playerInstance.UpdateVisualPosition();
                break;
            }
        }

        // Visualizza anche le altre entità/giocatori
        SpawnOtherEntities(state);
    }

    void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab non assegnato!");
            return;
        }

        // Istanzia il player in una posizione iniziale
        GameObject playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        playerInstance = playerObj.GetComponent<PlayerController>();

        if (playerInstance == null)
        {
            Debug.LogError("Impossibile trovare il componente PlayerController sul prefab!");
            return;
        }

        // Imposta posizione iniziale sulla griglia
        playerInstance.gridX = 0;
        playerInstance.gridY = 0;
        playerInstance.UpdateVisualPosition();
    }

    // Gestisce lo stato di gioco ricevuto dal server
    private void OnGameStateReceived(GameState state)
    {
        if (state == null) return;

        Debug.Log($"Stato ricevuto: Turno {state.currentTurn}, Giocatore corrente: {state.currentPlayerId}");

        // Aggiorna la fase del gioco
        if (state.currentPlayerId == playerId)
        {
            currentPhase = GamePhase.PlayerTurn;
        }
        else
        {
            currentPhase = GamePhase.OpponentTurn;
        }

        // Trova lo stato del nostro player
        foreach (var playerState in state.players)
        {
            if (playerState.playerId == playerId && playerInstance != null)
            {
                // Aggiorna lo stato del player locale
                playerInstance.gridX = playerState.position.x;
                playerInstance.gridY = playerState.position.y;
                playerInstance.health = playerState.health;
                playerInstance.maxHealth = playerState.maxHealth;
                playerInstance.actionPoints = playerState.actionPoints;
                playerInstance.UpdateVisualPosition();
                break;
            }
        }

        // Aggiorna anche le altre entità
        SpawnOtherEntities(state);
    }

    // Visualizza altre entità sul campo di battaglia
    private void SpawnOtherEntities(GameState state)
    {
        // Rimuovi entità esistenti (eccetto il player)
        foreach (Transform child in transform)
        {
            if (child.GetComponent<PlayerController>() != playerInstance)
            {
                Destroy(child.gameObject);
            }
        }

        // Crea o aggiorna le entità basate sullo stato del gioco
        foreach (var entityData in state.entities)
        {
            // Salta la nostra entità player, già gestita
            if (entityData.id == playerId)
            {
                continue;
            }

            // Qui puoi usare diversi prefab in base al tipo di entità
            // Per ora creiamo solo altri player
            if (entityData.type == "player")
            {
                GameObject opponentObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                // Usa un materiale diverso per distinguere
                Renderer renderer = opponentObj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.red; // Colore per opponent
                }

                PlayerController opponent = opponentObj.GetComponent<PlayerController>();
                if (opponent != null)
                {
                    opponent.gridX = entityData.position.x;
                    opponent.gridY = entityData.position.y;
                    opponent.health = entityData.health;
                    opponent.maxHealth = entityData.maxHealth;
                    opponent.UpdateVisualPosition();
                }
            }
        }
    }

    // Chiamato quando una cella della griglia viene cliccata
    public void OnTileClicked(int x, int z)
    {
        Debug.Log($"GameManager: Cella cliccata {x}, {z}");

        // Verifica lo stato del gioco
        if (currentPhase != GamePhase.PlayerTurn)
        {
            Debug.Log("Non è il tuo turno!");
            return;
        }

        switch (currentSelectionMode)
        {
            case SelectionMode.Moving:
                HandleMovementSelection(x, z);
                break;

            case SelectionMode.TargetingAbility:
                HandleAbilityTargetSelection(x, z);
                break;

            case SelectionMode.None:
            default:
                // Nessuna azione selezionata, mostra le opzioni
                ShowActionOptions(x, z);
                break;
        }
    }

    private void HandleMovementSelection(int x, int z)
    {
        // Verifica che il movimento sia valido
        int deltaX = Mathf.Abs(x - playerInstance.gridX);
        int deltaZ = Mathf.Abs(z - playerInstance.gridY);

        if ((deltaX == 1 && deltaZ == 0) || (deltaX == 0 && deltaZ == 1))
        {
            // Crea un'azione di movimento
            GameAction moveAction = new GameAction
            {
                gameId = gameId,
                playerId = playerId,
                type = GameAction.ActionType.Move,
                startPosition = new GridPosition(playerInstance.gridX, playerInstance.gridY),
                targetPosition = new GridPosition(x, z)
            };

            // Invia l'azione al server
            networkManager.SendAction(moveAction, (success, response) =>
            {
                if (success)
                {
                    // Movimento riuscito
                    playerInstance.TryMove(x, z);
                    currentSelectionMode = SelectionMode.None;
                    gridManager.ResetHighlights();
                }
                else
                {
                    Debug.LogError($"Errore nel movimento: {response}");
                }
            });
        }
        else
        {
            Debug.Log("Movimento non valido!");
        }
    }

    private void HandleAbilityTargetSelection(int x, int z)
    {
        if (string.IsNullOrEmpty(selectedAbilityId))
        {
            Debug.LogError("Nessuna abilità selezionata!");
            return;
        }

        // Ottieni l'abilità selezionata
        AbilityManager.Ability ability = abilityManager.GetAbilityById(selectedAbilityId);
        if (ability == null)
        {
            Debug.LogError($"Abilità {selectedAbilityId} non trovata!");
            return;
        }

        // Verifica punti azione
        if (playerInstance.actionPoints < ability.actionPointCost)
        {
            Debug.Log("Non hai abbastanza punti azione per questa abilità.");
            return;
        }

        // Crea un'azione per l'abilità
        GameAction abilityAction = new GameAction
        {
            gameId = gameId,
            playerId = playerId,
            type = GameAction.ActionType.UseAbility,
            abilityId = selectedAbilityId,
            startPosition = new GridPosition(playerInstance.gridX, playerInstance.gridY),
            targetPosition = new GridPosition(x, z)
        };

        // Invia l'azione al server
        networkManager.SendAction(abilityAction, (success, response) =>
        {
            if (success)
            {
                // Abilità utilizzata con successo
                playerInstance.TryUseAbility(selectedAbilityId, x, z);
                currentSelectionMode = SelectionMode.None;
                gridManager.ResetHighlights();
                selectedAbilityId = null;
            }
            else
            {
                Debug.LogError($"Errore nell'uso dell'abilità: {response}");
            }
        });
    }

    private void ShowActionOptions(int x, int z)
    {
        // Per ora mostriamo solo le opzioni di movimento
        currentSelectionMode = SelectionMode.Moving;
        HighlightMovableTiles();
    }

    // Evidenzia le celle dove il player può muoversi
    void HighlightMovableTiles()
    {
        int x = playerInstance.gridX;
        int y = playerInstance.gridY;

        // Evidenzia le celle adiacenti se sono nella griglia
        if (x > 0) gridManager.HighlightTile(x - 1, y, Color.green);
        if (x < gridManager.width - 1) gridManager.HighlightTile(x + 1, y, Color.green);
        if (y > 0) gridManager.HighlightTile(x, y - 1, Color.green);
        if (y < gridManager.height - 1) gridManager.HighlightTile(x, y + 1, Color.green);
    }

    // Metodo pubblico per selezionare un'abilità (sarà chiamato dai bottoni UI)
    public void SelectAbility(string abilityId)
    {
        if (currentPhase != GamePhase.PlayerTurn)
        {
            Debug.Log("Non è il tuo turno!");
            return;
        }

        AbilityManager.Ability ability = abilityManager.GetAbilityById(abilityId);
        if (ability == null)
        {
            Debug.LogError($"Abilità {abilityId} non trovata!");
            return;
        }

        // Verifica punti azione
        if (playerInstance.actionPoints < ability.actionPointCost)
        {
            Debug.Log("Non hai abbastanza punti azione per questa abilità.");
            return;
        }

        currentSelectionMode = SelectionMode.TargetingAbility;
        selectedAbilityId = abilityId;

        // Mostra il range dell'abilità
        abilityManager.HighlightAbilityRange(ability, playerInstance.gridX, playerInstance.gridY, gridManager);
    }

    // Termina il turno del player
    public void EndPlayerTurn()
    {
        Debug.Log("Fine turno player");

        // Crea un'azione di fine turno
        GameAction endTurnAction = new GameAction
        {
            gameId = gameId,
            playerId = playerId,
            type = GameAction.ActionType.EndTurn
        };

        // Invia l'azione al server
        networkManager.SendAction(endTurnAction, (success, response) =>
        {
            if (success)
            {
                // Cambia la fase
                currentPhase = GamePhase.OpponentTurn;
            }
            else
            {
                Debug.LogError($"Errore nella fine del turno: {response}");
            }
        });
    }

    // Bottone UI per terminare il turno manualmente
    public void OnEndTurnButtonPressed()
    {
        if (currentPhase == GamePhase.PlayerTurn)
        {
            EndPlayerTurn();
        }
    }

    private void OnDestroy()
    {
        // Rimuovi il callback quando l'oggetto viene distrutto
        if (networkManager != null)
        {
            networkManager.OnGameStateReceived -= OnGameStateReceived;
        }
    }
}