using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Boss15_Anemone : BossBase
{
    private BossSkillData m_SkillData;
    private static readonly int Skill21 = Animator.StringToHash("Skill2_1");
    private static readonly int SkillMultiplier = Animator.StringToHash("SkillMultiplier");
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

    private IEnumerator coroutineRushAttack(float attackRate, BulletData bulletData, float totalBullet, float bulletInteval)
    {
        var waitForSeconds = new WaitForSeconds(1.0f / attackRate);
        var gameScene = GameScene.Instance;
        var firstShotDelay = 0.2666667f;
        
        float totalShootAngle = 90f;
        float startAngle = 180f + (180f - totalShootAngle)/2;
        int totalAngleCount = 5;
        float anglePerBullet = totalShootAngle / totalAngleCount;
        var listAngles = new float[totalAngleCount];
        for (int i = 0; i < totalAngleCount; i++)
        {
            listAngles[i] = startAngle + i * anglePerBullet + anglePerBullet / 2;
        }
        
        float bulletSize = 2.7f;

        while (true)
        {
            if (m_BossState == BossState.INACTIVE) yield break;
            yield return waitForSeconds;

            m_Animator.SetTrigger(Skill21);
            m_Animator.SetBool(IsSkill2, true);
            m_Animator.SetFloat(SkillMultiplier, 2f);
            
            m_PathFollower.enabled = false;

            yield return new WaitForSeconds(firstShotDelay);
            
            var center = transform.position;
            var radiusMax = center.y - m_ScreenRect.yMin;
            var radiusMin = radiusMax / 3;
            for (int i = 0; i < totalBullet; i++)
            {
                if (i % listAngles.Length == 0) listAngles.Shuffle();
                
                float angle = listAngles[i % listAngles.Length];
                float radius = Random.Range(radiusMin, radiusMax);
                float x = center.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float y = center.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);

                if (x - bulletSize / 2 < m_ScreenRect.xMin) x = m_ScreenRect.xMin + bulletSize / 2;
                else if (x + bulletSize / 2 > m_ScreenRect.xMax) x = m_ScreenRect.xMax - bulletSize / 2;

                if (y - bulletSize / 2 < m_ScreenRect.yMin) y = m_ScreenRect.yMin + bulletSize / 2;
                else if (y + bulletSize / 2 > m_ScreenRect.yMax) y = m_ScreenRect.yMax - bulletSize / 2;
                
                gameScene.spawnBullet(new Vector3(x, y, 0f), bulletData, Vector2.zero);
                
                yield return new WaitForSeconds(bulletInteval);
            }

            m_Animator.SetBool(IsSkill2, false);
            m_PathFollower.enabled = true;
        }
    }
    
    
}