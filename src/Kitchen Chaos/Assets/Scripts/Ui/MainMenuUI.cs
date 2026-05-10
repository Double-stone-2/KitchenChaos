using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]  private Button playButton;
    [SerializeField]  private Button quitButton;
    [SerializeField]  private Button settingsButton;

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.SceneName.GameScene);
        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        Time.timeScale = 1f;
        settingsButton.onClick.AddListener(() =>
        {
           OptionsUI.Instance.Show();
        });
    }

}
