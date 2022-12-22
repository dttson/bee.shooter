#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using UnityEngine.Events;

public class TouchableObject : MonoBehaviour
{
    [SerializeField] private Camera m_Camera;
    [SerializeField] private BoxCollider2D m_BoxCollider2D;
    [SerializeField] private Vector2 m_Size;
    [SerializeField] private float m_OriginZ = 0f;
    [SerializeField] private UnityEvent m_OnTouchBegan;
    [SerializeField] private UnityEvent m_OnTouchMoved;
    [SerializeField] private UnityEvent m_OnTouchEnded;
    
    public bool m_UseCustomSize = false;
    [HideInInspector] public Vector2 m_CustomSize = Vector2.zero;

    private bool m_IsTouchBegan = false;
    private Vector3 m_DeltaPositon;
    private Rect m_ScreenRect;

    private void OnValidate()
    {
        m_BoxCollider2D = GetComponent<BoxCollider2D>();
        if (m_UseCustomSize)
        {
            m_Size = m_CustomSize;
        }
        else
        {
            m_Size = m_BoxCollider2D.size;
        }
    }

    private void Awake()
    {
        m_Camera = Camera.main;
        m_ScreenRect = m_Camera.getScreenRect();
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_OriginZ = transform.position.z;
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
            if (hit.collider)
            {
                m_IsTouchBegan = true;
                var newPosition= m_Camera.ScreenToWorldPoint(touchPosition);
                newPosition.z = m_OriginZ;

                m_DeltaPositon = newPosition - transform.position;
                m_OnTouchBegan?.Invoke();
            }
        }
        else if (m_IsTouchBegan && TouchUtils.isTouchMoved(out touchPosition))
        {
            touchPosition.z = -m_Camera.transform.position.z;
            var newPosition= m_Camera.ScreenToWorldPoint(touchPosition);
            newPosition.z = m_OriginZ;
            newPosition = newPosition - m_DeltaPositon;
            var localScale = transform.localScale;
            newPosition.x = Mathf.Min(newPosition.x, m_ScreenRect.xMax - m_Size.x * localScale.x / 2);
            newPosition.x = Mathf.Max(newPosition.x, m_ScreenRect.xMin + m_Size.x * localScale.x / 2);

            transform.position = newPosition;
            m_OnTouchMoved?.Invoke();
        }
        else if (m_IsTouchBegan && TouchUtils.isTouchEnd(out touchPosition))
        {
            m_DeltaPositon = Vector3.zero;
            m_IsTouchBegan = false;
            m_OnTouchEnded?.Invoke();
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TouchableObject))]
public class TouchableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TouchableObject myTarget = (TouchableObject)target;

        if (myTarget.m_UseCustomSize)
        {
            myTarget.m_CustomSize = EditorGUILayout.Vector2Field("Custom size", myTarget.m_CustomSize);
        }
    }
}
#endif