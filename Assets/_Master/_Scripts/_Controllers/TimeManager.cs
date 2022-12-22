using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float TimeScale = Config.DEFAULT_TIME_SCALE;

    public static TimeManager Instance { get; set; } 
    private void Awake()
    {
        var otherInstance = (TimeManager)FindObjectOfType(typeof(TimeManager));
        if (this != otherInstance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        Time.timeScale = TimeScale;
    }
}
