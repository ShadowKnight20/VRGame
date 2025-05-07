using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using PDollarGestureRecognizer;
using System.IO;
using UnityEngine.Events;

public class MovementRecogniser : MonoBehaviour
{
    public XRNode inputSource;
    public InputHelpers.Button inputButton;
    public float inputThreshold = 0.1f;
    public Transform movementSource;

    public float newPositionThresholdDistance = 0.05f;
    public GameObject debugCubePrefab;
    public bool creationMode = true;
    public string newGestureName;

    public float recognitionThreshold = 0.8f;

    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
    public UnityStringEvent OnRecognized;

    private List<Gesture> traningSet = new List<Gesture>();
    private bool isMoving = false;
    private List<Vector3> positionsList = new List<Vector3>();

    void Start()
    {
        string[] gestureFiles = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        foreach (var file in gestureFiles)
        {
            Gesture gesture = GestureIO.ReadGestureFromFile(file);
            if (IsValidGesture(gesture))
            {
                traningSet.Add(gesture);
            }
            else
            {
                Debug.LogWarning($"Skipped invalid gesture file: {file}");
            }
        }
    }

    void Update()
    {
        if (movementSource == null)
        {
            Debug.LogError("Movement source is not set.");
            return;
        }

        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), inputButton, out bool isPressed, inputThreshold);

        if (!isMoving && isPressed)
        {
            StartMovement();
        }
        else if (isMoving && !isPressed)
        {
            EndMovement();
        }
        else if (isMoving && isPressed)
        {
            UpdateMovement();
        }
    }

    void StartMovement()
    {
        isMoving = true;
        positionsList.Clear();
        positionsList.Add(movementSource.position);

        if (debugCubePrefab)
            Destroy(Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity), 3);
    }

    void EndMovement()
    {
        isMoving = false;

        if (positionsList.Count < 2)
        {
            Debug.LogWarning("Not enough points to create a gesture.");
            return;
        }

        // Convert positions to 2D screen points
        Point[] pointArray = new Point[positionsList.Count];
        for (int i = 0; i < positionsList.Count; i++)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(positionsList[i]);
            pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
        }

        if (System.Array.Exists(pointArray, p => p == null))
        {
            Debug.LogError("Null point detected in gesture.");
            return;
        }

        Gesture newGesture = new Gesture(pointArray);

        if (creationMode)
        {
            newGesture.Name = newGestureName;
            traningSet.Add(newGesture);

            string directoryPath = "Assets/RecordedSpells";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string fileName = Path.Combine(directoryPath, newGestureName + ".xml");
            GestureIO.WriteGesture(pointArray, newGestureName, fileName);
        }
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture, traningSet.ToArray());
            Debug.Log($"{result.GestureClass} ({result.Score})");

            if (result.Score > recognitionThreshold)
            {
                OnRecognized.Invoke(result.GestureClass);
            }
        }
    }

    void UpdateMovement()
    {
        Vector3 lastPosition = positionsList[positionsList.Count - 1];

        if (Vector3.Distance(movementSource.position, lastPosition) > newPositionThresholdDistance)
        {
            positionsList.Add(movementSource.position);

            if (debugCubePrefab)
                Destroy(Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity), 3);
        }
    }

    bool IsValidGesture(Gesture gesture)
    {
        return gesture != null &&
               gesture.Points != null &&
               gesture.Points.Length > 1 &&
               !System.Array.Exists(gesture.Points, p => p == null);
    }
}