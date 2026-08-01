using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryController : MonoBehaviour
{
    private const string MainMenuSceneName =
        "MainMenu";

    [Header("Referências")]
    [SerializeField]
    private BossEncounterController encounterController;

    [SerializeField]
    private GameObject victoryPanel;

    [SerializeField]
    private Button returnToMenuButton;

    private bool initialized;
    private bool victoryShown;

    private void Awake()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }

    private void Start()
    {
        if (!ValidateSetup())
        {
            enabled = false;
            return;
        }

        encounterController.BossBattleCompleted +=
            ShowVictory;

        returnToMenuButton.onClick.AddListener(
            ReturnToMainMenu
        );

        initialized = true;
    }

    private void ShowVictory()
    {
        if (victoryShown)
        {
            return;
        }

        if (PlayerBarrier.Instance == null ||
            PlayerBarrier.Instance.IsDepleted)
        {
            return;
        }

        victoryShown = true;
        victoryPanel.SetActive(true);

        Time.timeScale = 0f;

        Debug.Log(
            "[VictoryController] Vitória! " +
            "Cristal de Fogo recuperado."
        );
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            MainMenuSceneName
        );
    }

    private bool ValidateSetup()
    {
        if (encounterController == null)
        {
            Debug.LogError(
                "[VictoryController] " +
                "Encounter Controller não foi atribuído."
            );

            return false;
        }

        if (victoryPanel == null)
        {
            Debug.LogError(
                "[VictoryController] " +
                "Victory Panel não foi atribuído."
            );

            return false;
        }

        if (returnToMenuButton == null)
        {
            Debug.LogError(
                "[VictoryController] " +
                "Return To Menu Button não foi atribuído."
            );

            return false;
        }

        return true;
    }

    private void OnDestroy()
    {
        if (!initialized)
        {
            return;
        }

        encounterController.BossBattleCompleted -=
            ShowVictory;

        returnToMenuButton.onClick.RemoveListener(
            ReturnToMainMenu
        );
    }
}