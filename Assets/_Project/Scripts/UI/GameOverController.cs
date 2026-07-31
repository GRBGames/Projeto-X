using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    [Header("Referências da interface")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        Time.timeScale = 1f;

        gameOverPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
    }

    private void Start()
    {
        if (PlayerBarrier.Instance == null)
        {
            Debug.LogError(
                "GameOverController não encontrou o PlayerBarrier."
            );

            return;
        }

        PlayerBarrier.Instance.BarrierBroken += ShowGameOver;
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    private void OnDestroy()
    {
        if (PlayerBarrier.Instance != null)
        {
            PlayerBarrier.Instance.BarrierBroken -= ShowGameOver;
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
        }
    }
}