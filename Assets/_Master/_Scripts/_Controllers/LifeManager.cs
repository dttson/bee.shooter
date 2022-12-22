using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable InconsistentNaming

public class LifeManager : MonoBehaviour
{
    private const string KEY_LIFE_COUNT = "key_life_count";
    private const string KEY_LAST_UPDATE_LIFE_COUNT = "key_last_update_life_count";

    [SerializeField] private int m_CurrentLifeCount = Config.PLAYER_MAX_LIFE_COUNT;

    public static LifeManager Instance { get; set; }

    public int CurrentLifeCount => m_CurrentLifeCount;
    public int RemainTime { get; private set; }
    
    public UnityEvent<int> OnLifeCountUpdated;
    
    // duration is calculated by second
    private readonly int[] m_RestoreLifeCountDurations = {600, 1200, 1800};
    private int m_LastUpdateLifeCountTime = 0;

    private void Awake()
    {
        var otherInstance = (LifeManager) FindObjectOfType(typeof(LifeManager));
        if (this != otherInstance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        initLifeCount();
    }

    private IEnumerator coroutineCountdown()
    {
        var waitForSeconds = new WaitForSecondsRealtime(1f);
        while (true)
        {
            if (m_CurrentLifeCount == Config.PLAYER_MAX_LIFE_COUNT)
            {
                yield return waitForSeconds;
                continue;
            }

            int currentTime = Epoch.Current();
            int elapseTime = currentTime - m_LastUpdateLifeCountTime;
            int nextRestoreTime = m_RestoreLifeCountDurations[m_CurrentLifeCount];
            // Debug.LogFormat("Elapse time: {0}, nextRestoreTime: {1}, life count: {2}", elapseTime, nextRestoreTime, m_CurrentLifeCount);
            if (elapseTime > nextRestoreTime)
            {
                increaseLifeCount();
            }

            RemainTime = nextRestoreTime - elapseTime;
            yield return waitForSeconds;
        }
    }

    public void initLifeCount()
    {
        m_CurrentLifeCount = PlayerPrefs.GetInt(KEY_LIFE_COUNT, Config.PLAYER_MAX_LIFE_COUNT);
        m_LastUpdateLifeCountTime = PlayerPrefs.GetInt(KEY_LAST_UPDATE_LIFE_COUNT, Epoch.Current());

        int currentTime = Epoch.Current();
        if (m_LastUpdateLifeCountTime > currentTime)
        {
            // WARNING: Destructive action
            // If last update time > current time, mean that user has cheat by changing date/time of device
            // Once user change it back to normal, we will reset all life count to 0
            Debug.LogWarning("Reset all life count to 0 because of CHEAT!!!");
            setLifecount(0);
            OnLifeCountUpdated?.Invoke(m_CurrentLifeCount);
            return;
        }
        
        int elapseTime = currentTime - m_LastUpdateLifeCountTime;
        while (m_CurrentLifeCount < Config.PLAYER_MAX_LIFE_COUNT && elapseTime > m_RestoreLifeCountDurations[m_CurrentLifeCount])
        {
            m_LastUpdateLifeCountTime += m_RestoreLifeCountDurations[m_CurrentLifeCount];
            PlayerPrefs.SetInt(KEY_LAST_UPDATE_LIFE_COUNT, m_LastUpdateLifeCountTime);
            PlayerPrefs.Save();
            
            elapseTime -= m_RestoreLifeCountDurations[m_CurrentLifeCount];
            setLifecount(m_CurrentLifeCount + 1, false);
        }
        
        OnLifeCountUpdated?.Invoke(m_CurrentLifeCount);
        
        StartCoroutine(coroutineCountdown());
    }

    public bool decreaseLifeCount()
    {
        return setLifecount(m_CurrentLifeCount - 1);
    }

    public bool increaseLifeCount()
    {
        return setLifecount(m_CurrentLifeCount + 1);
    }

    public bool setLifecount(int lifeCount, bool changeLastUpdateTime = true)
    {
        if (lifeCount < 0 || lifeCount > Config.PLAYER_MAX_LIFE_COUNT || lifeCount == m_CurrentLifeCount) return false;
        
        m_CurrentLifeCount = lifeCount;
        PlayerPrefs.SetInt(KEY_LIFE_COUNT, m_CurrentLifeCount);
        
        if (changeLastUpdateTime)
        {
            m_LastUpdateLifeCountTime = Epoch.Current();
            PlayerPrefs.SetInt(KEY_LAST_UPDATE_LIFE_COUNT, m_LastUpdateLifeCountTime);
        }
        
        PlayerPrefs.Save();
        
        OnLifeCountUpdated?.Invoke(m_CurrentLifeCount);
        return true;
    }
}