using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text;

public class NetworkManager : MonoBehaviour {
    // Implementazione singleton per accessibilità globale
    public static NetworkManager Instance { get; private set; }
    
    // URL base del server Go
    public string serverUrl = "http://localhost:8080";
    
    // Callback per lo stato del gioco
    public event Action<GameState> OnGameStateReceived;
    
    private void Awake() {
        // Pattern singleton per assicurarsi che ci sia una sola istanza
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantiene l'oggetto quando si cambia scena
            Debug.Log("NetworkManager inizializzato come singleton");
        } else {
            Destroy(gameObject); // Elimina duplicati
            Debug.Log("NetworkManager duplicato eliminato");
        }
    }
    
    // Crea una nuova partita
    public void CreateGame(System.Action<string> callback) {
        StartCoroutine(CreateGameCoroutine(callback));
    }
    
    private IEnumerator CreateGameCoroutine(System.Action<string> callback) {
        string url = $"{serverUrl}/api/game/create";
        
        using (UnityWebRequest request = UnityWebRequest.Post(url, ""))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                string gameId = request.downloadHandler.text;
                callback?.Invoke(gameId);
            }
            else
            {
                Debug.LogError($"Errore nella creazione della partita: {request.error}");
                callback?.Invoke(null);
            }
        }
    }
    
    // Unisciti a una partita esistente
    public void JoinGame(string gameId, System.Action<bool> callback) {
        StartCoroutine(JoinGameCoroutine(gameId, callback));
    }
    
    private IEnumerator JoinGameCoroutine(string gameId, System.Action<bool> callback) {
        string url = $"{serverUrl}/api/game/{gameId}/join";
        
        using (UnityWebRequest request = UnityWebRequest.Post(url, ""))
        {
            yield return request.SendWebRequest();
            
            bool success = request.result == UnityWebRequest.Result.Success;
            if (!success)
            {
                Debug.LogError($"Errore nell'entrare nella partita: {request.error}");
            }
            
            callback?.Invoke(success);
        }
    }
    
    // Ottieni lo stato di una partita
    public void GetGameState(string gameId, System.Action<GameState> callback) {
        StartCoroutine(GetGameStateCoroutine(gameId, callback));
    }
    
    private IEnumerator GetGameStateCoroutine(string gameId, System.Action<GameState> callback) {
        string url = $"{serverUrl}/api/game/{gameId}/state";
        
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                GameState state = JsonUtility.FromJson<GameState>(json);
                
                // Notifica tramite evento
                OnGameStateReceived?.Invoke(state);
                
                callback?.Invoke(state);
            }
            else
            {
                Debug.LogError($"Errore nel recupero dello stato: {request.error}");
                callback?.Invoke(null);
            }
        }
    }
    
    // Invia un'azione al server
    public void SendAction(GameAction action, System.Action<bool, string> callback) {
        StartCoroutine(SendActionCoroutine(action, callback));
    }

    // Aggiungi queste modifiche al metodo SendActionCoroutine esistente
    private IEnumerator SendActionCoroutine(GameAction action, System.Action<bool, string> callback)
    {
        string url = $"{serverUrl}/api/game/{action.gameId}/action";
        string json = JsonUtility.ToJson(action);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Aggiungi questi header per CORS se necessario
            // request.SetRequestHeader("Origin", "http://localhost");

            yield return request.SendWebRequest();

            bool success = request.result == UnityWebRequest.Result.Success;
            string response = request.downloadHandler.text;

            if (!success)
            {
                Debug.LogError($"Errore nell'invio dell'azione: {request.error} - Response: {response}");
            }

            callback?.Invoke(success, response);
        }
    }

    // Ottieni la lista delle partite disponibili
    public void GetGamesList(System.Action<List<GameInfo>> callback) {
        StartCoroutine(GetGamesListCoroutine(callback));
    }
    
    [Serializable]
    public class GameInfo {
        public string id;
        public int playerCount;
        public int maxPlayers;
        public string status;
    }
    
    [Serializable]
    private class GameInfoList {
        public List<GameInfo> games;
    }
    
    private IEnumerator GetGamesListCoroutine(System.Action<List<GameInfo>> callback) {
        string url = $"{serverUrl}/api/games";
        
        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success) {
                string json = request.downloadHandler.text;
                // Wrappa il json in un oggetto per permettere a JsonUtility di fare il parsing
                json = "{\"games\":" + json + "}";
                
                GameInfoList gameList = JsonUtility.FromJson<GameInfoList>(json);
                List<GameInfo> games = gameList != null ? gameList.games : new List<GameInfo>();
                
                callback?.Invoke(games);
            } else {
                Debug.LogError($"Errore nel recupero della lista partite: {request.error}");
                callback?.Invoke(new List<GameInfo>());
            }
        }
    }
}