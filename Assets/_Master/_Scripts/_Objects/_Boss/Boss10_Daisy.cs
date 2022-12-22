using System.Collections;
using UnityEngine;

public class Boss10_Daisy : BossBase
{
    [Header("Boss 1")]
    [SerializeField] protected Transform[] m_Skill1ShootPositions;

    private static readonly int IsSkill1 = Animator.StringToHash("isSkill1");

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void startMoving()
    {
        base.startMoving();
        m_CoroutineShooting = StartCoroutine(coroutineSpawnBullet());
    }
    
    private IEnumerator coroutineSpawnBullet()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1.0f / m_Data.attackRate);
        GameScene gameScene = GameScene.Instance;

        var skill1 = m_Data.skillData[0];
        var skill2 = m_Data.skillData[0];
        
        while (true)
        {
            yield return waitForSeconds;

            yield return coroutineStep1(skill1.bulletData, skill1.bulletInterval, skill1.totalBullet);
            
            yield return new WaitForSeconds(1f);
            
            yield return coroutineStep2(skill2.bulletData, skill2.bulletInterval, skill2.totalBullet);
        }
    }

    private IEnumerator coroutineStep1(BulletData bulletData, float interval, int totalBullet)
    {
        GameScene gameScene = GameScene.Instance;
        int shootPositionCount = 8;
        float delayFirstShot = 0.23f;
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
    
    private IEnumerator coroutineStep2(BulletData bulletData, float interval, int totalBullet)
    {
        GameScene gameScene = GameScene.Instance;
        float anglePerBullet = 360f / totalBullet; //degree
        float radius = 0.1f;

        m_Animator.SetTrigger(AnimationTrigger.Skill2.ToString());
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

        yield return null;
    }
}
