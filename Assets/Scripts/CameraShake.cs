using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float _shakePower;
    private float _shakeDuration;
    private float _shakeFrequency;
    private float _stopTime;

    private bool shake;

    float shakeCurDur;
    float shakeCurFreq;
    float curStopTime;
    Vector3 InitPos;


    void Update()
    {
        if (shake)
        {
            goShaking();
            _shakePower = Mathf.Lerp(_shakePower, 0f, Time.deltaTime * (1f / _shakeDuration));
            shakeCurDur -= Time.deltaTime;
        }

        if (shakeCurDur <= 0f && shake)
        {
            transform.position = InitPos;
            shake = false;
        }
    }

    internal void Shake(float shakePower = .5f, float shakeDuration = .5f, float shakeFrequency = .03f, float stopTime = 0.1f)
    {
        _shakePower = shakePower;
        _shakeDuration = shakeDuration;
        _shakeFrequency = shakeFrequency;
        _stopTime = stopTime;
        shake = true;

        shakeCurDur = _shakeDuration;
        shakeCurFreq = _shakeFrequency;
        curStopTime = _stopTime;
        InitPos = transform.position;
    }

    void goShaking()
    {
        curStopTime -= Time.deltaTime;

        if (curStopTime <= 0f)
        {
            Time.timeScale = 1f;

            shakeCurFreq -= Time.deltaTime;
            if (shakeCurFreq <= 0f)
            {
                Vector3 newPosition = Random.insideUnitSphere * _shakePower;
                transform.position += newPosition;

                shakeCurFreq = _shakeFrequency;
            }
        }
        else
        {
            // Time.timeScale = 0f;
        }
    }
}
