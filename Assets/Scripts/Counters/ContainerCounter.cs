using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    // KitchenObject i�in ScriptableObject referans�, prefab bilgilerini i�erir
    [SerializeField] private KitchenObjectSO kitchenObjectSO;


    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject()) {
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
            

            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }
    
}
