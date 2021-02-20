using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Start is called before the first frame update

    public int pointsToGive = 5;
    [SerializeField]
    float rotSpeed = 1f;
    [SerializeField]
    float pulsingSpeed = 1f;
    [SerializeField]
    float pulsingAmplitude = 1f;
    [SerializeField]
    ParticleSystem ps;
    [SerializeField]
    float inactiveTime = 0.2f;

    float curInactiveTime;

    float curPulsingAnim;
    Vector3 initPos;

    void Start()
    {
        curPulsingAnim = 0f;
        curInactiveTime = inactiveTime;
        initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right * Time.deltaTime * rotSpeed);

        curPulsingAnim += Time.deltaTime;
        curInactiveTime -= Time.deltaTime;

        transform.position = new Vector3(initPos.x, initPos.y + (pulsingAmplitude * (Mathf.Sin(curPulsingAnim * pulsingSpeed) + 1f)), initPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (curInactiveTime <= 0f)
        {
            if (other.CompareTag("Player_One"))
            {
                GameManager.instance.AddScore(GameManager.instance.pl_one, pointsToGive);
                Instantiate(ps, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            else if (other.CompareTag("Player_Two"))
            {
                GameManager.instance.AddScore(GameManager.instance.pl_two, pointsToGive);
                Instantiate(ps, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        //play sound
        // Instantiate(ps, transform.position, Quaternion.identity);
    }
}
