using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "BossData", menuName = "Boss Data", order = 52)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class BossData : ScriptableObject
{
    public BossType bossType;
    public int level;
    public float maxHP;
    public float speed;
    public float damage; //damage for player when hit enemy
    public float attackRate; // attacks per second
    public EffectData destroyEffect;
    public EffectData hitBulletEffect;
    public BossSkillData[] skillData;
    public HiddenPart[] hiddenParts;
}

[Serializable]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public struct BossSkillData
{
    public int skillLevel;
    public bool hasBullet;
    // for skill has bullet
    public BulletData bulletData;
    public int totalBullet;
    public float bulletInterval;
    
    // for skill has no bullet
    public float speed;
    public int subAttackCount;
}

[Serializable]
public struct HiddenPart
{
    public int level;
    public string[] parts;
}

public enum BossType
{
    BLOSSOM,
    CHAMOMILE,
    SUNFLOWER,
    ANEMONE,
    DAISY
}

// #if UNITY_EDITOR
// [CustomEditor(typeof(BossSkillData))]
// public class BossSkillDataEditor : Editor
// {
//     private SerializedObject m_Object;
//     private SerializedProperty m_SpriteProperty;
//     private SerializedProperty m_ParticleProperty;
//     private SerializedProperty m_AudioClipProperty;
//  
//     void OnEnable() {
//         m_Object = new SerializedObject(target);
//     }
//     
//     public override void OnInspectorGUI()
//     {
//         var effectData = target as EffectData;
//         effectData.type = (EffectType) EditorGUILayout.EnumPopup("Effect Type", effectData.type);
//         
//         if (effectData.type == EffectType.SPRITE)
//         {
//             m_SpriteProperty = m_Object.FindProperty("sprites");
//             EditorGUILayout.PropertyField(m_SpriteProperty, new GUIContent("Sprites"), true);
//         
//         
//             effectData.framePerSecond = EditorGUILayout.IntField("FPS", effectData.framePerSecond);
//         }
//         else
//         {
//             m_ParticleProperty = m_Object.FindProperty("particle");
//             EditorGUILayout.PropertyField(m_ParticleProperty, new GUIContent("Particle"), true);
//         }
//
//         m_AudioClipProperty = m_Object.FindProperty("soundEffect");
//         EditorGUILayout.PropertyField(m_AudioClipProperty, new GUIContent("Destroy Sound Effect"), true);
//         m_Object.ApplyModifiedProperties();
//     }
// }
// #endif