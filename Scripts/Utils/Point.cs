using UnityEngine;

[System.Serializable]
public struct Point
{
    public int x;
    public int y;
    public byte z;

    public Point(int x, int y, byte z = 0)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Point(float x, float y, byte z = 0)
    {
        this.x = (int)x;
        this.y = (int)y;
        this.z = z;
    }

    public Point(Vector3 vec)
    {
        x = (int)vec.x;
        y = (int)vec.y;
        z = 0;
    }

    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + (z == 0 ? "" : (", " + z)) + ")";
    }

    public override bool Equals(object obj)
    {
        return obj is Point point &&
               x == point.x &&
               y == point.y;
    }

    public override int GetHashCode()
    {
        int hashCode = 1502939027;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        return hashCode;
    }

    public static Point operator +(Point a, Point b)
    {
        return new Point(a.x + b.x, a.y + b.y, (byte)Mathf.Min(a.z, b.z));
    }

    public static bool operator ==(Point a, Point b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Point a, Point b)
    {
        return a.x != b.x || a.y != b.y;
    }

    public static Point operator *(Point a, int i)
    {
        return new Point(a.x * i, a.y * i, a.z);
    }
}