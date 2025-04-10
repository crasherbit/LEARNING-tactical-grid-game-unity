using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [System.Serializable]
    public class Ability
    {
        public string id;
        public string name;
        public string iconName;
        public int minRange;
        public int maxRange;
        public int actionPointCost;
        public bool needsLineOfSight;
        public enum RangeType { SingleTarget, Area }
        public RangeType rangeType;
        public enum TargetType { Enemy, Ally, Both }
        public TargetType targetType;
        public int damage;
        public string description;
    }
    
    public List<Ability> abilities = new List<Ability>();
    private Dictionary<string, Ability> abilityLookup = new Dictionary<string, Ability>();
    
    void Awake()
    {
        // Inizializza il dizionario delle abilità
        foreach (Ability ability in abilities)
        {
            if (!abilityLookup.ContainsKey(ability.id))
            {
                abilityLookup.Add(ability.id, ability);
            }
            else
            {
                Debug.LogWarning($"Abilità duplicata con ID: {ability.id}");
            }
        }
        
        // Se non ci sono abilità predefinite, aggiungiamo alcune abilità di esempio
        if (abilities.Count == 0)
        {
            CreateDefaultAbilities();
        }
    }
    
    void CreateDefaultAbilities()
    {
        // Abilità 1: Fireball
        Ability fireball = new Ability
        {
            id = "fireball",
            name = "Fireball",
            iconName = "fireball_icon",
            minRange = 2,
            maxRange = 5,
            actionPointCost = 2,
            needsLineOfSight = true,
            rangeType = Ability.RangeType.SingleTarget,
            targetType = Ability.TargetType.Enemy,
            damage = 20,
            description = "Lancia una palla di fuoco verso un bersaglio nemico."
        };
        
        // Abilità 2: Heal
        Ability heal = new Ability
        {
            id = "heal",
            name = "Heal",
            iconName = "heal_icon",
            minRange = 0,
            maxRange = 1,
            actionPointCost = 3,
            needsLineOfSight = false,
            rangeType = Ability.RangeType.SingleTarget,
            targetType = Ability.TargetType.Ally,
            damage = -25, // Valore negativo per curare
            description = "Cura un alleato o se stessi."
        };
        
        abilities.Add(fireball);
        abilities.Add(heal);
        
        // Aggiungi al dizionario
        abilityLookup.Add(fireball.id, fireball);
        abilityLookup.Add(heal.id, heal);
    }
    
    // Ottiene una abilità dal suo ID
    public Ability GetAbilityById(string abilityId)
    {
        if (abilityLookup.TryGetValue(abilityId, out Ability ability))
        {
            return ability;
        }
        
        Debug.LogWarning($"Abilità con ID {abilityId} non trovata!");
        return null;
    }
    
    // Evidenzia le celle nel raggio dell'abilità
    public void HighlightAbilityRange(Ability ability, int playerX, int playerZ, GridManager gridManager)
    {
        gridManager.ResetHighlights();
        
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int z = 0; z < gridManager.height; z++)
            {
                // Calcola la distanza manhattan
                int distance = Mathf.Abs(x - playerX) + Mathf.Abs(z - playerZ);
                
                // Verifica se la cella è nel raggio dell'abilità
                if (distance >= ability.minRange && distance <= ability.maxRange)
                {
                    // Colore per evidenziazione
                    Color highlightColor;
                    
                    switch (ability.targetType)
                    {
                        case Ability.TargetType.Enemy:
                            highlightColor = Color.red;
                            break;
                        case Ability.TargetType.Ally:
                            highlightColor = Color.blue;
                            break;
                        default:
                            highlightColor = Color.yellow;
                            break;
                    }
                    
                    gridManager.HighlightTile(x, z, highlightColor);
                }
            }
        }
    }
}