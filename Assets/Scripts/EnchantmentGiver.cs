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
            if (obj == null) continue;

            obj.SetActive(false); // Start all inactive

            string key = obj.name.Trim().ToLower(); // Normalize key
            objectLookup[key] = obj;

            Debug.Log($"Registered object: '{key}'");
        }
    }

    public void Spawn(string objectName)
    {
        Debug.Log("Spawn activated");
        string key = objectName.Trim().ToLower(); // Normalize input

        if (objectLookup.TryGetValue(key, out GameObject target))
        {
            foreach (var obj in objectLookup.Values)
                obj.SetActive(false); // Deactivate all

            target.SetActive(true); // Activate matched
            Debug.Log("Activated: " + target.name);
        }
        else
        {
            Debug.LogWarning($"No object matched: '{objectName}' (searched as '{key}')");
        }
    }
}
