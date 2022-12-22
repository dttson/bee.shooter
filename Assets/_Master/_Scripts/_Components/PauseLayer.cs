using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseLayer : MonoBehaviour
{
    [SerializeField] private Slider m_SliderSound;
    [SerializeField] private Slider m_SliderMusic;

    private bool m_IgnoreSliderEvent = false;
    private SoundManager m_SoundManager;
    
    private void Start()
    {
        m_SoundManager = SoundManager.Instance; 
        m_IgnoreSliderEvent = true;
        m_SliderSound.value = m_SoundManager.CurrentSoundVolume;
        m_SliderMusic.value = m_SoundManager.CurrentMusicVolume;
        m_IgnoreSliderEvent = false;
        
    }

    private void OnDisable()
    {
        m_SoundManager.savePlayerPrefs();
    }


    public void onSliderMusic(float value)
    {
        if (m_IgnoreSliderEvent)
        {
            return;
        }

        m_SoundManager.CurrentMusicVolume = value;

        m_SoundManager.saveMusicValue();
    }
    
    public void onSliderSound(float value)
    {
        if (m_IgnoreSliderEvent)
        {
            return;
        }

        m_SoundManager.CurrentSoundVolume = value;
        
        m_SoundManager.saveSoundValue();
    }
}
