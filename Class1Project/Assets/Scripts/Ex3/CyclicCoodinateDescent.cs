using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclicCoodinateDescent : MonoBehaviour
{
    //Robots Arm
    public Transform Joint0;
    public Transform Joint1;
    public Transform Joint2;
    public Transform end;

    private int index = 2;
    private Vector3[] Joints = new Vector3[3];

    //Target
    public Transform target;

    //Method's Parameter
    public float tolerance = 1.0f;
    public float maxIterations = 1e5f;
    private int countIteration = 0;
    private float distance;


    //Rotation varaibles
    private float angleRotation;
    private Vector3 axisRotation;


    //Conexions bwtween Joints
    private Vector3[] Links;


    public LineRenderer lineRenderer1;
    public LineRenderer lineRenderer2;
    public LineRenderer lineRenderer3;

    void Start()
    {
        Joints[0] = Joint0.position;
        Joints[1] = Joint1.position;
        Joints[2] = Joint2.position;

        getLinks();

        distance = Vector3.Magnitude(Links[2]);





        InitializeLineRenderer(lineRenderer1, Joint0, Joint1);
        InitializeLineRenderer(lineRenderer2, Joint1, Joint2);
        InitializeLineRenderer(lineRenderer3, Joint2, end);
    }

    // Update is called once per frame
    void Update()
    {
        if (countIteration < maxIterations && distance > tolerance)
        {
            Vector3 currentJoint = Joints[index];
            Vector3[] referenceVector;
            referenceVector = GetVector(currentJoint);
            angleRotation = getRotation(referenceVector);
            axisRotation = getAxisRotation(referenceVector);
            Quaternion rotation = Quaternion.AngleAxis(angleRotation * 180 / Mathf.PI, axisRotation);

            UpdatePosition(index, rotation);

            if(index == 0)
            {
                index = 2;
            }
            else { index--; }

            countIteration++;
        }


        UpdateVisualLinks(lineRenderer1, Joint0, Joint1);
        UpdateVisualLinks(lineRenderer2, Joint1, Joint2);
        UpdateVisualLinks(lineRenderer3, Joint2, end);
    }

    void UpdatePosition(int index, Quaternion q)
    {
        if(index < 2)
        {
            for(int i = index; i < 2; i++)
            {
                Joints[i+1]= Joints[i] + q * Links[i];
            }
        }

        end.position = Joints[2] + q * Links[2];

        Joint0.position = Joints[0];
        Joint1.position = Joints[1];
        Joint2.position = Joints[2];

        getLinks();
    }

    private Vector3 getAxisRotation(Vector3[] vectors)
    {
        Vector3 axis = Vector3.Cross(vectors[0], vectors[1]);
        axis.Normalize();
        return axis;
    }

    private float getRotation(Vector3[] vectors)
    {
        float angle;
        angle = Mathf.Acos(Mathf.Clamp(Vector3.Dot(vectors[0], vectors[1]), -1.0f, 1.0f));

        return angle;
    }

    private Vector3[] GetVector(Vector3 joint)
    {
        Vector3[] referenceVectors = new Vector3[2];

        referenceVectors[0] = Vector3.Normalize(end.position - joint);
        referenceVectors[1] = Vector3.Normalize(target.position - joint);

        return referenceVectors;
    }

    void getLinks()
    {
        Links = new Vector3[3];
        for (int i = 0; i < 3 - 1; i++)
        {
            Links[i] = Joints[i + 1] - Joints[i];
        }

        Links[2] = end.position - Joints[2];
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
