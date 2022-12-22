using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Boss12_Sunflower : BossBase
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
            var attackEndPos = new Vector3(m_ScreenRect.xMax - bossSize.x / 2, m_ScreenRect.yMin + bossSize.x / 2, 0f);
            m_Sequence?.Kill();

            m_Sequence = DOTween.Sequence();

            var playerPos = player.transform.position;
            List<Vector3> attackPositions = new List<Vector3>();
            float y = attackStartPos.y;
            float heightStep = (attackStartPos.y - attackEndPos.y) / (subAttackCount - 1);
            for (int i = 0; i < subAttackCount; i++)
            {
                float x = i % 2 == 0 ? m_ScreenRect.xMin + bossSize.x / 2 : m_ScreenRect.xMax - bossSize.x / 2;
                y -= heightStep;
                attackPositions.Add(new Vector3(x, y, 0f));
            }
            
            DOVirtual.DelayedCall( 1f / rushSpeed / 2, () =>
            {
                Debug.Log("Shake camera");
                GameScene.Instance.shakeCamera();                
            });

            for (int i = 0; i < subAttackCount; i++)
            {
                m_Animator.SetTrigger(AnimationTrigger.Skill2.ToString());
                
                // dash attack
                if (i < subAttackCount - 1)
                {
                    m_Sequence.Append(transform.DOMove(attackPositions[i], 1f / rushSpeed).SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            // explosion bullet (no direction)
                            gameScene.spawnBullet(transform.position, bulletData, Vector2.zero);
                            
                            DOVirtual.DelayedCall( 1f / rushSpeed / 2, () =>
                            {
                                Debug.Log("Shake camera");
                                GameScene.Instance.shakeCamera();                
                            });
                        }));
                }
                else
                {
                    m_Sequence.Append(transform.DOMove(attackPositions[i], 1f / rushSpeed).SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            // explosion bullet (no direction)
                            gameScene.spawnBullet(transform.position, bulletData, Vector2.zero);
                            
                            // disable damage when boss moving back
                            m_Damage = 0f;
                            Debug.LogFormat("End rush {0}", m_Damage);
                        }));
                }
            }
            
            // move back
            m_Sequence.Append(transform.DOMove(attackStartPos, 1f / m_Data.speed).OnComplete(() =>
            {
                if (m_BossState == BossState.INACTIVE) return;
                m_PathFollower.enabled = true;
                m_BossState = BossState.IDLE;
                        
                // enable damage again
                m_Damage = m_Data.damage;

                m_Animator.SetTrigger(AnimationTrigger.Idle.ToString());
            }));

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