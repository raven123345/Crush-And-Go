using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum Player
    {
        pl_one,
        pl_two
    }

    public class MyTransform
    {
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        public MyTransform()
        {
            pos = Vector3.zero;
            rot = Quaternion.identity;
            scale = Vector3.one;
        }
    }


    public Player player;
    Transform target;
    Rigidbody car_rigidbody;

    [SerializeField]
    private float following_speed = 1f;
    [SerializeField]
    float max_velocity = 2500f;
    [SerializeField]
    float init_camera_angle = 60f;
    [SerializeField]
    float max_camera_angle_multiplier = 45f;
    [SerializeField]
    float max_distance_multiplier = 2f;

    private MyTransform init_offset = new MyTransform();
    public MyTransform offset = new MyTransform();

    public float initDistanceFromPlayer = 1f;

    MyTransform cameraMovement = new MyTransform();
    public bool followPosition = true;
    public bool followRotation = true;

    bool toPosition = false;
    float distanceFromPlayer;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        distanceFromPlayer = initDistanceFromPlayer;
        cam = GetComponent<Camera>();
        cam.fieldOfView = init_camera_angle;
    }

    // Update is called once per frame
    void Update()
    {
        if (!target)
        {
            if (player == Player.pl_one)
            {
                target = GameManager.instance.pl_one.player.transform;
            }
            else
            {
                target = GameManager.instance.pl_two.player.transform;
            }
            init_offset.pos = transform.position - target.position;
            init_offset.rot = Quaternion.LookRotation(target.position - transform.position);
            offset = init_offset;
            car_rigidbody = target.gameObject.GetComponent<Rigidbody>();
        }
        else
        {
            cameraMovement.pos = target.position + (offset.pos * distanceFromPlayer);
            cameraMovement.rot = Quaternion.LookRotation(target.position - transform.position);

            if (followPosition || followRotation)
                FollowTarget(cameraMovement);
            distanceFromPlayer = Mathf.Lerp(initDistanceFromPlayer, initDistanceFromPlayer * max_distance_multiplier, car_rigidbody.velocity.sqrMagnitude / max_velocity);
            cam.fieldOfView = Mathf.Lerp(init_camera_angle, init_camera_angle * max_camera_angle_multiplier, (car_rigidbody.velocity.sqrMagnitude / max_velocity));


        }
    }

    void FollowTarget(MyTransform targetToFollow)
    {
        if (followPosition)
            transform.position = Vector3.Lerp(transform.position, targetToFollow.pos, Time.deltaTime * following_speed * 0.2f);
        if (followRotation)
            transform.rotation = Quaternion.Lerp(transform.rotation, targetToFollow.rot, Time.deltaTime * following_speed);
    }

    public void ChangeOffset(MyTransform newOffset, bool followPosition, bool followRotation, float distanceFromPlayerMultiplier = 1f)
    {
        if (!toPosition)
        {
            this.followPosition = false;
            this.followRotation = false;

            transform.position = Vector3.Lerp(transform.position, Vector3.Lerp(target.position, newOffset.pos, distanceFromPlayerMultiplier), Time.deltaTime * following_speed);

            if (!followRotation)
                transform.rotation = Quaternion.Lerp(transform.rotation, newOffset.rot, Time.deltaTime * following_speed);
            else
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), Time.deltaTime * following_speed);

            if ((newOffset.pos - transform.position).sqrMagnitude < 15f)
            {
                toPosition = true;
                this.followPosition = followPosition;
                this.followRotation = followRotation;
                //cameraMovement.pos = target.position + offset.pos;
                //offset.pos = (transform.position - target.position);
                //offset.rot = newOffset.rot;
            }
        }
    }

    public void SetToInitOffset()
    {
        offset = init_offset;
        distanceFromPlayer = initDistanceFromPlayer;
        this.followPosition = true;
        this.followRotation = true;
        toPosition = false;
    }
}
