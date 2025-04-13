using UnityEngine;

public class TileController : MonoBehaviour
{
  // Posizione nella griglia
  public int gridX;
  public int gridZ;

  void OnMouseDown()
  {
    // Trova GameManager e notifica il click
    //  GameManager gameManager = FindObjectOfType<GameManager>();
    // if (gameManager != null)
    // {
    // gameManager.OnTileClicked(gridX, gridZ);
    // }
    // on gamemanager:
    //   public void OnTileClicked(int x, int z)
    // {
    //     Debug.Log($"Tile clicked at ({x}, {z})");
    //     // Invia il messaggio al server
    //     webSocketClient.SendMessageAsync("tile_clicked", JsonUtility.ToJson(new { x, z })).ConfigureAwait(false);
    // }
  }
}