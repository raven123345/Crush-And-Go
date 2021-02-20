using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDirection : MonoBehaviour
{
    [HideInInspector]
    public Vector3 hitDirection;

    [SerializeField]
    float hitDirectionTimer = 0.01f;
    float curHitDirectionTime;
    Vector3 previsousPosition;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //hitDirection = GetHitDirection();
    }

    public Vector3 GetHitDirection()
    {
        Vector3 hitDir = Vector3.zero;
        curHitDirectionTime -= Time.deltaTime;

        if (curHitDirectionTime <= hitDirectionTimer)
        {
            curHitDirectionTime = hitDirectionTimer;
            hitDir = transform.position - previsousPosition;
            previsousPosition = transform.position;

            return hitDir;
        }
        else
        {
            return hitDir;
        }
    }
}
