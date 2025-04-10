using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LobbyUIManager : MonoBehaviour
{
  public UIDocument lobbyUIDocument;
  public string gameSceneName = "GameScene";

  private NetworkManager networkManager;
  private TextField playerNameField;
  private TextField gameIdField;
  private Button createGameButton;
  private Button joinGameButton;
  private Label statusLabel;
  private VisualElement gameListContainer;

  // ID delle partite disponibili
  private List<string> availableGames = new List<string>();

  void Start()
  {
    networkManager = NetworkManager.Instance; // Usa l'istanza singleton invece di FindObjectOfType
    if (networkManager == null)
    {
      Debug.LogError("NetworkManager non trovato!");
      return;
    }
    InitializeUI();
    // Ottieni la lista di partite reali
    GetAvailableGames();

    // Aggiorna la lista ogni 5 secondi
    InvokeRepeating("GetAvailableGames", 5f, 5f);
  }

  void InitializeUI()
  {
    if (lobbyUIDocument == null || lobbyUIDocument.rootVisualElement == null)
    {
      Debug.LogError("UIDocument non configurato correttamente");
      return;
    }

    var root = lobbyUIDocument.rootVisualElement;

    playerNameField = root.Q<TextField>("player-name-field");
    gameIdField = root.Q<TextField>("game-id-field");
    createGameButton = root.Q<Button>("create-game-button");
    joinGameButton = root.Q<Button>("join-game-button");
    statusLabel = root.Q<Label>("status-label");
    gameListContainer = root.Q<VisualElement>("game-list-container");

    if (playerNameField != null && createGameButton != null && joinGameButton != null && statusLabel != null)
    {
      // Imposta nome player predefinito
      playerNameField.value = "Player" + Random.Range(100, 999);

      createGameButton.clicked += OnCreateGameClicked;
      joinGameButton.clicked += OnJoinGameClicked;
    }
    else
    {
      Debug.LogError("Alcuni elementi UI non sono stati trovati");
    }
  }

  void GetAvailableGames()
  {
    networkManager.GetGamesList((games) =>
    {
      availableGames.Clear();

      foreach (var game in games)
      {
        if (game.status == "WaitingForPlayers" && game.playerCount < game.maxPlayers)
        {
          availableGames.Add(game.id);
        }
      }

      UpdateGameListUI();
    });
  }

  void UpdateGameListUI()
  {
    if (gameListContainer != null)
    {
      gameListContainer.Clear();

      // Aggiungi un titolo
      Label titleLabel = new Label("Partite disponibili:");
      titleLabel.AddToClassList("game-list-title");
      gameListContainer.Add(titleLabel);

      if (availableGames.Count == 0)
      {
        Label noGamesLabel = new Label("Nessuna partita disponibile. Creane una nuova!");
        noGamesLabel.AddToClassList("no-games-label");
        gameListContainer.Add(noGamesLabel);
      }
      else
      {
        foreach (string gameId in availableGames)
        {
          CreateGameListItem(gameId);
        }
      }
    }
  }

  void CreateGameListItem(string gameId)
  {
    VisualElement gameItem = new VisualElement();
    gameItem.AddToClassList("game-list-item");

    Label gameLabel = new Label(gameId);
    gameLabel.AddToClassList("game-id-label");

    Button joinButton = new Button(() =>
    {
      gameIdField.value = gameId;
      OnJoinGameClicked();
    });
    joinButton.text = "Unisciti";
    joinButton.AddToClassList("join-button");

    gameItem.Add(gameLabel);
    gameItem.Add(joinButton);

    gameListContainer.Add(gameItem);
  }

  void OnCreateGameClicked()
  {
    string playerName = playerNameField.value;
    if (string.IsNullOrEmpty(playerName))
    {
      statusLabel.text = "Inserisci un nome giocatore valido";
      return;
    }

    statusLabel.text = "Creazione partita in corso...";

    // Salva il nome del giocatore per la scena di gioco
    PlayerPrefs.SetString("PlayerName", playerName);

    networkManager.CreateGame((gameId) =>
    {
      if (!string.IsNullOrEmpty(gameId))
      {
        // Salva l'ID della partita per la scena di gioco
        PlayerPrefs.SetString("GameID", gameId);
        PlayerPrefs.SetString("PlayerID", "player1");

        // Carica la scena di gioco
        SceneManager.LoadScene(gameSceneName);
      }
      else
      {
        statusLabel.text = "Errore nella creazione della partita";
      }
    });
  }

  void OnJoinGameClicked()
  {
    string playerName = playerNameField.value;
    string gameId = gameIdField.value;

    if (string.IsNullOrEmpty(playerName))
    {
      statusLabel.text = "Inserisci un nome giocatore valido";
      return;
    }

    if (string.IsNullOrEmpty(gameId))
    {
      statusLabel.text = "Inserisci un ID partita valido";
      return;
    }

    statusLabel.text = "Ingresso nella partita in corso...";

    // Salva il nome del giocatore per la scena di gioco
    PlayerPrefs.SetString("PlayerName", playerName);

    networkManager.JoinGame(gameId, (success) =>
    {
      if (success)
      {
        // Salva l'ID della partita per la scena di gioco
        PlayerPrefs.SetString("GameID", gameId);
        PlayerPrefs.SetString("PlayerID", "player2");

        // Carica la scena di gioco
        SceneManager.LoadScene(gameSceneName);
      }
      else
      {
        statusLabel.text = "Errore nell'ingresso alla partita";
      }
    });
  }
}