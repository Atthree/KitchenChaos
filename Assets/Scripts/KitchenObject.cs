using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    // Bu KitchenObject'in sahip olduðu ScriptableObject, nesne bilgilerini içerir
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    // Bu nesnenin þu anda hangi ClearCounter'a baðlý olduðunu tutar
    private IKitchenObjectParents kitchenObjectParent;

    // KitchenObjectSO'yu dönen bir getter metodu
    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    // Bu KitchenObject'in hangi ClearCounter'da olduðunu ayarlar
    public void SetKitchenObjectParent(IKitchenObjectParents kitchenObjectParent)
    {
        if (this.kitchenObjectParent != null)
        {
            // Mevcut Counter'dan çýk
            this.kitchenObjectParent.ClearKitchenObject();
        }

        // Yeni Counter'ý ayarla
        this.kitchenObjectParent = kitchenObjectParent;

        // Eðer clearCounter null deðilse ve orada zaten bir KitchenObject varsa hata ver
        if (kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError("IKitchenObjectParent already has a KitchenObject!");            
        }

            kitchenObjectParent.SetKitchenObject(this);
            transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
            transform.localPosition = Vector3.zero;
        
    }

    // Bu KitchenObject'in hangi ClearCounter'da olduðunu döner
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
