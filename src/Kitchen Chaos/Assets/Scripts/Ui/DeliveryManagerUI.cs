using System;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    private void Awake()
    {
        recipeTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSpawned  += OnRecipeListChanged;
        DeliveryManager.Instance.OnRecipeCompleted += OnRecipeListChanged;
        DeliveryManager.Instance.OnRecipeExpired   += OnRecipeListChanged;
        UpdateVisual();
    }

    private void OnDestroy()
    {
        DeliveryManager.Instance.OnRecipeSpawned -= OnRecipeListChanged;
        DeliveryManager.Instance.OnRecipeCompleted -= OnRecipeListChanged;
        DeliveryManager.Instance.OnRecipeExpired -= OnRecipeListChanged;
    }

    private void OnRecipeListChanged(object sender, EventArgs e) => UpdateVisual();

    private void UpdateVisual()
    {
        foreach (Transform child in container)
        {
            if (child == recipeTemplate) 
            continue;
            Destroy(child.gameObject);
        }

        foreach (WaitingRecipe waitingRecipe in DeliveryManager.Instance.GetWaitingRecipes())
        {
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            recipeTransform.gameObject.SetActive(true);
            recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetWaitingRecipe(waitingRecipe);
        }
    }
}