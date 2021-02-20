using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Timer : MonoBehaviour
{
    public Text timerText;

    private TimeSpan time;
    public Color pulsingColor;

    public AnimationCurve pulsingModel;
    public float secondsToStartPulsing = 10f;

    public float scaleFactor = 2f;
    public float pulsingSpeed = 1f;

    private bool startPulsing = false;
    private float curPulse = 0f;
    private float curTime;
    private Vector3 initScale;

    private Transform textTransform;
    void Start()
    {
        textTransform = GetComponent<RectTransform>();
        curTime = GameManager.instance.gameLength;
        initScale = textTransform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        //todo if start game
        if (GameManager.instance.gameStarted)
        {
            curTime -= Time.deltaTime;
            time = TimeSpan.FromSeconds(curTime);
            timerText.text = time.ToString("mm':'ss");

            if (time.Minutes == 0f && time.Seconds <= secondsToStartPulsing)
            {
                startPulsing = true;
            }
            if (time.Minutes <= 0f && time.Seconds <= 0f)
            { 
                GameManager.instance.gameStarted = false;
                GameManager.instance.GamePlayUI.gameObject.SetActive(false);
                GameManager.instance.end_screen.gameObject.SetActive(true);
            }
            if (startPulsing)
            {
                Pulsing();
            }
        }
    }

    void Pulsing()
    {
        curPulse += Time.deltaTime * pulsingSpeed;
        timerText.color = pulsingColor;
        if (curPulse >= 1f)
        { curPulse = 0f; }
        textTransform.localScale = Vector3.Lerp(initScale, initScale * scaleFactor, pulsingModel.Evaluate(curPulse));
    }
}
