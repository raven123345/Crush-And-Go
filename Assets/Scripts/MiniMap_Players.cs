using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap_Players : MonoBehaviour
{
    public enum player_minimap
    {
        pl_one, pl_two
    };
    public player_minimap player;

    GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        switch(player)
        {
            case player_minimap.pl_one:
                target = GameManager.instance.pl_one.player;
                break;
            case player_minimap.pl_two:
                target = GameManager.instance.pl_two.player;
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(target)
        {
            transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        }
        else
        {
            switch (player)
            {
                case player_minimap.pl_one:
                    target = GameManager.instance.pl_one.player;
                    break;
                case player_minimap.pl_two:
                    target = GameManager.instance.pl_two.player;
                    break;
            }
        }
    }
}
