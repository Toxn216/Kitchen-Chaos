using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawn;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    public static DeliveryManager Instance { get; private set; }


    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer = 4f;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;
    private int succsessFullRecipesAmount;

    private void Awake()
    {
        Instance = this;

        waitingRecipeSOList = new List<RecipeSO>();
    }
    private void Update()
    {
        if(!IsServer)
        {
            return;
        }
        spawnRecipeTimer -= Time.deltaTime;
        if(spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if(KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax )
            {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);

  

                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);           
            }
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];

        waitingRecipeSOList.Add(waitingRecipeSO);

        OnRecipeSpawn?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetkitchenObjectSOList().Count)
            {
                //ѕолучаем список ингридиентов
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKItchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    //цикл всех ингридиентов в рецепте
                    bool ingridientFound = false;
                    foreach (KitchenObjectSO plateKItchenObjectSO in plateKitchenObject.GetkitchenObjectSOList())
                    {
                        //цикл всех ингридиентов в тарелке
                        if (plateKItchenObjectSO == recipeKItchenObjectSO)
                        {
                            //игридиенты совпадают
                            ingridientFound = true;
                            break;
                        }
                    }
                    if (!ingridientFound)
                    {
                        //этот рецепт игридиентов не был найден на тарелочке
                        plateContentsMatchesRecipe = false;
                    }
                }
                if (plateContentsMatchesRecipe)
                {
                    //»грок правильно доставил рецепт
                    DeliveryCorrectRecipeServerRpc(i);
                    return;
                }
            }
        }
        //ничего не найдено!
        //игрок не правильно доставил рецепт!!!
        DeliveryIncorrectRecipeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliveryIncorrectRecipeServerRpc()
    {
        DeliveryIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliveryIncorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliveryCorrectRecipeServerRpc(int waitingRecipeSoListIndex)
    {
        DeliveryCorrectRecipeClientRpc(waitingRecipeSoListIndex);
    }

    [ClientRpc]
    private void DeliveryCorrectRecipeClientRpc(int waitingRecipeSoListIndex)
    {
        succsessFullRecipesAmount++;

        waitingRecipeSOList.RemoveAt(waitingRecipeSoListIndex);

        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccsessFullRecipesAmount()
    {
        return succsessFullRecipesAmount;
    }
}
