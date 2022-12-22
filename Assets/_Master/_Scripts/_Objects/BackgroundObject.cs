using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BackgroundType
{
    LAND,
    WATER
}

public enum BackgroundItemType
{
    LAND,
    ROCK,
    WATER_FERN
}

public class BackgroundObject : MonoBehaviour
{
    struct ItemArea
    {
        public float minX;
        public float maxX;
        public float minY;
        public float maxY;
    }

    public Bounds bounds => m_SpriteRenderer.bounds;

    [SerializeField] private Camera m_Camera;
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private bool m_IsDayLight = true;

    //color to blend all background items for day/night
    [SerializeField] private BackgroundType m_BackgroundType = BackgroundType.WATER;
    [SerializeField] private Sprite m_BackgroundLand;
    [SerializeField] private Sprite m_BackgroundWater;
    [SerializeField] private Transform m_ItemContainerLeft;
    [SerializeField] private Transform m_ItemContainerRight;
    [SerializeField] private List<Transform> m_SideItems;
    [SerializeField] private List<ItemArea> m_Areas = new List<ItemArea>();
    [SerializeField] private int m_MinItemPerType = 1;
    [SerializeField] private int m_MaxItemPerType = 4;
    [SerializeField] private SpriteRenderer m_SeashoreWaterToLand;
    [SerializeField] private SpriteRenderer m_SeashoreLandToWater;

    [Header("Land")] [SerializeField] private Transform[] m_LandItems;
    [SerializeField] private Transform[] m_RockItems;

    [Header("Water")] [SerializeField] private Transform[] m_WaterItems;
    [SerializeField] private Transform[] m_WaterEffectItems;

    private void Awake()
    {
        disableItems(m_LandItems);
        disableItems(m_RockItems);
        disableItems(m_WaterItems);
        disableItems(m_WaterEffectItems);
        setBackground(BackgroundType.LAND);
    }

    public void changeBackgroundColor(Vector4 color)
    {
        blendColorBackground(new Transform[]{m_SpriteRenderer.transform}, color);
        blendColorBackground(m_SideItems.ToArray(), color);
        blendColorBackground(m_LandItems, color);
        blendColorBackground(m_RockItems, color);
        blendColorBackground(m_WaterItems, color);
        blendColorBackground(new [] {m_SeashoreLandToWater.transform, m_SeashoreWaterToLand.transform}, color);
    }

    private void blendColorBackground(Transform[] renderers, Vector4 color)
    {
        foreach (var item in renderers)
        {
            var spriteRenderer = item.GetComponent<SpriteRenderer>();
            spriteRenderer.material.DOColor(color, "_BlendColor", Config.BLEND_BACKGROUND_DURATION);
            spriteRenderer.material.DOFloat(color.w, "_Opacity", Config.BLEND_BACKGROUND_DURATION);
        }
    }

