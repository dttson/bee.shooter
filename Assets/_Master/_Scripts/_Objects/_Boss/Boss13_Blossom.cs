using System;
using System.Collections;
using UnityEngine;

public class Boss13_Blossom : BossBase
{
    private static readonly int Skill31 = Animator.StringToHash("Skill3_1");
    private static readonly int IsSkill31 = Animator.StringToHash("IsSkill3_1");
    private static readonly int Skill32 = Animator.StringToHash("Skill3_2");
    private static readonly int IsSkill32 = Animator.StringToHash("IsSkill3_2");
    
    [Header("Boss 13")]
    [SerializeField] private Transform[] m_Step1ShootPositions;
    [SerializeField] private Transform[] m_Step2ShootPositions;
    [SerializeField] private float m_AnimationBlendDuration = 0.14f; //second
    [SerializeField] private float[] m_ListShootTimeStep1 = {0.4f, 0.3f};
    [SerializeField] private float[] m_ListShootTimeStep2 = {0.4274075f, 0.686002f, 0.28f};
    

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
        
        while (true)
        {
            yield return waitForSeconds;

            m_Animator.SetTrigger(Skill31);
            m_Animator.SetBool(IsSkill31, true);

            m_PathFollower.enabled = false;

            yield return coroutineStep1(m_Data.skillData[0].bulletData, m_Data.skillData[0].totalBullet);
            m_Animator.SetBool(IsSkill31, false);
            
            m_PathFollower.enabled = true;
            yield return  new WaitForSeconds(1f);
            
            m_Animator.SetTrigger(Skill32);
            m_Animator.SetBool(IsSkill32, true);
            yield return coroutineStep2(m_Data.skillData[1].bulletData, m_Data.skillData[1].totalBullet);
            
            m_Animator.SetBool(IsSkill32, false);
        }
    }

    private IEnumerator coroutineStep1(BulletData bulletData, int totalBullet)
    {
        GameScene gameScene = GameScene.Instance;

        float delayFirstShot = 0.1f;
        yield return new WaitForSeconds(delayFirstShot);
        
        float shootAngle = 120f;
        float startAngle = 180f + (180f - shootAngle)/2;
        
        float animDuration = 0.6667f;
        int bulletPerAnim = 4;
        float bulletInterval = animDuration / bulletPerAnim;
        float anglePerBullet = shootAngle / (bulletPerAnim - 1);

        var center = transform.position;
        float radius = Vector3.Distance(center, m_Step1ShootPositions[0].position);
        for (int i = 0; i < totalBullet; i++)
        {
            if (i % 4 == 0) playSoundSkill();
            
            var angleIndex = PingPong(i, bulletPerAnim - 1);

            float angle = startAngle + anglePerBullet * angleIndex;
            float x = center.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            gameScene.spawnBullet(new Vector3(x, radius, 0f), bulletData, Vector2.down);

            yield return new WaitForSeconds(bulletInterval);
        }
    }
    
    private IEnumerator coroutineStep2(BulletData bulletData, int totalBullet)
    {
        GameScene gameScene = GameScene.Instance;

        playSoundSkill();
        
        float delayFirstShot = 0.1080571f;
        yield return new WaitForSeconds(delayFirstShot);

        int timeIndex = 0;
        
        for (int i = 0; i < totalBullet; i+=2)
        {
            if (timeIndex % 2 == 0) playSoundSkill();
            
            var positionLeft = m_Step2ShootPositions[i % m_Step1ShootPositions.Length];
            gameScene.spawnBullet(positionLeft.position, bulletData, positionLeft.localPosition.normalized);
            if (i + 1 < totalBullet)
            {
                var positionRight = m_Step2ShootPositions[(i + 1) % m_Step1ShootPositions.Length];
                gameScene.spawnBullet(positionRight.position, bulletData, positionRight.localPosition.normalized);    
            }

            timeIndex++;
            yield return new WaitForSeconds(m_ListShootTimeStep2[timeIndex % m_ListShootTimeStep1.Length]);
        }
    }
    
    public int PingPong(int t, int length)
    {
        int q = t / length;
        int r = t % length;
 
        if ((q % 2) == 0)
            return r;
        else
            return length - r;
    }
}