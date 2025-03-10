using UnityEngine;

[CreateAssetMenu(fileName = "NewCustomerProblem", menuName = "Customer/Problem")]
public class CustomerProblem : ScriptableObject
{
    [TextArea] public string problemDescription;
    public string[] requiredEffects;  // Effects needed for a "good" enchantment
    public string[] validEnchants;     // Enchant that guarantee a "Perfect" score
    public string positiveReaction;
    public string negativeReaction;
}