using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    // Bu KitchenObject'in sahip oldu�u ScriptableObject, nesne bilgilerini i�erir
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    // Bu nesnenin �u anda hangi ClearCounter'a ba�l� oldu�unu tutar
    private IKitchenObjectParents kitchenObjectParent;

    // KitchenObjectSO'yu d�nen bir getter metodu
    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    // Bu KitchenObject'in hangi ClearCounter'da oldu�unu ayarlar
    public void SetKitchenObjectParent(IKitchenObjectParents kitchenObjectParent)
    {
        if (this.kitchenObjectParent != null)
        {
            // Mevcut Counter'dan ��k
            this.kitchenObjectParent.ClearKitchenObject();
        }

        // Yeni Counter'� ayarla
        this.kitchenObjectParent = kitchenObjectParent;

        // E�er clearCounter null de�ilse ve orada zaten bir KitchenObject varsa hata ver
        if (kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError("IKitchenObjectParent already has a KitchenObject!");            
        }

            kitchenObjectParent.SetKitchenObject(this);
            transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
            transform.localPosition = Vector3.zero;
        
    }

    // Bu KitchenObject'in hangi ClearCounter'da oldu�unu d�ner
    public IKitchenObjectParents GetKitchenObjectParent()
    {
        return kitchenObjectParent;
    }

    public void DestroySelf()
    {
        kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);

    }
    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if(this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else
        {
            plateKitchenObject = null;
            return false;
        }
    }

    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO,IKitchenObjectParents kitchenObjectParent)
    {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);

        return kitchenObject;
    }
}
