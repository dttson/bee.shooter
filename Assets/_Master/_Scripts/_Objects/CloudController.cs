using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    [SerializeField] private bool m_IsDayLight = true;
    [SerializeField] private Camera m_Camera;
    [SerializeField] private float m_Frequency = 3f;
    [SerializeField] private float m_MinSpeed = 0.1f;
    [SerializeField] private float m_MaxSpeed = 0.5f;
    [SerializeField] private List<CloudObject> m_DayClouds;
    [SerializeField] private List<CloudObject> m_NightClouds;
    [SerializeField] private List<CloudObject> m_ActiveClouds;
    
    [Header("Rain")]
    [SerializeField] private float m_RainFrequency = 20f;
    [SerializeField] private float m_DurationMin = 5f;
    [SerializeField] private float m_DurationMax = 7f;
    [SerializeField] private GameObject m_RainEffect;

    // Start is called before the first frame update
    private void Awake()
    {
        m_Camera = Camera.main;
        foreach (CloudObject cloud in m_DayClouds)
        {
            cloud.gameObject.SetActive(false);
        }
        
        foreach (CloudObject cloud in m_NightClouds)
        {
            cloud.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        StartCoroutine(coroutineSpawnCloud());
        StartCoroutine(coroutineRain());
    }

    private IEnumerator coroutineRain()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_RainFrequency);

            if (CommonUtils.getRandomBoolean())
            {
                m_RainEffect.SetActive(true);
            }
            
            yield return new WaitForSeconds(Random.Range(m_DurationMin, m_DurationMax));
            
            m_RainEffect.SetActive(false);
        }
    }

    private IEnumerator coroutineSpawnCloud()
    {
        Vector3 topLeftPos = m_Camera.getTopLeftPosition();
        Vector3 bottomRightPos = m_Camera.getBottomRightPosition();
        float screenHeight = topLeftPos.y - bottomRightPos.y;
        
        while (true)
        {
            yield return new WaitForSeconds(m_Frequency);

            var clouds = m_IsDayLight ? m_DayClouds : m_NightClouds;
            int count = Mathf.Min(Random.Range(1, 3), clouds.Count);
            for (int i = count - 1; i >= 0; i--)
            {
                var cloud = clouds[i];

                float direction = Random.Range(0, 2) % 2 == 0 ? 1 : -1;
                float speed = direction * Random.Range(m_MinSpeed, m_MaxSpeed);
                float x = direction > 0
                    ? topLeftPos.x - cloud.bounds.size.x / 2
                    : bottomRightPos.x + cloud.bounds.size.x / 2;
                float y = Random.Range(bottomRightPos.y + screenHeight/5, screenHeight - screenHeight/5);
                
                cloud.transform.localPosition = new Vector3(x, y, 0f);
                cloud.startMoving(speed, onCloudFinishMove);
                m_ActiveClouds.Add(cloud);
                clouds.RemoveAt(i);
                
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private void onCloudFinishMove(CloudObject cloud)
    {
        cloud.gameObject.SetActive(false);
        
        m_ActiveClouds.Remove(cloud);
        var clouds = m_IsDayLight ? m_DayClouds : m_NightClouds;
        clouds.Add(cloud);
    }
}