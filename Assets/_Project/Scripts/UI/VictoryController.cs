using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryController : MonoBehaviour
{
    private const string MainMenuSceneName =
        "MainMenu";

    private const string WorldMapSceneName =
        "WorldMap";

    [Header("Referências")]
    [SerializeField]
    private BossEncounterController encounterController;

    [SerializeField]
    private GameObject victoryPanel;

    [SerializeField]
    private Button returnToMenuButton;

    [SerializeField]
    private TMP_Text returnButtonLabel;

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
            ReturnAfterVictory
        );

        UpdateReturnButtonLabel();

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
            "Chefe derrotado."
        );
    }

    private void UpdateReturnButtonLabel()
{
    if (returnButtonLabel == null)
    {
        Debug.LogWarning(
            "[VictoryController] Texto do botão de retorno " +
            "não foi atribuído."
        );

        return;
    }

    returnButtonLabel.text =
        StageSelectionData.HasSelection
            ? "VOLTAR AO MAPA"
            : "VOLTAR AO MENU";
}   

    private void ReturnAfterVictory()
    {
        Time.timeScale = 1f;

        string destinationScene =
            StageSelectionData.HasSelection
                ? WorldMapSceneName
                : MainMenuSceneName;

        if (Application.CanStreamedLevelBeLoaded(
                destinationScene
            ))
        {
            SceneManager.LoadScene(destinationScene);
        }
        else
        {
            Debug.LogWarning(
                $"A cena '{destinationScene}' não foi adicionada " +
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

        if (returnToMenuButton == null)
        {
            Debug.LogError(
                "[VictoryController] " +
                "Return To Menu Button não foi atribuído."
            );

            return false;
        }

        if (returnButtonLabel == null)
{
    Debug.LogError(
        "[VictoryController] " +
        "Return Button Label não foi atribuído."
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
            ReturnAfterVictory
        );
    }
}