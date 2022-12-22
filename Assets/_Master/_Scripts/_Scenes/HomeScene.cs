using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
public class HomeScene : MonoBehaviour
{
    [SerializeField] private GameObject m_SettingMenu;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void onButtonPlay()
    {
        SceneManager.LoadScene(SceneName.Game.ToString());
    }

    public void onOpenSetting()
    {
        m_SettingMenu.SetActive(true);   
    }

    public void onCloseSetting()
    {
        m_SettingMenu.SetActive(false);
    }

    public void onButtonShare()
    {
        
    }

    public void onButtonRanking()
    {
        SceneManager.LoadScene(SceneName.Leaderboard.ToString());
    }
}
