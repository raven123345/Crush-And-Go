using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloveController : MonoBehaviour
{
    private BoxingGloveController boxingGloveController;
    // Start is called before the first frame update
    void Start()
    {
        boxingGloveController = GetComponentInParent<BoxingGloveController>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == boxingGloveController.otherTag || other.transform.tag == "Enemy" || other.transform.tag == "Breakable" || other.transform.tag == "Hittable")
        {
            Rigidbody[] rb = other.GetComponents<Rigidbody>();

            foreach (Rigidbody rigidBody in rb)
            {
                if (rigidBody)
                {
                    rigidBody.AddForce(boxingGloveController.hitDirection * boxingGloveController.hitPower * 10000f, ForceMode.Impulse);
                    rigidBody.AddForce(Vector3.zero);
                }
            }
        }

        if (other.transform.tag == boxingGloveController.otherTag)
        {
            GameManager.instance.AddScore(boxingGloveController.playerToGivePoints, 2);

            Damage otherDamage = other.GetComponent<Damage>();
            if (otherDamage)
            {
                otherDamage.GiveDamage(boxingGloveController.damagePoints);
            }
        }

        if (other.transform.tag == "Enemy")
        {
            GameManager.instance.AddScore(boxingGloveController.playerToGivePoints, 1);

            Damage otherDamage = other.GetComponent<Damage>();
            if (otherDamage)
            {
                otherDamage.GiveDamage(boxingGloveController.damagePoints);
            }
        }
    }
}
