using System;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public static event EventHandler OnAnyDeliveryAttempted;

    public static new void ResetStaticData()
    {
        OnAnyDeliveryAttempted = null;
    }

    public static DeliveryCounter Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject()) return;
        if (!player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) return;

        OnAnyDeliveryAttempted?.Invoke(this, EventArgs.Empty);

        if (DeliveryManager.Instance != null)
            DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);

        player.GetKitchenObject().DestroySelf();
    }
}