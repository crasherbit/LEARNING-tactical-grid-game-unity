// using System.Collections.Generic;
// using System;
// using UnityEngine;

// public class Pathfinder : MonoBehaviour
// {
//     private GridManager gridManager;

//     // Struttura per rappresentare i nodi del percorso
//     private class PathNode : IComparable<PathNode>
//     {
//         public int x, y;
//         public int gCost; // Costo dal punto di partenza
//         public int hCost; // Euristica (costo stimato verso la destinazione)
//         public int FCost => gCost + hCost; // Costo totale
//         public PathNode parent; // Nodo precedente nel percorso

//         public PathNode(int x, int y)
//         {
//             this.x = x;
//             this.y = y;
//         }

//         public int CompareTo(PathNode other)
//         {
//             // Confronto per ordinare i nodi in base al FCost
//             int comparison = FCost.CompareTo(other.FCost);
//             if (comparison == 0)
//                 comparison = hCost.CompareTo(other.hCost);
//             return comparison;
//         }
//     }

//     void Start()
//     {
//         gridManager = GetComponent<GridManager>();
//         if (gridManager == null)
//         {
//             gridManager = FindObjectOfType<GridManager>();
//             if (gridManager == null)
//                 Debug.LogError("Pathfinder: GridManager non trovato!");
//         }
//     }

//     // Calcola un percorso dalla posizione iniziale a quella finale
//     public List<Vector2Int> FindPath(int startX, int startY, int endX, int endY, int maxMovement)
//     {
//         // Inizializza le liste per l'algoritmo A*
//         List<PathNode> openSet = new List<PathNode>();
//         HashSet<PathNode> closedSet = new HashSet<PathNode>(new PathNodeComparer());
//         Dictionary<Vector2Int, PathNode> allNodes = new Dictionary<Vector2Int, PathNode>();

//         // Crea il nodo iniziale
//         PathNode startNode = new PathNode(startX, startY);
//         openSet.Add(startNode);
//         allNodes[new Vector2Int(startX, startY)] = startNode;

//         while (openSet.Count > 0)
//         {
//             // Ordina e prendi il nodo con il minor costo totale
//             openSet.Sort();
//             PathNode currentNode = openSet[0];
//             openSet.RemoveAt(0);

//             // Aggiungi il nodo corrente al set chiuso
//             closedSet.Add(currentNode);

//             // Se abbiamo raggiunto la destinazione, ricostruisci il percorso
//             if (currentNode.x == endX && currentNode.y == endY)
//             {
//                 return ReconstructPath(currentNode);
//             }

//             // Controlla tutti i nodi adiacenti
//             foreach (Vector2Int offset in GetAdjacentOffsets())
//             {
//                 int neighborX = currentNode.x + offset.x;
//                 int neighborY = currentNode.y + offset.y;

//                 // Controlla se il nodo è nella griglia
//                 if (neighborX < 0 || neighborX >= gridManager.width || 
//                     neighborY < 0 || neighborY >= gridManager.height)
//                 {
//                     continue;
//                 }

//                 // Salta se la cella contiene un ostacolo
//                 if (IsCellOccupied(neighborX, neighborY))
//                 {
//                     continue;
//                 }

//                 // Calcola il costo per raggiungere questo nodo
//                 int newMovementCost = currentNode.gCost + 1;

//                 // Se il costo supera il movimento massimo, salta
//                 if (newMovementCost > maxMovement)
//                 {
//                     continue;
//                 }

//                 // Crea o recupera il nodo adiacente
//                 Vector2Int neighborPos = new Vector2Int(neighborX, neighborY);
//                 PathNode neighborNode;

//                 if (!allNodes.TryGetValue(neighborPos, out neighborNode))
//                 {
//                     neighborNode = new PathNode(neighborX, neighborY);
//                     allNodes[neighborPos] = neighborNode;
//                 }

//                 // Salta se il nodo è già stato valutato
//                 if (closedSet.Contains(neighborNode))
//                 {
//                     continue;
//                 }

//                 // Se il percorso a questo nodo è migliore o se non è ancora nella lista aperta
//                 if (newMovementCost < neighborNode.gCost || !openSet.Contains(neighborNode))
//                 {
//                     neighborNode.gCost = newMovementCost;
//                     neighborNode.hCost = CalculateHCost(neighborX, neighborY, endX, endY);
//                     neighborNode.parent = currentNode;

//                     if (!openSet.Contains(neighborNode))
//                     {
//                         openSet.Add(neighborNode);
//                     }
//                 }
//             }
//         }

//         // Nessun percorso trovato
//         return null;
//     }

//     // Ricostruisci il percorso seguendo i nodi parent
//     private List<Vector2Int> ReconstructPath(PathNode endNode)
//     {
//         List<Vector2Int> path = new List<Vector2Int>();
//         PathNode currentNode = endNode;

//         // Aggiungi ogni nodo al percorso
//         while (currentNode != null)
//         {
//             path.Add(new Vector2Int(currentNode.x, currentNode.y));
//             currentNode = currentNode.parent;
//         }

//         // Il percorso è dall'arrivo alla partenza, quindi invertiamo
//         path.Reverse();

//         return path;
//     }

//     // Calcola l'euristica (distanza Manhattan)
//     private int CalculateHCost(int x, int y, int targetX, int targetY)
//     {
//         return Math.Abs(targetX - x) + Math.Abs(targetY - y);
//     }

//     // Ottieni gli offset delle celle adiacenti
//     private Vector2Int[] GetAdjacentOffsets()
//     {
//         return new Vector2Int[] {
//             new Vector2Int(0, 1),  // Sopra
//             new Vector2Int(1, 0),  // Destra
//             new Vector2Int(0, -1), // Sotto
//             new Vector2Int(-1, 0), // Sinistra
//         };
//     }

//     // Verifica se una cella è occupata da un'entità
//     private bool IsCellOccupied(int x, int y)
//     {
//         // Qui puoi implementare la logica per verificare se una cella è occupata
//         // Ad esempio, potresti controllare se c'è un nemico o un ostacolo
//         // Per ora, ritorniamo false per semplicità
//         return false;
//     }

//     // Classe per confrontare i nodi PathNode nel HashSet
//     private class PathNodeComparer : IEqualityComparer<PathNode>
//     {
//         public bool Equals(PathNode x, PathNode y)
//         {
//             return x.x == y.x && x.y == y.y;
//         }

//         public int GetHashCode(PathNode obj)
//         {
//             return obj.x.GetHashCode() ^ obj.y.GetHashCode();
//         }
//     }
// }