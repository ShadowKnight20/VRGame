using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class ProblemManager : MonoBehaviour
{
    public List<CustomerProblem> customerProblems;
    public TextMeshProUGUI customerProblemText; // Drag & drop in Inspector

    void Start()
    {
        ShowNewProblem();
    }

    public void ShowNewProblem()
    {
        CustomerProblem newProblem = GetRandomProblem();
        if (newProblem != null)
        {
            customerProblemText.text = newProblem.problemDescription;
        }
    }

    public CustomerProblem GetRandomProblem()
    {
        if (customerProblems.Count == 0) return null;
        return customerProblems[Random.Range(0, customerProblems.Count)];
    }

    public string GradePotion(string playerPotion, string[] potionEffects, CustomerProblem problem)
    {
        // Check for perfect match
        foreach (string validPotion in problem.validEnchants)
        {
            if (playerPotion == validPotion)
            {
                return "⭐⭐⭐ Perfect! " + problem.positiveReaction;
            }
        }

        // Check for acceptable match (contains all required effects)
        bool containsAllEffects = true;
        foreach (string requiredEffect in problem.requiredEffects)
        {
            if (!System.Array.Exists(potionEffects, effect => effect == requiredEffect))
            {
                containsAllEffects = false;
                break;
            }
        }

        if (containsAllEffects)
        {
            return "⭐⭐ Good! " + problem.positiveReaction;
        }

        // If it fails both checks
        return "❌ Bad! " + problem.negativeReaction;
    }
}