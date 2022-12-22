using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    private const string KEY_SOUND_VOLUME = "key_volume_sound";
    private const string KEY_SOUND_TOGGLE = "key_toggle_sound";
    private const string KEY_MUSIC_VOLUME = "key_volume_music";
    private const string KEY_MUSIC_TOGGLE = "key_toggle_music";
    
    [SerializeField] private Toggle m_ToggleSound;
    [SerializeField] private Toggle m_ToggleMusic;
    [SerializeField] private Slider m_SliderSound;
    [SerializeField] private Slider m_SliderMusic;

    private bool m_IgnoreSliderEvent = false;
    private bool m_IgnoreToggleEvent = false;
    private bool m_IsSoundOn = true;
    private bool m_IsMusicOn = true;
    private float m_CurrentSoundVolume = 1.0f;
    private float m_CurrentMusicVolume = 1.0f;
    
    private void Start()
    {
        m_CurrentMusicVolume = PlayerPrefs.GetFloat(KEY_MUSIC_VOLUME, 1f);
        m_CurrentSoundVolume = PlayerPrefs.GetFloat(KEY_SOUND_VOLUME, 1f);
        m_IsMusicOn = PlayerPrefs.GetInt(KEY_MUSIC_TOGGLE, 1) == 1;
        m_IsSoundOn = PlayerPrefs.GetInt(KEY_SOUND_TOGGLE, 1) == 1;
        
        m_ToggleSound.isOn = m_IsSoundOn;
        m_ToggleMusic.isOn = m_IsMusicOn;
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }

    public void onToggleSound(bool isOn)
    {
        if (m_IgnoreToggleEvent)
        {
            return;
        }

        m_IgnoreSliderEvent = true;
        m_IsSoundOn = isOn;
        m_SliderSound.value = isOn ? m_CurrentSoundVolume : 0f;
        m_IgnoreSliderEvent = false;
        saveSoundValue();
    }

    public void onToggleMusic(bool isOn)
    {
        if (m_IgnoreToggleEvent)
        {
            return;
        }

        m_IgnoreSliderEvent = true;
        m_IsMusicOn = isOn;
        m_SliderMusic.value = isOn ? m_CurrentMusicVolume : 0f;
        m_IgnoreSliderEvent = false;
        saveMusicValue();
    }

    public void onSliderMusic(float value)
    {
        if (m_IgnoreSliderEvent)
        {
            return;
        }

        m_IgnoreToggleEvent = true;
        m_CurrentMusicVolume = value;
        m_IsMusicOn = value > 0f;
        if (m_ToggleMusic.isOn != m_IsMusicOn)
        {
            m_ToggleMusic.isOn = m_IsMusicOn;
        }
        m_IgnoreToggleEvent = false;
        
        saveMusicValue();
    }
    
    public void onSliderSound(float value)
    {
        if (m_IgnoreSliderEvent)
        {
            return;
        }

        m_IgnoreToggleEvent = true;
        m_CurrentSoundVolume = value;
        m_IsSoundOn = value > 0f;
        if (m_ToggleSound.isOn != m_IsSoundOn)
        {
            m_ToggleSound.isOn = m_IsSoundOn;
        }
        m_IgnoreToggleEvent = false;
        
        saveSoundValue();
    }

    private void saveMusicValue()
    {
        PlayerPrefs.SetFloat(KEY_MUSIC_VOLUME, m_CurrentMusicVolume);
        PlayerPrefs.SetInt(KEY_MUSIC_TOGGLE, m_IsMusicOn ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    private void saveSoundValue()
    {
        PlayerPrefs.SetFloat(KEY_SOUND_VOLUME, m_CurrentSoundVolume);
        PlayerPrefs.SetInt(KEY_SOUND_TOGGLE, m_IsSoundOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}
