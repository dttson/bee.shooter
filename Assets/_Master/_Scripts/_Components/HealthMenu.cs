using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthMenu : MonoBehaviour
{
    [SerializeField] private Image[] m_HeartImages;
    
    // Start is called before the first frame update
    private void Start()
    {
        m_HeartImages = GetComponentsInChildren<Image>();
    }

    public void setHeart(int heartCount)
    {
        if (heartCount < 0 || heartCount > m_HeartImages.Length) return;
        foreach (var image in m_HeartImages)
        {
            image.gameObject.SetActive(false);
        }

        for (int i = 0; i < heartCount; i++)
        {
            m_HeartImages[i].gameObject.SetActive(true);
        }
    }
}
