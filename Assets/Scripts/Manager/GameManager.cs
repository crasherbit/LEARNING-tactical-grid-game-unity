using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    private string playerId;
    private string currentTurnPlayerId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        WebSocketClient.Instance.OnMessageReceived += HandleServerMessage;

        // Richiedi il matchmaking al server
        RequestMatchmaking();
    }

    private void RequestMatchmaking()
    {
        WebSocketClient.Instance.Send("matchmaking_request", new Dictionary<string, object>()).ConfigureAwait(false);
    }

    private void HandleServerMessage(string eventName, Dictionary<string, object> data)
    {
        switch (eventName)
        {
            case "lobby_ready":
                InitializeGame(data);
                break;

            case "turn_changed":
                currentTurnPlayerId = data["current_turn"].ToString();
                break;

            case "player_moved":
                string movedPlayerId = data["my_player_id"].ToString();
                int x = int.Parse(data["x"].ToString());
                int y = int.Parse(data["y"].ToString());
                GridManager.Instance.UpdatePlayerPosition(movedPlayerId, x, y);
                break;

            default:
                Debug.LogWarning($"Evento non riconosciuto: {eventName}");
                break;
        }
    }

    private void InitializeGame(Dictionary<string, object> data)
    {
        playerId = data["my_player_id"].ToString();
        List<Player> playersData = JsonConvert.DeserializeObject<List<Player>>(data["players"].ToString());
        foreach (var playerData in playersData)
        {
            string id = playerData.ID;
            int x = playerData.PositionX;
            int y = playerData.PositionY;
                GridManager.Instance.SpawnPlayer(id, x, y);
        }

        currentTurnPlayerId = data["current_turn"].ToString();
    }

    public bool IsMyTurn()
    {
        return playerId == currentTurnPlayerId;
    }

    [Serializable]
    private class Player
    {
        public string ID { get; set; }
        public string CurrentLobbyID { get; set; }
        public string Name { get; set; }
        public int HealthPoints { get; set; }
        public int ActionPoints { get; set; }
        public int MovementPoints { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public bool IsMyTurn { get; set; }
    }
}