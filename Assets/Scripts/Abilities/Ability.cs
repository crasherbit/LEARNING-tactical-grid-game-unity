using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Ability
{
    public string id;
    public string name;
    public string iconName;

    [Header("Range Settings")]
    public int minRange;
    public int maxRange;
    public bool needsLineOfSight;

    [Header("Cost and Cooldown")]
    public int actionPointCost;
    public int cooldown;
    public int currentCooldown; // Turni rimanenti di cooldown

    public enum RangeType { SingleTarget, Area, Line }
    public RangeType rangeType;

    public enum TargetType { Enemy, Ally, Both, Self, Ground }
    public TargetType targetType;

    [Header("Area Effect")]
    public int areaRadius; // Per abilità ad area

    [Header("Effect")]
    public int damage;
    public int healing;

    [Header("Status Effects")]
    public bool causesStun;
    public int stunDuration;

    public bool causesPoison;
    public int poisonDamage;
    public int poisonDuration;

    public bool causesBarrier;
    public int barrierAmount;
    public int barrierDuration;

    [Header("Movement")]
    public bool isPush;
    public int pushDistance;

    public bool isJump;
    public int jumpDistance;

    [Header("UI")]
    [TextArea(3, 5)]
    public string description;
    public Color rangeHighlightColor = Color.yellow;

    // Pattern di celle per abilità ad area
    // Usato per definire forme di aree speciali oltre al semplice raggio
    public List<Vector2Int> areaPattern;

    // Verifica se l'abilità è disponibile (non in cooldown)
    public bool IsAvailable()
    {
        return currentCooldown <= 0;
    }

    // Applica il cooldown dopo l'uso
    public void ApplyCooldown()
    {
        currentCooldown = cooldown;
    }

    // Riduce il cooldown di un turno
    public void ReduceCooldown()
    {
        if (currentCooldown > 0)
            currentCooldown--;
    }
}