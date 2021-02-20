using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollbarController : MonoBehaviour
{
    public float hitPower = 50f;

    [SerializeField]
    private GameObject rollbarMesh;

    [SerializeField]
    private Transform basePos;
    [SerializeField]
    private Transform hitPos;

    [SerializeField]
    private AnimationCurve movementModel;

    [SerializeField]
    private float movementSpeed = 5f;
    [SerializeField]
    private float forwardMoveAnimationCurve = .3f;
    [SerializeField]
    private int damagePoints = 2;
    [SerializeField]
    private ParticleSystem impactSystem;


    private Transform carInstallationPos;

    private float eval = 0f;

    private HitDirection hitDir;

    private Vector3 hitDirection;

    private string otherTag;

    private CarController car;
    internal Player playerToDamage;
    internal Player playerToGivePoints;

    void Start()
    {
        hitDir = rollbarMesh.GetComponent<HitDirection>();
        car = transform.parent.GetComponent<CarController>();

        if (car)
        {
            switch (car.player)
            {
                case CarController.cc_Player.Player_One:

                    otherTag = CarController.cc_Player.Player_Two.ToString();

                    playerToDamage = GameManager.instance.pl_two;
                    playerToGivePoints = GameManager.instance.pl_one;
                    break;

                case CarController.cc_Player.Player_Two:

                    otherTag = CarController.cc_Player.Player_One.ToString();

                    playerToDamage = GameManager.instance.pl_one;
                    playerToGivePoints = GameManager.instance.pl_two;
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        eval += Time.deltaTime * movementSpeed;
        if (eval <= forwardMoveAnimationCurve)
            hitDirection = hitDir.GetHitDirection();

        if (eval >= 1f)
            eval = 0f;

        movementModel.Evaluate(eval);
        rollbarMesh.transform.position = Vector3.Lerp(basePos.position, hitPos.position, movementModel.Evaluate(eval));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == otherTag || other.transform.tag == "Enemy" || other.transform.tag == "Breakable" || other.transform.tag == "Hittable")
        {
            Rigidbody[] rb = other.GetComponents<Rigidbody>();

            foreach (Rigidbody rigidBody in rb)
            {
                if (rigidBody)
                {
                    rigidBody.AddForce(hitDirection * hitPower * 10000f, ForceMode.Impulse);
                    rigidBody.AddForce(Vector3.zero);
                }
            }
        }
        if (other.transform.tag == otherTag)
        {
            GameManager.instance.AddScore(playerToGivePoints, 2);

            Damage otherDamage = other.GetComponent<Damage>();
            if (otherDamage)
            {
                otherDamage.GiveDamage(this.damagePoints);
            }
        }
        if (other.transform.tag == "Enemy")
        {
            GameManager.instance.AddScore(playerToGivePoints, 1);

            Damage otherDamage = other.GetComponent<Damage>();
            if (otherDamage)
            {
                otherDamage.GiveDamage(this.damagePoints);
            }
        }
        if (other.transform.tag == "Hittable")
        {
            //break bjects
        }
        if (impactSystem)
            Instantiate(impactSystem, other.transform.position, Quaternion.identity);
    }
}
