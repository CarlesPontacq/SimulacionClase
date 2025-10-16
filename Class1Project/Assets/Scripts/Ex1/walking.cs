using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walking : MonoBehaviour
{

    public Transform hipJoint;
    public Transform kneeJoint;
    public Transform ankleJoint;
    public Transform footJoint;

    public LineRenderer lineRenderer1;  // LineRenderer for joint1 -> joint2
    public LineRenderer lineRenderer2;  // LineRenderer for joint2 -> joint3
    public LineRenderer lineRenderer3;  // LineRenderer for joint2 -> joint3
    
   
    public float walkSpeed = 2f ;
    public float hipAmplitude = 30f;

    public float kneeAmplitude = 20f;

    public float ankleAmplitude = 10f;

    void Start()
    {
     InitializeLineRenderer(lineRenderer1);
     InitializeLineRenderer(lineRenderer2);
     InitializeLineRenderer(lineRenderer2);    
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time*walkSpeed;

        float hipRotaion = Mathf.Sin(time)*hipAmplitude;
        float kneeRotation = Mathf.Sin(-(time + Mathf.PI/4))*kneeAmplitude;
        float ankleRotation = Mathf.Sin(-(time + Mathf.PI/2))*ankleAmplitude;

        hipJoint.localRotation = Quaternion.Euler(0,0,hipRotaion);
        kneeJoint.localRotation = Quaternion.Euler(0,0,kneeRotation);
        ankleJoint.localRotation = Quaternion.Euler(0,0, ankleRotation);
       UpdateVisualLinks();
    }


    void InitializeLineRenderer(LineRenderer lineRenderer)
    {
        // Set up the LineRenderer properties like width, color, etc.
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2; // Each bone only needs 2 points (start and end)
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));  // Basic material for 2D
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }

    void UpdateVisualLinks()
    {
        // Update line from joint1 to joint2
        lineRenderer1.SetPosition(0, hipJoint.position); // Start point at joint1
        lineRenderer1.SetPosition(1, kneeJoint.position); // End point at joint2

        // Update line from joint2 to joint3
        lineRenderer2.SetPosition(0, kneeJoint.position); // Start point at joint2
        lineRenderer2.SetPosition(1, ankleJoint.position); // End point at joint3

        //lineRenderer3.SetPosition(0, ankleJoint.position); // Start point at joint1
        //lineRenderer3.SetPosition(1, footJoint.position);
    }
}