using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    [HideInInspector]
    public bool launched;

    public int damagePoints = 2;

    [SerializeField]
    private float speed = 50f;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float detectionRadius;
    [SerializeField]
    private Transform detector;
    [SerializeField]
    private float lifeTime = 2f;
    private float curLifetime;
    [SerializeField]
    private ParticleSystem impactSystem;
    [SerializeField]
    private float hitPower = 100f;
    [SerializeField]
    private float explosionWaveRange = 2f;


    internal string otherTag;
    internal Transform parent;
    internal Player playerToDamage;
    internal Player playerToGivePoints;
    internal Transform launchPos;

    private Collider[] objectsInRange;
    private List<GameObject> targets = new List<GameObject>();
    private HitDirection hitDir;
    private Vector3 hitDirection;


    void Start()
    {
        curLifetime = lifeTime;
        hitDir = GetComponent<HitDirection>();
    }

    // Update is called once per frame
    void Update()
    {
        if (launched)
        {
            FlyTo(nearestTarget(targets));

            parent = null;

            curLifetime -= Time.deltaTime;
            //Debug.DrawLine(detector.position, transform.right * detectionRadius, Color.blue);


            if (curLifetime <= 0f)
            {
                Destroy(gameObject);
                if (impactSystem)
                    Instantiate(impactSystem, transform.position, Quaternion.identity);

                ExplosionWave();
            }
        }
        else
        {
            transform.position = launchPos.position;
            transform.rotation = launchPos.rotation;
        }
    }

    private void FixedUpdate()
    {
        if (launched)
        {
            DetetectTargets();
        }
    }

    void FlyTo(GameObject target)
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        hitDirection = hitDir.GetHitDirection();

        if (target != null)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), Time.deltaTime * rotationSpeed);
        }
    }
    void ExplosionWave()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, explosionWaveRange);
        foreach (Collider obj in objectsInRange)
        {
            Vector3 hitDir = obj.transform.position - transform.position;
            if (obj.transform.tag == otherTag)
            {
                Rigidbody rb = obj.gameObject.GetComponent<Rigidbody>();
                rb.AddForce(hitDir * hitPower * 500f, ForceMode.Impulse);
                rb.AddForce(Vector3.zero);
                GameManager.instance.AddScore(playerToGivePoints, 4);

                Damage otherDamage = obj.GetComponent<Damage>();
                if (otherDamage)
                {
                    otherDamage.GiveDamage(this.damagePoints);
                }
            }
            if (obj.transform.tag == "Enemy")
            {
                Rigidbody rb = obj.gameObject.GetComponent<Rigidbody>();
                rb.AddForce(hitDir * hitPower * 500f, ForceMode.Impulse);
                rb.AddForce(Vector3.zero);
                GameManager.instance.AddScore(playerToGivePoints, 3);

                Damage otherDamage = obj.GetComponent<Damage>();
                if (otherDamage)
                {
                    otherDamage.GiveDamage(this.damagePoints);
                }
            }
            if (obj.transform.tag == "Hittable")
            {
                Rigidbody rb = obj.gameObject.GetComponent<Rigidbody>();
                rb.AddForce(hitDir * hitPower * 500f, ForceMode.Impulse);
                rb.AddForce(Vector3.zero);
                Crates crate = obj.GetComponent<Crates>();
                if (crate)
                {
                    crate.Destroy();
                }
                GameManager.instance.AddScore(playerToGivePoints, 1);
            }
        }
    }
    void DetetectTargets()
    {
        objectsInRange = Physics.OverlapSphere(transform.position, detectionRadius);
        targets = new List<GameObject>();

        foreach (Collider potential_target in objectsInRange)
        {
            if (potential_target.transform.tag == otherTag || potential_target.transform.tag == "Enemy" || potential_target.transform.tag == "Breakable")
            {
                //check
                if (targets.Count > 0)
                {
                    foreach (GameObject trg in targets)
                    {
                        if (trg == null)
                        {
                            targets.Remove(trg);
                            return;
                        }

                        if (potential_target.gameObject == trg)
                        { return; }
                    }
                    targets.Add(potential_target.gameObject);
                }
                if (targets.Count == 0)
                {
                    targets.Add(potential_target.gameObject); // add first element}
                }
            }
        }
    }

    GameObject nearestTarget(List<GameObject> targets)
    {
        if (targets.Count > 0)
        {
            GameObject target = null;

            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i])
                {
                    if (targets[i])
                    {
                        target = targets[i];
                        i = targets.Count;
                    }
                }
            }

            if (target)
            {
                foreach (GameObject trg in targets)
                {
                    if (Vector3.Magnitude(trg.transform.position - transform.position) < Vector3.Magnitude(target.transform.position - transform.position) && trg != null)
                    {
                        target = trg;
                    }
                }
            }

            if (target != null)
                return target;
            else
                return null;
        }
        else return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == otherTag)
        {
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(hitDirection * hitPower * 10000f, ForceMode.Impulse);
            rb.AddForce(Vector3.zero);
            GameManager.instance.AddScore(playerToGivePoints, 4);

            Damage otherDamage = other.GetComponent<Damage>();
            if (otherDamage)
            {
                otherDamage.GiveDamage(this.damagePoints);
            }

            if (impactSystem)
                Instantiate(impactSystem, other.transform.position, Quaternion.identity);

            Destroy(gameObject);

        }
        if (other.transform.tag == "Enemy")
        {
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(hitDirection * hitPower * 10000f, ForceMode.Impulse);
            rb.AddForce(Vector3.zero);
            GameManager.instance.AddScore(playerToGivePoints, 3);

            Damage otherDamage = other.GetComponent<Damage>();
            if (otherDamage)
            {
                otherDamage.GiveDamage(this.damagePoints);
            }

            if (impactSystem)
                Instantiate(impactSystem, other.transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
        if (other.transform.tag == "Hittable")
        {
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(hitDirection * hitPower * 10000f, ForceMode.Impulse);
            rb.AddForce(Vector3.zero);
            GameManager.instance.AddScore(playerToGivePoints, 1);

            if (impactSystem)
                Instantiate(impactSystem, other.transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (impactSystem)
            Instantiate(impactSystem, other.transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
