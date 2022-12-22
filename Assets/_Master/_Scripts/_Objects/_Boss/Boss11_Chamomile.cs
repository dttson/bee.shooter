using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Boss11_Chamomile : BossBase
{
    [SerializeField] private Transform[] m_ShootPositions;
    private BossSkillData m_SkillData;
    private Player m_Player;
    private static readonly int IsSkill3 = Animator.StringToHash("isSkill3");

    protected override void Awake()
    {
        base.Awake();
        m_SkillData = m_Data.skillData[0];
        m_Player = Player.CurrentPlayer;
    }

    protected override void startMoving()
    {
        base.startMoving();
        m_CoroutineShooting = StartCoroutine(coroutineSpawnBullet(m_SkillData.bulletData,
            m_SkillData.bulletInterval, m_SkillData.totalBullet));
    }

    // get bullet angles so bullet always appear more on "player side" rather than appear evenly around boss
    private List<float> getAngles(int numberOfAngle)
    {
        List<float> results = new List<float>();
        var playerToBossVector = (m_Player.transform.position - transform.position).normalized;
        var zeroVector = Vector2.right;
        var angleFromPlayeToBoss = Vector2.SignedAngle(zeroVector, playerToBossVector);
        //generate angle for "player side", account for 80%
        int playerSideNumAngle = (int) (numberOfAngle * 0.8f);
        float playerSideAnglePerBullet = 180f / playerSideNumAngle;
        float playerSideAngleStart = angleFromPlayeToBoss - 90f;
        float playerSideAngleEnd = angleFromPlayeToBoss + 90f;
        
        for (int i = 0; i < playerSideNumAngle; i++)
        {    
            results.Add(playerSideAngleStart + i * playerSideAnglePerBullet);
        }
        
        //generate angle for other side, account for 20%
        int otherSideNumAngle = numberOfAngle - playerSideNumAngle;
        float otherSideAnglePerBullet = 180f / otherSideNumAngle;
        for (int i = 0; i < otherSideNumAngle; i++)
        {
            results.Add(playerSideAngleEnd + i * otherSideAnglePerBullet);
        }

        return results;
    }

    private IEnumerator coroutineSpawnBullet(BulletData bulletData, float interval, int totalBullet)
    {
        var waitForSeconds = new WaitForSeconds(1.0f / m_Data.attackRate);
        var numOfAnimation = 3;
        var shootPositionCountPerAnim = 10;
        var numberOfAngle = numOfAnimation * shootPositionCountPerAnim;

        // var shootPositionCount = m_ShootPositions.Length;
        var delayFirstShot = 0.5f;
        var radius = 0.1f;
        while (true)
        {
            yield return waitForSeconds;
            
            var listAngles = getAngles(numberOfAngle);
            listAngles.Shuffle();
            Debug.Log("=========");
            foreach (float angle in listAngles)
            {
                Debug.LogFormat("Angle {0}", angle);
            }

            m_Animator.SetBool(IsSkill3, true);
            m_Animator.SetTrigger(AnimationTrigger.Skill3.ToString());

            yield return new WaitForSeconds(delayFirstShot);
            
            playSoundSkill();

            for (var i = 0; i < numOfAnimation; i++)
            {
                for (var j = 0; j < shootPositionCountPerAnim; j += 2)
                {
                    var angleIndex1 =  i * shootPositionCountPerAnim + j;
                    var angleIndex2 = i * shootPositionCountPerAnim + j + 1;

                    shootBullet(listAngles[angleIndex1 % listAngles.Count], radius, bulletData);
                    shootBullet(listAngles[angleIndex2 % listAngles.Count], radius, bulletData);
                    // yield return new WaitForSeconds(0.4f);
                    // shootBullet(listAngles[angleIndex1 % listAngles.Length], radius, bulletData);
                    // shootBullet(listAngles[angleIndex2 % listAngles.Length], radius, bulletData);
                    if (i < numOfAnimation - 1)
                    {
                        yield return new WaitForSeconds(interval);
                    }
                }
            }

            m_Animator.SetBool(IsSkill3, false);
        }
    }

    private void shootBullet(float angle, float radius, BulletData bulletData)
    {
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin( angle * Mathf.Deg2Rad));
        var thisPosition = transform.position;
        float x = thisPosition.x + radius * dir.x;
        float y = thisPosition.y + radius * dir.y;
        var position = new Vector3(x, y, 0f);
        
        GameScene.Instance.spawnBullet(position, bulletData, (position - thisPosition).normalized);
    }
}