using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour
{

    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetBool("Hit", true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            anim.SetBool("Hit", false);
        }
    }
}
