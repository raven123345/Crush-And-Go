
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartTimer : MonoBehaviour
{
    public float startCountDownSecond = 3f;
    // private TimeSpan time;
    //public UnityEngine.UI.Text timerText;
    public AnimationCurve animationModel;
    public float animationSpeed = 1f;
    public float scaleFactor = 2f;

    [SerializeField]
    GameObject[] numbers;
    [SerializeField]
    Transform numberParent;
    private float curAnimTime = 0f;
    private float curAnimVal;
    private float curTime;
    private GameObject curNumber = null;
    int seconds;
    int curSeconds;
    void Start()
    {
        curTime = startCountDownSecond + 1;
        curAnimTime = 1f;
        curSeconds = (int)curTime;

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.gameStarted)
        {
            curTime -= Time.deltaTime;
            seconds = TimeSpan.FromSeconds(curTime).Seconds;


            curAnimTime -= Time.deltaTime;

            if (curAnimTime <= 0f)
            { curAnimTime = 1f; }

            curAnimVal = animationModel.Evaluate(curAnimTime) * scaleFactor;

            if (numbers.Length > 0)
            {
                switch (seconds)
                {
                    case 5:
                        CreateNumber(numberParent, false);
                        break;
                    case 4:
                        CreateNumber(numberParent, false);
                        break;
                    case 3:
                        CreateNumber(numberParent, false);
                        break;
                    case 2:
                        CreateNumber(numberParent, false);
                        break;
                    case 1:
                        CreateNumber(numberParent, false);
                        break;
                    case 0:
                        CreateNumber(numberParent, true);
                        break;
                }
                // timerText.text = seconds.ToString();
                // timerText.transform.localScale = Vector3.one * curAnimVal;
            }
            if (seconds <= 0)
            {
                GameManager.instance.gameStarted = true;
                GameManager.instance.GamePlayUI.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }

        }
    }

    void CreateNumber(Transform parent, bool destroyAfterUse)
    {
        if (curSeconds != seconds)
        {
            if (curNumber)
            { Destroy(curNumber); }

            curSeconds = seconds;
        }
        if (!curNumber)
        { curNumber = Instantiate(numbers[seconds], parent); }

        curNumber.transform.localScale = Vector3.one * curAnimVal;
        if (destroyAfterUse && curSeconds != seconds)
        { Destroy(curNumber); }
    }
}
