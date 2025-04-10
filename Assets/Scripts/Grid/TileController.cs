using UnityEngine;

public class TileController : MonoBehaviour
{
  // Posizione nella griglia
  public int gridX;
  public int gridZ;

  void OnMouseDown()
  {
    // Trova GameManager e notifica il click
    GameManager gameManager = FindObjectOfType<GameManager>();
    if (gameManager != null)
    {
      gameManager.OnTileClicked(gridX, gridZ);
    }
  }
}