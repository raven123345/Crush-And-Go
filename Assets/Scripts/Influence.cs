using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[ExecuteInEditMode]
public class Influence : MonoBehaviour
{
    public Transform targetA;
    public Transform targetB;

    public Vector3 rotationOffset;

    [Range(0f, 1f)]
    public float influenceOffset = 0f;
    public float influencePower = 1f;

    private float initInfluence;
    private Vector3 initPos;
    private Vector3 initOffset;


    // Start is called before the first frame update
    void Start()
    {
        CalculateInfluence();
        initPos = transform.position;
        initOffset = Vector3.Lerp(targetA.position, targetB.position, Mathf.Pow(initInfluence + influenceOffset, influencePower)) - initPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetA && targetB)
        {
            transform.position = Vector3.Lerp(targetA.position, targetB.position, Mathf.Pow(initInfluence + influenceOffset, influencePower)) - initOffset;
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(targetA.rotation.eulerAngles + rotationOffset), targetB.rotation, Mathf.Pow(initInfluence + influenceOffset, influencePower));
        }
    }

    void CalculateInfluence()
    {
        if (targetA && targetB)
        {
            float toTargetA = Vector3.Magnitude(targetA.position - transform.position);
            float toTargetB = Vector3.Magnitude(targetB.position - transform.position);
            float maxDistance = Vector3.Magnitude(targetA.position - targetB.position);

            initInfluence = toTargetA / maxDistance;
        }
    }
}
