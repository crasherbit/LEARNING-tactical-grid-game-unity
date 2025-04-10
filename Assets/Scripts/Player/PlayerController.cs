using UnityEngine;

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
        // Controlla se la destinazione è adiacente
        int deltaX = Mathf.Abs(newX - gridX);
        int deltaZ = Mathf.Abs(newZ - gridY);
        
        // Movimento consentito solo nelle celle adiacenti
        if ((deltaX == 1 && deltaZ == 0) || (deltaX == 0 && deltaZ == 1))
        {
            // Verifica se ci sono punti azione sufficienti
            if (actionPoints >= 1)
            {
                // Aggiorna posizione
                gridX = newX;
                gridY = newZ;
                UpdateVisualPosition();
                
                // Consuma un punto azione
                actionPoints -= 1;
                
                return true;
            }
            else
            {
                Debug.Log("Non hai abbastanza punti azione!");
                return false;
            }
        }
        
        Debug.Log("Destinazione non valida!");
        return false;
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