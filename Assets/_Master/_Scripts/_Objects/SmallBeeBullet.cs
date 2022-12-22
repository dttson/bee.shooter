using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SmallBeeBullet : Bullet
{
    private bool m_HasTarget = false;
    private EnemyBase m_Target;
    private float m_RandomSpeed = 0f;

    public override void reload(BulletData data, Vector2 direction, Dictionary<string, object> otherData)
    {
        base.reload(data, direction, otherData);
        
        m_HasTarget = true;
        m_Target = (EnemyBase) otherData["target"];
        m_RandomSpeed = data.speed * Random.Range(0.9f, 1.1f);
    }

    protected override void Update()
    {
        base.Update();

        if (m_HasTarget)
        {
            if (!m_Target.IsActive)
            {
                m_HasTarget = false;
                destroy();
                return;
            }

            float singleStep = m_RandomSpeed * Time.deltaTime;
            var direction = m_Target.transform.position.toVector2() - transform.position.toVector2();
            transform.Translate(direction.normalized * singleStep);
            
            transform.up = direction;
        }
    }
}