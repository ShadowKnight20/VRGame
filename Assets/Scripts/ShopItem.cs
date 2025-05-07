using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class ShopItem : MonoBehaviour
{
    public int itemPrice = 100;
    private int amountPaid = 0;

    public GameObject itemToUnlock;

    [Header("UI References")]
    public TextMeshProUGUI costText;
    public TextMeshProUGUI remainingText;

    [Header("Payment Settings")]
    public float paymentRadius = 0.5f;

    private XRGrabInteractable grabInteractable;

    void Start()
    {
        if (itemToUnlock == null)
        {
            Debug.LogError("No itemToUnlock assigned.");
            return;
        }

        grabInteractable = itemToUnlock.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.enabled = false; // Make it ungrabbable until paid
        }

        UpdateUI();
    }

    void Update()
    {
        CheckForNearbyMoney();
    }

    void CheckForNearbyMoney()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, paymentRadius);

        foreach (Collider col in hits)
        {
            Money money = col.GetComponent<Money>();
            if (money != null)
            {
                ApplyPayment(money.value);
                Destroy(col.gameObject); // Use up the money object
                break;
            }
        }
    }

    void ApplyPayment(int payment)
    {
        amountPaid += payment;
        amountPaid = Mathf.Min(amountPaid, itemPrice);

        UpdateUI();

        if (amountPaid >= itemPrice)
            UnlockItem();
    }

    void UnlockItem()
    {
        if (grabInteractable != null)
        {
            grabInteractable.enabled = true;
            Debug.Log("Item unlocked and now grabbable!");
        }
    }

    void UpdateUI()
    {
        if (costText != null)
            costText.text = $"Cost: ${itemPrice}";

        if (remainingText != null)
        {
            int remaining = Mathf.Max(0, itemPrice - amountPaid);
            remainingText.text = $"Remaining: ${remaining}";
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, paymentRadius);
    }
}
