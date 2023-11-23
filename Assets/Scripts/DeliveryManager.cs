using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawn;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    public static DeliveryManager Instance { get; private set; }


    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;
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
        spawnRecipeTimer -= Time.deltaTime;
        if(spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if(KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax )
            {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];

                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawn?.Invoke(this, EventArgs.Empty);
            }
        }
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
                    succsessFullRecipesAmount++;

                    waitingRecipeSOList.RemoveAt(i);

                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }
        //ничего не найдено!
        //игрок не правильно доставил рецепт!!!
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
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
