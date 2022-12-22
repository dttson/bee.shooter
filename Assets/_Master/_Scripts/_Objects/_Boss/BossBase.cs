using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using PathCreation;
using PathCreation.Examples;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

public enum BossState
{
    APPEAR,
    IDLE,
    ACTIVE,
    ATTACKING,
    INACTIVE
}

public class BossBase : MonoBehaviour
{
    protected enum AnimationTrigger
    {
        Idle,
        Skill1,
        Skill2,
        Skill3,
        Hit,
    }

    public int Level => m_Data.level;
    public float Damage => m_Damage;
    public float Speed => m_Speed;
    public EffectData DestroyEffect => m_Data.destroyEffect;
    public EffectData HitBulletEffect => m_Data.hitBulletEffect;
    public BossType Type => m_Data.bossType;
    public bool IsAttacking => m_BossState == BossState.ATTACKING;

    public float ZPosition { get; set; }

    [SerializeField] protected BossData m_Data;
    [SerializeField] protected float m_Speed = 0f;
    [SerializeField] protected float m_CurrentHP = 0f;
    [SerializeField] protected float m_Damage = 0f;
    [SerializeField] protected float m_AttackRate = 0f;

    [Header("Setting from scene")] [SerializeField]
    protected MeshRenderer m_MeshRenderer;

    [SerializeField] protected Camera m_Camera;
    [SerializeField] protected PathFollower m_PathFollower;
    [SerializeField] protected SpriteRenderer m_SpriteHPFill;
    [SerializeField] protected Animator m_Animator;
    [SerializeField] protected Vector3 m_Size;
    [SerializeField] protected float m_AppearDuration = 5f; //seconds
    [SerializeField] protected AudioSource m_AudioSource;
    [SerializeField] protected AudioClip[] m_SkillAudioClips;

    private float m_MaxHPFill = 0f;

    protected BossState m_BossState;

    protected UnityAction<BossBase> m_OnDestroy;
    protected UnityAction<BossBase, Bullet> m_OnHitBullet;
    protected UnityAction<float, float> m_OnHPChanged;

    protected Rect m_ScreenRect;
    protected Vector3 m_LastPosition = Vector3.positiveInfinity;
    protected Coroutine m_CoroutineShooting = null;


    public static BossBase createBoss(BossData data,
        PathCreator pathCreator,
        UnityAction<BossBase, Bullet> onHitBullet,
        UnityAction<BossBase> onDestroy,
        UnityAction<float, float> onHPChanged)
    {
        int level = data.level;
        BossBase boss;
        switch (data.bossType)
        {
            case BossType.BLOSSOM:
                boss = Instantiate(Resources.Load<BossBase>(string.Format("_Boss/Boss{0}_Blossom", level)));
                break;
            case BossType.CHAMOMILE:
                boss = Instantiate(Resources.Load<BossBase>(string.Format("_Boss/Boss{0}_Chamomile", level)));
                break;
            case BossType.SUNFLOWER:
                boss = Instantiate(Resources.Load<BossBase>(string.Format("_Boss/Boss{0}_Sunflower", level)));
                break;
            case BossType.ANEMONE:
                boss = Instantiate(Resources.Load<BossBase>(string.Format("_Boss/Boss{0}_Anemone", level)));
                break;
            default:
                boss = Instantiate(Resources.Load<BossBase>(string.Format("_Boss/Boss{0}_Daisy", level)));
                break;
        }

        boss.m_Data = data;
        boss.m_Speed = data.speed;
        boss.m_CurrentHP = data.maxHP;
        boss.m_Damage = data.damage;
        boss.m_AttackRate = data.attackRate;
        boss.m_OnHPChanged = onHPChanged;
        boss.m_OnHitBullet = onHitBullet;
        boss.m_OnDestroy = onDestroy;
        boss.m_PathFollower.enabled = false;
        boss.m_PathFollower.speed = data.speed;
        boss.m_PathFollower.pathCreator = pathCreator;
        return boss;
    }

    #region Unity methods

