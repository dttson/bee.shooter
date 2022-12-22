using System;
using System.Collections;
using UnityEngine;

public class Boss14_Daisy : BossBase
{
    [SerializeField] protected EffectData m_EffectDataTarget;
    
    private static readonly int Skill31 = Animator.StringToHash("Skill3_1");
    private static readonly int IsSkill3 = Animator.StringToHash("IsSkill3");

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
        Transform playerTransform = Player.CurrentPlayer.transform;

        var skillData = m_Data.skillData[0];
        var bulletData = skillData.bulletData;
        var totalBullet = skillData.totalBullet;
        var bulletInterval = skillData.bulletInterval;
        var delayFirstShot = 0.25f;

        while (true)
        {
            yield return waitForSeconds;
            
            yield return new WaitForSeconds(delayFirstShot);
            
            m_Animator.SetTrigger(Skill31);
            m_Animator.SetBool(IsSkill3, true);

            for (int i = 0; i < totalBullet; i++)
            {
                StartCoroutine(coroutineTargetPlayer(playerTransform.position, bulletData));
                yield return new WaitForSeconds(bulletInterval);
            }
            
            m_Animator.SetBool(IsSkill3, false);
        }
    }

    private IEnumerator coroutineTargetPlayer(Vector3 playerPosition, BulletData bulletData)
    {
        GameScene gameScene = GameScene.Instance;
        gameScene.spawnEffect(playerPosition, m_EffectDataTarget);
        yield return new WaitForSeconds(1f);
        gameScene.spawnBullet(playerPosition, bulletData, Vector2.zero);
    }
}
