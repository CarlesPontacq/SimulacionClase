using UnityEngine;

public class ForwardKinematics2D : MonoBehaviour
{
    public Transform joint1;  // Root joint (e.g., shoulder)
    public Transform joint2;  // Middle joint (e.g., elbow)
    public Transform joint3;  // End joint (e.g., wrist)


    public LineRenderer lineRenderer1;  // LineRenderer for joint1 -> joint2
    public LineRenderer lineRenderer2;  // LineRenderer for joint2 -> joint3

    public float joint1RotationSpeed = 30f;  // Speed of rotation for joint 1
    public float joint2RotationSpeed = 30f;  // Speed of rotation for joint 2




    void Start()
    {

        InitializeLineRenderer(lineRenderer1);
        InitializeLineRenderer(lineRenderer2);

    }

    void Update()
    {

        // Rotate each joint based on input
        RotateJoints();
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
        lineRenderer1.SetPosition(0, joint1.position); // Start point at joint1
        lineRenderer1.SetPosition(1, joint2.position); // End point at joint2

        // Update line from joint2 to joint3
        lineRenderer2.SetPosition(0, joint2.position); // Start point at joint2
        lineRenderer2.SetPosition(1, joint3.position); // End point at joint3
    }


    void RotateJoints()
    {
        // Control joint1 (root) rotation
        joint1.Rotate(Vector3.forward, joint1RotationSpeed * Time.deltaTime * Input.GetAxis("Vertical"));

        // Control joint2 (middle) rotation
        joint2.Rotate(Vector3.forward, joint2RotationSpeed * Time.deltaTime * Input.GetAxis("Horizontal"));


    }
}
