using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameBalance : MonoBehaviour
{
    public static GameBalance Instance { get; private set; }

    public float ShieldTime => m_ShieldTime;

    [SerializeField] private float m_PercentShowItem = 1f;
    [SerializeField] private List<PlayerWeaponData> m_WeaponData = new List<PlayerWeaponData>();
    [SerializeField] private float m_ShieldTime = 5; //seconds

    private void Awake()
    {
        Instance = this;
    }

    public bool shouldSpawnItem()
    {
        if (Random.Range(0f, 1f) < m_PercentShowItem)
        {
            return true;
        }
        return false;
    }

    #region Player

    public PlayerWeaponData getWeaponData(int level)
    {
        var result =  m_WeaponData.FirstOrDefault((data) => data.level == level);
        if (result.Equals(default(PlayerWeaponData)))
        {
            result =  m_WeaponData[m_WeaponData.Count - 1];
        }
        return result;
    }

    #endregion
}
