using System.Collections;
using UnityEngine;

public class Boss1_Blossom : BossBase
{
    [Header("Boss 1")]
    [SerializeField] private float shootAngle = 20f; //degree
    [SerializeField] protected Transform[] m_Skill1ShootPositions;

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
        GameScene gameScene = GameScene.Instance;;
        float angle = Mathf.Tan(shootAngle * Mathf.Deg2Rad);
        while (true)
        {
            yield return waitForSeconds;

            m_Animator.SetBool(IsSkill1, true);
            m_Animator.SetTrigger(AnimationTrigger.Skill1.ToString());
            
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < totalBullet / 4; i++)
            {
                playSoundSkill();
                gameScene.spawnBullet(m_Skill1ShootPositions[0].position, bulletData, new Vector2(0f, -1f));
                gameScene.spawnBullet(m_Skill1ShootPositions[1].position, bulletData, new Vector2(0f, -1f));
                yield return new WaitForSeconds(interval);
                gameScene.spawnBullet(m_Skill1ShootPositions[2].position, bulletData, new Vector2(-angle, -1f));
                gameScene.spawnBullet(m_Skill1ShootPositions[3].position, bulletData, new Vector2(angle, -1f));
                if (i < totalBullet / 4 - 1)
                {
                    yield return new WaitForSeconds(interval);    
                }
            }
            
            m_Animator.SetBool(IsSkill1, false);
        }
    }
}
