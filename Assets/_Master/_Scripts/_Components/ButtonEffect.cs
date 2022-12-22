using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEffect : MonoBehaviour
{
    private static readonly Color TEXT_GRAY_SCALE_COLOR = new Color(0.54f, 0.54f, 0.54f);
    private static readonly int EffectAmount = Shader.PropertyToID("_EffectAmount");
    
    // [SerializeField] private Graphic[] m_Graphics;
    [SerializeField] private List<Graphic> m_OtherGraphics = new List<Graphic>();
    [SerializeField] private List<Graphic> m_Texts = new List<Graphic>();
    [SerializeField] private List<Color> m_TextOriginColors = new List<Color>();

    protected virtual void OnValidate()
    {
        m_TextOriginColors.Clear();
        foreach (var text in m_Texts)
        {
            m_TextOriginColors.Add(text.color);
        }
    }

    public void turnToGrayscale()
    {
        foreach (var graphic in m_OtherGraphics)
        {
            graphic.material.SetFloat(EffectAmount, 1f);    
        }

        foreach (var text in m_Texts)
        {
            text.color = TEXT_GRAY_SCALE_COLOR;
        }
    }

    public void turnToFullColor()
    {
        foreach (var graphic in m_OtherGraphics)
        {
            graphic.material.SetFloat(EffectAmount, 0f);    
        }


        for (int i = 0; i < m_Texts.Count; i++)
        {
            m_Texts[i].color = m_TextOriginColors[i];
        }
    }
}
