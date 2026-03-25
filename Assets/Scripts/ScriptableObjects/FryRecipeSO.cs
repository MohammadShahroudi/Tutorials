using UnityEngine;

[CreateAssetMenu()]
public class FryRecipeSO : ScriptableObject
{
   public KitchenObjectSO input;
   public KitchenObjectSO output;
   public int cuttingProgressMax;
}
