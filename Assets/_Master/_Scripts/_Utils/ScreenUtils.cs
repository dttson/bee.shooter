using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScreenUtils
{
    public static Rect getScreenRect(this Camera camera)
    {
        Vector3 bottomLeft = camera.ScreenToWorldPoint(Vector3.zero);
        Vector3 topRight = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        return new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
    }
}
