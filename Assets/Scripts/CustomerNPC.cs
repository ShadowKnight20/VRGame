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
    public Transform ingredientDropPoint;
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
                // Convert the drop point to world space
                Vector3 worldPosition = ingredientDropPoint.TransformPoint(ingredientDropPoint.localPosition);

                // Instantiate the ingredient in world space
                Instantiate(ingredientPrefab, worldPosition, Quaternion.identity);
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
        onComplete?.Invoke();
    }
}