using System.Collections.Generic;
using UnityEngine;

public class EnchantmentGiver : MonoBehaviour
{
    private BoxCollider enchantZone;
    private Dictionary<string, GameObject> objectLookup = new Dictionary<string, GameObject>();

    void Start()
    {
        // Try to find the EnchantZone in the scene
        GameObject zoneObj = GameObject.FindWithTag("EnchantZone");
        if (zoneObj == null)
        {
            zoneObj = GameObject.Find("EnchantZone");
        }

        if (zoneObj != null)
        {
            enchantZone = zoneObj.GetComponent<BoxCollider>();
        }

        if (enchantZone == null)
        {
            Debug.LogError("EnchantZone not found in scene. Please tag it 'EnchantZone' or name it exactly 'EnchantZone'.");
            return;
        }

        foreach (Transform child in transform)
        {
            var obj = child.gameObject;
            obj.SetActive(false);

            string key = obj.name.Trim().ToLower();
            objectLookup[key] = obj;
        }
    }

    public void Spawn(string objectName)
    {
        string key = objectName.Trim().ToLower();

        Debug.Log("Entered Spawn");

        if (!objectLookup.TryGetValue(key, out GameObject target))
        {
            Debug.LogWarning($"No object matched: '{objectName}' (searched as '{key}')");
            return;
        }

        if (!IsInsideEnchantZone(target))
        {
            Debug.LogWarning($"'{target.name}' is NOT inside the EnchantZone. Activation skipped.");
            return;
        }

        foreach (var obj in objectLookup.Values)
            obj.SetActive(false);

        target.SetActive(true);
        Debug.Log("Activated: " + target.name);

        // ✅ Get the Potion script on the parent of this EnchantmentGiver
        Potion potion = transform.parent?.GetComponent<Potion>();
        if (potion != null)
        {
            List<string> updatedEffects = new List<string>(potion.effects ?? new string[0]);

            if (!updatedEffects.Contains(objectName))
            {
                updatedEffects.Add(objectName);
                potion.effects = updatedEffects.ToArray();

                Debug.Log("Effect added to potion: " + objectName);
            }
            else
            {
                Debug.Log("Effect already exists in potion: " + objectName);
            }
        }
        else
        {
            Debug.LogWarning("No Potion script found on parent object.");
        }
    }



    private bool IsInsideEnchantZone(GameObject obj)
    {
        if (enchantZone == null) return false;

        // Use world position
        Vector3 worldPos = obj.transform.position;
        return enchantZone.bounds.Contains(worldPos);
    }

}
