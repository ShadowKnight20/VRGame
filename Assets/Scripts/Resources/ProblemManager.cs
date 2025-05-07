using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class ProblemManager : MonoBehaviour
{
    public List<CustomerProblem> customerProblems;
    public TextMeshProUGUI customerProblemText; // Drag & drop in Inspector
    public CustomerNPC customer;

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
            customer.currentProblem = newProblem; // Ensure the customer has a problem assigned
        }
        else
        {
            Debug.LogError("No problems available in customerProblems list!");
        }
    }

    public CustomerProblem GetRandomProblem()
    {
        if (customerProblems.Count == 0) return null;
        return customerProblems[Random.Range(0, customerProblems.Count)];
    }

    public string GradePotion(string playerPotion, string[] potionEffects, CustomerProblem problem, out int moneyValue)
    {
        moneyValue = 0; // Default value

        if (problem == null)
        {
            Debug.LogError("GradePotion received a null problem!");
            return " Error: No problem assigned.";
        }

        // Check for a perfect potion
        if (problem.validPotions != null)
        {
            foreach (string validPotion in problem.validPotions)
            {
                if (playerPotion == validPotion)
                {
                    moneyValue = 100;
                    return " Perfect!" + problem.positiveReaction;
                }
            }
        }

        // Check for an acceptable potion (contains required effects)
        bool containsAllEffects = true;
        if (problem.requiredEffects != null)
        {
            foreach (string requiredEffect in problem.requiredEffects)
            {
                if (!System.Array.Exists(potionEffects, effect => effect == requiredEffect))
                {
                    containsAllEffects = false;
                    break;
                }
            }
        }

        if (containsAllEffects)
        {
            moneyValue = 50;
            return " Acceptable! " + problem.positiveReaction;
        }

        // If the potion is incorrect
        moneyValue = 10;
        return " Bad! " + problem.negativeReaction;
    }
}