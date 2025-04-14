using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void Move(Vector2Int targetPosition)
    {
        if (!gameManager.IsMyTurn())
        {
            Debug.LogWarning("Non è il tuo turno!");
            return;
        }

        var data = new Dictionary<string, object>
        {
            { "x", targetPosition.x },
            { "y", targetPosition.y }
        };

        WebSocketClient.Instance.Send("move_player", data).ConfigureAwait(false);
    }

    public void EndTurn()
    {
        if (!gameManager.IsMyTurn())
        {
            Debug.LogWarning("Non è il tuo turno!");
            return;
        }

        WebSocketClient.Instance.Send("end_turn", new Dictionary<string, object>()).ConfigureAwait(false);
    }
}