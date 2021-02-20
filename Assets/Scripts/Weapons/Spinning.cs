using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinning : MonoBehaviour
{

    [SerializeField]
    AnimationCurve pulsing = new AnimationCurve();
    [SerializeField]
    float pulsing_speed = 2f;
    [SerializeField]
    float pulsing_mul = 0.02f;
    [SerializeField]
    float rotating_speed = 100f;

    float cur_time = 0f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        cur_time += Time.deltaTime * pulsing_speed;

        if (cur_time >= 1f)
            cur_time = 0f;

        transform.position = new Vector3(transform.position.x, transform.position.y + (pulsing.Evaluate(cur_time) * pulsing_mul), transform.position.z);
        transform.Rotate(Vector3.up * Time.deltaTime * rotating_speed);
    }
}
