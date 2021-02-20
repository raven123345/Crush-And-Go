using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingGloveController : MonoBehaviour
{

    public string actionName = "Fire";
    public float hitPower = 50f;


    [HideInInspector]
    public List<GameObject> target = new List<GameObject>();
    [HideInInspector]
    public string actionPrefix;

    [HideInInspector]
    public Vector3 hitDirection;
    [SerializeField]
    private GameObject glove;

    [SerializeField]
    public Transform gloveHomePos;
    [SerializeField]
    private float hitMoveSpeed;
    [SerializeField]
    private float hitRotateSpeed;
    [SerializeField]
    private float rotationTreshold = 10f;
    [SerializeField]
    private float minDistance = 1f;
    [SerializeField]
    private float hitRange = 15f;
    [SerializeField]
    private float distanceToTargetToMultiplyRotationSpeed = 3f;

    public int damagePoints = 2;
    [SerializeField]
    ParticleSystem impactSystem;
    [SerializeField]
    TrailRenderer trail;


    internal string otherTag;
    private HitDirection hitDir;
    private float initRotationSpeed;
    private bool isHitting = false;
    private bool wasHit = false;

    private Vector3 previsousPosition;
    private float hitDirectionTimer = 0.1f;
    private float curHitDirectionTime;
    private Collider[] objectsInRange;
    private CarController car;
    internal Player playerToGivePoints;

    private bool give_target = true;
    Vector3 hit_target = Vector3.zero;

    ParticleSystem impact;

    // Start is called before the first frame update
    void Start()
    {
        if (glove)
        {
            gloveHomePos.position = glove.transform.position;
            gloveHomePos.rotation = glove.transform.rotation;
        }
        initRotationSpeed = hitRotateSpeed;

        hitDir = glove.GetComponent<HitDirection>();

        car = transform.parent.GetComponent<CarController>();

        if (car)
        {
            switch (car.player)
            {
                case CarController.cc_Player.Player_One:

                    otherTag = CarController.cc_Player.Player_Two.ToString();
                    actionPrefix = car.playerOnePrefix;

                    playerToGivePoints = GameManager.instance.pl_one;
                    break;

                case CarController.cc_Player.Player_Two:

                    otherTag = CarController.cc_Player.Player_One.ToString();
                    actionPrefix = car.playerTwoPrefix;

                    playerToGivePoints = GameManager.instance.pl_two;
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(actionPrefix + actionName))
        {
            isHitting = true;

        }
        trail.emitting = isHitting;

        if (isHitting)
        {
            if (give_target)
            {
                GameObject o = nearestTarget(target);
                if (o)
                    hit_target = nearestTarget(target).transform.position;
                give_target = false;
            }

            if (hit_target.sqrMagnitude > 0f)
                Hit(hit_target, true);
            else
                Hit(gloveHomePos.transform.position + gloveHomePos.forward * hitRange);


        }
    }

    private void FixedUpdate()
    {
        objectsInRange = Physics.OverlapSphere(transform.position, hitRange);
        target = new List<GameObject>();

        foreach (Collider potential_target in objectsInRange)
        {
            if (potential_target.transform.tag == otherTag || potential_target.transform.tag == "Enemy" || potential_target.transform.tag == "Breakable" || potential_target.transform.tag == "Hittable")
            {
                // check
                if (target.Count > 0)
                {
                    foreach (GameObject trg in target)
                    {
                        if (trg == null)
                        {
                            target.Remove(trg);
                            return;
                        }

                        if (potential_target.gameObject == trg)
                        { return; }
                    }
                    target.Add(potential_target.gameObject);
                }
                if (target.Count == 0)
                {
                    target.Add(potential_target.gameObject); // add first element}
                }
            }
        }

        // Debug.Log("target cout : " + target.Count);
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
                    if (trg)
                    {
                        if (Vector3.Magnitude(trg.transform.position - transform.position) < Vector3.Magnitude(target.transform.position - transform.position))
                        {
                            target = trg;
                        }
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

    void Hit(Vector3 target, bool activate_impact = false)
    {
        float distanceToTarget = Vector3.Magnitude(target - gloveHomePos.position);

        if (distanceToTarget < distanceToTargetToMultiplyRotationSpeed)
        {
            hitRotateSpeed *= 2f;
        }
        else
        {
            hitRotateSpeed = initRotationSpeed;
        }
        Vector3 targetDir = target - glove.transform.position;

        if (targetDir.magnitude > minDistance && !wasHit)
        {
            glove.transform.rotation = Quaternion.Lerp(glove.transform.rotation, Quaternion.LookRotation(targetDir), hitRotateSpeed * Time.deltaTime);
            glove.transform.position += glove.transform.forward * hitMoveSpeed * 2f * Time.deltaTime;

            hitDirection = hitDir.GetHitDirection();
            // Debug.Log(" targetDir.magnitude " + targetDir.magnitude);
        }
        else // когато достигне таргета
        {
            wasHit = true;
            if (activate_impact)
            {
                if (!impact)
                {
                    impact = Instantiate(impactSystem);
                }
                else
                {
                    impact.transform.position = target;
                    impact.Play();
                }
            }
            // Debug.Log("has target hit");
        }

        if (wasHit)
        {
            glove.transform.position = Vector3.Lerp(glove.transform.position, gloveHomePos.position, hitMoveSpeed * Time.deltaTime);
            glove.transform.rotation = Quaternion.Lerp(glove.transform.rotation, gloveHomePos.rotation, hitRotateSpeed * Time.deltaTime);

            float distanceToHome = Vector3.SqrMagnitude(glove.transform.position - gloveHomePos.position);
            // Debug.Log("distanceToHome" + distanceToHome);

            if (distanceToHome <= 0.01f)
            {
                wasHit = false;
                isHitting = false;
                give_target = true;
                hit_target = Vector3.zero;

                glove.transform.rotation = gloveHomePos.rotation;
            }
        }

        // else // когато няма таргет
        // {
        //     Vector3 maxIdleHit = gloveHomePos.transform.position + gloveHomePos.forward * hitRange;
        //     float distanceToHitRange = Mathf.Abs(Vector3.Magnitude((maxIdleHit) - glove.transform.position));
        //     glove.transform.rotation = gloveHomePos.rotation;

        //     if (distanceToHitRange >= 0.01f && !wasHit)
        //     {
        //         glove.transform.position = Vector3.Lerp(glove.transform.position, maxIdleHit, Time.deltaTime * hitMoveSpeed * 1.5f);

        //     }
        //     else // когато достигне най далечната точка
        //     {
        //         wasHit = true;
        //     }
        //     if (wasHit)
        //     {
        //         glove.transform.position = Vector3.Lerp(glove.transform.position, gloveHomePos.position, hitMoveSpeed * Time.deltaTime * 1.5f);

        //         float distanceToHome = Vector3.SqrMagnitude(glove.transform.position - gloveHomePos.position);

        //         if (distanceToHome <= 0.001f)
        //         {
        //             wasHit = false;
        //             isHitting = false;
        //             give_target = true;
        //         }
        //     }
        //     Debug.Log("Dont have target");
        // }
    }
}
