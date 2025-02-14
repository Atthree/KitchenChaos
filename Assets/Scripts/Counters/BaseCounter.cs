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
    // Counter'�n �st�nde nesnelerin yerle�tirilece�i nokta (Transform)
    [SerializeField] private Transform counterTopPoint;

    // Bu counter'da �u anda bulunan KitchenObject referans�
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
        return counterTopPoint; // Bu, counter'�n �st�ndeki nokta
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

    // KitchenObject'i d�ner
    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    // KitchenObject referans�n� temizler (null yapar)
    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    // Counter'da KitchenObject olup olmad���n� kontrol eder
    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
