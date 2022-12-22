using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable InconsistentNaming

public class TouchUtils
{
    public static bool isTouchBegan(out Vector3 touchPosition)
    {
#if UNITY_EDITOR
        touchPosition = Input.mousePosition;
        return Input.GetMouseButtonDown(0);
#elif UNITY_ANDROID || UNITY_IOS
        touchPosition = Input.GetTouch(0).position;
        return (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
#else
        return false;
#endif
    }

    public static bool isTouchMoved(out Vector3 touchPosition)
    {
#if UNITY_EDITOR
        touchPosition = Input.mousePosition;
        return Input.GetMouseButton(0);
#elif UNITY_ANDROID || UNITY_IOS
        touchPosition = Input.GetTouch(0).position;
        return (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved);
#else
        return false;
#endif
    }

    public static bool isTouchEnd(out Vector3 touchPosition)
    {
#if UNITY_EDITOR
        touchPosition = Input.mousePosition;
        return Input.GetMouseButtonUp(0);
#elif UNITY_ANDROID || UNITY_IOS
        touchPosition = Input.GetTouch(0).position;
        return (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);
#else
        return false;
#endif
    }
}