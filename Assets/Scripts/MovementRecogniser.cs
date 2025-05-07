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
    public float inputThreshold;
    public Transform movementSource;

    public float newPositionThresholdDistance;
    public GameObject debugCubePrefab;
    public bool creationMode = true;
    public string newGestureName;

    public float recognitionThreshold;

    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
    public UnityStringEvent OnRecognized;

    private List<Gesture> traningSet = new List<Gesture>();
    private bool isMoving = false;
    private List<Vector3> positionsList = new List<Vector3>();

    private string gestureSavePath;

    void Start()
    {
        // Set the save/load path inside Assets
        gestureSavePath = Path.Combine(Application.dataPath, "RecordedSpells");

        // Create the directory if it doesn’t exist
        if (!Directory.Exists(gestureSavePath))
        {
            Directory.CreateDirectory(gestureSavePath);
        }

        // Load all gestures from that folder
        string[] gestureFiles = Directory.GetFiles(gestureSavePath, "*.xml");
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

        // Convert world positions to 2D screen points
        Point[] pointArray = new Point[positionsList.Count];
        for (int i = 0; i < positionsList.Count; i++)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(positionsList[i]);
            pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
        }

        Gesture newGesture = new Gesture(pointArray);

        if (creationMode)
        {
            newGesture.Name = newGestureName;
            traningSet.Add(newGesture);

            string fileName = Path.Combine(gestureSavePath, newGestureName + ".xml");
            GestureIO.WriteGesture(pointArray, newGestureName, fileName);

            Debug.Log($"Gesture '{newGestureName}' saved to: {fileName}");
        }
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture, traningSet.ToArray());
            Debug.Log($"{result.GestureClass} ({result.Score})");

            if (result.Score > recognitionThreshold)
            {
                OnRecognized.Invoke(result.GestureClass);

                // Dynamically find and trigger EnchantmentGiver
                EnchantmentGiver giver = FindObjectOfType<EnchantmentGiver>();
                if (giver != null)
                {
                    giver.Spawn(result.GestureClass);
                }
                else
                {
                    Debug.LogWarning("EnchantmentGiver not found in scene.");
                }
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
