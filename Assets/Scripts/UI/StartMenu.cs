using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField]
    private Transform startPosition;
    [SerializeField]
    private Transform quitPosition;
    [SerializeField]
    private float cursorMoveSpeed;
    [SerializeField]
    private string sceneToLoad;

    private Vector3 moveToPos;

    private int curPos = 0;
    private bool moving = false;

    void Start()
    {
        transform.position = startPosition.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Vertical"))
        {
            if (curPos == 0)
            {
                curPos = 1;
                moveToPos = quitPosition.position;
            }
            else
            {
                curPos = 0;
                moveToPos = startPosition.position;
            }

            moving = true;
        }

        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveToPos, Time.deltaTime * cursorMoveSpeed);
            if (transform.position.magnitude == moveToPos.magnitude)
            {
                moving = false;
            }
        }

        if(Input.GetButtonDown("Submit"))
        {
            switch(curPos)
            {
                case 0:
                    SceneManager.LoadScene(sceneToLoad);
                    break;
                case 1:
                    Application.Quit();
                    break;
            }
        }
    }
}
