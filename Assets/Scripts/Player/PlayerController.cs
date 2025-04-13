using UnityEngine;
using System.Collections.Generic;
public class PlayerController : MonoBehaviour
{
    [Header("Grid Position")]
    public int gridX;
    public int gridY;
    
    [Header("Stats")]
    public int health = 100;
    public int maxHealth = 100;
    public int actionPoints = 5;
    public int maxActionPoints = 5;
    
    [Header("References")]
    private GridManager gridManager;
    
    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("PlayerController: GridManager non trovato!");
        }
        
        // Posiziona il personaggio sulla griglia
        UpdateVisualPosition();
    }
    
    // Aggiorna la posizione visiva in base alle coordinate griglia
    public void UpdateVisualPosition()
    {
        if (gridManager != null)
        {
            transform.position = gridManager.GetWorldPosition(gridX, gridY) + new Vector3(0, 0.5f, 0);
        }
    }
    
    // Tenta di muovere il personaggio in una nuova cella
    public bool TryMove(int newX, int newZ)
    {
        // Controlla se ci sono punti azione sufficienti
        // Il costo verrà calcolato in base alla distanza
        int distance = Mathf.Abs(newX - gridX) + Mathf.Abs(newZ - gridY);

        if (actionPoints >= distance)
        {
            // Aggiorna posizione
            gridX = newX;
            gridY = newZ;
            UpdateVisualPosition();

            // Consuma punti azione basati sulla distanza
            actionPoints -= distance;

            return true;
        }
        else
        {
            Debug.Log("Non hai abbastanza punti azione!");
            return false;
        }
    }
    public bool TryMoveAlongPath(List<Vector2Int> path)
    {
        if (path == null || path.Count <= 1)
        {
            Debug.LogWarning("Percorso non valido!");
            return false;
        }

        // Il costo è il numero di passi (escluso il punto di partenza)
        int cost = path.Count - 1;

        if (actionPoints >= cost)
        {
            // L'ultima posizione nel percorso è la destinazione
            Vector2Int destination = path[path.Count - 1];
            gridX = destination.x;
            gridY = destination.y;
            UpdateVisualPosition();

            // Consuma punti azione
            actionPoints -= cost;

            return true;
        }
        else
        {
            Debug.Log($"Non hai abbastanza punti azione! (Richiesti: {cost}, Disponibili: {actionPoints})");
            return false;
        }
    }
    // Tentativo di usare un'abilità
    public bool TryUseAbility(string abilityId, int targetX, int targetZ)
    {
        // Questa è una versione semplificata, in seguito implementeremo abilità reali
        Debug.Log($"Uso abilità {abilityId} sulla cella [{targetX}, {targetZ}]");
        
        // Controllo punti azione
        if (actionPoints >= 2)
        {
            // Consuma punti azione
            actionPoints -= 2;
            return true;
        }
        
        Debug.Log("Non hai abbastanza punti azione!");
        return false;
    }
    
    // Inizia un nuovo turno (ripristina punti azione)
    public void StartTurn()
    {
        actionPoints = maxActionPoints;
        Debug.Log($"Inizio turno del player. Punti azione ripristinati: {actionPoints}");
    }
}