    private void OnValidate()
    {
        m_SideItems.Clear();
        foreach (Transform child in m_ItemContainerLeft)
        {
            m_SideItems.Add(child);
        }

        foreach (Transform child in m_ItemContainerRight)
        {
            m_SideItems.Add(child);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        Vector3 topLeftPos = m_Camera.getTopLeftPosition();
        Vector3 bottomRightPos = m_Camera.getBottomRightPosition();

        m_ItemContainerLeft.transform.localPosition = new Vector3(topLeftPos.x, 0f, 0f);
        m_ItemContainerRight.transform.localPosition = new Vector3(bottomRightPos.x, 0f, 0f);
        randomBackground();

        var screenRect = m_Camera.getScreenRect();
        float scale = screenRect.width / m_SeashoreWaterToLand.bounds.size.x;
        m_SeashoreWaterToLand.transform.localScale = new Vector3(scale, scale, 0f);
        m_SeashoreLandToWater.transform.localScale = new Vector3(scale, scale, 0f);
    }

    private void disableItems(Transform[] items)
    {
        foreach (Transform item in items)
        {
            item.gameObject.SetActive(false);
        }
    }

    public void randomBackground(BackgroundObject previousBG = null)
    {
        if (previousBG != null)
        {
            m_BackgroundType = Random.Range(0, 2) % 2 == 0 ? BackgroundType.LAND : BackgroundType.WATER;
            m_BackgroundType = BackgroundType.WATER;
        }


        // disable all items
        disableItems(new[] {m_SeashoreWaterToLand.transform, m_SeashoreLandToWater.transform});
        disableItems(m_LandItems);
        disableItems(m_RockItems);
        disableItems(m_WaterItems);
        disableItems(m_WaterEffectItems);
        disableItems(m_SideItems.ToArray());

        // add seashore if background change type
        float deltaY = 0f;
        if (previousBG != null)
        {
            if (m_BackgroundType == BackgroundType.LAND && previousBG.m_BackgroundType == BackgroundType.WATER)
            {
                m_SeashoreWaterToLand.gameObject.SetActive(true);
                var seashoreHeight = m_SeashoreWaterToLand.bounds.size.y;
                m_SeashoreWaterToLand.transform.localPosition =
                    new Vector3(0f, -bounds.size.y / 2 - seashoreHeight, 0f);
                transform.localPosition += new Vector3(0, seashoreHeight, 0f);
            }
            else if (m_BackgroundType == BackgroundType.WATER && previousBG.m_BackgroundType == BackgroundType.LAND)
            {
                m_SeashoreLandToWater.gameObject.SetActive(true);
                var seashoreHeight = m_SeashoreWaterToLand.bounds.size.y;
                m_SeashoreLandToWater.transform.localPosition = 
                    new Vector3(0f, -bounds.size.y / 2 - seashoreHeight, 0f);
                transform.localPosition += new Vector3(0, seashoreHeight, 0f);
            }
        }

        // init new background and items
        setupArea(m_BackgroundType);
        setBackground(m_BackgroundType);

        if (m_BackgroundType == BackgroundType.LAND)
        {
            m_MinItemPerType = 1;
            m_MaxItemPerType = 4;
            randomCenterItems(m_LandItems, m_MinItemPerType, m_MaxItemPerType);
            randomCenterItems(m_RockItems, m_MinItemPerType, m_MaxItemPerType);
            randomSideItems();
        }
        else
        {
            m_MinItemPerType = 2;
            m_MaxItemPerType = 3;
            randomWaterFerns(m_WaterItems, m_MinItemPerType, m_MaxItemPerType);
            randomWaterEffects(m_WaterEffectItems, 8, 16);
        }
    }

    private void setupArea(BackgroundType type)
    {
        Vector3 topLeftPos = m_Camera.getTopLeftPosition();
        Vector3 bottomRightPos = m_Camera.getBottomRightPosition();
        Vector3 centerPos = (topLeftPos + bottomRightPos) / 2;
        float screenWidth = bottomRightPos.x - topLeftPos.x;
        float screenHeight = topLeftPos.y - bottomRightPos.y;

        if (m_BackgroundType == BackgroundType.LAND)
        {
            m_Areas.Add(
                new ItemArea
                {
                    minX = topLeftPos.x, maxX = centerPos.x, minY = centerPos.y, maxY = topLeftPos.y
                }
            );
            m_Areas.Add(
                new ItemArea
                {
                    minX = centerPos.x, maxX = bottomRightPos.x, minY = centerPos.y, maxY = topLeftPos.y
                }
            );
            m_Areas.Add(
                new ItemArea
                {
                    minX = bottomRightPos.x, maxX = centerPos.x, minY = bottomRightPos.y, maxY = centerPos.y
                }
            );
            m_Areas.Add(
                new ItemArea
                {
                    minX = centerPos.x, maxX = bottomRightPos.x, minY = bottomRightPos.y, maxY = centerPos.y
                }
            );
        }
        else
        {
            m_Areas.Add(
                new ItemArea
                {
                    minX = topLeftPos.x,
                    maxX = topLeftPos.x + screenWidth / 4,
                    minY = centerPos.y + screenHeight / 5,
                    maxY = centerPos.y + screenHeight / 4
                }
            );
            m_Areas.Add(
                new ItemArea
                {
                    minX = centerPos.x + screenWidth / 4,
                    maxX = bottomRightPos.x,
                    minY = centerPos.y + screenHeight / 5,
                    maxY = centerPos.y + screenHeight / 4
                }
            );
            m_Areas.Add(
                new ItemArea
                {
                    minX = topLeftPos.x,
                    maxX = topLeftPos.x + screenWidth / 4,
                    minY = bottomRightPos.y + screenHeight / 5,
                    maxY = bottomRightPos.y + screenHeight / 4
                }
            );
            m_Areas.Add(
                new ItemArea
                {
                    minX = centerPos.x + screenWidth / 4,
                    maxX = bottomRightPos.x,
                    minY = bottomRightPos.y + screenHeight / 5,
                    maxY = bottomRightPos.y + screenHeight / 4
                }
            );
        }
    }

    public void setBackground(BackgroundType type)
    {
        if (type == BackgroundType.LAND)
        {
            m_SpriteRenderer.sprite = m_BackgroundLand;
        }
        else
        {
            m_SpriteRenderer.sprite = m_BackgroundWater;
        }
    }

    private void randomSideItems()
    {
        foreach (Transform item in m_SideItems)
        {
            item.gameObject.SetActive(true);
            item.transform.localPosition = new Vector3(0f, Random.Range(-bounds.size.y / 2, bounds.size.y / 2), 0f);
        }
    }

    private void randomCenterItems(Transform[] items, int minItemCount, int maxItemCount)
    {
        m_Areas.Shuffle();

        int areaIndex = 0;
        int count = Math.Min(items.Length, Random.Range(minItemCount, maxItemCount + 1));
        for (int i = 0; i < count; i++)
        {
            var area = m_Areas[areaIndex % m_Areas.Count];
            float x = Random.Range(area.minX, area.maxX);
            float y = Random.Range(area.minY, area.maxY);
            items[i].localPosition = new Vector3(x, y, 0f);
            items[i].gameObject.SetActive(true);
            areaIndex++;
        }
    }
    
    private void randomWaterFerns(Transform[] items, int minItemCount, int maxItemCount)
    {
        m_Areas.Shuffle();

        int areaIndex = 0;
        int count = Math.Min(items.Length, Random.Range(minItemCount, maxItemCount + 1));
        for (int i = 0; i < count; i++)
        {
            var area = m_Areas[areaIndex % m_Areas.Count];
            var item = items[i];
            var fernHeight = item.GetComponent<SpriteRenderer>().bounds.size.y;
            float x = Random.Range(area.minX, area.maxX);
            float y = Random.Range(area.minY + fernHeight/2, area.maxY - fernHeight/2);
            item.localPosition = new Vector3(x, y, 0f);
            item.gameObject.SetActive(true);
            areaIndex++;
        }
    }

    private void randomWaterEffects(Transform[] items, int minItemCount, int maxItemCount)
    {
        m_Areas.Shuffle();

        var screenRect = m_Camera.getScreenRect();
        int count = Math.Min(items.Length, Random.Range(minItemCount, maxItemCount + 1));
        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(screenRect.xMin, screenRect.xMax);
            float y = Random.Range(screenRect.yMin, screenRect.yMax);
            items[i].localPosition = new Vector3(x, y, 0f);
            items[i].gameObject.SetActive(true);
        }
    }
}