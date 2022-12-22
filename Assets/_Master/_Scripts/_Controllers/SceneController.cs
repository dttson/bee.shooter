using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    private Coroutine m_CoroutineReload;
    public void reloadScene(SceneName scene, UnityAction onFinish = null)
    {
        if (m_CoroutineReload != null) StopCoroutine(m_CoroutineReload);
        
        m_CoroutineReload = StartCoroutine(coroutineReloadScene(scene.ToString(), onFinish));
    }
    
    IEnumerator coroutineReloadScene(string sceneName, UnityAction onFinish = null)
    {
        yield return SceneManager.UnloadSceneAsync(sceneName);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        onFinish?.Invoke();
    }

    public void loadHome()
    {
        SceneManager.LoadScene(SceneName.Home.ToString());
    }

    public void loadGame(UnityAction onFinish = null)
    {
        IEnumerator loadSceneAsync()
        {
            yield return SceneManager.LoadSceneAsync(SceneName.Game.ToString(), LoadSceneMode.Additive);
            onFinish?.Invoke();
        }

        StartCoroutine(loadSceneAsync());
    }
}
