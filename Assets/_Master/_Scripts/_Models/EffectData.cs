using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

public enum EffectType
{
    SPRITE,
    PARTICLE
}
[CreateAssetMenu(fileName = "EffectData", menuName = "Effect Data", order = 51)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[System.Serializable]
public class EffectData : ScriptableObject
{
    public EffectType type;
    public Sprite[] sprites;
    public int framePerSecond;
    public GameObject particle;
}

[CustomEditor(typeof(EffectData))]
public class EffectDataEditor : Editor
{
    private SerializedObject m_Object;
    private SerializedProperty m_SpriteProperty;
    private SerializedProperty m_ParticleProperty;
 
    void OnEnable() {
        m_Object = new SerializedObject(target);
    }
    
    public override void OnInspectorGUI()
    {
        var effectData = target as EffectData;
        effectData.type = (EffectType) EditorGUILayout.EnumPopup("Effect Type", effectData.type);
        
        if (effectData.type == EffectType.SPRITE)
        {
            m_SpriteProperty = m_Object.FindProperty("sprites");
            EditorGUILayout.PropertyField(m_SpriteProperty, new GUIContent("Sprites"), true);


            effectData.framePerSecond = EditorGUILayout.IntField("FPS", effectData.framePerSecond);
        }
        else
        {
            m_ParticleProperty = m_Object.FindProperty("particle");
            EditorGUILayout.PropertyField(m_ParticleProperty, new GUIContent("Particle"), true);
        }
        
        m_Object.ApplyModifiedProperties();
    }
}