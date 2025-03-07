using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class ProblemManager : MonoBehaviour
{
    public List<CustomerProblem> customerProblems; // Drag & drop problems in Unity Inspector

    public CustomerProblem GetRandomProblem()
    {
        if (customerProblems.Count == 0) return null; // Prevent errors if no problems exist
        return customerProblems[Random.Range(0, customerProblems.Count)];
    }
}