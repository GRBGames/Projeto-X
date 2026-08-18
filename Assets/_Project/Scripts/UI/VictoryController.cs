using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryController : MonoBehaviour
{
    private const string MainMenuSceneName = "MainMenu";
    private const string WorldMapSceneName = "WorldMap";

    private static readonly string[] FireMemoryPages =
    {
        "<b>LUMI</b>\n\n" +
        "A Fagulha carrega mais do que poder, Lyren. " +
        "Ela também guarda uma memória.",

        "<b>LUMI</b>\n\n" +
        "Antes de ser chamado de Vazio, ele foi um mago " +
        "da Ordem Arcana... um dos nossos.",

        "<b>LYREN</b>\n\n" +
        "Então ele conhece os segredos da Ordem. " +
        "Mas o que fez um de seus magos se voltar contra " +
        "tudo o que jurou proteger?"
    };

    [Header("Referências")]
    [SerializeField]
    private PhaseController phaseController;

    [SerializeField]
    private BossEncounterController encounterController;

    [Header("Vitória do chefe")]
    [SerializeField]
    private GameObject victoryPanel;

    [SerializeField]
    private Button returnToWorldMapButton;

    [SerializeField]
    private Button returnToMenuButton;

    [Header("Vitória das fases comuns")]
    [SerializeField]
    private GameObject stageVictoryPanel;

    [SerializeField]
    private Button stageReturnToWorldMapButton;

    [SerializeField]
    private TMP_Text stageResultText;

    [SerializeField]
    private Button stageReturnToMenuButton;

    [Header("Memória do Vazio")]
    [SerializeField]
    private GameObject bossMemoryPanel;

    [SerializeField]
    private TMP_Text bossMemoryText;

    [SerializeField]
    private Button bossMemoryContinueButton;

    [SerializeField]
    private TMP_Text bossMemoryContinueButtonText;

    private bool initialized;
    private bool victoryShown;
    private int currentMemoryPage;

    private void Awake()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        if (stageVictoryPanel != null)
        {
            stageVictoryPanel.SetActive(false);
        }

        if (bossMemoryPanel != null)
        {
            bossMemoryPanel.SetActive(false);
        }
    }

    private void Start()
    {
        if (!ValidateSetup())
        {
            enabled = false;
            return;
        }

        phaseController.PhaseFinished +=
            HandlePhaseFinished;

        encounterController.BossBattleCompleted +=
            ShowBossVictory;

        returnToWorldMapButton.onClick.AddListener(
            ShowBossMemory
        );

        stageReturnToWorldMapButton.onClick.AddListener(
            ReturnToWorldMap
        );

        stageReturnToMenuButton.onClick.AddListener(
            ReturnToMainMenu
        );

        bossMemoryContinueButton.onClick.AddListener(
            AdvanceBossMemory
        );

        ConfigureReturnButtons();

        initialized = true;
    }

    private void HandlePhaseFinished(
        int completedPhaseNumber
    )
    {
        if (completedPhaseNumber == 3)
        {
            return;
        }

        stageResultText.text =
            $"FASE {completedPhaseNumber} CONCLUÍDA";

        ShowVictoryPanel(
            stageVictoryPanel,
            $"Fase {completedPhaseNumber} concluída."
        );
    }

    private void ShowBossVictory()
    {
        ShowVictoryPanel(
            victoryPanel,
            "Chefe derrotado."
        );
    }

    private void ShowVictoryPanel(
        GameObject panel,
        string victoryMessage
    )
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
        panel.SetActive(true);

        Time.timeScale = 0f;

        Debug.Log(
            $"[VictoryController] Vitória! {victoryMessage}"
        );
    }

    private void ShowBossMemory()
    {
        if (!victoryShown)
        {
            return;
        }

        victoryPanel.SetActive(false);
        bossMemoryPanel.SetActive(true);

        currentMemoryPage = 0;
        DisplayCurrentMemoryPage();

        Debug.Log(
            "[VictoryController] Memória do Vazio iniciada."
        );
    }

    private void AdvanceBossMemory()
    {
        if (currentMemoryPage <
            FireMemoryPages.Length - 1)
        {
            currentMemoryPage++;
            DisplayCurrentMemoryPage();
            return;
        }

        ReturnToWorldMap();
    }

    private void DisplayCurrentMemoryPage()
    {
        bossMemoryText.text =
            FireMemoryPages[currentMemoryPage];

        bool isLastPage =
            currentMemoryPage ==
            FireMemoryPages.Length - 1;

        bossMemoryContinueButtonText.text =
            isLastPage
                ? "VOLTAR AO MAPA"
                : "CONTINUAR";
    }

    private void ConfigureReturnButtons()
    {
        returnToWorldMapButton.gameObject.SetActive(true);
        returnToMenuButton.gameObject.SetActive(false);

        stageReturnToWorldMapButton.gameObject.SetActive(true);
        stageReturnToMenuButton.gameObject.SetActive(true);
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
        if (!ValidateReference(
                phaseController,
                "Phase Controller"
            ))
        {
            return false;
        }

        if (!ValidateReference(
                encounterController,
                "Encounter Controller"
            ))
        {
            return false;
        }

        if (!ValidateReference(
                victoryPanel,
                "Victory Panel"
            ))
        {
            return false;
        }

        if (!ValidateReference(
                returnToWorldMapButton,
                "Return To World Map Button"
            ))
        {
            return false;
        }

        if (!ValidateReference(
                returnToMenuButton,
                "Return To Menu Button"
            ))
        {
            return false;
        }

        if (!ValidateReference(
                stageVictoryPanel,
                "Stage Victory Panel"
            ))
        {
            return false;
        }

        if (!ValidateReference(
                stageReturnToWorldMapButton,
                "Stage Return To World Map Button"
            ))
        {
            return false;
        }

        if (!ValidateReference(
                stageReturnToMenuButton,
                "Stage Return To Menu Button"
            ))
        {
            return false;
        }

        if (!ValidateReference(
                bossMemoryPanel,
                "Boss Memory Panel"
            ))
        {
            return false;
        }

        if (!ValidateReference(
                bossMemoryText,
                "Boss Memory Text"
            ))
        {
            return false;
        }

        if (!ValidateReference(
                bossMemoryContinueButton,
                "Boss Memory Continue Button"
            ))
        {
            return false;
        }

        if (!ValidateReference(
                bossMemoryContinueButtonText,
                "Boss Memory Continue Button Text"
            ))
        {
            return false;
        }

        return true;
    }

    private bool ValidateReference(
        Object reference,
        string referenceName
    )
    {
        if (reference != null)
        {
            return true;
        }

        Debug.LogError(
            $"[VictoryController] {referenceName} " +
            "não foi atribuído."
        );

        return false;
    }

    private void OnDestroy()
    {
        if (!initialized)
        {
            return;
        }

        phaseController.PhaseFinished -=
            HandlePhaseFinished;

        encounterController.BossBattleCompleted -=
            ShowBossVictory;

        returnToWorldMapButton.onClick.RemoveListener(
            ShowBossMemory
        );

        stageReturnToWorldMapButton.onClick.RemoveListener(
            ReturnToWorldMap
        );

        stageReturnToMenuButton.onClick.RemoveListener(
            ReturnToMainMenu
        );

        bossMemoryContinueButton.onClick.RemoveListener(
            AdvanceBossMemory
        );
    }
}