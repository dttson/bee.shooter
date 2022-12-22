using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    [SerializeField] private Camera m_Camera;

    private bool m_IsTouchBegan = false;
    
    private void Awake()
    {
        m_Camera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    private void Update()
    {
        Vector3 touchPosition;
        if (TouchUtils.isTouchBegan(out touchPosition))
        {
            var origin = m_Camera.ScreenToWorldPoint(touchPosition);
            var direction = Vector3.forward;
            
            RaycastHit2D hit = Physics2D.Raycast(origin, direction);
            if (hit.collider != null)
            {
                m_IsTouchBegan = true;
                transform.position = m_Camera.ScreenToWorldPoint(touchPosition);
            }
        }
        else if (m_IsTouchBegan && TouchUtils.isTouchMoved(out touchPosition))
        {
            touchPosition.z = -m_Camera.transform.position.z;
            transform.position = m_Camera.ScreenToWorldPoint(touchPosition);
        }
        else if (m_IsTouchBegan && TouchUtils.isTouchEnd(out touchPosition))
        {
            m_IsTouchBegan = false;
        }
    }
}
