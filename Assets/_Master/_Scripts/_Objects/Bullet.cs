using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.Events;

public enum BulletType
{
    STRAIGHT,
    ROCKET
}

public class Bullet : MonoBehaviour
{
    public BulletType Type => m_Data.type;
    public float Damage => m_Damage;
    public bool IsPlayerBullet => m_Data.isPlayerBullet;
    
    [SerializeField] protected Camera m_Camera;
    [SerializeField] protected Vector2 m_Direction;
    [SerializeField] protected float m_Speed = 0.1f; //unit per second
    [SerializeField] protected float m_Damage = 1f; //unit per second
    [SerializeField] protected SpriteRenderer m_SpriteRenderer;
    
    protected Rect m_ScreenRect;
    protected BulletData m_Data;
    protected UnityAction<Bullet> m_OnDestroy;

    public static Bullet createBullet(BulletData data, UnityAction<Bullet> onDestroy)
    {
        Bullet bullet  = Instantiate(Resources.Load<Bullet>("Bullet"));
        bullet.m_Data = data;
        bullet.m_Speed = data.speed;
        bullet.m_Damage = data.damage;
        bullet.m_OnDestroy = onDestroy;
        bullet.transform.localScale = new Vector3(data.scale, data.scale, 1);
        return bullet;
    }

    public void reload(BulletData data, Vector2 direction)
    {
        m_Data = data;
        m_Direction = direction;
        m_Speed = data.speed;
        m_Damage = data.damage;
        m_SpriteRenderer.sprite = data.sprite;
        if (data.isPlayerBullet)
        {
            gameObject.tag = TagDefine.PLAYER_BULLET;
        }
        else
        {
            gameObject.tag = TagDefine.ENEMY_BULLET;
        }

        transform.up = m_Direction;
        transform.localScale = new Vector3(data.scale, data.scale, 1);
    }

    protected virtual void Awake()
    {
        m_Camera = Camera.main;
        m_ScreenRect = m_Camera.getScreenRect();
    }

    protected virtual void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteRenderer.sprite = m_Data.sprite;
    }

    protected virtual void Update()
    {
        var bounds = m_SpriteRenderer.bounds;
        if (bounds.max.x < m_ScreenRect.xMin || bounds.min.x > m_ScreenRect.xMax || bounds.max.y < m_ScreenRect.yMin ||
            bounds.min.y > m_ScreenRect.yMax)
        {
            destroy();
        }
    }

    public virtual void destroy()
    {
        m_OnDestroy?.Invoke(this);
    }

    public virtual void hit()
    {
        destroy();
    }
}

[Serializable]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public struct BulletData
{
    public BulletType type;
    public bool isPlayerBullet;
    public Sprite sprite;
    public float speed;
    public float damage;
    public float scale;
}
