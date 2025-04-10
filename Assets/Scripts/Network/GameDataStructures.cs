using System;
using System.Collections.Generic;

[Serializable]
public class GameState {
    public string gameId;
    public List<PlayerState> players = new List<PlayerState>();
    public int currentTurn;
    public string currentPlayerId;
    public GameStatus status;
    public List<BaseEntity> entities = new List<BaseEntity>();  // Rinominato da 'entities' a 'entities'
    
    public enum GameStatus {
        WaitingForPlayers,
        InProgress,
        Finished
    }
}

[Serializable]
public class PlayerState {
    public string playerId;
    public string name;
    public int health;
    public int maxHealth;
    public int actionPoints;
    public List<string> abilityIds = new List<string>();
    public GridPosition position;
}

[Serializable]
public class BaseEntity {
    public string id;
    public string type;
    public GridPosition position;
    public int health;
    public int maxHealth;
    public string ownerId; // Se applicabile
}

[Serializable]
public class GameAction {
    public string gameId;
    public string playerId;
    public ActionType type;
    public GridPosition startPosition;
    public GridPosition targetPosition;
    public string abilityId;
    public List<string> targetIds = new List<string>();
    
    public enum ActionType {
        Move,
        UseAbility,
        EndTurn
    }
}

[Serializable]
public struct GridPosition {
    public int x;
    public int y;
    
    public GridPosition(int x, int y) {
        this.x = x;
        this.y = y;
    }
}