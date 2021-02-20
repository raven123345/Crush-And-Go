using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public GameObject weapon;
    public string searchForWeaponNamePosition; 

    private CarController car;

    [SerializeField]
    float inactiveTime = 0.5f;

    float curIncativeTimer;
    private void Start()
    {
        curIncativeTimer = inactiveTime;
    }

    private void Update()
    {
        curIncativeTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Player_One" || other.gameObject.tag == "Player_Two") && curIncativeTimer < 0f)
        {
            car = other.GetComponent<CarController>();
            if (!car.hasWeapon)
            {
                foreach (Transform position in car.weaponPositions)
                {
                    if (position.name == searchForWeaponNamePosition)
                    {
                        car.weapon = Instantiate(weapon, position.position, position.rotation, other.transform);
                        car.hasWeapon = true;                       
                    }
                }

                Destroy(gameObject);
            }
        }
    }
}
