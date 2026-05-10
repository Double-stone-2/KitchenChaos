using UnityEngine;
using TMPro;

public class GamePlayingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject[] lifeIcons; 

    private void Start()
    {
        KitchenGameManager.Instance.OnLivesChanged += OnLivesChanged;
        KitchenGameManager.Instance.OnScoreChanged += OnScoreChanged;

        RefreshLives();
        RefreshScore();
    }

    private void OnLivesChanged(object sender, System.EventArgs e)  => RefreshLives();
    private void OnScoreChanged(object sender, System.EventArgs e)  => RefreshScore();

    private void RefreshLives()
    {
        int lives = KitchenGameManager.Instance.Lives;
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].SetActive(i < lives);
        }
    }

    private void RefreshScore()
    {
        scoreText.text = $"Score: {KitchenGameManager.Instance.Score}";
        highScoreText.text = $"Best: {KitchenGameManager.Instance.HighScore}";
    }
}