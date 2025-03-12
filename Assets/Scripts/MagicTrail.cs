using UnityEngine;
using Oculus.Interaction.Input;

public class MagicDrawing : MonoBehaviour
{
    public OVRInput.Controller controller = OVRInput.Controller.RTouch; // Right controller
    public LineRenderer lineRenderer;
    public Transform handTransform; // Hand position for tracking

    private bool isDrawing = false;
    private Vector3 lastPosition;

    void Start()
    {
        if (!lineRenderer)
            lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        // Get hand position
        handTransform.position = OVRInput.GetLocalControllerPosition(controller);

        // Check if trigger is held down
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            if (!isDrawing)
            {
                StartDrawing();
            }
            UpdateDrawing();
        }
        else if (isDrawing)
        {
            StopDrawing();
        }
    }

    void StartDrawing()
    {
        isDrawing = true;
        lineRenderer.positionCount = 0; // Reset line
        lastPosition = handTransform.position;
    }

    void UpdateDrawing()
    {
        Vector3 newPos = handTransform.position;
        if (Vector3.Distance(lastPosition, newPos) > 0.01f) // Avoid redundant points
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPos);
            lastPosition = newPos;
        }
    }

    void StopDrawing()
    {
        isDrawing = false;
        CastMagic(); // Trigger magic effect after drawing
    }

    void CastMagic()
    {
        Debug.Log("Magic Casted!");
        // Here you can add effects like turning the drawing into a spell.
    }
}