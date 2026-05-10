using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{
    private void Start()
    {
        CuttingCounter.ResetStaticData();
        BaseCounter.ResetStaticData();
        TrashCounter.ResetStaticData();
    }


}
