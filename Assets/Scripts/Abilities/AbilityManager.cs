// using System.Collections.Generic;
// using UnityEngine;

// public class AbilityManager : MonoBehaviour
// {
//     public List<Ability> abilities = new List<Ability>();
//     private Dictionary<string, Ability> abilityLookup = new Dictionary<string, Ability>();

//     // Riferimento a GridManager
//     private GridManager gridManager;

//     void Awake()
//     {
//         // Inizializza il dizionario delle abilità
//         foreach (Ability ability in abilities)
//         {
//             if (!abilityLookup.ContainsKey(ability.id))
//             {
//                 abilityLookup.Add(ability.id, ability);
//             }
//             else
//             {
//                 Debug.LogWarning($"Abilità duplicata con ID: {ability.id}");
//             }
//         }

//         // Se non ci sono abilità predefinite, aggiungiamo alcune abilità di esempio
//         if (abilities.Count == 0)
//         {
//             CreateDefaultAbilities();
//         }

//         gridManager = FindObjectOfType<GridManager>();
//         if (gridManager == null)
//         {
//             Debug.LogError("AbilityManager: GridManager non trovato!");
//         }
//     }

//     void CreateDefaultAbilities()
//     {
//         // 1. Fireball - Danno ad area
//         Ability fireball = new Ability
//         {
//             id = "fireball",
//             name = "Fireball",
//             iconName = "fireball_icon",
//             minRange = 2,
//             maxRange = 5,
//             actionPointCost = 2,
//             cooldown = 1,
//             currentCooldown = 0,
//             needsLineOfSight = true,
//             rangeType = Ability.RangeType.Area,
//             targetType = Ability.TargetType.Ground,
//             areaRadius = 1,
//             damage = 20,
//             rangeHighlightColor = new Color(1f, 0.5f, 0),
//             description = "Lancia una palla di fuoco che esplode, danneggiando tutti i bersagli nel raggio di 1 cella."
//         };

//         // 2. Heal - Cura un alleato o se stesso
//         Ability heal = new Ability
//         {
//             id = "heal",
//             name = "Heal",
//             iconName = "heal_icon",
//             minRange = 0,
//             maxRange = 3,
//             actionPointCost = 3,
//             cooldown = 2,
//             currentCooldown = 0,
//             needsLineOfSight = false,
//             rangeType = Ability.RangeType.SingleTarget,
//             targetType = Ability.TargetType.Ally,
//             healing = 30,
//             rangeHighlightColor = Color.green,
//             description = "Ripristina 30 punti vita a un alleato o a te stesso."
//         };

//         // 3. Lightning Strike - Danno in linea
//         Ability lightning = new Ability
//         {
//             id = "lightning",
//             name = "Lightning Strike",
//             iconName = "lightning_icon",
//             minRange = 1,
//             maxRange = 4,
//             actionPointCost = 3,
//             cooldown = 3,
//             currentCooldown = 0,
//             needsLineOfSight = true,
//             rangeType = Ability.RangeType.Line,
//             targetType = Ability.TargetType.Enemy,
//             damage = 35,
//             causesStun = true,
//             stunDuration = 1,
//             rangeHighlightColor = new Color(0.5f, 0.5f, 1f),
//             description = "Invoca un fulmine che colpisce in linea retta, danneggiando e stordendo i nemici."
//         };

//         // 4. Poison Cloud - Danno nel tempo
//         Ability poison = new Ability
//         {
//             id = "poison",
//             name = "Poison Cloud",
//             iconName = "poison_icon",
//             minRange = 2,
//             maxRange = 4,
//             actionPointCost = 2,
//             cooldown = 3,
//             currentCooldown = 0,
//             needsLineOfSight = false,
//             rangeType = Ability.RangeType.Area,
//             targetType = Ability.TargetType.Ground,
//             areaRadius = 1,
//             damage = 5,
//             causesPoison = true,
//             poisonDamage = 8,
//             poisonDuration = 3,
//             rangeHighlightColor = new Color(0.5f, 1f, 0.5f),
//             description = "Crea una nube velenosa che causa danno iniziale e avvelena i nemici per 3 turni."
//         };

