using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientInfinity : MonoBehaviour
{
    public List<Transform> joints;
    public Transform endEffector;
    public Transform target;

    public float alpha = 0.1f;
    private float tolerance = 1f;
    private float costFunction;
    private Vector3 theta;
    private Vector3 gradient;

    private int size;
    private List<float> ls;
    private float l2;
    private float l3;


    void Start()
    {
        size = joints.Count;
        int index = 0;
        foreach(Transform joint in joints)
        {
            if(joint != joints[size - 1])
            {
                ls[index] = Vector3.Distance(joint.position, joints[index + 1].position);
            }
            else
            {
                ls[index] = Vector3.Distance(joint.position, endEffector.position);
            }
            index++;
        }
        //l1 = Vector3.Distance(joint.position, joint1.position);
        //l2 = Vector3.Distance(joint1.position, joint2.position);
        //l3 = Vector3.Distance(joint2.position, endEffector.position);

        costFunction = Mathf.Pow(Vector3.Distance(endEffector.position, target.position), 2);
        theta = Vector3.zero;
    }

    void Update()
    {
        if (costFunction > tolerance)
        {
            //gradient = CalculateGradient();
            theta += -alpha * gradient;
            //endEffector.position = UpdateEfectorPosition();

        //    joint1.position = UpdateJoint1();
        //    joint2.position = UpdateJoint2();
        }

        costFunction = Mathf.Pow(Vector3.Distance(endEffector.position, target.position), 2);
    }

    //private Vector3 UpdateJoint1()
    //{
    //    Vector3 newPosition;

    //    newPosition.x = joint0.position.x + l1 * Mathf.Cos(theta.x);
    //    newPosition.y = joint0.position.y + l1 * Mathf.Sin(theta.x);
    //    newPosition.z = 0;

    //    return newPosition;
    //}

    //private Vector3 UpdateJoint2()
    //{
    //    Vector3 newPosition;

    //    newPosition.x = joint0.position.x + l1 * Mathf.Cos(theta.x) + l2 * Mathf.Cos(theta.x + theta.y);
    //    newPosition.y = joint0.position.y + l1 * Mathf.Sin(theta.x) + l2 * Mathf.Sin(theta.x + theta.y);
    //    newPosition.z = 0;

    //    return newPosition;
    //}

    //private Vector3 UpdateEfectorPosition()
    //{
    //    Vector3 newPosition;

    //    newPosition.x = joint0.position.x + l1 * Mathf.Cos(theta.x) + l2 * Mathf.Cos(theta.x + theta.y) + l3 * Mathf.Cos(theta.x + theta.y + theta.z);
    //    newPosition.y = joint0.position.y + l1 * Mathf.Sin(theta.x) + l2 * Mathf.Sin(theta.x + theta.y) + l3 * Mathf.Sin(theta.x + theta.y + theta.z);
    //    newPosition.z = 0;

    //    return newPosition;
    //}

    //private Vector3 CalculateGradient()
    //{
    //    Vector3 gradientVector;

    //    Vector3 coeff = 2 * (endEffector.position - target.position);

    //    gradientVector.x = -coeff.x * (l1 * Mathf.Sin(theta.x) + l2 * Mathf.Sin(theta.x + theta.y) + l3 * Mathf.Sin(theta.x + theta.y + theta.z))
    //        + coeff.y * (l1 * Mathf.Cos(theta.x) + l2 * Mathf.Cos(theta.x + theta.y) + l3 * Mathf.Cos(theta.x + theta.y + theta.z));

    //    gradientVector.y = -coeff.x * (l2 * Mathf.Sin(theta.x + theta.y) + l3 * Mathf.Sin(theta.x + theta.y + theta.z))
    //        + coeff.y * (l2 * Mathf.Cos(theta.x + theta.y) + l3 * Mathf.Cos(theta.x + theta.y + theta.z));

    //    gradientVector.z = -coeff.x * (l3 * Mathf.Sin(theta.x + theta.y + theta.z)) + coeff.y * (l3 * Mathf.Cos(theta.x + theta.y + theta.z));

    //    gradientVector.Normalize();

    //    return gradientVector;
    //}
}
