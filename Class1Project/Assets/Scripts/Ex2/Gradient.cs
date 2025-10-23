using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gradient : MonoBehaviour
{
    public Transform joint0;
    public Transform joint1;
    public Transform joint2;
    public Transform endEffector;
    public Transform target;

    public float alpha = 0.1f;
    private float tolerance = 1f;
    private float costFunction;
    private Vector3 theta;
    private Vector3 gradient;

    private float l1;
    private float l2;
    private float l3;

    //Parameters Adam's mehod - Adapt Learning Rate
    float beta1 = 0.9f;
    float beta2 = 0.99f;
    private float epsilon = 1e-8f;
    private int t = 1;
    Vector3 m_t = Vector3.zero;
    Vector3 v_t = Vector3.zero;

    // Extras
    public LineRenderer lineRenderer1;
    public LineRenderer lineRenderer2;
    public LineRenderer lineRenderer3;

    // Start is called before the first frame update
    void Start()
    {
        l1 = Vector3.Distance(joint0.position, joint1.position);
        l2 = Vector3.Distance(joint1.position, joint2.position);
        l3 = Vector3.Distance(joint2.position, endEffector.position);

        costFunction = Vector3.Distance(endEffector.position, target.position) * Vector3.Distance(endEffector.position, target.position);
        theta = Vector3.zero;

        InitializeLineRenderer(lineRenderer1, joint0, joint1);
        InitializeLineRenderer(lineRenderer2, joint1, joint2);
        InitializeLineRenderer(lineRenderer3, joint2, endEffector);
    }

    // Update is called once per frame
    void Update()
    {

        if (costFunction > tolerance)
        {
            t++;
            gradient = CalculateGradient();
            Vector3 newAlpha = AdaptativeLearningRate(gradient);
            theta += -newAlpha;
            endEffector.position = GetEndEffectorPosition(theta);


            joint1.position = GetJoint1Position();
            joint2.position = GetJoint2Position();
        }

        costFunction = Vector3.Distance(endEffector.position, target.position) * Vector3.Distance(endEffector.position, target.position);

        UpdateVisualLinks(lineRenderer1, joint0, joint1);
        UpdateVisualLinks(lineRenderer2, joint1, joint2);
        UpdateVisualLinks(lineRenderer3, joint2, endEffector);
    }

    Vector3 AdaptativeLearningRate(Vector3 gradient)
    {
        m_t = beta1 * m_t + (1 - beta1) * gradient;
        v_t = beta2 * v_t + (1 - beta2) * Vector3.Scale(gradient, gradient);

        Vector3 m_hat = m_t / (1 - Mathf.Pow(beta1, t));
        Vector3 v_hat = v_t / (1 - Mathf.Pow(beta2, t));

        Vector3 adaptativeAlpha = new Vector3(alpha * m_hat.x / (Mathf.Sqrt(v_hat.x) + epsilon),
            alpha * m_hat.y / (Mathf.Sqrt(v_hat.y) + epsilon),
            alpha * m_hat.z / (Mathf.Sqrt(v_hat.z) + epsilon));

        return adaptativeAlpha;
    }


    float Cost(Vector3 theta)
    {

        Vector3 endEffector = GetEndEffectorPosition(theta);

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


    Vector3 GetEndEffectorPosition(Vector3 theta)
    {
        Vector3 newPosition;

        newPosition.x = joint0.position.x + l1 * Mathf.Cos(theta.x)
                       + l2 * Mathf.Cos(theta.x + theta.y)
                       + l3 * Mathf.Cos(theta.x + theta.y + theta.z);
        newPosition.y = joint0.position.y + l1 * Mathf.Sin(theta.x)
                       + l2 * Mathf.Sin(theta.x + theta.y)
                       + l3 * Mathf.Sin(theta.x + theta.y + theta.z);

        newPosition.z = 0;

        return newPosition;
    }

    Vector3 GetJoint2Position()
    {
        Vector3 newPosition;

        newPosition.x = joint0.position.x + l1 * Mathf.Cos(theta.x)
                       + l2 * Mathf.Cos(theta.x + theta.y);
        newPosition.y = joint0.position.y + l1 * Mathf.Sin(theta.x)
                       + l2 * Mathf.Sin(theta.x + theta.y);

        newPosition.z = 0;

        return newPosition;
    }

    Vector3 GetJoint1Position()
    {
        Vector3 newPosition;

        newPosition.x = joint0.position.x + l1 * Mathf.Cos(theta.x);
        newPosition.y = joint0.position.y + l1 * Mathf.Sin(theta.x);

        newPosition.z = 0;

        return newPosition;
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
