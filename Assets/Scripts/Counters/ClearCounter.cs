using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{


    [SerializeField] private KitchenObjectSO kitchenObjectSO;//подрубаем китчен обджектсы 
    
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //китчен обджект не сдеся
            if (player.HasKitchenObject())
            {
                //игрок несет что-либо
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                //игрок ничего не несет
            }

        }
        else
        {
            //Китчен обджект здеся
            if (player.HasKitchenObject())
            {
                //игрок несет что-то
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {                               
                    //игрок держит тарелочку
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                }
                else
                {
                    //Игрок не несет тарелочку ибо что то произошло(не захотелось например таскать тяжести)
                    if(GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        //каунтер(тумбочка) занята тарелочкой
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else
            {
                //игрок ничего не несет
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}
