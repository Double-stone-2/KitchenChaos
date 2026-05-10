using System;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    public event EventHandler<OnStepChangedEventArgs> OnStepChanged;
    public class OnStepChangedEventArgs : EventArgs
    {
        public TutorialStep step;
        public string instruction;
        public Transform targetTransform;
    }

    public event EventHandler OnTutorialComplete;

    [SerializeField] private PlatesCounter platesCounter;
    [SerializeField] private ClearCounter clearCounter;
    [SerializeField] private ContainerCounter ingredientContainer;
    [SerializeField] private CuttingCounter cuttingCounter;
    [SerializeField] private ContainerCounter breadCounter;
    [SerializeField] private DeliveryCounter deliveryCounter;

    private TutorialStep currentStep;
    private bool isActive;
    private bool chopComplete;

    public bool IsActive => isActive;
    private void Awake()
    {
        Instance = this;
        currentStep = TutorialStep.PickUpPlate;
    }

    private void Start()
    {
        cuttingCounter.OnProgressChanged += OnCuttingProgressChanged;
        DeliveryCounter.OnAnyDeliveryAttempted += OnDeliveryAttempted;

        isActive = true;
        SetStep(TutorialStep.PickUpPlate);
    }

    private void OnDestroy()
    {
        cuttingCounter.OnProgressChanged -= OnCuttingProgressChanged;
        DeliveryCounter.OnAnyDeliveryAttempted -= OnDeliveryAttempted;
    }

    private void Update()
    {
        if (!isActive) return;
        CheckStepCompletion();
    }
    private void CheckStepCompletion()
    {
        Player player = Player.Instance;

        switch (currentStep)
        {
            case TutorialStep.PickUpPlate:
                if (player.HasKitchenObject() && player.GetKitchenObject().TryGetPlate(out _))
                    AdvanceStep();
                break;

            case TutorialStep.PlacePlateOnClearCounter:
                if (!player.HasKitchenObject() && clearCounter.HasKitchenObject())
                    AdvanceStep();
                break;

            case TutorialStep.PickUpIngredient:
                if (player.HasKitchenObject() && !player.GetKitchenObject().TryGetPlate(out _))
                    AdvanceStep();
                break;

            case TutorialStep.PlaceIngredientOnCuttingCounter:
                if (!player.HasKitchenObject() && cuttingCounter.HasKitchenObject())
                    AdvanceStep();
                break;

            case TutorialStep.ChopIngredient:
                if (chopComplete) AdvanceStep();
                break;

            case TutorialStep.PickUpChoppedIngredient:
                if (player.HasKitchenObject() && !player.GetKitchenObject().TryGetPlate(out _) && !cuttingCounter.HasKitchenObject())
                    AdvanceStep();
                break;

            case TutorialStep.AddIngredientToPlate:
                if (!player.HasKitchenObject() && clearCounter.HasKitchenObject())
                    AdvanceStep();
                break;
           case TutorialStep.PickUpBread:
                if (player.HasKitchenObject() && !player.GetKitchenObject().TryGetPlate(out _))
                    AdvanceStep();
                break;
            case TutorialStep.AddBreadToPlate:
                 if (!player.HasKitchenObject() && clearCounter.HasKitchenObject())
                    AdvanceStep();
                break;
            case TutorialStep.PickUpPlateWithIngredient:
                if (player.HasKitchenObject() && player.GetKitchenObject().TryGetPlate(out _) && !clearCounter.HasKitchenObject())
                    AdvanceStep();
                break;

            case TutorialStep.DeliverPlate:
                // Handled by event.
                break;
        }
    }

    private void AdvanceStep() => SetStep(currentStep + 1);

    private void SetStep(TutorialStep step)
    {
        currentStep = step;
        chopComplete = false;

        if (step == TutorialStep.Complete)
        {
            isActive = false;
            OnTutorialComplete?.Invoke(this, EventArgs.Empty);
            Loader.Load(Loader.SceneName.GameScene);
            return;
        }

        OnStepChanged?.Invoke(this, new OnStepChangedEventArgs
        {
            step = step,
            instruction = GetInstruction(step),
            targetTransform = GetTargetTransform(step)
        });
    }
    private void OnCuttingProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        if (currentStep == TutorialStep.ChopIngredient && e.progressNormalized >= 1f)
            chopComplete = true;
    }

    private void OnDeliveryAttempted(object sender, EventArgs e)
    {
        if (currentStep == TutorialStep.DeliverPlate)
            SetStep(TutorialStep.Complete);
    }
    public bool CanInteract(BaseCounter counter)
    {
        if (!isActive || counter == null) 
        return true;

        switch (currentStep)
        {
            case TutorialStep.PickUpPlate: 
            return counter == platesCounter;

            case TutorialStep.PlacePlateOnClearCounter: 
            return counter == clearCounter;

            case TutorialStep.PickUpIngredient: 
            return counter == ingredientContainer;

            case TutorialStep.PlaceIngredientOnCuttingCounter: 
            return counter == cuttingCounter;

            case TutorialStep.ChopIngredient: 
            return false;

            case TutorialStep.PickUpChoppedIngredient: 
            return counter == cuttingCounter;

            case TutorialStep.AddIngredientToPlate: 
            return counter == clearCounter;

            case TutorialStep.PickUpBread:
            return counter == breadCounter;

            case TutorialStep.AddBreadToPlate:
            return counter == clearCounter;

            case TutorialStep.PickUpPlateWithIngredient: 
            return counter == clearCounter;

            case TutorialStep.DeliverPlate: 
            return counter == deliveryCounter;
            default: return false;
        }
    }

    public bool CanInteractAlternate(BaseCounter counter)
    {
        if (!isActive || counter == null) 
        return true;
        return currentStep == TutorialStep.ChopIngredient && counter == cuttingCounter;
    }

    public string GetCurrentInstruction() => GetInstruction(currentStep);
    public Transform GetCurrentTargetTransform() => GetTargetTransform(currentStep);

    private string GetInstruction(TutorialStep step)
    {
        string interactKey = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        string altInteractKey = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
        
        // Dynamic instructions with the inputs saved
        switch (step)
        {
            case TutorialStep.PickUpPlate:
            return $"Press [{interactKey}] to pick up a plate from the plate shelf. To Deliver a recipe you need to always start with a plate.";

            case TutorialStep.PlacePlateOnClearCounter: 
            return $"Press [{interactKey}] to place the plate on the empty counter.";

            case TutorialStep.PickUpIngredient: 
            return $"Press [{interactKey}] to pick up an ingredient from the container.";
            
            case TutorialStep.PlaceIngredientOnCuttingCounter: 
            return $"Press [{interactKey}] to place the ingredient on the cutting counter.";

            case TutorialStep.ChopIngredient: 
            return $"Chop the ingredient! Press [{altInteractKey}] repeatedly.";

            case TutorialStep.PickUpChoppedIngredient: 
            return $"Press [{interactKey}] to pick up the chopped ingredient.";

            case TutorialStep.AddIngredientToPlate: 
            return $"Press [{interactKey}] to add the ingredient to the plate on the counter.";

            case TutorialStep.PickUpBread:
            return $"Press [{interactKey}] to pick up a bread from the bread counter.";

            case TutorialStep.AddBreadToPlate:
            return $"Press [{interactKey}] to add the bread to the plate on the counter.";

            case TutorialStep.PickUpPlateWithIngredient: 
            return $"Press [{interactKey}] to pick up the completed plate.";

            case TutorialStep.DeliverPlate: 
            return $"Deliver the plate to the delivery counter and press [{interactKey}]!";

            default: return string.Empty;
        }
    }

    private Transform GetTargetTransform(TutorialStep step)
    {
        switch (step)
        {
            case TutorialStep.PickUpPlate: return platesCounter.transform;
            case TutorialStep.PlacePlateOnClearCounter: return clearCounter.transform;
            case TutorialStep.PickUpIngredient: return ingredientContainer.transform;
            case TutorialStep.PlaceIngredientOnCuttingCounter: return cuttingCounter.transform;
            case TutorialStep.ChopIngredient: return cuttingCounter.transform;
            case TutorialStep.PickUpChoppedIngredient: return cuttingCounter.transform;
            case TutorialStep.AddIngredientToPlate: return clearCounter.transform;
            case TutorialStep.PickUpBread: return breadCounter.transform;
            case TutorialStep.AddBreadToPlate: return clearCounter.transform;
            case TutorialStep.PickUpPlateWithIngredient:return clearCounter.transform;
            case TutorialStep.DeliverPlate: return deliveryCounter.transform;
            default: return null;
        }
    }
}