using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public CarController car;
    public float invincibleTime = 1f;

    private float curInvincibleTime;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (curInvincibleTime > 0)
            curInvincibleTime -= Time.deltaTime;
    }

    public void GiveDamage(int damage)
    {
        switch (car.player)
        {
            case CarController.cc_Player.Player_One:
                GameManager.instance.ChangeHealthPoints(GameManager.instance.pl_one, -damage, true);
                break;

            case CarController.cc_Player.Player_Two:
                GameManager.instance.ChangeHealthPoints(GameManager.instance.pl_two, -damage, true);
                break;
        }
        curInvincibleTime = invincibleTime;
    }
}
