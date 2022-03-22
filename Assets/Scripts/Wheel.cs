using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{

    public bool torqueWheel;
    public bool steerWheel;
    public bool frontWheel;

    [SerializeField]
    private Transform wheelMesh;
    [SerializeField]
    private Vector3 rotationCompensation = new Vector3(0f, 90f, 0f);

    [HideInInspector]
    public WheelCollider _wheelCollider;

    [HideInInspector]
    public float torque;
    [HideInInspector]
    public float steerAngle;
    [HideInInspector]
    public float frontBrakeTorque;
    [HideInInspector]
    public float rearBrakeTorque;

    [SerializeField]
    ParticleSystem dust;
    [SerializeField]
    TrailRenderer trail;

    [System.Serializable]
    public class ParticleProperties
    {
        public float maxRate = 150f;
        public float maxRPM = 600f;
        public float minRPM = 15f;
        public float maxSize = 6f;

    }
    public ParticleProperties particleProperties;

    void Start()
    {
        _wheelCollider = GetComponent<WheelCollider>();
    }

    void Update()
    {
        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        _wheelCollider.GetWorldPose(out pos, out rot);

        wheelMesh.position = pos;
        wheelMesh.rotation = rot * Quaternion.Euler(rotationCompensation);
        GenerateDust();
        GenerateTrails();
    }
    private void FixedUpdate()
    {
        if (torqueWheel)
        {
            _wheelCollider.motorTorque = torque;
        }
        if (steerWheel)
        {
            _wheelCollider.steerAngle = steerAngle;
        }
        if (frontWheel)
        {
            _wheelCollider.brakeTorque = frontBrakeTorque;
        }
        if (!frontWheel)
        {
            _wheelCollider.brakeTorque = rearBrakeTorque;
        }
    }
    void GenerateDust()
    {
        if (dust)
        {
            float n_velocity = Mathf.Abs(_wheelCollider.rpm) / particleProperties.maxRPM;
            var emission = dust.emission;
            var size = dust.main;
            if (_wheelCollider.isGrounded && Mathf.Abs(_wheelCollider.rpm) > particleProperties.minRPM)
            {
                emission.rateOverTime = n_velocity * particleProperties.maxRate;
                size.startSizeMultiplier = n_velocity * particleProperties.maxSize;
            }
            else
            {
                emission.rateOverTime = 0f;
            }
        }
    }

    void GenerateTrails()
    {
        if (_wheelCollider.isGrounded && _wheelCollider.brakeTorque>0f)
        {
            trail.emitting = true;
        }
        else
        {
            trail.emitting = false;
        }
    }
}
