using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class EffectObject : MonoBehaviour
{
    public EffectType Type => m_EffectType;
    
    [SerializeField] private EffectType m_EffectType;
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private Sprite[] m_Sprites;
    [SerializeField] private GameObject m_ParticleSystem;

    private EffectData m_Data;
    private UnityAction<EffectObject> m_OnCompleteEffect;

    private bool m_IsValid = true;
    private float m_LastCalculateTime = 0.0f;
    private float m_AccumulateTime = 0.0f;
    private float m_SecondPerFrame = 0.0f;
    private int m_SpriteIndex = 0;

    public static EffectObject createEffect(EffectData data, UnityAction<EffectObject> onCompleteEffect)
    {
        EffectObject effect;
        switch (data.type)
        {
            case EffectType.SPRITE:
                effect = Instantiate(Resources.Load<EffectObject>("EffectSprite"));
                effect.m_Sprites = data.sprites;
                effect.m_SecondPerFrame = 1.0f / data.framePerSecond;
                break;
            case EffectType.PARTICLE:
            default:
                effect = Instantiate(Resources.Load<EffectObject>("EffectParticle"));
                var particleSystem = Instantiate(data.particle, effect.transform, false);
                particleSystem.SetActive(false);
                effect.m_ParticleSystem = particleSystem;
                break;
        }
        effect.m_Data = data;
        effect.m_EffectType = data.type;
        effect.m_OnCompleteEffect = onCompleteEffect;
        return effect;
    }

    public void reload(EffectData data)
    {
        if (m_EffectType != data.type)
        {
            Debug.LogError(string.Format("Cannot update effect data type {0} to type {1}", data.type, m_EffectType));
            return;
        }

        m_Data = data;
        if (m_Data.type == EffectType.SPRITE)
        {
            m_Sprites = m_Data.sprites;
            m_SecondPerFrame = 1.0f / m_Data.framePerSecond;
        }
        else
        {
            Destroy(m_ParticleSystem);
            var particleSystem = Instantiate(m_Data.particle, transform, false);
            m_ParticleSystem = particleSystem;
        }
    }

    private void OnEnable()
    {
        m_IsValid = true;
        m_SpriteIndex = 0;
        m_LastCalculateTime = m_AccumulateTime = 0f;
        if (m_Data != null && m_Data.type == EffectType.PARTICLE)
        {
            m_ParticleSystem.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        m_IsValid = false;
    }

    private void Start()
    {
        m_IsValid = true;
        if (m_Data.type == EffectType.PARTICLE)
        {
            var autoDestruct = m_ParticleSystem.GetComponent<CFX_AutoDestructShuriken>();
            autoDestruct.OnlyDeactivate = true;
            autoDestruct.OnFinishParticle = onCompleteEffect;
        }
    }
    
    private void Update()
    {
        if (!m_IsValid || m_EffectType == EffectType.PARTICLE)
        {
            return;
        }
        
        if (m_AccumulateTime > m_LastCalculateTime && m_SecondPerFrame < Mathf.Infinity)
        {
            m_SpriteIndex++;
            if (m_SpriteIndex > m_Sprites.Length - 1)
            {
                m_IsValid = false;
                onCompleteEffect();
                return;
            }
            m_SpriteRenderer.sprite = m_Sprites[m_SpriteIndex];
            m_LastCalculateTime = m_AccumulateTime + m_SecondPerFrame;
        }
        m_AccumulateTime += Time.deltaTime;
    }

    private void onCompleteEffect()
    {
        m_OnCompleteEffect?.Invoke(this);
    }
}