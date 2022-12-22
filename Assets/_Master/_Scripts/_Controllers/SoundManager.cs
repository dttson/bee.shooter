using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum BgmType
{
    MainMenu,
    Game1,
    Game2,
    FightBoss1,
    FightBoss2,
    WarningBoss,
}

public enum SfxType
{
    ButtonStart,
    ItemGun,
    ItemHeal,
    ItemHive,
    MiniBee,
    GiantBeeHive,
    WarningBoss,
    BossDie,
    BossHitBeeBullet,
    BossAnemoneSkill,
    BossBlossomSkill1And2,
    BossBlossomSkill3,
    BossChamomileFire,
    BossChamomileSkill3,
    BossDaisySkill1And2,
    BossDaisySkill3,
    BossSunflowerSkill1,
    BossSunflowerSkill2And3,
}

[Serializable]
public struct BgmData
{
    public BgmType type;
    public AudioClip clip;
}

[Serializable]
public struct SfxData
{
    public SfxType type;
    public AudioClip clip;
}
public class SoundManager : Singleton<SoundManager>
{
    public static SoundManager Instance { get; set; }
    
    private const string KEY_SOUND_VOLUME = "key_volume_sound";
    private const string KEY_MUSIC_VOLUME = "key_volume_music";
    private const string KEY_NOTIFICATIONS_TOGGLE = "key_toggle_notification";

    [SerializeField] private AudioSource m_BgmSource;
    [SerializeField] private AudioSource m_SfxSource;
    [SerializeField] private BgmData[] m_BgmData;
    [SerializeField] private SfxData[] m_SfxData;
    
    
    public float CurrentSoundVolume { get; set; } = 1.0f;
    public float CurrentMusicVolume { get; set; } = 1.0f;
    public bool IsNotificationOn { get; set; }

    private void Awake()
    {
        var otherInstance = FindObjectOfType<SoundManager>(true);
        if (!otherInstance.Equals(this))
        {
            otherInstance.gameObject.SetActive(true);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        CurrentSoundVolume = PlayerPrefs.GetFloat(KEY_SOUND_VOLUME, 1f);
        CurrentMusicVolume = PlayerPrefs.GetFloat(KEY_MUSIC_VOLUME, 1f);
        IsNotificationOn =  PlayerPrefs.GetInt(KEY_NOTIFICATIONS_TOGGLE, 1) == 1;
        
        m_SfxSource.volume = CurrentSoundVolume;
        m_BgmSource.volume = CurrentMusicVolume;
    }

    public void savePlayerPrefs()
    {
        PlayerPrefs.Save();
    }
    public void saveMusicValue()
    {
        m_BgmSource.volume = CurrentMusicVolume;
        PlayerPrefs.SetFloat(KEY_MUSIC_VOLUME, CurrentMusicVolume);
        PlayerPrefs.Save();

    }
    
    public void saveSoundValue()
    {
        m_SfxSource.volume = CurrentSoundVolume;
        PlayerPrefs.SetFloat(KEY_SOUND_VOLUME, CurrentSoundVolume);
        PlayerPrefs.Save();
    }
    
    public void saveNotificationValue()
    {
        PlayerPrefs.SetFloat(KEY_NOTIFICATIONS_TOGGLE, IsNotificationOn ? 1 :0);
        PlayerPrefs.Save();
    }

    public void playSfx(AudioClip clip)
    {
        m_SfxSource.PlayOneShot(clip);
    }
    
    public void playSfx(SfxType type, UnityAction onComplete = null)
    {
        var data = m_SfxData.FirstOrDefault(sfx => sfx.type == type);
        if (!data.Equals(default(SfxData)))
        {
            m_SfxSource.Stop();
            m_SfxSource.clip = data.clip;
            m_SfxSource.Play();

            if (onComplete != null)
            {
                IEnumerator checkClipEnd()
                {
                    yield return new WaitWhile(() => m_SfxSource.isPlaying);
                    
                    onComplete.Invoke();
                }

                StartCoroutine(checkClipEnd());
            }
        }
    }

    public void stopBgm()
    {
        m_BgmSource.Stop();
    }

    public void playBgm(BgmType type, bool loop = true, UnityAction onComplete = null)
    {
        var data = m_BgmData.FirstOrDefault(bgm => bgm.type == type);
        if (!data.Equals(default(BgmData)))
        {
            m_BgmSource.Stop();
            m_BgmSource.loop = loop;
            m_BgmSource.clip = data.clip;
            m_BgmSource.Play();
            
            if (onComplete != null)
            {
                IEnumerator checkClipEnd()
                {
                    yield return new WaitWhile(() => m_BgmSource.isPlaying);
                    
                    onComplete.Invoke();
                }

                StartCoroutine(checkClipEnd());
            }
        }
    }

    public void playBgmHome()
    {
        playBgm(BgmType.MainMenu);
    }
    
    public void playBgmGame()
    {
        playBgm(Random.Range(0, 2) % 2 == 0 ? BgmType.Game1 : BgmType.Game2);
    }
    
    public void playBgmFightBoss()
    {
        playBgm(Random.Range(0, 2) % 2 == 0 ? BgmType.FightBoss1 : BgmType.FightBoss2);
    }
}