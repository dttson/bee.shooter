using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
// ReSharper disable CommentTypo

public class BackgroundLayer : ParralaxLayer
{
    [SerializeField] private Camera m_Camera;
    [SerializeField] private List<SpriteRenderer> m_ListItems;

    private float m_BgSize = 0f;
    private Rect m_ScreenRect;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_Camera = Camera.main;
        m_BgSize = m_ListItems[0].bounds.size.x;
        m_ScreenRect = m_Camera.getScreenRect();
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    // ReSharper disable once IdentifierTypo
    void OnDrawGizmos()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        var bgItem = m_ListItems[0];

        float itemMin, itemMax, screenMin, screenMax = 0f;
        if (m_IsHorizontal)
        {
            itemMin = bgItem.bounds.min.x;
            itemMax = bgItem.bounds.max.x;
            screenMin = m_ScreenRect.min.x;
            screenMax = m_ScreenRect.max.x;
        }
        else
        {
            itemMin = bgItem.bounds.min.y;
            itemMax = bgItem.bounds.max.y;
            screenMin = m_ScreenRect.min.y;
            screenMax = m_ScreenRect.max.y;
        }

        if (m_Speed > 0)
        {    
            if (itemMin > screenMax)
            {
                var lastBgItem = m_ListItems[m_ListItems.Count - 1];
                if (m_IsHorizontal)
                {
                    bgItem.transform.updateX(lastBgItem.transform.position.x - m_BgSize);
                }
                else
                {
                    bgItem.transform.updateY(lastBgItem.transform.position.y - m_BgSize);
                }
                
                m_ListItems.RemoveAt(0);
                m_ListItems.Add(bgItem);
            }
        }
        else
        {
            if (itemMax < screenMin)
            {
                var lastBgItem = m_ListItems[m_ListItems.Count - 1];
                if (m_IsHorizontal)
                {
                    bgItem.transform.updateX(lastBgItem.transform.position.x + m_BgSize);
                }
                else
                {
                    bgItem.transform.updateY(lastBgItem.transform.position.y + m_BgSize);
                }
                m_ListItems.RemoveAt(0);
                m_ListItems.Add(bgItem);
            }
        }
    }

}
