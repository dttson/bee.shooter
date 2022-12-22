using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public static GameUIController Instance { get; private set; }

    [SerializeField] private GameObject m_LayerPause;
    [SerializeField] private HealthMenu m_HealthMenu;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()    
    {
        m_LayerPause.SetActive(false);
    }

    public void updateHeathBar(int hp)
    {
        m_HealthMenu.setHeart(hp);
    }

    public void onPauseGame()
    {
        Time.timeScale = 0f;
        m_LayerPause.SetActive(true);
    }

    public void onBackGame()
    {
        Time.timeScale = 1f;
        m_LayerPause.SetActive(false);
    }
}
