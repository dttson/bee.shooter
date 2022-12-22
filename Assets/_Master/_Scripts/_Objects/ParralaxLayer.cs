using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParralaxLayer : MonoBehaviour
{
    public static bool IsActive { get; set; } = true;

    [SerializeField] protected float m_Speed = 1.0f;
    [SerializeField] protected bool m_IsHorizontal = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update() 
    {
        if (IsActive)
        {
            if (m_IsHorizontal)
            {
                transform.Translate(m_Speed * Time.deltaTime, 0f, 0f);
            }
            else
            {
                transform.Translate(0f, m_Speed * Time.deltaTime, 0f);
            }
            
        }
    }
}
