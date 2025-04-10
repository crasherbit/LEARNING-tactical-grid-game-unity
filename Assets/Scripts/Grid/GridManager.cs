using UnityEngine;

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
    
    // Array di GameObject per tenere traccia delle celle
    private GameObject[,] tiles;
    
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
                TileController tileController = tile.AddComponent<TileController>();
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
    }
    
    // Ottieni la posizione mondo di una cella
    public Vector3 GetWorldPosition(int x, int z)
    {
        // Offset per centrare la griglia
        float offsetX = -(width * cellSize) / 2 + cellSize / 2;
        float offsetZ = -(height * cellSize) / 2 + cellSize / 2;
        
        return new Vector3(x * cellSize + offsetX, 0, z * cellSize + offsetZ);
    }
}