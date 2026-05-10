using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;
    [SerializeField] private Image timerFillImage;

    [SerializeField] private Color colourFull = Color.green;
    [SerializeField] private Color colourWarning = Color.yellow;
    [SerializeField] private Color colourCritical = Color.red;

    [SerializeField] [Range(0f, 1f)] private float warningThreshold  = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float criticalThreshold = 0.25f;

    private WaitingRecipe trackedRecipe;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (trackedRecipe == null) return;

        float t = trackedRecipe.TimeNormalised;
        timerFillImage.fillAmount = t;
        timerFillImage.color      = t > warningThreshold  ? colourFull
                                  : t > criticalThreshold ? colourWarning
                                                          : colourCritical;
    }

    public void SetWaitingRecipe(WaitingRecipe waitingRecipe)
    {
        trackedRecipe      = waitingRecipe;
        recipeNameText.text = waitingRecipe.RecipeSO.recipeName;

        foreach (Transform child in iconContainer)
        {
            if (child == iconTemplate) 
            continue;
            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in waitingRecipe.RecipeSO.kitchenObjectSOList)
        {
            Transform icon = Instantiate(iconTemplate, iconContainer);
            icon.gameObject.SetActive(true);
            icon.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
        }
    }
}