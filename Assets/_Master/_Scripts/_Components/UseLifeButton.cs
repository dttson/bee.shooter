using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseLifeButton : ButtonEffect
{
    [SerializeField] private Button m_Button;
    [SerializeField] private GameObject m_CountdownContainer;
    [SerializeField] private Text m_TextCountdown;

    public void setEnable(bool isEnabled)
    {
        m_Button.interactable = isEnabled;

        if (isEnabled)
        {
            turnToFullColor();
            m_CountdownContainer.SetActive(false);
        }
        else
        {
            turnToGrayscale();
            m_CountdownContainer.SetActive(true);
        }
    }

    private void onUpdateCountdown(int remainTime)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(remainTime);
        m_TextCountdown.text = $"{timeSpan.Minutes:D2} : {timeSpan.Seconds:D2}";
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        m_Button = GetComponent<Button>();
    }

    private void Update()
    { 
        onUpdateCountdown(LifeManager.Instance.RemainTime);
    }
}
