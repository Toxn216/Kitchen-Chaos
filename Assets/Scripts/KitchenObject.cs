using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;//��� ������ �������� ����� ��� (��� � �����)

    private IKitchenObjectParent kitchenObjectParent;

    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }
    public void SetKitchenObjectParent(IKitchenObjectParent KitchenObjectParent)
    {
        if (this.kitchenObjectParent != null)//���������� ������ �������� ���� �� ����
        {
            this.kitchenObjectParent.ClearKitchenObject();//���� ��������
        }

        this.kitchenObjectParent = KitchenObjectParent;//������ ����� �������� �������� ������� ����� �������

        if (KitchenObjectParent.HasKitchenObject())//��������� ���� ��� ��� ������ ������ ��������, �� ������ ������
        {
            Debug.LogError("IKitchenObjectParent already has a kitchenobject!");
        }

        KitchenObjectParent.SetKitchenObject(this);//��������� ������ ��������

        //transform.parent = KitchenObjectParent.GetKitchenObjectFollowTransform();
        //transform.localPosition = Vector3.zero;
    }
    public IKitchenObjectParent GetKitchenObjectParent()
    {
        return kitchenObjectParent;
    }

    public void DestroySelf()//���������� ��� ������ ��� � ��������� �� ��������
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
