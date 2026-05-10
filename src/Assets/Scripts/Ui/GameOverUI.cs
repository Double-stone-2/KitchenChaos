using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipesDeliveredText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += OnStateChanged;
        Hide();
    }

    private void OnStateChanged(object sender, System.EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGameOver()) { Hide(); return; }

        recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();
        scoreText.text = $"Score: {KitchenGameManager.Instance.Score}";
        highScoreText.text = $"Best: {KitchenGameManager.Instance.HighScore}";
        Show();
    }

    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}