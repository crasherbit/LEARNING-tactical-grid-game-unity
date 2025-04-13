using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1.0f;
    
    [Header("References")]
    public GameObject tilePrefab;
    public Material defaultMaterial;
    public Material highlightMaterial;
    public Material pathMaterial;

    // Array di GameObject per tenere traccia delle celle
    private GameObject[,] tiles;

    // Riferimento al pathfinder
    private Pathfinder pathfinder;

    // Percorso attualmente visualizzato
    private List<Vector2Int> currentPath;

    void Awake()
    {
        // Crea il componente Pathfinder se non esiste
        pathfinder = GetComponent<Pathfinder>();
        if (pathfinder == null)
        {
            pathfinder = gameObject.AddComponent<Pathfinder>();
        }
    }

    void Start()
    {
        Debug.Log("GridManager: Creazione griglia");
        CreateGrid();
    }
    
    void CreateGrid()
    {
        tiles = new GameObject[width, height];
        
        // Crea un container per le celle
        GameObject gridParent = new GameObject("Grid");
        
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Calcola la posizione in mondo reale
                Vector3 position = new Vector3(x * cellSize, 0, z * cellSize);
                
                // Offset per centrare la griglia
                position.x -= (width * cellSize) / 2 - cellSize / 2;
                position.z -= (height * cellSize) / 2 - cellSize / 2;
                
                // Crea la cella
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, gridParent.transform);
                tile.name = $"Tile_{x}_{z}";

                // Aggiungi un componente TileController
                TileController tileController = tile.GetComponent<TileController>();
                if (tileController == null)
                {
                    tileController = tile.AddComponent<TileController>();
                }
                tileController.gridX = x;
                tileController.gridZ = z;
                
                // Memorizza il riferimento
                tiles[x, z] = tile;
            }
        }
    }
    
    // Evidenzia una cella specifica
    public void HighlightTile(int x, int z, Color color)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            Material highlightMat = new Material(highlightMaterial);
            highlightMat.color = color;
            tiles[x, z].GetComponent<Renderer>().material = highlightMat;
        }
    }

    // Evidenzia una cella come parte di un percorso
    public void HighlightPathTile(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            tiles[x, z].GetComponent<Renderer>().material = pathMaterial;
        }
    }

    // Rimuove l'evidenziazione da tutte le celle
    public void ResetHighlights()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                tiles[x, z].GetComponent<Renderer>().material = defaultMaterial;
            }
        }

        // Reset del percorso corrente
        currentPath = null;
    }

    // Evidenzia le celle raggiungibili con un certo movimento
    public void HighlightMovableCells(int currentX, int currentZ, int moveRange, Color color)
    {
        // Reset prima dell'evidenziazione
        ResetHighlights();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Salta la posizione corrente
                if (x == currentX && z == currentZ)
                    continue;

                // Trova un percorso verso questa cella
                List<Vector2Int> path = pathfinder.FindPath(currentX, currentZ, x, z, moveRange);

                // Se esiste un percorso valido e non è troppo lungo
                if (path != null && path.Count > 1 && path.Count <= moveRange + 1)
                {
                    // Evidenzia la cella come raggiungibile
                    HighlightTile(x, z, color);
                }
            }
        }
    }

    // Trova e visualizza un percorso tra due punti
    public List<Vector2Int> FindAndShowPath(int startX, int startZ, int endX, int endZ, int maxMovement)
    {
        List<Vector2Int> path = pathfinder.FindPath(startX, startZ, endX, endZ, maxMovement);

        if (path != null)
        {
            // Evidenzia il percorso
            foreach (Vector2Int pos in path)
            {
                // Non evidenziare la posizione iniziale
                if (pos.x != startX || pos.y != startZ)
                {
                    HighlightPathTile(pos.x, pos.y);
                }
            }

            currentPath = path;
        }

        return path;
    }
    
    // Ottieni la posizione mondo di una cella
    public Vector3 GetWorldPosition(int x, int z)
    {
        // Offset per centrare la griglia
        float offsetX = -(width * cellSize) / 2 + cellSize / 2;
        float offsetZ = -(height * cellSize) / 2 + cellSize / 2;
        
        return new Vector3(x * cellSize + offsetX, 0, z * cellSize + offsetZ);
    }

    // Ottieni la cella dalla posizione mondo
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        float offsetX = -(width * cellSize) / 2 + cellSize / 2;
        float offsetZ = -(height * cellSize) / 2 + cellSize / 2;

        int x = Mathf.FloorToInt((worldPosition.x - offsetX) / cellSize);
        int z = Mathf.FloorToInt((worldPosition.z - offsetZ) / cellSize);

        return new Vector2Int(x, z);
    }

    // Ottieni il percorso attualmente visualizzato
    public List<Vector2Int> GetCurrentPath()
    {
        return currentPath;
    }
}