//         // 5. Dash - Movimento rapido
//         Ability dash = new Ability
//         {
//             id = "dash",
//             name = "Dash",
//             iconName = "dash_icon",
//             minRange = 2,
//             maxRange = 4,
//             actionPointCost = 1,
//             cooldown = 1,
//             currentCooldown = 0,
//             needsLineOfSight = true,
//             rangeType = Ability.RangeType.SingleTarget,
//             targetType = Ability.TargetType.Ground,
//             isJump = true,
//             jumpDistance = 4,
//             rangeHighlightColor = new Color(0.7f, 0.7f, 1f),
//             description = "Scatta rapidamente in una nuova posizione, ignorando gli ostacoli."
//         };

//         abilities.Add(fireball);
//         abilities.Add(heal);
//         abilities.Add(lightning);
//         abilities.Add(poison);
//         abilities.Add(dash);

//         // Aggiungi al dizionario
//         abilityLookup.Add(fireball.id, fireball);
//         abilityLookup.Add(heal.id, heal);
//         abilityLookup.Add(lightning.id, lightning);
//         abilityLookup.Add(poison.id, poison);
//         abilityLookup.Add(dash.id, dash);
//     }

//     // Ottiene un'abilità dal suo ID
//     public Ability GetAbilityById(string abilityId)
//     {
//         if (abilityLookup.TryGetValue(abilityId, out Ability ability))
//         {
//             return ability;
//         }

//         Debug.LogWarning($"Abilità con ID {abilityId} non trovata!");
//         return null;
//     }

//     // Evidenzia le celle nel raggio dell'abilità
//     public void HighlightAbilityRange(Ability ability, int playerX, int playerY)
//     {
//         if (gridManager == null) return;

//         gridManager.ResetHighlights();

//         switch (ability.rangeType)
//         {
//             case Ability.RangeType.SingleTarget:
//                 HighlightSingleTargetRange(ability, playerX, playerY);
//                 break;

//             case Ability.RangeType.Area:
//                 HighlightAreaRange(ability, playerX, playerY);
//                 break;

//             case Ability.RangeType.Line:
//                 HighlightLineRange(ability, playerX, playerY);
//                 break;
//         }
//     }

//     // Evidenzia il raggio per abilità singolo bersaglio
//     private void HighlightSingleTargetRange(Ability ability, int playerX, int playerY)
//     {
//         for (int x = 0; x < gridManager.width; x++)
//         {
//             for (int y = 0; y < gridManager.height; y++)
//             {
//                 // Calcola la distanza manhattan
//                 int distance = Mathf.Abs(x - playerX) + Mathf.Abs(y - playerY);

//                 // Verifica se la cella è nel raggio dell'abilità
//                 if (distance >= ability.minRange && distance <= ability.maxRange)
//                 {
//                     // Controlla line of sight se necessario
//                     bool hasLineOfSight = true;
//                     if (ability.needsLineOfSight)
//                     {
//                         hasLineOfSight = CheckLineOfSight(playerX, playerY, x, y);
//                     }

//                     if (hasLineOfSight)
//                     {
//                         gridManager.HighlightTile(x, y, ability.rangeHighlightColor);
//                     }
//                 }
//             }
//         }
//     }

//     // Evidenzia il raggio per abilità ad area
//     private void HighlightAreaRange(Ability ability, int playerX, int playerY)
//     {
//         // Prima evidenzia le celle dove può essere centrata l'abilità
//         for (int x = 0; x < gridManager.width; x++)
//         {
//             for (int y = 0; y < gridManager.height; y++)
//             {
//                 int distance = Mathf.Abs(x - playerX) + Mathf.Abs(y - playerY);

//                 if (distance >= ability.minRange && distance <= ability.maxRange)
//                 {
//                     bool hasLineOfSight = true;
//                     if (ability.needsLineOfSight)
//                     {
//                         hasLineOfSight = CheckLineOfSight(playerX, playerY, x, y);
//                     }

