using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SG_CarController : MonoBehaviour
{
    //Actions for the Input System
    private SG_InputActions SG_Actions;
    private InputAction accelerateAction;
    private InputAction turnAction;
    private InputAction brakeAction;
    private InputAction carResetAction;

    public enum controlSpace
    {
        CameraSpace, CarSpace
    };
    public controlSpace controllingSpace;
    public Camera playerCamera;

    public enum cc_Player
    {
        Player_One, Player_Two
    };
    public cc_Player player;
    private string otherTag;

    public List<Wheel> wheels = new List<Wheel>();

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
    AudioClip engineSound;
    [SerializeField]
    [Range(0f, 1f)]
    float init_volume = 0.5f;

    AudioSource audio_source;
    float engine_max_pitch = 2.5f;

    private Rigidbody rb;

    float accelerate = 0f;
    float reverse_counter = 0f;
    Vector2 directions;


    private void Awake()
    {
        SG_Actions = new SG_InputActions();
    }

    private void OnEnable()
    {
        if (player == cc_Player.Player_One)
        {
            accelerateAction = SG_Actions.PlayerOne.Accelerate;
            accelerateAction.Enable();
            accelerateAction.started += Accelerate;
            accelerateAction.canceled += Accelerate;

            brakeAction = SG_Actions.PlayerOne.Brake;
            brakeAction.Enable();
            brakeAction.started += Brake;
            brakeAction.canceled += Brake;

            carResetAction = SG_Actions.PlayerOne.CarReset;
            carResetAction.Enable();
            carResetAction.performed += ResetCar;

            turnAction = SG_Actions.PlayerOne.Directions;
            turnAction.Enable();
        }
        else if (player == cc_Player.Player_Two)
        {


        }
    }

    private void OnDisable()
    {
        accelerateAction.Disable();
        turnAction.Disable();
    }

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

        PlaySoundFX(engineSound, false);
    }

    void Update()
    {
        directions = turnAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        switch (controllingSpace)
        {
            case controlSpace.CameraSpace:
                CarControl_CameraSpace(directions);
                break;
            case controlSpace.CarSpace:
                CarControl_CarSpace(directions);
                break;
        }
    }

    public void Accelerate(InputAction.CallbackContext context)
    {
        if (context.started && (context.time - reverse_counter) > reverse_treshold)
        {
            accelerate = 1f;
        }
        else if (context.started && (context.time - reverse_counter) <= reverse_treshold)
        {
            accelerate = -1f;
        }
        else if (context.canceled)
        {
            accelerate = 0f;
            reverse_counter = (float)context.time;
        }
    }

    void Brake(InputAction.CallbackContext context)
    {
        foreach (Wheel wheel in wheels)
        {
            if (context.started)
            {
                wheel.frontBrakeTorque = brake;
                wheel.rearBrakeTorque = brake;
                wheel.torque = 0f;
            }
            else if (context.canceled)
            {
                wheel.frontBrakeTorque = 0f;
                wheel.rearBrakeTorque = 0f;
            }
        }
    }

    public void PlaySoundFX(AudioClip sound, bool once)
    {
        if (once)
        { audio_source.PlayOneShot(sound); }
        else
        {
            audio_source.clip = sound;
            audio_source.Play();
        }
    }

    void CarControl_CameraSpace(Vector2 directions)
    {
        Vector3 car_right = new Vector3(transform.right.x, 0f, transform.right.z);
        Vector3 camera_right = new Vector3(playerCamera.transform.right.x, 0f, playerCamera.transform.right.z);
        Vector3 camera_forward = new Vector3(playerCamera.transform.forward.x, 0f, playerCamera.transform.forward.z);

        Vector3 turnDirection = Vector3.ClampMagnitude((camera_forward * directions.y) + (camera_right * directions.x), 1f);

        float steer_dir = (-(Vector3.Angle(car_right, turnDirection) - 90f) * turnDirection.sqrMagnitude);


        if (Mathf.Abs(steer_dir) > steerAngle)
        {
            steer_dir = Mathf.Sign(steer_dir) * steerAngle;
        }

        foreach (Wheel wheel in wheels)
        {
            wheel.torque = Mathf.Lerp(wheel._wheelCollider.motorTorque, accelerate * speed, Time.deltaTime * acceleration);
            wheel.steerAngle = Mathf.Lerp(wheel._wheelCollider.steerAngle, steer_dir, Time.deltaTime * steerSpeed);
            audio_source.pitch = Mathf.Lerp(1f, engine_max_pitch, rb.velocity.sqrMagnitude / speed);
            audio_source.volume = Mathf.Lerp(init_volume, 1f, rb.velocity.sqrMagnitude / speed);
        }
    }

    void CarControl_CarSpace(Vector2 directions)
    {
        foreach (Wheel wheel in wheels)
        {
            wheel.torque = Mathf.Lerp(wheel._wheelCollider.motorTorque, accelerate * speed, Time.deltaTime * acceleration);
            wheel.steerAngle = Mathf.Lerp(wheel._wheelCollider.steerAngle, steerAngle * directions.x, Time.deltaTime * steerSpeed);
            audio_source.pitch = Mathf.Lerp(1f, engine_max_pitch, rb.velocity.sqrMagnitude / speed);
            audio_source.volume = Mathf.Lerp(init_volume, 1f, rb.velocity.sqrMagnitude / speed);
        }
    }

    void ResetCar(InputAction.CallbackContext context)
    {
        StartCoroutine("ResetCountdown");
    }

    public void selfDestroy()
    {
        Destroy(gameObject);
    }

    IEnumerator ResetCountdown()
    {
        yield return new WaitForSeconds(resetCountDown);
        transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y, 0f));
        transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
    }
}
