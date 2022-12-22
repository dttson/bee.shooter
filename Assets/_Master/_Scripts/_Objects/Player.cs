using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable InconsistentNaming

public class Player : MonoBehaviour
{
    public int CurrentLifeCount => m_CurrentLifeCount;
    public float ShootingSpeed => m_ShootingSpeed;
    public Transform CenterShootPosition => m_CenterShootPosition;
    public bool IsImmortal => m_IsImmortal;
    
    public bool IsDie { get; private set; }
    public EffectData DestroyEffect => m_DestroyEffectData;
    public UnityAction<EnemyBase> OnHitEnemy;
    public UnityAction<Bullet> OnHitEnemyBullet;
    public UnityAction OnDie;

    private int m_BulletLevel = 1;
    
    [Header("Configuration")]
    [SerializeField] private float m_ShootingSpeed = 0.5f; //1 time / 0.1 sec
    [SerializeField] private int m_CurrentLifeCount = Config.PLAYER_MAX_LIFE;
    [SerializeField] private BulletData m_BulletData;
    [SerializeField] private EffectData m_DestroyEffectData;
    [SerializeField] private GameObject m_ParticleShield;
    [SerializeField] private float m_FireAngle = 0f;
    [SerializeField] private bool m_IsImmortal = false;

    [Header("Setting from scene")]
    [SerializeField] private Transform m_CenterShootPosition;
    [SerializeField] private Transform m_LeftShootPosition;
    [SerializeField] private Transform m_RightShootPosition;
    //use for double center gun
    [SerializeField] private Transform m_CenterLeftShootPosition;
    [SerializeField] private Transform m_CenterRightShootPosition;

    private Coroutine m_CoroutineImmortal;
    #region Action

    public void becomImmortalIn(float seconds)
    {
        m_IsImmortal = true;

        m_ParticleShield.SetActive(true);
        
        if (m_CoroutineImmortal != null)
        {
            StopCoroutine(m_CoroutineImmortal);
        }
        m_CoroutineImmortal = StartCoroutine((coroutineImmortal(seconds)));
    }

    private IEnumerator coroutineImmortal(float duration)
    {
        yield return new WaitForSeconds(duration);

        m_IsImmortal = false;
        m_ParticleShield.SetActive(false);
    }

    public void upgradeBullet()
    {
        m_BulletLevel++;
        var weaponData = GameBalance.Instance.getWeaponData(m_BulletLevel);
        m_ShootingSpeed = weaponData.fireRate;
        m_BulletData.scale = weaponData.bulletSize;
        m_FireAngle = weaponData.fireAngle;
    }

    private void die()
    {
        IsDie = true;
        OnDie?.Invoke();
    }

    private void hitBulletEnemy(Bullet bullet)
    {
        OnHitEnemyBullet?.Invoke(bullet);
        
        if (m_IsImmortal) return;
        
        decreaseLifeCount((int)bullet.Damage);
        
        if (m_CurrentLifeCount <= 0)
        {
            die();
        }
    }
    
    private void hitEnemy(EnemyBase enemy)
    {
        OnHitEnemy?.Invoke(enemy);
        
        if (m_IsImmortal) return;
        
        decreaseLifeCount((int)enemy.Damage);

        if (m_CurrentLifeCount <= 0)
        {
            die();
        }
    }
    
    public void increaseLifeCount(int byCount)
    {
        m_CurrentLifeCount += byCount;
        if (m_CurrentLifeCount > Config.PLAYER_MAX_LIFE)
        {
            m_CurrentLifeCount = Config.PLAYER_MAX_LIFE;
        }
    }

    public void decreaseLifeCount(int byCount)
    {
        m_CurrentLifeCount -= byCount;
        if (m_CurrentLifeCount < 0)
        {
            m_CurrentLifeCount = 0;
        }
    }

    #endregion
    
    #region Unity methods
    private void Start()
    {
        IsDie = false;
        m_ParticleShield.SetActive(false);
        StartCoroutine(coroutineSpawnBullet());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsDie)
        {
            return;
        }
        var otherGameObject = other.gameObject;
        if (otherGameObject.CompareTag(TagDefine.ENEMY))
        {
            var enemy = other.gameObject.GetComponent<EnemyBase>();
            hitEnemy(enemy);
        }
        else if (otherGameObject.CompareTag(TagDefine.ENEMY_BULLET))
        {
            var bullet = otherGameObject.GetComponent<Bullet>();
            hitBulletEnemy(bullet);
        }
    }

    #endregion
    
    private IEnumerator coroutineSpawnBullet()
    {
        var gameScene = GameScene.Instance;
        while (true)
        {
            gameScene.spawnBullet(m_CenterShootPosition.position, m_BulletData, new Vector2(0f, 1f));
            if (m_FireAngle > 0.01f)
            {    
                float leftMostAngle = Mathf.Tan(m_FireAngle * Mathf.Deg2Rad / 2 );
                float haflLeftMostAngle = Mathf.Tan(m_FireAngle * Mathf.Deg2Rad / 4);
                gameScene.spawnBullet(m_LeftShootPosition.position, m_BulletData, new Vector2(-leftMostAngle, 1f));
                gameScene.spawnBullet(m_RightShootPosition.position, m_BulletData, new Vector2(leftMostAngle, 1f));
                gameScene.spawnBullet(m_CenterLeftShootPosition.position, m_BulletData, new Vector2(-haflLeftMostAngle, 1f));
                gameScene.spawnBullet(m_CenterRightShootPosition.position, m_BulletData, new Vector2(haflLeftMostAngle, 1f));
            }   
            yield return new WaitForSeconds(m_ShootingSpeed);
        }
    }
}
