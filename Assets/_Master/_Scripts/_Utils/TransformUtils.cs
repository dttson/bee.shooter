using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformUtils
{
    public static void updateX(this Transform transform, float x)
    {
        var pos = transform.position;
        pos.x = x;
        transform.position = pos;
    }

    public static void updateY(this Transform transform, float y)
    {
        var pos = transform.position;
        pos.y = y;
        transform.position = pos;
    }

    public static void updateZ(this Transform transform, float z)
    {
        var pos = transform.position;
        pos.z = z;
        transform.position = pos;
    }
}
