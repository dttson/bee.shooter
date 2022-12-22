using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Boss8_Anemone : BossBase
{
    private BossSkillData m_SkillData;
    private Sequence m_Sequence;
    private static readonly int Skill2_1 = Animator.StringToHash("Skill2_1");
    private static readonly int IsSkill2 = Animator.StringToHash("IsSkill2");

    protected override void Awake()
    {
        base.Awake();
        m_SkillData = m_Data.skillData[0];
    }

    protected override void startMoving()
    {
        base.startMoving();
        m_CoroutineShooting = StartCoroutine(coroutineRushAttack(m_Data.attackRate, m_SkillData.bulletData, m_SkillData
                .totalBullet,
            m_SkillData.bulletInterval));
    }

    private IEnumerator coroutineRushAttack(float attackRate,
        BulletData bulletData,
        float totalBullet,
        float bulletInteval)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1.0f / attackRate);
        GameScene gameScene = GameScene.Instance;
        Player player = gameScene.CurrentPlayer;
        var meshRenderSize = m_MeshRenderer.bounds.size;
        float firstShotDelay = 0.5333f;
        float bulletSize = 2.7f;
        while (true)
        {
            if (m_BossState == BossState.INACTIVE) yield break;

            m_BossState = BossState.IDLE;

            yield return waitForSeconds;

            m_Animator.SetTrigger(Skill2_1);
            m_Animator.SetBool(IsSkill2, true);

            m_BossState = BossState.ATTACKING;
            m_PathFollower.enabled = false;

            // playSoundSkill();

            yield return new WaitForSeconds(firstShotDelay);

            var center = transform.position;
            float startAngle = 0f;
            for (int i = 0; i < totalBullet; i++)
            {
                // spawn bullet at each quarter
                switch (i)
                {
                    case 0: // top right
                        startAngle = 0f;
                        break;
                    case 1: // bottom right
                        startAngle = 270f;
                        break;
                    case 2: // top left
                        startAngle = 90f;
                        break;
                    case 3: // bottom left
                        startAngle = 180f;
                        break;
                }

                float endAngle = startAngle + 90f;
                float radius = Random.Range(meshRenderSize.y / 1.5f, meshRenderSize.y);
                float angle = Random.Range(startAngle, endAngle);
                float x = center.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float y = center.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);

                
                if (x - bulletSize / 2 < m_ScreenRect.xMin) x = center.x + Mathf.Abs(center.x - x);
                else if (x + bulletSize / 2 > m_ScreenRect.xMax) x = center.x - Mathf.Abs(center.x - x);
                
                if (y - bulletSize / 2 < m_ScreenRect.yMin) y = center.y + Mathf.Abs(center.y - y);
                else if (y + bulletSize / 2 > m_ScreenRect.yMax) y = center.y - Mathf.Abs(center.y - y);

                gameScene.spawnBullet(new Vector3(x, y, 0f), bulletData, Vector2.zero);
                if (i == 0) continue;
                yield return new WaitForSeconds(bulletInteval);
            }
            
            m_Animator.SetBool(IsSkill2, false);

            m_PathFollower.enabled = true;
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