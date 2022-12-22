using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Utils
{
    public static Vector2 toVector2(this Vector3 self)
    {
        return new Vector2(self.x, self.y);
    }
}
