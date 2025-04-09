using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantmentGiver : MonoBehaviour
{
    public List<GameObject> objects;

    public void Spawn(string objectName)
    {
        foreach(var item in objects)
        {
            item.SetActive(objectName == item.name);
        }
    }
}
