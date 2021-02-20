using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenPieceFlyingDirection : MonoBehaviour
{


    [SerializeField]
    float partflyingForce = 5f;

    [SerializeField]
    float lifeTime = 2f;

    float curTimer;
    Rigidbody partRigidBody;

    void Start()
    {
        partRigidBody = GetComponent<Rigidbody>();

        if (partRigidBody)
        {
            Vector3 randomForceVector = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f));
            Vector3 randomTorqueVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));

            float randomForceMultiplier = Random.Range(partflyingForce, partflyingForce * 1.5f);
            float randomTorqueMultiplier = Random.Range(-partflyingForce, partflyingForce);

            partRigidBody.AddForce(randomForceVector * randomForceMultiplier, ForceMode.Impulse);
            partRigidBody.AddTorque(randomTorqueVector * randomTorqueMultiplier, ForceMode.Impulse);
        }

        curTimer = Random.Range(lifeTime, lifeTime * 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        curTimer -= Time.deltaTime;
        if (curTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
