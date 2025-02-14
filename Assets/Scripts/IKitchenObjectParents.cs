using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKitchenObjectParents 
{
    public Transform GetKitchenObjectFollowTransform();

    // KitchenObject'i ayarlar
    public void SetKitchenObject(KitchenObject kitchenObject);


    // KitchenObject'i d�ner
    public KitchenObject GetKitchenObject();


    // KitchenObject referans�n� temizler (null yapar)
    public void ClearKitchenObject();


    // Counter'da KitchenObject olup olmad���n� kontrol eder
    public bool HasKitchenObject();
    
}
