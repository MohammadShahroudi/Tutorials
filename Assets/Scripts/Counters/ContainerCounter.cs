using UnityEngine;
using System;

public class ContainerCounter : BaseCounter
{
    
    public event EventHandler OnPlayerGrabbedObject;
    
    
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    
    
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // Player is not carrying anything
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
            
            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }
}
