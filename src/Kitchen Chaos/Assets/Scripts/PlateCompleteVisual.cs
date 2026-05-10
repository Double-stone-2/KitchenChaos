using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitcheObjectSO_GameObject
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;

    }
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitcheObjectSO_GameObject> kitchenObjectSOGameOBjectList;


    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
        foreach (KitcheObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameOBjectList)
        {
        
            kitchenObjectSOGameObject.gameObject.SetActive(false);
        
        }
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        foreach (KitcheObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameOBjectList)
        {
            if (kitchenObjectSOGameObject.kitchenObjectSO == e.kitchenObjectSO)
            {
                kitchenObjectSOGameObject.gameObject.SetActive(true);
            }
        }
    }
}
