using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField]
    int checkPointNumber;


    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.transform.tag)
        {
            case "Player_One":
                GameManager.instance.CheckPointAchieved(GameManager.instance.pl_one, checkPointNumber);
                break;
            case "Player_Two":
                GameManager.instance.CheckPointAchieved(GameManager.instance.pl_two, checkPointNumber);
                break;
        }

    }
}
