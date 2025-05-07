using System.Collections.Generic;
using UnityEngine;

public class EnchantmentGiver : MonoBehaviour
{
    public List<GameObject> objects;

    private Dictionary<string, GameObject> objectLookup;

    void Start()
    {
        objectLookup = new Dictionary<string, GameObject>();
        foreach (var obj in objects)
        {
            obj.SetActive(false); // Start all inactive
            objectLookup[obj.name] = obj;
        }
    }

    public void Spawn(string objectName)
    {
        if (objectLookup.TryGetValue(objectName, out GameObject target))
        {
            foreach (var obj in objectLookup.Values)
                obj.SetActive(false); // Deactivate all

            target.SetActive(true); // Activate matched
            Debug.Log("Spawned: " + objectName);
        }
        else
        {
            Debug.LogWarning("No object matched: " + objectName);
        }
    }
}
