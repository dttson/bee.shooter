using System;
using System.Collections.Generic;
using UnityEngine;

public static class CameraExtension
{
    public static Vector3 getTopLeftPosition(this Camera self)
    {
        return self.ViewportToWorldPoint(new Vector3(0, 1, 0));
    }
    
    public static Vector3 getBottomRightPosition(this Camera self)
    {
        return self.ViewportToWorldPoint (new Vector3 (1, 0, 0));
    }
}