using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crates : MonoBehaviour
{
    [SerializeField]
    private GameObject[] dropUps;

    [SerializeField]
    private GameObject[] crateParts;

    [SerializeField]
    float pieceRandomPosition = 2f;
    [SerializeField]
    float dropChance = 20f;

    public float minBreakForce = 15f;
    public float waitToDestroy = .2f;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Destroy()
    {
        foreach (GameObject part in crateParts)
        {
            Vector3 randomizeCreatePosition = new Vector3(Random.Range(-pieceRandomPosition, pieceRandomPosition), Random.Range(0f, pieceRandomPosition), Random.Range(-pieceRandomPosition, pieceRandomPosition));
            Instantiate(part, transform.position + randomizeCreatePosition, Quaternion.Euler(new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f))));
        }

        if (Random.Range(0f, 100f) <= dropChance)
        {
            int randomObject = Random.Range(0, dropUps.Length);
            Instantiate(dropUps[randomObject], transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Player_One" || other.transform.tag == "Player_Two" || other.transform.tag == "Weapon")
        {
            if (other.relativeVelocity.magnitude >= minBreakForce)
            {
                StartCoroutine(WaitAndBreak(waitToDestroy));
                // Destroy();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Weapon")
        {
            Destroy();
        }
    }

    IEnumerator WaitAndBreak(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy();
    }

}
