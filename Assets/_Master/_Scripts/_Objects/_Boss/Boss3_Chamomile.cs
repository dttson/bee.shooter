using System.Collections;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Boss3_Chamomile : BossBase
{
    private BossSkillData m_SkillData;
    private static readonly int IsSkill1 = Animator.StringToHash("isSkill1");

    protected override void Awake()
    {
        base.Awake();
        m_SkillData = m_Data.skillData[0];
    }

    protected override void startMoving()
    {
        base.startMoving();
        m_CoroutineShooting = StartCoroutine(coroutineSpawnBullet(m_SkillData.bulletData,
            m_SkillData.bulletInterval, m_SkillData.totalBullet));
    }

    private IEnumerator coroutineSpawnBullet(BulletData bulletData, float interval, int totalBullet)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1.0f / m_Data.attackRate);
        GameScene gameScene = GameScene.Instance;
        float anglePerBullet = 360f / totalBullet; //degree
        float radius = 0.1f;

        while (true)
        {
            yield return waitForSeconds;

            m_Animator.SetTrigger(AnimationTrigger.Skill1.ToString());
            playSoundSkill();
            
            var center = transform.position;
            for (int i = 0; i < totalBullet; i++)
            {
                Vector2 direction = new Vector2(Mathf.Cos(i * anglePerBullet * Mathf.Deg2Rad),
                    Mathf.Sin(i * anglePerBullet * Mathf.Deg2Rad));
                float x = center.x + radius * direction.x;
                float y = center.y + radius * direction.y;

                gameScene.spawnBullet(new Vector3(x, y, 0f), bulletData, direction);
            }
        }
    }
}