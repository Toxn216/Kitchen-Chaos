using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;//все китчен обджексы будут тут (как я понил)

    private IKitchenObjectParent kitchenObjectParent;

    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }
    public void SetKitchenObjectParent(IKitchenObjectParent KitchenObjectParent)
    {
        if (this.kitchenObjectParent != null)//Назаначаем нового родителя если не ноль
        {
            this.kitchenObjectParent.ClearKitchenObject();//новій родитель
        }

        this.kitchenObjectParent = KitchenObjectParent;//новому клиар каунтеру назначем прошлый клиер каунтер

        if (KitchenObjectParent.HasKitchenObject())//сообщение если уже был создан китчен обджектс, на всякий случай
        {
            Debug.LogError("IKitchenObjectParent already has a kitchenobject!");
        }

        KitchenObjectParent.SetKitchenObject(this);//Назначили китчен обджектс

        //transform.parent = KitchenObjectParent.GetKitchenObjectFollowTransform();
        //transform.localPosition = Vector3.zero;
    }
    public IKitchenObjectParent GetKitchenObjectParent()
    {
        return kitchenObjectParent;
    }

    public void DestroySelf()//уничтожаем как обьект так и очишаемся от родителя
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

    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        KitchenGameMultiplayer.Instanse.SpawnKitchenObject(kitchenObjectSO,kitchenObjectParent);       
    }
}
