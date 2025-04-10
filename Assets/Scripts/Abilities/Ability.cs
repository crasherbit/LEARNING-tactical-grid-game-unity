using UnityEngine;

[System.Serializable]
public class Ability {
    public string id;
    public string name;
    public string iconName;
    public int minRange;
    public int maxRange;
    public int actionPointCost;
    public bool needsLineOfSight;
    public RangeType rangeType;
    public TargetType targetType;
    public int damage;
    public string description;
    
    public enum RangeType {
        Linear,
        Adjacent,
        Free
    }
    
    public enum TargetType {
        SingleTarget,
        AreaOfEffect,
        Line
    }
}