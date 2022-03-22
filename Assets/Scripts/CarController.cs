using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    public enum cc_Player
    {
        Player_One, Player_Two
    };
    [HideInInspector]
    public Camera camera;

    public cc_Player player;
    public string playerOnePrefix = "PL_One_";
    public string playerTwoPrefix = "PL_Two_";

    public Slider health_slider;


    public Canvas canvas_for_me;
    public Canvas canvas_for_other;

    public Transform[] weaponPositions;

    public List<Wheel> wheels = new List<Wheel>();
    public Transform massCenter;
    public bool use_custom_center_of_mass = false;

    [SerializeField]
    private float speed = 100f;
    [SerializeField]
    private float steerAngle = 20f;
    [SerializeField]
    private float acceleration = 5f;
    [SerializeField]
    private float steerSpeed = 5f;
    [SerializeField]
    private float brake = 50f;
    [SerializeField]
    float reverse_treshold = 0.1f;
    [SerializeField]
    private float resetCountDown = .5f;
    [SerializeField]
    private float minDamageSpeedTreshold = 10f;

    [SerializeField]
    [InspectorName("Weapon Timer(sec)")]
    private float hasWeaponTimer = 60f;

    private float curWeaponTimer;

    [HideInInspector]
    public bool holdBrake = false;
    [HideInInspector]
    public bool hasWeapon;
    [HideInInspector]
    public GameObject weapon;

    private Rigidbody rb;
    private string otherTag;

    private bool triggerEnter = false;

    AudioSource audio_source;
    float engine_max_pitch = 2.5f;
    [SerializeField]
    float init_volume = 0.5f;

    float accelerate = 0f;

    float reverse_counter;


    // Start is called before the first frame update
    void Start()
    {
        audio_source = GetComponent<AudioSource>();

        transform.tag = player.ToString();
        switch (transform.tag)
        {
            case "Player_One":
                otherTag = "Player_Two";
                break;
            case "Player_Two":
                otherTag = "Player_One";
                break;
        }

        rb = GetComponent<Rigidbody>();
        if (use_custom_center_of_mass)
        {
            rb.centerOfMass = massCenter.position;
        }

        curWeaponTimer = hasWeaponTimer;
        if (player == cc_Player.Player_One)
        {
            GameManager.instance.pl_one.HPSlider = health_slider;
            GameManager.instance.pl_one.setHP();
            PlaySoundFX(5, false);
        }
        else
        {
            GameManager.instance.pl_two.HPSlider = health_slider;
            GameManager.instance.pl_two.setHP();
            PlaySoundFX(1, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == cc_Player.Player_One)
        {
            
        }
        else if (player == cc_Player.Player_Two)
        {
            
        }

        if (hasWeapon)
        {
            DestroyWeaponInTime();
        }

        if (player == cc_Player.Player_One)
        {

            if (Input.GetButtonDown(playerOnePrefix + "DestroyWeapon"))
            {
                DestroyWeapon();
            }
            ResetCar(playerOnePrefix);
            Accelerate(playerOnePrefix);
        }
        else
        {
            if (Input.GetButtonDown(playerTwoPrefix + "DestroyWeapon"))
            {
                DestroyWeapon();
            }
            Accelerate(playerTwoPrefix);
            ResetCar(playerTwoPrefix);
        }
    }

    void Accelerate(string controlPrefix)
    {
        if (Input.GetButton(controlPrefix + "Accelerate") && reverse_counter <= 0f && accelerate >= 0f)
        {
            accelerate = 1f;
        }
        else if (Input.GetButton(controlPrefix + "Accelerate"))
        {
            accelerate = -1f;
        }
        else
        {
            accelerate = 0f;
        }

        if (Input.GetButtonUp(controlPrefix + "Accelerate"))
        {
            reverse_counter = reverse_treshold;
        }

        if (reverse_counter > 0f)
        {
            reverse_counter -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (player == cc_Player.Player_One)
        {
            CarControl_Camera_Space(playerOnePrefix);
        }
        else if (player == cc_Player.Player_Two)
        {
            CarControl_Camera_Space(playerTwoPrefix);
        }
    }

    public void PlaySoundFX(int index, bool once)
    {
        if (once)
        { audio_source.PlayOneShot(GameManager.instance.sound_effects[index]); }
        else
        {
            audio_source.clip = GameManager.instance.sound_effects[index];
            audio_source.Play();
        }
    }
    public void SetCanvasCamera(Camera camera_for_me, Camera camera_for_other)
    {
        canvas_for_me.worldCamera = camera_for_me;
        canvas_for_other.worldCamera = camera_for_other;
    }

    void CarControl_Camera_Space(string controlPrefix)
    {
        if (GameManager.instance.gameStarted)
        {
            Vector3 car_right = new Vector3(transform.right.x, 0f, transform.right.z);
            Vector3 camera_right = new Vector3(camera.transform.right.x, 0f, camera.transform.right.z);
            Vector3 camera_forward = new Vector3(camera.transform.forward.x, 0f, camera.transform.forward.z);

            Vector3 dir = Vector3.ClampMagnitude((camera_forward * Input.GetAxis(controlPrefix + "Vertical")) + (camera_right * Input.GetAxis(controlPrefix + "Horizontal")), 1f);

            float steer_dir = (-(Vector3.Angle(car_right, dir) - 90f) * dir.sqrMagnitude);


            if (Mathf.Abs(steer_dir) > steerAngle)
            {
                steer_dir = Mathf.Sign(steer_dir) * steerAngle;
            }

            //Debug.DrawRay(transform.position, dir * vector_length * 2f, Color.blue);

     

            foreach (Wheel wheel in wheels)
            {
                wheel.torque = Mathf.Lerp(wheel._wheelCollider.motorTorque, accelerate * speed, Time.deltaTime * acceleration);
                wheel.steerAngle = Mathf.Lerp(wheel._wheelCollider.steerAngle, steer_dir, Time.deltaTime * steerSpeed);
                audio_source.pitch = Mathf.Lerp(1f, engine_max_pitch, rb.velocity.sqrMagnitude / speed);
                audio_source.volume = Mathf.Lerp(init_volume, 1f, rb.velocity.sqrMagnitude / speed);


                if (Input.GetButton(controlPrefix + "Brake"))
                {
                    wheel.frontBrakeTorque = brake;
                    wheel.rearBrakeTorque = brake;
                    wheel.torque = 0f;
                }
                else
                {
                    wheel.frontBrakeTorque = 0f;
                    wheel.rearBrakeTorque = 0f;
                }
            }
        }
        else
        {
            foreach (Wheel wheel in wheels)
            {

                wheel.frontBrakeTorque = brake;
                wheel.rearBrakeTorque = brake;
                wheel.torque = 0f;
            }
        }

    }

    //void CarControl(string controlPrefix)
    //{

    //    if (GameManager.instance.gameStarted)
    //    {
    //        foreach (Wheel wheel in wheels)
    //        {
    //            if (!Input.GetButton(controlPrefix + "FrontBrake") && !Input.GetButton(controlPrefix + "HandBrake"))
    //            {
    //                holdBrake = false;
    //                wheel.torque = Mathf.Lerp(wheel._wheelCollider.motorTorque, Input.GetAxis(controlPrefix + "Vertical") * speed, Time.deltaTime * acceleration);

    //                audio_source.pitch = Mathf.Lerp(1f, engine_max_pitch, rb.velocity.sqrMagnitude / speed);
    //                audio_source.volume = Mathf.Lerp(init_volume, 1f, rb.velocity.sqrMagnitude / speed);

    //                wheel.frontBrakeTorque = 0f;
    //                wheel.rearBrakeTorque = 0f;
    //            }
    //            else if (Input.GetButton(controlPrefix + "FrontBrake"))
    //            {
    //                wheel.frontBrakeTorque = brake;
    //                if (wheel.frontWheel && wheel.activeWheel)
    //                {
    //                    wheel.torque = 0f;
    //                }
    //                else if (!wheel.frontWheel && wheel.activeWheel)
    //                {
    //                    wheel.torque = Mathf.Lerp(wheel._wheelCollider.motorTorque, Input.GetAxis(controlPrefix + "Vertical") * speed, Time.deltaTime * acceleration);
    //                }
    //            }
    //            else if (Input.GetButton(controlPrefix + "HandBrake"))
    //            {
    //                wheel.frontBrakeTorque = brake;
    //                wheel.rearBrakeTorque = brake;
    //                wheel.torque = 0f;
    //            }

    //            wheel.steerAngle = Mathf.Lerp(wheel._wheelCollider.steerAngle, steerAngle * Input.GetAxis(controlPrefix + "Horizontal"), Time.deltaTime * steerSpeed);

    //            if (holdBrake)
    //            {
    //                wheel.frontBrakeTorque = brake;
    //                wheel.rearBrakeTorque = brake;
    //                wheel.torque = 0f;
    //            }

    //        }
    //    }
    //    else
    //    {
    //        foreach (Wheel wheel in wheels)
    //        {

    //            wheel.frontBrakeTorque = brake;
    //            wheel.rearBrakeTorque = brake;
    //            wheel.torque = 0f;
    //        }
    //    }
    //}

    void ResetCar(string controlPrefix)
    {
        if (Input.GetButtonDown(controlPrefix + "ResetCar"))
        {
            StartCoroutine("ResetCountdown");
        }
    }

    public void StopCar()
    {
        // holdBrake = true;
    }

    public void selfDestroy()
    {
        Destroy(gameObject);
    }

    public void DestroyWeaponInTime()
    {
        curWeaponTimer -= Time.deltaTime;
        if (curWeaponTimer <= 0f && hasWeapon)
        {
            Destroy(weapon);
            hasWeapon = false;
            curWeaponTimer = hasWeaponTimer;
        }
    }

    public void DestroyWeapon()
    {
        Destroy(weapon);
        hasWeapon = false;
        curWeaponTimer = hasWeaponTimer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "LevelBorder")
        {
            switch (gameObject.tag)
            {
                case "Player_One":
                    GameManager.instance.ResetCar(GameManager.instance.pl_one);
                    break;
                case "Player_Two":
                    GameManager.instance.ResetCar(GameManager.instance.pl_two);
                    break;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("CameraTrigger"))
        {
            if (player == cc_Player.Player_One)
            {
                ChangeCameraOffset(other, GameManager.instance.pl_one);
            }
            else
            {
                ChangeCameraOffset(other, GameManager.instance.pl_two);
            }
        }
    }

    private void ChangeCameraOffset(Collider other, Player player)
    {
        CameraController.MyTransform newTransform = new CameraController.MyTransform();

        Camera_trigger trigger = other.GetComponent<Camera_trigger>();
        CameraController camController = player.relatedCamera.GetComponent<CameraController>();

        if (trigger)
        {
            newTransform.pos = trigger.offset_pos.position;
            newTransform.rot = trigger.offset_pos.rotation;
            camController.ChangeOffset(newTransform, trigger.followTargetPosition, trigger.followTargetRotation, trigger.distanceFromPlayerMultiplier);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("CameraTrigger"))

        {
            if (player == cc_Player.Player_One)
            {
                GameManager.instance.pl_one.relatedCamera.GetComponent<CameraController>().SetToInitOffset();
            }
            else
            {
                GameManager.instance.pl_two.relatedCamera.GetComponent<CameraController>().SetToInitOffset();
            }
            // triggerEnter = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.transform.tag == otherTag)
        {
            if (other.impulse.magnitude > minDamageSpeedTreshold)
            {
                other.gameObject.GetComponent<Damage>().GiveDamage(1);
            }
        }
    }

    IEnumerator ResetCountdown()
    {
        yield return new WaitForSeconds(resetCountDown);
        transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y, 0f));
        transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        switch (player)
        {
            case cc_Player.Player_One:
                GameManager.instance.ChangeHealthPoints(GameManager.instance.pl_one, -1, true);
                break;
            case cc_Player.Player_Two:
                GameManager.instance.ChangeHealthPoints(GameManager.instance.pl_two, -1, true);
                break;
        }
    }
}
