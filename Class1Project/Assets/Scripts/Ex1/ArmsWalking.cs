using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmsWalking : MonoBehaviour
{
    public Transform shoulderJoint; // antes hipJoint
    public Transform elbowJoint;    // antes kneeJoint
    public Transform wristJoint;    // antes ankleJoint
    public Transform handJoint;     // antes footJoint

    public LineRenderer lineRenderer1;  // hombro -> codo
    public LineRenderer lineRenderer2;  // codo -> muñeca
    public LineRenderer lineRenderer3;  // muñeca -> mano

    [Header("Arm Swing Settings")]
    public float swingSpeed = 2f;
    public float shoulderAmplitude = 25f;
    public float elbowAmplitude = 15f;
    public float wristAmplitude = 10f;

    void Start()
    {
        InitializeLineRenderer(lineRenderer1);
        InitializeLineRenderer(lineRenderer2);
        InitializeLineRenderer(lineRenderer3);
    }

    void Update()
    {
        float time = Time.time * swingSpeed;

        // Movimiento alternado y más suave que las piernas
        float shoulderRotation = Mathf.Sin(time) * shoulderAmplitude;
        float elbowRotation = Mathf.Sin(-(time + Mathf.PI / 4)) * elbowAmplitude;
        float wristRotation = Mathf.Sin(-(time + Mathf.PI / 2)) * wristAmplitude;

        // Rotar en Z para un movimiento de péndulo lateral
        shoulderJoint.localRotation = Quaternion.Euler(0, 0, shoulderRotation);
        elbowJoint.localRotation = Quaternion.Euler(0, 0, elbowRotation);
        wristJoint.localRotation = Quaternion.Euler(0, 0, wristRotation);

        UpdateVisualLinks();
    }

    void InitializeLineRenderer(LineRenderer lineRenderer)
    {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }

    void UpdateVisualLinks()
    {
        lineRenderer1.SetPosition(0, shoulderJoint.position);
        lineRenderer1.SetPosition(1, elbowJoint.position);

        lineRenderer2.SetPosition(0, elbowJoint.position);
        lineRenderer2.SetPosition(1, wristJoint.position);

        lineRenderer3.SetPosition(0, wristJoint.position);
        lineRenderer3.SetPosition(1, handJoint.position);
    }
}
