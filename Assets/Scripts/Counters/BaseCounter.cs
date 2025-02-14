using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCounter : MonoBehaviour,IKitchenObjectParents
{
    public static event EventHandler OnAnyObjectPlacedHere;


    public static void ResetStaticData()
    {
        OnAnyObjectPlacedHere = null;
    }
    // Counter'ýn üstünde nesnelerin yerleþtirileceði nokta (Transform)
    [SerializeField] private Transform counterTopPoint;

    // Bu counter'da þu anda bulunan KitchenObject referansý
    private KitchenObject kitchenObject;
    public virtual void Interact(Player player)
    {
        Debug.LogError("BaseCounter.Interact();");
    }
    public virtual void InteractAlternate(Player player)
    {
        //Debug.LogError("BaseCounter.InteractAlternate();");
    }
    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint; // Bu, counter'ýn üstündeki nokta
    }

    // KitchenObject'i ayarlar
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null )
        {
            OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
        }
    }

    // KitchenObject'i döner
    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    // KitchenObject referansýný temizler (null yapar)
    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    // Counter'da KitchenObject olup olmadýðýný kontrol eder
    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
