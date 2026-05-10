using UnityEngine;
using System;

public class KitchenGameManager : MonoBehaviour
{
    public static KitchenGameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    public event EventHandler OnLivesChanged;
    public event EventHandler OnScoreChanged;

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    private const string HighScoreKey = "EndlessHighScore";
    private const int StartingLives = 5;
    private const int StreakForBonusLife = 3;
    private int deliveryStreak;

    //[SerializeField] private float waitingToStartTimer = 1f;
    [SerializeField] private float countdownToStartTimer = 3f;

    private State state;
    private bool  isGamePaused;
    private int lives;
    private int score;
    public int Lives => lives;
    public int Score => score;
    public int HighScore => PlayerPrefs.GetInt(HighScoreKey, 0);

    private void Awake()
    {
        Instance = this;
        state = State.WaitingToStart;
        lives = StartingLives;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += OnPauseActionInput;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        DeliveryManager.Instance.OnRecipeFailed += OnRecipeFailed;
        DeliveryManager.Instance.OnRecipeSuccess += OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeExpired += OnRecipeExpired;
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (state == State.WaitingToStart)
        {
            state = State.CountdownToStart;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:

                break;

            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f)
                {
                    lives = StartingLives;
                    score = 0;
                    // Reset Streak
                    deliveryStreak = 0;
                    state = State.GamePlaying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;

            case State.GamePlaying:
                // no win condition in endless mode 
                // just wait until the player loses all lives 
                break;

            case State.GameOver:
                break;
        }
    }

    private void OnPauseActionInput(object sender, EventArgs e) => TogglePauseGame();

    private void OnRecipeFailed(object sender, EventArgs e)
    {
        if (state != State.GamePlaying) 
        return;
        deliveryStreak = 0;
        LoseLife();
    }

    private void OnRecipeSuccess(object sender, EventArgs e)
    {
        if (state != State.GamePlaying)
        return;
        deliveryStreak++;
        AddScore(1);

        if (deliveryStreak >= StreakForBonusLife)
        {
            deliveryStreak = 0;
            GainLife();
        }
    }

    private void OnRecipeExpired(object sender, EventArgs e)
    {
        if (state != State.GamePlaying) return;
        deliveryStreak = 0;
        LoseLife();
    }

    private void GainLife()
{
    lives++;
    OnLivesChanged?.Invoke(this, EventArgs.Empty);
}

    private void LoseLife()
    {
        lives = Mathf.Max(0, lives - 1);
        OnLivesChanged?.Invoke(this, EventArgs.Empty);

        if (lives <= 0) TriggerGameOver();
    }

    private void AddScore(int amount)
    {
        score += amount;
        OnScoreChanged?.Invoke(this, EventArgs.Empty);

        if (score > PlayerPrefs.GetInt(HighScoreKey, 0))
        {
            PlayerPrefs.SetInt(HighScoreKey, score);
            PlayerPrefs.Save();
        }
    }

    private void TriggerGameOver()
    {
        state = State.GameOver;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }
    public bool IsGamePlaying() => state == State.GamePlaying;
    public bool IsCountdownToStartActive() => state == State.CountdownToStart;
    public bool IsGameOver() => state == State.GameOver;
    public float GetCountdownToStartTimer() => countdownToStartTimer;

    public void TogglePauseGame()
    {
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0f : 1f;

        if (isGamePaused) OnGamePaused?.Invoke(this, EventArgs.Empty);
        else OnGameUnpaused?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        DeliveryManager.Instance.OnRecipeFailed  -= OnRecipeFailed;
        DeliveryManager.Instance.OnRecipeSuccess -= OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeExpired -= OnRecipeExpired;
    }

}