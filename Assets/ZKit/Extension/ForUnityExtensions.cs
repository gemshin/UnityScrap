using UnityEngine;
using ZKit;

public static partial class ForUnityExtensions
{
    public static Vector2 ToVec2(this Point p)
    {
        return new Vector2(p.x, p.y);
    }

    public static Vector3 ToVec3(this Point p, float centerValue = 0)
    {
        return new Vector3(p.x, centerValue, p.y);
    }
}
