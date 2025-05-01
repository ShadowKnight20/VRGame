using UnityEngine;
using TMPro;
using System.Collections;

public class CustomerNPC : MonoBehaviour
{
    public TextMeshProUGUI feedbackText;
    public Transform exitPoint;
    public Transform counterPoint;
    public float moveSpeed = 2f;
    public GameObject[] possibleIngredientPrefabs;
    public CustomerProblem currentProblem;

    private bool reachedCounter = false;
    private CustomerSpawner spawner;

    public void SetPoints(Transform counter, Transform exit, CustomerSpawner customerSpawner)
    {
        counterPoint = counter;
        exitPoint = exit;
        spawner = customerSpawner;
    }

    void Start()
    {
        MoveToCounter();
    }

    public void ReceiveNewProblem()
    {
        ProblemManager problemManager = FindObjectOfType<ProblemManager>();
        if (problemManager != null)
        {
            currentProblem = problemManager.GetRandomProblem();
            DropIngredients();
        }
        else
        {
            Debug.LogError("ProblemManager not found in the scene!");
        }
    }

    void DropIngredients()
    {
        if (currentProblem == null || currentProblem.requiredIngredients == null) return;

        foreach (string ingredientName in currentProblem.requiredIngredients)
        {
            GameObject ingredientPrefab = FindIngredientPrefab(ingredientName);
            if (ingredientPrefab != null)
            {
                // Convert the counter's position to world space
                Vector3 spawnPosition = GetPositionInFrontOfCustomer();

                // Instantiate the ingredient in world space
                Instantiate(ingredientPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    GameObject FindIngredientPrefab(string ingredientName)
    {
        foreach (GameObject ingredient in possibleIngredientPrefabs)
        {
            if (ingredient.name == ingredientName)
            {
                return ingredient;
            }
        }
        return null;
    }

    public void GiveFeedback(string feedback)
    {
        feedbackText.text = feedback;
        Invoke("ClearFeedback", 3f);
    }

    void ClearFeedback()
    {
        feedbackText.text = "";
    }

    public void Leave()
    {
        if (gameObject.activeInHierarchy) // Ensure the customer is active before starting coroutine
        {
            StartCoroutine(MoveToExit());
        }
        else
        {
            Debug.LogError("Customer is inactive and cannot leave!");
        }
    }

    private IEnumerator MoveToExit()
    {
        while (Vector3.Distance(transform.position, exitPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, exitPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        if (spawner != null)
        {
            spawner.CustomerLeft();
        }

        Destroy(gameObject);
    }

    private void MoveToCounter()
    {
        if (gameObject.activeInHierarchy) // Ensure the GameObject is active
        {
            StartCoroutine(MoveToPosition(counterPoint.position, () => reachedCounter = true));
        }
        else
        {
            Debug.LogError("Customer is inactive and cannot start movement.");
        }
    }

    private IEnumerator MoveToPosition(Vector3 target, System.Action onComplete)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Once the customer reaches the counter, drop ingredients in front of them
        DropRandomIngredientInFront();

        onComplete?.Invoke();
    }

    private void DropRandomIngredientInFront()
    {
        if (possibleIngredientPrefabs.Length == 0)
        {
            Debug.LogError("No ingredient prefabs assigned!");
            return;
        }

        // Pick a random ingredient prefab from the list
        GameObject randomIngredient = possibleIngredientPrefabs[Random.Range(0, possibleIngredientPrefabs.Length)];

        if (randomIngredient != null)
        {
            // Get the spawn position in front of the customer
            Vector3 spawnPosition = GetPositionInFrontOfCustomer();

            // Log spawn position to confirm
            Debug.Log("Spawning ingredient at position: " + spawnPosition);

            // Instantiate the random ingredient at the spawn position
            Instantiate(randomIngredient, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No valid ingredient prefab found!");
        }
    }

    private Vector3 GetPositionInFrontOfCustomer()
    {
        // Offset distance in front of the customer
        float offsetDistance = -1f;

        // Add an offset for height (for example, to ensure the item spawns above the ground)
        float verticalOffset = 1f;  // Adjust this value to control the height

        // Calculate the position in front of the customer by using their forward vector
        Vector3 frontPosition = transform.position + transform.forward * offsetDistance;

        // Add the vertical offset (elevation) to make sure it spawns higher
        frontPosition.y += verticalOffset;

        return frontPosition;
    }
}
