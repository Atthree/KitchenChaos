using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKitchenObjectParents 
{
    public Transform GetKitchenObjectFollowTransform();

    // KitchenObject'i ayarlar
    public void SetKitchenObject(KitchenObject kitchenObject);


    // KitchenObject'i döner
    public KitchenObject GetKitchenObject();


    // KitchenObject referansýný temizler (null yapar)
    public void ClearKitchenObject();


    // Counter'da KitchenObject olup olmadýðýný kontrol eder
    public bool HasKitchenObject();
    
}
