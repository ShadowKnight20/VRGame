using UnityEngine;
using System.Collections;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public Transform counterPoint;
    public Transform exitPoint;
    private CustomerNPC currentCustomer; // Change type to CustomerNPC

    void Start()
    {
        SpawnCustomer();
    }

    public void SpawnCustomer()
    {
        if (currentCustomer == null)
        {
            GameObject customerObj = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
            customerObj.SetActive(true); // Ensure the customer is active
            currentCustomer = customerObj.GetComponent<CustomerNPC>();

            if (currentCustomer != null)
            {
                currentCustomer.SetPoints(counterPoint, exitPoint, this);
            }
            else
            {
                Debug.LogError("Customer prefab is missing the CustomerNPC script!");
            }
        }
    }

    public void CustomerLeft()
    {
        currentCustomer = null;
        StartCoroutine(SpawnNextCustomer());
    }

    IEnumerator SpawnNextCustomer()
    {
        yield return new WaitForSeconds(2f);
        SpawnCustomer();
    }
}