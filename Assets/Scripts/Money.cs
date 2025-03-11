using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Money : MonoBehaviour
{
    public int value;
    public TextMeshProUGUI valueText; // Attach a floating text UI

    public void SetValue(int amount)
    {
        value = amount;
        if (valueText != null)
        {
            valueText.text = "$" + amount.ToString();
        }
    }
}
