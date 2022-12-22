using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsOn
    {
        get => m_isOn;
        set
        {
            m_isOn = value;
            updateUI();
        }
    }

    [SerializeField] private bool m_isOn;
    [SerializeField] private RectTransform m_Background;
    [SerializeField] private GameObject m_TextOn;
    [SerializeField] private GameObject m_TextOff;
    [SerializeField] private Transform m_Switcher;
    [SerializeField] private UnityEvent<bool> m_OnSwitched;

    private bool m_wasTouchDown = false;

    // Start is called before the first frame update
    private void Start()
    {
        updateUI();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_wasTouchDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!m_wasTouchDown) return;
        m_isOn = !m_isOn;
        updateUI();
        m_OnSwitched?.Invoke(m_isOn);
    }

    private void updateUI()
    {
        float x;
        if (m_isOn)
        {
            x = m_Background.rect.xMax;
        }
        else
        {
            x = m_Background.rect.xMin;
        }
        
        m_TextOn.SetActive(m_isOn);
        m_TextOff.SetActive(!m_isOn);
        
        m_Switcher.DOKill();
        m_Switcher.DOLocalMoveX(x,0.2f, true);
    }
}
