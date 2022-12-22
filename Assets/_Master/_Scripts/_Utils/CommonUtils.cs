using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonUtils
{
    public static bool getRandomBoolean()
    {
        return Random.Range(0, 2) % 2 == 0;
    }
}
