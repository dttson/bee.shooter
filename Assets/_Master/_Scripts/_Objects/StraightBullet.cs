using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightBullet : Bullet
{
    // Update is called once per frame
    protected override void Update()
    {
        transform.Translate(m_Direction * m_Speed, Space.World);
        base.Update();
    }
}
