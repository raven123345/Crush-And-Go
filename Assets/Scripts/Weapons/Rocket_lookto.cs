using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket_lookto : MonoBehaviour
{
    public GameObject target;
    public float speed = 5f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), Time.deltaTime * speed);
        //transform.position += transform.forward * speed * Time.deltaTime;
        Debug.DrawRay(transform.position, gameObject.transform.forward - transform.position);
    }
}
