using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasProgressGameObject;
    [SerializeField] private Image BarImage;

    private IHasProgress hasProgress;

    private void Start()
    {
        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
        if (hasProgress == null)
        {
            Debug.LogError("Game Object" + hasProgressGameObject + " has no IHasProgress component!");
        }
        hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;
        BarImage.fillAmount = 0f;
        hide();
    }

    private void HasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        BarImage.fillAmount = e.progressNormalized;

        if (e.progressNormalized == 0f || e.progressNormalized == 1f)
        {
            hide();
        }
        else
        {
            show();
        }
    }

    private void show()
    {
        gameObject.SetActive(true);
    }   

    private void hide()
    {
        gameObject.SetActive(false);
    }
}
