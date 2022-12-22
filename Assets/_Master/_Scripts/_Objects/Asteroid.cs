using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private float m_MoveSpeedMin = 0.1f;
    [SerializeField] private float m_MoveSpeedMax = 0.5f;
    [SerializeField] private float m_RotateSpeedMin = 180f; //degree
    [SerializeField] private float m_RotateSpeedMax = 360f; //degree
    
    private float m_MoveSpeed = 0f;
    private float m_RotateSpeed = 0f;
    private UnityAction<Asteroid> m_OnDestroy;
    private Rect m_ScreenRect;
    
    public static Asteroid createAsteroid(UnityAction<Asteroid> onDestroy)
    {
        var asteroid = Instantiate(Resources.Load<Asteroid>("Asteroid"));
        asteroid.m_OnDestroy = onDestroy;
        return asteroid;
    }

    public void reload(Sprite sprite)
    {
        m_SpriteRenderer.sprite = sprite;
        m_MoveSpeed = Random.Range(m_MoveSpeedMin, m_MoveSpeedMax);
        m_RotateSpeed = Random.Range(m_RotateSpeedMin, m_RotateSpeedMax);
    }

    private void Start()
    {
        m_ScreenRect = Camera.main.getScreenRect();
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Translate(0f, -m_MoveSpeed * Time.deltaTime, 0f);
        m_SpriteRenderer.transform.Rotate(Vector3.forward, m_RotateSpeed * Time.deltaTime, Space.Self);
        
        var bounds = m_SpriteRenderer.bounds;
        if (bounds.max.x < m_ScreenRect.xMin || bounds.min.x > m_ScreenRect.xMax || bounds.max.y < m_ScreenRect.yMin)
        {
            m_OnDestroy?.Invoke(this);
        }
    }
}
