using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gradient : MonoBehaviour
{
    public Transform Joint0;
    public Transform Joint1;
    public Transform Joint2;
    public Transform endEffector;
    public Transform target;


    public float alpha = 0.1f;
    private float tolerance = 1f;
    private float costFunction;
    private Vector3 gradient;
    private Vector3 theta;


    private float l1;
    private float l2;
    private float l3;


    // Parameters Adam's method - Adapt learning rate
    private float beta1 = 0.9f;
    private float beta2 = 0.999f;
    private float epsilon = 1e-8f;
    private int t = 1;
    private Vector3 m_t = Vector3.zero;
    private Vector3 v_t = Vector3.zero;

    //Angle constraints

    public Vector2 joint1Limits = new Vector2(-Mathf.PI * 0.25f, Mathf.PI * 0.25f); // -135 degrees to 135 degrees
    public Vector2 joint2Limits = new Vector2(-Mathf.PI * 0.5f, Mathf.PI * 0.5f); // -90 degrees to 90 degrees
    public Vector2 joint3Limits = new Vector2(-Mathf.PI * 0.5f, Mathf.PI * 0.5f);//-90 to 90

    // Extras
    public LineRenderer lineRenderer1;
    public LineRenderer lineRenderer2;
    public LineRenderer lineRenderer3;

    // Start is called before the first frame update
    void Start()
    {
        l1 = Vector3.Distance(Joint0.position, Joint1.position);
        l2 = Vector3.Distance(Joint1.position, Joint2.position);
        l3 = Vector3.Distance(Joint2.position, endEffector.position);

        costFunction = Vector3.Distance(endEffector.position, target.position) * Vector3.Distance(endEffector.position, target.position);
        theta = Vector3.zero;

        InitializeLineRenderer(lineRenderer1, Joint0, Joint1);
        InitializeLineRenderer(lineRenderer2, Joint1, Joint2);
        InitializeLineRenderer(lineRenderer3, Joint2, endEffector);
    }

    // Update is called once per frame
    void Update()
    {

        if (costFunction > tolerance)
        {

            gradient = CalculateGradient();

            Vector3 newAlpha = AdaptativeLearningRate(gradient); //adaptative alpha * gradient
            theta += -newAlpha;

            theta = ApplyingConstraints(theta); //angle constraints

            ForwardKinematics(theta); //update position

        }

        costFunction = Vector3.Distance(endEffector.position, target.position) * Vector3.Distance(endEffector.position, target.position);


        UpdateVisualLinks(lineRenderer1, Joint0, Joint1);
        UpdateVisualLinks(lineRenderer2, Joint1, Joint2);
        UpdateVisualLinks(lineRenderer3, Joint2, endEffector);
    }

    Vector3 ApplyingConstraints(Vector3 proposedAngles)
    {

        Vector3 constrainedAngles = proposedAngles;

        // theta0 - proposedAngles.x
        if (proposedAngles.x < joint1Limits.x)
        {

            constrainedAngles.x = joint1Limits.x;
        }
        else if (proposedAngles.x > joint1Limits.y)
        {

            constrainedAngles.x = joint1Limits.y;
        }

        //theta1 - proposedAngles.y

        if (proposedAngles.y < joint2Limits.x)
        {

            constrainedAngles.y = joint2Limits.x;
        }
        else if (proposedAngles.y > joint2Limits.y)
        {
            constrainedAngles.y = joint2Limits.y;
        }

        //theta2 - proposedAngles.z 
        if (proposedAngles.z < joint3Limits.x)
        {
            constrainedAngles.z = joint3Limits.x;
        }
        else if (proposedAngles.z > joint3Limits.y)
        {
            constrainedAngles.z = joint3Limits.y;
        }

        return constrainedAngles;
    }

    Vector3 AdaptativeLearningRate(Vector3 gradient)
    {

        m_t = beta1 * m_t + (1 - beta1) * gradient;
        v_t = beta2 * v_t + (1 - beta2) * Vector3.Scale(gradient, gradient);

        Vector3 m_hat = m_t / (1 - Mathf.Pow(beta1, t));
        Vector3 v_hat = v_t / (1 - Mathf.Pow(beta2, t));

        Vector3 adaptiveAlpha = new Vector3(alpha * m_hat.x / Mathf.Sqrt(v_hat.x) + epsilon,
                                alpha * m_hat.y / Mathf.Sqrt(v_hat.y) + epsilon,
                                alpha * m_hat.z / Mathf.Sqrt(v_hat.z) + epsilon);

        return adaptiveAlpha;

    }


    float Cost(Vector3 theta)
    {
        Vector3 oldJ2 = Joint2.position;

        Vector3 endEffector = GetEndEffectorPosition(theta, oldJ2);

        return Vector3.Distance(endEffector, target.position) * Vector3.Distance(endEffector, target.position);

    }

    Vector3 CalculateGradient()
    {

        Vector3 gradientVector;

        float step = 0.0001f;

        Vector3 thetaXPlus = new Vector3(theta.x + step, theta.y, theta.z);
        Vector3 thetaYPlus = new Vector3(theta.x, theta.y + step, theta.z);
        Vector3 thetaZPlus = new Vector3(theta.x, theta.y, theta.z + step);

        float DCostDX = (Cost(thetaXPlus) - Cost(theta)) / step;
        float DCostDY = (Cost(thetaYPlus) - Cost(theta)) / step;
        float DCostDZ = (Cost(thetaZPlus) - Cost(theta)) / step;


        gradientVector = new Vector3(DCostDX, DCostDY, DCostDZ);

        return gradientVector;


    }


    Vector3 GetEndEffectorPosition(Vector3 theta, Vector3 oldJ2)
    {
        Vector3 newPosition;

        Quaternion rot2 = Quaternion.Euler(0, 0, (theta.x + theta.y + theta.z) * Mathf.Rad2Deg);
        newPosition = Joint2.position + rot2 * (endEffector.position - oldJ2);

        return newPosition;
    }

    Vector3 GetJoint2Position(Vector3 oldJ1)
    {
        Vector3 newPosition;

        Quaternion rot1 = Quaternion.Euler(0, 0, (theta.x + theta.y) * Mathf.Rad2Deg);
        newPosition = Joint1.position + rot1 * (Joint2.position - oldJ1);

        return newPosition;
    }

    Vector3 GetJoint1Position()
    {
        Vector3 newPosition;

        Quaternion rot0 = Quaternion.Euler(0, 0, theta.x * Mathf.Rad2Deg);

        newPosition = Joint0.position + rot0 * (Joint1.position - Joint0.position);

        newPosition.z = 0;

        return newPosition;
    }

    void ForwardKinematics(Vector3 angle)
    {
        Vector3 oldJ1 = Joint1.position;
        Vector3 oldJ2 = Joint2.position;

        endEffector.position = GetEndEffectorPosition(angle, oldJ2);
        Joint1.position = GetJoint1Position();
        Joint2.position = GetJoint2Position(oldJ1);
    }

    void InitializeLineRenderer(LineRenderer lineRenderer, Transform startpoint, Transform endPoint)
    {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = startpoint.GetComponent<SpriteRenderer>().color;
        lineRenderer.endColor = endPoint.GetComponent<SpriteRenderer>().color;
        lineRenderer.sortingOrder = -1;
    }

    void UpdateVisualLinks(LineRenderer lineRenderer, Transform startpoint, Transform endPoint)
    {
        lineRenderer.SetPosition(0, startpoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
    }
}
