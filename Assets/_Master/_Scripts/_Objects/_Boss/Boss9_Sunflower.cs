using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Boss9_Sunflower : BossBase
{
    private BossSkillData m_SkillData;
    private Sequence m_Sequence;
    private static readonly int IsSkill1 = Animator.StringToHash("isSkill1");

    protected override void Awake()
    {
        base.Awake();
        m_SkillData = m_Data.skillData[0];
    }

    protected override void startMoving()
    {
        m_ScreenRect = m_Camera.getScreenRect();
        base.startMoving();
        m_CoroutineShooting = StartCoroutine(coroutineRushAttack(m_Data.attackRate, m_SkillData.speed,
            m_SkillData.subAttackCount, m_SkillData.bulletData));
    }

    private IEnumerator coroutineRushAttack(float attackRate, float rushSpeed, int subAttackCount, BulletData bulletData)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1.0f / attackRate);
        GameScene gameScene = GameScene.Instance;
        Player player = gameScene.CurrentPlayer;
        var bossSize = m_MeshRenderer.bounds.size;
        while (true)
        {
            if (m_BossState == BossState.INACTIVE) yield break;

            yield return waitForSeconds;

            m_BossState = BossState.ATTACKING;
            m_PathFollower.enabled = false;

            // init positions to attack
            var attackStartPos = transform.position;
            m_Sequence?.Kill();

            m_Sequence = DOTween.Sequence();

            var playerPos = player.transform.position;
            List<Vector3> attackPositions = new List<Vector3>();
            float y = playerPos.y + bossSize.y / 3;
            float z = playerPos.z;
            attackPositions.Add(new Vector3(playerPos.x, y, z));

            // if first hit on the left, then second hit will on the right and vice versa
            float x = playerPos.x < attackStartPos.x
                ? m_ScreenRect.xMin + bossSize.x / 3
                : m_ScreenRect.xMax - bossSize.x / 3;

            attackPositions.Add(new Vector3(x, y, z));
            
            float rushDuration = 1f / rushSpeed;
            float subAttackDuration = rushDuration * 3;

            for (int i = 0; i < subAttackCount; i++)
            {
                m_Animator.SetTrigger(AnimationTrigger.Skill2.ToString());
                
                DOVirtual.DelayedCall(subAttackDuration * i + rushDuration, () =>
                {
                    GameScene.Instance.shakeCamera();   
                    playSoundSkill();
                });
                
                // dash attack
                m_Sequence.Append(transform.DOMove(attackPositions[i], rushDuration).SetEase(Ease.InBounce)
                    .OnComplete(() =>
                        {
                            // explosion bullet (no direction)
                            gameScene.spawnBullet(transform.position, bulletData, Vector2.zero);
                            
                            // disable damage when boss moving back
                            m_Damage = 0f;
                            Debug.LogFormat("End rush {0}", m_Damage);
                        }));
                
                // move back
                if (i < subAttackCount - 1)
                {
                    m_Sequence.Append(transform.DOMove(attackStartPos, rushDuration * 2).OnComplete(() =>
                    {
                        m_Animator.SetTrigger(AnimationTrigger.Idle.ToString());
                        
                        // enable damage again
                        m_Damage = m_Data.damage;
                    }));
                }
                else
                {
                    m_Sequence.Append(transform.DOMove(attackStartPos, 1f / rushSpeed * 2).OnComplete(() =>
                    {
                        if (m_BossState == BossState.INACTIVE) return;
                        m_PathFollower.enabled = true;
                        m_BossState = BossState.IDLE;
                        
                        // enable damage again
                        m_Damage = m_Data.damage;

                        m_Animator.SetTrigger(AnimationTrigger.Idle.ToString());
                    }));
                }
            }

            m_Sequence.Play();

            yield return new WaitWhile(() => m_BossState == BossState.ATTACKING);
        }
    }

    public override void destroy()
    {
        base.destroy();
        if (m_Sequence != null)
        {
            m_Sequence.Kill();
        }
    }
}