using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCustomerProblem", menuName = "Customer/Problem")]
public class CustomerProblem : ScriptableObject
{
    public string problemDescription;
    public string requiredEnchantment;
}
