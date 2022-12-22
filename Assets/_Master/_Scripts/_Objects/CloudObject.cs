using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CloudObject : MonoBehaviour
{
    public Bounds bounds => m_SpriteRenderer.bounds;
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    public float Speed { get; private set; }
    private UnityAction<CloudObject> m_OnFinish;
    private Rect m_ScreenRect;


    public void startMoving(float speed, UnityAction<CloudObject> onFinish)
    {
        Speed = speed;
        m_OnFinish = onFinish;
        gameObject.SetActive(true);
    }
    
    // Start is called before the first frame update
    private void Awake()
    {
        m_ScreenRect = Camera.main.getScreenRect();
    }

    private void OnValidate()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Translate(new Vector3(Speed, 0f, 0f) * Time.deltaTime, Space.World);
        var bounds = m_SpriteRenderer.bounds;
        if (bounds.max.x < m_ScreenRect.xMin || bounds.min.x > m_ScreenRect.xMax || bounds.max.y < m_ScreenRect.yMin ||
            bounds.min.y > m_ScreenRect.yMax)
        {
            m_OnFinish?.Invoke(this);
        }
    }
}
