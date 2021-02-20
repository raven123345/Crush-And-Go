using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropUps : MonoBehaviour
{
    public static DropUps instance;

    public Transform[] dropUpPoints;

    [SerializeField]
    GameObject[] dropItems;

    [SerializeField]
    float dropRate = 20f;
    [SerializeField]
    float dropUpRadius = 15f;
    bool start_dropping = false;

    public void Awake()
    {
        instance = this;
    }
    void Start()
    {

    }

    void Update()
    {
        if (GameManager.instance.gameStarted && !start_dropping)
        {
            StartCoroutine(DropUpTimer());
            start_dropping = true;
        }
    }

    void DropItem()
    {

        int randomDropUpPoint = (int)Random.Range(0f, dropUpPoints.Length);
        int randomDropUpItem = (int)Random.Range(0f, dropItems.Length);

        Vector3 randomDropUpSpot = dropUpPoints[randomDropUpPoint].position + (Random.insideUnitSphere * dropUpRadius);
        randomDropUpSpot = new Vector3(randomDropUpSpot.x, transform.position.y, randomDropUpSpot.z);

        Ray ray = new Ray();
        ray.origin = randomDropUpSpot;
        ray.direction = new Vector3(randomDropUpSpot.x, randomDropUpSpot.y - 1f, randomDropUpSpot.z) - ray.origin;

        RaycastHit hit;

        Physics.Raycast(ray, out hit);

        randomDropUpSpot = new Vector3(hit.point.x, hit.point.y + Random.Range(5f, 10f), hit.point.z);

        Instantiate(dropItems[randomDropUpItem], randomDropUpSpot, Quaternion.Euler(Random.insideUnitSphere * 360f));

    }

    IEnumerator DropUpTimer()
    {
        if (GameManager.instance.gameStarted)
        {
            yield return new WaitForSeconds(dropRate);
            DropItem();
            StartCoroutine(DropUpTimer());
        }
    }
}
