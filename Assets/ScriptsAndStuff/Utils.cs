using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector2 XYtoVector2(Vector3 vec)
    {
        return new Vector2(vec.x, vec.y);
    }
    public static Vector2 XZtoVector2(Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }
    public static Vector2 YZtoVector2(Vector3 vec)
    {
        return new Vector2(vec.y, vec.z);
    }
}
