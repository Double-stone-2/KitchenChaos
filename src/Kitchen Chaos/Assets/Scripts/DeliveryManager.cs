using System;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    public event EventHandler OnRecipeExpired;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;
    [SerializeField] private float spawnTimerMax = 4f;
    [SerializeField] private float spawnTimerMin = 1.5f;
    [SerializeField] private float difficultyRampRate = 0.05f;
    [SerializeField] private int   waitingRecipesMax  = 4;
    [SerializeField] private float recipeTimeLimit    = 30f;

    private readonly List<WaitingRecipe> waitingRecipes = new();
    private float spawnRecipeTimer;
    private float currentSpawnTimerMax;
    private int   successfulRecipesAmount;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentSpawnTimerMax = spawnTimerMax;
        spawnRecipeTimer     = currentSpawnTimerMax;
    }

    private void Update()
    {
        if (!KitchenGameManager.Instance.IsGamePlaying() && waitingRecipes.Count < waitingRecipesMax)
        {
            return;
        }

        TickSpawnTimer();
        TickRecipeTimers();
    }

    private void TickSpawnTimer()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer > 0f)
        {
            return;
        }

        spawnRecipeTimer = currentSpawnTimerMax;

        if (waitingRecipes.Count >= waitingRecipesMax)
        {
            return;
        }

        RecipeSO recipe = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];

        waitingRecipes.Add(new WaitingRecipe(recipe, recipeTimeLimit));
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void TickRecipeTimers()
    {
        // Iterate backwards so RemoveAt doesn't skip entries
        for (int i = waitingRecipes.Count - 1; i >= 0; i--)
        {
            if (!waitingRecipes[i].Tick(Time.deltaTime))
            continue;


            waitingRecipes.RemoveAt(i);
            OnRecipeExpired?.Invoke(this, EventArgs.Empty);
        }
    }

    private bool PlateMatchesRecipe(PlateKitchenObject plate, RecipeSO recipe)
    {
        if (recipe.kitchenObjectSOList.Count != plate.GetKitchenObjectSOList().Count)
            return false;

        foreach (KitchenObjectSO required in recipe.kitchenObjectSOList)
        {
            bool found = false;
            foreach (KitchenObjectSO onPlate in plate.GetKitchenObjectSOList())
            {
                if (onPlate != required)
                continue;

                found = true;
                break;
            }
            if (!found)
            return false;
            
        }
        return true;
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipes.Count; i++)
        {
            if (!PlateMatchesRecipe(plateKitchenObject, waitingRecipes[i].RecipeSO)) continue;

            successfulRecipesAmount++;
            waitingRecipes.RemoveAt(i);

            // Tighten spawn interval on every successful delivery
            currentSpawnTimerMax = Mathf.Max(spawnTimerMin, currentSpawnTimerMax - difficultyRampRate);

            OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
            OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public IReadOnlyList<WaitingRecipe> GetWaitingRecipes() => waitingRecipes;
    public int GetSuccessfulRecipesAmount() => successfulRecipesAmount;
}