using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    [SerializeField]
    private GameObject rocket;
    public Transform rocketLaunchPos;
    [SerializeField]
    private string actionName = "Fire";
    [SerializeField]
    private float reloadTimer = 1f;
    private float curReloadTimer;
    private bool launched;
    private bool reloaded;
    private RocketController rocketController;
    // private GameObject reloadedRocket;

    private CarController car;
    private Player playerToDamage;
    private Player playerToGivePoints;
    private string actionPrefix;
    private string otherTag;
    // Start is called before the first frame update
    void Start()
    {
        curReloadTimer = reloadTimer;

        car = transform.parent.GetComponent<CarController>();

        switch (car.player)
        {
            case CarController.cc_Player.Player_One:
                otherTag = CarController.cc_Player.Player_Two.ToString();
                actionPrefix = car.playerOnePrefix;

                playerToDamage = GameManager.instance.pl_two;
                playerToGivePoints = GameManager.instance.pl_one;
                break;
            case CarController.cc_Player.Player_Two:
                otherTag = CarController.cc_Player.Player_One.ToString();
                actionPrefix = car.playerTwoPrefix;

                playerToDamage = GameManager.instance.pl_one;
                playerToGivePoints = GameManager.instance.pl_two;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (curReloadTimer == reloadTimer && !reloaded)
        {
            rocketController = Instantiate(rocket, rocketLaunchPos.position, rocketLaunchPos.rotation).GetComponent<RocketController>();
            rocketController.otherTag = otherTag;
            rocketController.parent = transform;
            rocketController.playerToDamage = playerToDamage;
            rocketController.playerToGivePoints = playerToGivePoints;
            rocketController.launchPos = rocketLaunchPos;

            reloaded = true;
        }
        if (Input.GetButtonDown(actionPrefix + actionName) && !launched && curReloadTimer == reloadTimer)
        {
            rocketController.launched = true;
            launched = true;
        }
        if (launched)
        {
            curReloadTimer -= Time.deltaTime;
            if (curReloadTimer <= 0f)
            {
                curReloadTimer = reloadTimer;
                launched = false;
                reloaded = false;
            }
        }
    }
    private void OnDestroy()
    {
        if (rocketController)
            Destroy(rocketController.gameObject);
    }
}