    private void OnValidate()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_Animator = GetComponent<Animator>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    protected virtual void Awake()
    {
        m_Camera = Camera.main;
        m_MaxHPFill = m_SpriteHPFill.size.x;
        m_ScreenRect = m_Camera.getScreenRect();

        int thisBossLevel = (m_Data.level - 1) / 5 + 1;
        hidePartOfLevel(thisBossLevel);
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_Size = m_MeshRenderer.bounds.size;

        var topLeftPos = m_Camera.getTopLeftPosition();
        transform.position = new Vector3(0f, topLeftPos.y + m_Size.y / 2 + 0.2f, 0f);

        appear();
    }

    // Update is called once per frame
    protected void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (m_BossState == BossState.INACTIVE) return;

        if (other.gameObject.CompareTag(TagDefine.PLAYER_BULLET))
        {
            var bullet = other.GetComponent<Bullet>();
            if (bullet.IsPlayerBullet)
            {
                hitBullet(other.GetComponent<Bullet>());
            }
        }
    }

    #endregion

    #region Action

    protected virtual void appear()
    {
        var startPosition = m_PathFollower.pathCreator.path.GetPoint(0);
        transform.DOMove(startPosition, m_AppearDuration).OnComplete(startMoving);
    }

    protected virtual void startMoving()
    {
        if (m_BossState == BossState.INACTIVE) return;

        m_PathFollower.enabled = true;

        if (m_CoroutineShooting != null)
        {
            StopCoroutine(m_CoroutineShooting);
        }
    }

    public void hitBullet(Bullet bullet)
    {
        if (m_BossState == BossState.INACTIVE) return;

        // m_Animator.SetTrigger(AnimationTrigger.Hit.ToString());

        updateHP(m_CurrentHP - bullet.Damage);
        m_OnHitBullet?.Invoke(this, bullet);

        if (m_CurrentHP <= 0)
        {
            destroy();
        }
    }

    public virtual void destroy()
    {
        if (m_BossState == BossState.INACTIVE) return;

        StopAllCoroutines();
        transform.DOKill();
        m_Animator.enabled = false;
        m_BossState = BossState.INACTIVE;
        m_PathFollower.enabled = false;

        SoundManager.Instance.stopBgm();

        StartCoroutine(coroutineBlink());
    }

    private IEnumerator coroutineBlink()
    {
        int count = 0;
        int blinkCount = 4;
        float interval = 0.4f;
        for (int i = 0; i < 3; i++)
        {
            while (count < blinkCount)
            {
                m_MeshRenderer.enabled = !m_MeshRenderer.enabled;
                yield return new WaitForSeconds(interval);
                count++;
            }

            blinkCount *= 2;
            interval /= 2;
        }

        float t = 0f;
        while (t < 0.5f)
        {
            foreach (Material material in m_MeshRenderer.materials)
            {
                material.SetFloat("_FillPhase", Mathf.Lerp(0f, 1f, t / 0.5f));
            }
            yield return null;
            t += Time.deltaTime;
        }

        m_OnDestroy?.Invoke(this);
    }

    private void updateHP(float newValue)
    {
        if (m_BossState == BossState.INACTIVE) return;

        m_CurrentHP = newValue;
        if (m_CurrentHP <= 0)
        {
            m_CurrentHP = 0f;
        }

        m_SpriteHPFill.size = new Vector2(m_CurrentHP / m_Data.maxHP * m_MaxHPFill, m_SpriteHPFill.size.y);
        m_OnHPChanged?.Invoke(m_CurrentHP, m_Data.maxHP);
    }

    #endregion

    protected virtual void playSoundSkill()
    {
        m_AudioSource.clip = m_SkillAudioClips[0];
        m_AudioSource.Play();
    }

    private void hidePartOfLevel(int level)
    {
        if (m_Data.hiddenParts.Length == 0)
        {
            return;
        }
        
        var hiddenPart = m_Data.hiddenParts.FirstOrDefault(p => p.level == level);
        if (hiddenPart.Equals(default(HiddenPart)))
        {
            return;
        }
        
        Debug.LogFormat("Hide part of level {0}", level);
        var skeleton = GetComponent<SkeletonMecanim>().Skeleton;
        foreach (var part in hiddenPart.parts)
        {
            skeleton.SetAttachment(part, null);
        }
    }
}