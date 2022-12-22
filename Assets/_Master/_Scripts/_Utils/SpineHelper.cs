using System;
using Spine.Unity;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class SpineHelper : MonoBehaviour
{
    public SkeletonAnimation m_SkeletonAnimation;
    private int fontSize = 50;

    // Start is called before the first frame update
    void Start()
    {
        m_SkeletonAnimation = GetComponent<SkeletonAnimation>();
        m_SkeletonAnimation.AnimationName = "SKILL3";
    }

    // Update is called once per frame
    void Update()
    {
        // var currentAnim = m_SkeletonAnimation.state.GetCurrent(0);
        // if (currentAnim != null)
        // {
        //     if (currentAnim.TrackTime >= currentAnim.AnimationEnd)
        //         m_SkeletonAnimation.timeScale = 0f;
        // }
    }

    private void OnGUI()
    {
        GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = fontSize;
        var currentAnim = m_SkeletonAnimation.state.GetCurrent(0);
        if (currentAnim != null)
        {
            GUI.Box(new Rect(500, 0, 200, 200), currentAnim.TrackTime.ToString());
        }

        var text = m_SkeletonAnimation.timeScale > 0 ? "Pause" : "Resume";
        var timeScale = m_SkeletonAnimation.timeScale > 0 ? 0f : 0.05f;
        if (GUI.Button(new Rect(0, 0, 200, 200), text))
        {
            m_SkeletonAnimation.timeScale = timeScale;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SpineHelper))]
public class BossBlossomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SpineHelper myTarget = (SpineHelper) target;

        if (GUILayout.Button("Get time"))
        {
            var currentAnim = myTarget.m_SkeletonAnimation.state.GetCurrent(0);
            Debug.LogFormat("Current time {0}, maxTime = {1}", currentAnim.TrackTime, currentAnim.AnimationEnd);
        }
    }
}
#endif