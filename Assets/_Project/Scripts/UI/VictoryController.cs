using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryController : MonoBehaviour
{
    private const string MainMenuSceneName = "MainMenu";
    private const string WorldMapSceneName = "WorldMap";

    [Header("Referências")]
    [SerializeField]
    private BossEncounterController encounterController;

    [SerializeField]
    private GameObject victoryPanel;

    [Header("Botões")]
    [SerializeField]
    private Button returnToWorldMapButton;

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

        encounterController.BossBattleCompleted += ShowVictory;

        returnToWorldMapButton.onClick.AddListener(
            ReturnToWorldMap
        );

        returnToMenuButton.onClick.AddListener(
            ReturnToMainMenu
        );

        ConfigureReturnButtons();

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
            "[VictoryController] Vitória! Chefe derrotado."
        );
    }

    private void ConfigureReturnButtons()
    {
        bool enteredFromWorldMap =
            StageSelectionData.HasSelection;

        returnToWorldMapButton.gameObject.SetActive(
            enteredFromWorldMap
        );

        returnToMenuButton.gameObject.SetActive(
            !enteredFromWorldMap
        );
    }

    private void ReturnToWorldMap()
    {
        LoadSceneIfAvailable(WorldMapSceneName);
    }

    private void ReturnToMainMenu()
    {
        LoadSceneIfAvailable(MainMenuSceneName);
    }

    private void LoadSceneIfAvailable(string sceneName)
    {
        Time.timeScale = 1f;

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning(
                $"A cena '{sceneName}' não foi adicionada " +
                "ao Build Profile."
            );
        }
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

        if (returnToWorldMapButton == null)
        {
            Debug.LogError(
                "[VictoryController] " +
                "Return To World Map Button não foi atribuído."
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

        encounterController.BossBattleCompleted -= ShowVictory;

        returnToWorldMapButton.onClick.RemoveListener(
            ReturnToWorldMap
        );

        returnToMenuButton.onClick.RemoveListener(
            ReturnToMainMenu
        );
    }
}