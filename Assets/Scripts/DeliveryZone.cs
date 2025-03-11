using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    public ProblemManager problemManager;
    public GameObject moneyPrefab; // Prefab for money
    public Transform moneySpawnPoint;
    private CustomerNPC customer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Potion"))
        {
            Potion potion = other.GetComponent<Potion>();
            if (potion != null)
            {
                // Find the current active customer in the scene
                customer = FindObjectOfType<CustomerNPC>();

                if (customer == null)
                {
                    Debug.LogError("No active customer found!");
                    return;
                }

                // Grade the potion
                string feedback = problemManager.GradePotion(potion.potionName, potion.effects, customer.currentProblem, out int moneyValue);

                // Give customer feedback
                customer.GiveFeedback(feedback);

                // Destroy potion
                Destroy(other.gameObject);

                // Make the customer leave
                customer.Leave();

                // Spawn money with the calculated value
                SpawnMoney(moneyValue);

                // Assign new customer problem
                problemManager.ShowNewProblem();
            }
        }
    }

    void SpawnMoney(int amount)
    {
        GameObject money = Instantiate(moneyPrefab, moneySpawnPoint.position, Quaternion.identity);
        Money moneyScript = money.GetComponent<Money>();
        if (moneyScript != null)
        {
            moneyScript.SetValue(amount);
        }
    }
}