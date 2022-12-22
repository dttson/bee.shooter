using System.Collections;
using UnityEngine;

public class Boss5_Daisy : BossBase
{
    [Header("Boss 1")]
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
        GameScene gameScene = GameScene.Instance;
        int shootPositionCount = 8;
        float delayFirstShot = 0.23f;
        while (true)
        {
            yield return waitForSeconds;

            m_Animator.SetBool(IsSkill1, true);
            m_Animator.SetTrigger(AnimationTrigger.Skill1.ToString());

            for (int count = 0; count < totalBullet / shootPositionCount; count++)
            {
                playSoundSkill();
                
                yield return new WaitForSeconds(delayFirstShot);
                for (int j = 0; j < shootPositionCount; j+=2)
                {
                    gameScene.spawnBullet(m_Skill1ShootPositions[j].position, bulletData, 
                        m_Skill1ShootPositions[j].localPosition.normalized);
                    gameScene.spawnBullet(m_Skill1ShootPositions[j+1].position, bulletData, 
                        m_Skill1ShootPositions[j+1].localPosition.normalized);
                    if (count < totalBullet / shootPositionCount - 1)
                    {
                        yield return new WaitForSeconds(interval);    
                    }
                }
            }
            
            m_Animator.SetBool(IsSkill1, false);
        }
    }
}