//                     if (hasLineOfSight)
//                     {
//                         gridManager.HighlightTile(x, y, ability.rangeHighlightColor);
//                     }
//                 }
//             }
//         }
//     }

//     // Evidenzia il raggio per abilità in linea
//     private void HighlightLineRange(Ability ability, int playerX, int playerY)
//     {
//         // Evidenzia nelle quattro direzioni cardinali
//         HighlightLine(playerX, playerY, 1, 0, ability); // Destra
//         HighlightLine(playerX, playerY, -1, 0, ability); // Sinistra
//         HighlightLine(playerX, playerY, 0, 1, ability); // Su
//         HighlightLine(playerX, playerY, 0, -1, ability); // Giù
//     }

//     // Evidenzia una linea in una direzione specifica
//     private void HighlightLine(int startX, int startY, int dirX, int dirY, Ability ability)
//     {
//         int x = startX;
//         int y = startY;

//         for (int distance = 1; distance <= ability.maxRange; distance++)
//         {
//             x += dirX;
//             y += dirY;

//             // Verifica che la cella sia nella griglia
//             if (x < 0 || x >= gridManager.width || y < 0 || y >= gridManager.height)
//             {
//                 break;
//             }

//             // Salta se non è nel raggio minimo
//             if (distance < ability.minRange)
//             {
//                 continue;
//             }

//             // Evidenzia la cella
//             gridManager.HighlightTile(x, y, ability.rangeHighlightColor);

//             // Se l'abilità richiede linea di vista, controlla se è bloccata
//             // Per semplicità, assumiamo che non ci siano ostacoli per ora
//         }
//     }

//     // Visualizza l'area d'effetto di un'abilità
//     public void ShowAreaOfEffect(Ability ability, int centerX, int centerY)
//     {
//         if (ability.rangeType != Ability.RangeType.Area)
//             return;

//         for (int x = centerX - ability.areaRadius; x <= centerX + ability.areaRadius; x++)
//         {
//             for (int y = centerY - ability.areaRadius; y <= centerY + ability.areaRadius; y++)
//             {
//                 // Verifica che sia nella griglia
//                 if (x < 0 || x >= gridManager.width || y < 0 || y >= gridManager.height)
//                     continue;

//                 // Verifica che sia dentro il raggio (distanza Manhattan)
//                 int distance = Mathf.Abs(x - centerX) + Mathf.Abs(y - centerY);
//                 if (distance <= ability.areaRadius)
//                 {
//                     // Colore più intenso per l'area d'effetto
//                     Color aoeColor = new Color(
//                         ability.rangeHighlightColor.r,
//                         ability.rangeHighlightColor.g,
//                         ability.rangeHighlightColor.b,
//                         0.8f); // Più opaco

//                     gridManager.HighlightTile(x, y, aoeColor);
//                 }
//             }
//         }
//     }

//     // Verifica se c'è linea di vista tra due punti
//     private bool CheckLineOfSight(int x1, int y1, int x2, int y2)
//     {
//         // Implementazione semplificata di Bresenham per tracciare una linea
//         int dx = Mathf.Abs(x2 - x1);
//         int dy = Mathf.Abs(y2 - y1);
//         int sx = x1 < x2 ? 1 : -1;
//         int sy = y1 < y2 ? 1 : -1;
//         int err = dx - dy;

//         while (x1 != x2 || y1 != y2)
//         {
//             // Controlla se c'è un ostacolo in questa cella
//             // Per ora, assumiamo che non ci siano ostacoli
//             // In futuro, si potrebbe aggiungere un controllo qui

//             int e2 = 2 * err;
//             if (e2 > -dy)
//             {
//                 err -= dy;
//                 x1 += sx;
//             }
//             if (e2 < dx)
//             {
//                 err += dx;
//                 y1 += sy;
//             }
//         }

//         return true;
//     }

//     // Riduce il cooldown di tutte le abilità a inizio turno
//     public void ReduceAllCooldowns()
//     {
//         foreach (Ability ability in abilities)
//         {
//             ability.ReduceCooldown();
//         }
//     }
// }