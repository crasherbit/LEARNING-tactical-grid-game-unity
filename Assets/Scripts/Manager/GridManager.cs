using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject playerPrefab;

    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

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
    public void GenerateGrid(int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Instantiate(cellPrefab, new Vector3(x, 0, y), Quaternion.identity);
            }
        }
    }

    public void SpawnPlayer(string playerId, int x, int y)
    {
        if (players.ContainsKey(playerId)) return;

        GameObject player = Instantiate(playerPrefab, new Vector3(x, 0, y), Quaternion.identity);
        player.name = $"Player_{playerId}";
        players[playerId] = player;
    }

    public void UpdatePlayerPosition(string playerId, int x, int y)
    {
        if (!players.ContainsKey(playerId))
        {
            Debug.LogWarning($"Giocatore {playerId} non trovato nella griglia.");
            return;
        }

        GameObject player = players[playerId];
        player.transform.position = new Vector3(x, 0, y);
    }
}