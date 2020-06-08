using UnityEngine;

public struct TargetTile
{
    public Point point;
    public TargetType type;

    public TargetTile(Point point, TargetType type)
    {
        this.point = point;
        this.type = type;
    }
}

public enum TargetType { Valid, NotValid, Blocked }
