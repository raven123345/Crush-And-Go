using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : MonoBehaviour
{
    // Start is called before the first frame update
    public float knockDownForce;
    public float breakDownForce;
    [SerializeField]
    private GameObject[] fenceParts;
    [SerializeField]
    float pieceRandomPosition = 2f;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.tag == "Player_One" || collision.gameObject.transform.tag == "Player_Two")
        {

            if (collision.relativeVelocity.magnitude >= knockDownForce && collision.relativeVelocity.magnitude < breakDownForce)
            {
                rb.isKinematic = false;
            }
            else if (collision.relativeVelocity.magnitude >= knockDownForce && collision.relativeVelocity.magnitude >= breakDownForce)
            {
                Destroy();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Weapon"))
        {
            Destroy();
        }
    }

    private void Destroy()
    {
        foreach (GameObject part in fenceParts)
        {
            Vector3 randomizeCreatePosition = new Vector3(Random.Range(-pieceRandomPosition, pieceRandomPosition), Random.Range(0f, pieceRandomPosition), Random.Range(-pieceRandomPosition, pieceRandomPosition));
            Instantiate(part, transform.position + randomizeCreatePosition, Quaternion.Euler(new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f))));
        }
        Destroy(gameObject);
    }
}
