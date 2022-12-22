using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Boss2_Anemone : BossBase
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
        base.startMoving();
        m_CoroutineShooting = StartCoroutine(coroutineRushAttack(m_Data.attackRate, m_SkillData.speed));
    }

    private IEnumerator coroutineRushAttack(float attackRate, float rushSpeed)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1.0f / attackRate);
        GameScene gameScene = GameScene.Instance;
        Player player = gameScene.CurrentPlayer;
        float shakeDuration = 0f;
        float delay = shakeDuration / 2;
        while (true)
        {
            if (m_BossState == BossState.INACTIVE) yield break;
            
            yield return waitForSeconds;

            m_Animator.SetTrigger(AnimationTrigger.Skill1.ToString());

            m_BossState = BossState.ATTACKING;
            m_PathFollower.enabled = false;

            var attackStartPos = transform.position;
            var attackEndPos = player.transform.position;
            attackEndPos.y = m_ScreenRect.yMin + m_MeshRenderer.bounds.size.y / 2;
            
            transform.DOKill();
                
            DOVirtual.DelayedCall(1f / rushSpeed, () =>
            {
                GameScene.Instance.shakeCamera();
                playSoundSkill();
            });
            m_Sequence = DOTween.Sequence();
            m_Sequence.Append(transform.DOMove(attackEndPos, 1f / rushSpeed).SetEase(Ease.InBounce).OnComplete(() =>
            {
                // disable damage when boss moving back
                m_Damage = 0f;
            }));
            m_Sequence.Append(transform.DOMove(attackStartPos, 1f / rushSpeed * 2).OnComplete(() =>
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
