using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    public static TimerController instance;

    public Text timeCounter;

    private System.TimeSpan timePlaying;
    private bool timerGoing;

    private float elapsedTime;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        timeCounter.text = "Time: 00:00.00";
        timerGoing = false;
        
        if (Application.loadedLevelName == "MainScene")
        {
            BeginTimer();
        }
        if (Application.loadedLevelName == "SecondLevel")
        {
            BeginTimer();
        }
        if (Application.loadedLevelName == "EndScreen")
        {
            EndTimer();
        }
    }

    public void BeginTimer()
    {
        timerGoing = true;
        elapsedTime = 0f;

        StartCoroutine(UpdateTimer());
    }

    public void EndTimer()
    {
        timerGoing = false;
    }

    public IEnumerator UpdateTimer()
    {
        while (timerGoing)
        {
            elapsedTime += Time.deltaTime;
            timePlaying = System.TimeSpan.FromSeconds(elapsedTime);
            string timePlayingString = "Time: " + timePlaying.ToString("mm':'ss'.'ff");
            timeCounter.text = timePlayingString;

            yield return null;
        }
    }
}
