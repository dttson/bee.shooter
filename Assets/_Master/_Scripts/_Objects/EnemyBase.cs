using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using PathCreation;
using PathCreation.Examples;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// ReSharper disable once CheckNamespace
public enum EnemyState
{
    IDLE,
    ACTIVE,
    INACTIVE
}

public class EnemyBase : MonoBehaviour
{
    public float Damage => m_Damage;
    public float Speed => m_Speed;
    public EffectData DestroyEffect => m_Data.destroyEffect;

    [SerializeField] protected Camera m_Camera;
    [SerializeField] protected SpriteRenderer m_SpriteRenderer;
    [SerializeField] protected EnemyData m_Data;
    [SerializeField] protected float m_Speed = 0f;
    [SerializeField] protected float m_CurrentHP = 0f;
    [SerializeField] protected float m_Damage = 0f;
    [SerializeField] protected PathFollower m_PathFollower;
    [SerializeField] protected Transform m_ShootPosition;
    
    [Header("UI")]
    [SerializeField] protected Transform m_UIContainer;
    [SerializeField] protected SpriteRenderer m_ImageHPMax;
    [SerializeField] protected SpriteRenderer m_ImageHPFill;
    protected EnemyState m_EnemyState;
    
    protected UnityAction<EnemyBase, bool> m_OnDestroy;
    protected UnityAction<EnemyBase, Bullet> m_OnHitBullet;
    
    protected Rect m_ScreenRect;
    private Vector3 m_LastPosition = Vector3.positiveInfinity;
    private Coroutine m_CoroutineShooting = null;
    
    public static EnemyBase createEnemy(EnemyData data, PathCreator pathCreator, UnityAction<EnemyBase, Bullet> onHitBullet, UnityAction<EnemyBase, bool> onDestroy)
    {
        var enemy = Instantiate(Resources.Load<EnemyBase>("EnemyBase"));
        enemy.m_Data = data;
        enemy.m_Speed = data.speed;
        enemy.m_CurrentHP = data.maxHP;
        enemy.m_Damage = data.damage;
        enemy.m_OnHitBullet = onHitBullet;
        enemy.m_OnDestroy = onDestroy;
        enemy.m_PathFollower.enabled = false;
        enemy.m_PathFollower.speed = data.speed;
        enemy.m_PathFollower.pathCreator = pathCreator;
        enemy.gameObject.SetActive(false);
        return enemy;
    }

    public void reloadData(EnemyData data, PathCreator pathCreator)
    {
        m_Data = data;
        m_Speed = data.speed;
        m_CurrentHP = data.maxHP;
        m_Damage = data.damage;
        m_SpriteRenderer.sprite = data.sprite;
        m_PathFollower.speed = data.speed;
        m_PathFollower.pathCreator = pathCreator;

        var boxCollider = gameObject.GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Destroy(boxCollider);
        }
        boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        
        updateHP(data.maxHP);
    }

    public void startMoving()
    {
        m_PathFollower.reset();
        m_PathFollower.enabled = true;
        m_EnemyState = EnemyState.ACTIVE;
        if (m_Data.hasBullet)
        {
            startShooting();
        }
    }

    public void startShooting()
    {
        if (!m_Data.hasBullet)
        {
            return;
        }

        if (m_CoroutineShooting != null)
        {
            StopCoroutine(m_CoroutineShooting);
        }
        
        m_CoroutineShooting = StartCoroutine(coroutineSpawnBullet());
    }
    
    private IEnumerator coroutineSpawnBullet()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(m_Data.shootingSpeed);
        while (true)
        {
            var direction = GameScene.Instance.CurrentPlayer.transform.position - transform.position;
            GameScene.Instance.spawnBullet(m_ShootPosition.position, m_Data.bulletData, direction.normalized);
            yield return waitForSeconds;
        }
    }

    public void hitBullet(Bullet bullet)
    {
        updateHP(m_CurrentHP - bullet.Damage);
        m_OnHitBullet?.Invoke(this, bullet);
        
        if (m_CurrentHP <= 0)
        {
            destroy();
        }
    }

    public void destroy(bool withEffect = true)
    {
        m_EnemyState = EnemyState.INACTIVE;
        m_PathFollower.enabled = false;
        m_OnDestroy?.Invoke(this, withEffect);
    }

    private void updateHP(float newValue)
    {
        m_CurrentHP = newValue;
        if (m_CurrentHP <= 0)
        {
            m_CurrentHP = 0f;
        }

        var size = m_ImageHPFill.size;
        size.x =  m_CurrentHP / m_Data.maxHP * m_ImageHPMax.size.x;
        m_ImageHPFill.size = size;
    }

    private void Awake()
    {
        m_Camera = Camera.main;
        m_EnemyState = EnemyState.IDLE;
    }

    private void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteRenderer.sprite = m_Data.sprite;
    }

    protected virtual void Update()
    {
        if (m_EnemyState != EnemyState.ACTIVE)
        {
            return;
        }
        
        if (m_PathFollower.isReachTheEnd())
        {
            destroy(false);
        }
        
        //update HP bar transform
        m_UIContainer.transform.position = transform.position;
        m_UIContainer.transform.rotation = Quaternion.identity;

        m_LastPosition = transform.position;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(TagDefine.PLAYER_BULLET))
        {
            var bullet = other.GetComponent<Bullet>();
            if (bullet.IsPlayerBullet)
            {
                hitBullet(other.GetComponent<Bullet>());
            }
        }
    }
}
