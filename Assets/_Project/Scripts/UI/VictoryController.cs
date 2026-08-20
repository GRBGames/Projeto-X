using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryController : MonoBehaviour
{
    private const string MainMenuSceneName = "MainMenu";
    private const string WorldMapSceneName = "WorldMap";

    [Header("Referências")]
    [SerializeField]
    private PhaseController phaseController;

    [SerializeField]
    private BossEncounterController encounterController;

    [Header("Vitória do chefe")]
    [SerializeField]
    private GameObject victoryPanel;

    [SerializeField]
    private TMP_Text bossRewardText;

    [SerializeField]
    private Image bossRewardImage;

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

    [Header("Narrativas por região")]
    [SerializeField]
    private BossNarrativeConfig[] narrativeConfigs =
    new BossNarrativeConfig[1];

    [Header("Memória do Vazio")]
    [SerializeField]
    private GameObject bossMemoryPanel;

    [SerializeField]
    private TMP_Text bossMemoryText;

    [SerializeField]
    private Image bossMemoryImage;  

    [SerializeField]
    private Button bossMemoryContinueButton;

    [SerializeField]
    private TMP_Text bossMemoryContinueButtonText;

    private bool initialized;
    private bool victoryShown;
    private int currentMemoryPage;
    private BossNarrativeConfig activeNarrativeConfig;  

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
    activeNarrativeConfig =
        FindNarrativeConfig(
            phaseController.CurrentRegion
        );

    if (activeNarrativeConfig == null)
    {
        Debug.LogError(
            "[VictoryController] Não existe narrativa " +
            $"configurada para {phaseController.CurrentRegion}."
        );

        return;
    }

    bossRewardText.text =
        activeNarrativeConfig.RewardMessage;

    bossRewardImage.sprite =
        activeNarrativeConfig.RewardIllustration;

    ShowVictoryPanel(
        victoryPanel,
        $"Chefe de {phaseController.CurrentRegion} derrotado."
    );
}

    private BossNarrativeConfig FindNarrativeConfig(
    StageRegion region
)
{
    if (narrativeConfigs == null)
    {
        return null;
    }

    for (int i = 0; i < narrativeConfigs.Length; i++)
    {
        BossNarrativeConfig narrativeConfig =
            narrativeConfigs[i];

        if (narrativeConfig != null &&
            narrativeConfig.Region == region)
        {
            return narrativeConfig;
        }
    }

    return null;
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

    if (activeNarrativeConfig == null)
    {
        Debug.LogError(
            "[VictoryController] A narrativa ativa não foi definida."
        );

        return;
    }

    victoryPanel.SetActive(false);
    bossMemoryPanel.SetActive(true);

    currentMemoryPage = 0;
    DisplayCurrentMemoryPage();

    Debug.Log(
        $"[VictoryController] Memória de " +
        $"{activeNarrativeConfig.Region} iniciada."
    );
}

    private void AdvanceBossMemory()
{
    if (activeNarrativeConfig == null)
    {
        return;
    }

    if (currentMemoryPage <
        activeNarrativeConfig.MemoryPageCount - 1)
    {
        currentMemoryPage++;
        DisplayCurrentMemoryPage();
        return;
    }

    ReturnToWorldMap();
}

    private void DisplayCurrentMemoryPage()
{
    if (activeNarrativeConfig == null)
    {
        return;
    }

    bool pageFound =
        activeNarrativeConfig.TryGetMemoryPage(
            currentMemoryPage,
            out BossMemoryPage memoryPage
        );

    if (!pageFound)
    {
        Debug.LogError(
            "[VictoryController] A página de memória " +
            $"{currentMemoryPage} é inválida."
        );

        return;
    }

    bossMemoryText.text =
        memoryPage.FormattedDialogue;

    bossMemoryImage.sprite =
        memoryPage.Illustration;

    bool isLastPage =
        currentMemoryPage ==
        activeNarrativeConfig.MemoryPageCount - 1;

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
        bossRewardText,
        "Boss Reward Text"
    ))
        {
            return false;
        }

        if (!ValidateReference(
        bossRewardImage,
        "Boss Reward Image"
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
        bossMemoryImage,
        "Boss Memory Image"
    ))
        {
    return false;
        }

        if (narrativeConfigs == null ||
    narrativeConfigs.Length == 0)
{
    Debug.LogError(
        "[VictoryController] Nenhuma narrativa regional " +
        "foi configurada."
    );

    return false;
}

for (int i = 0; i < narrativeConfigs.Length; i++)
{
    BossNarrativeConfig narrativeConfig =
        narrativeConfigs[i];

    if (!ValidateReference(
            narrativeConfig,
            $"Narrative Config {i + 1}"
        ))
    {
        return false;
    }

    if (!narrativeConfig.IsValid())
    {
        Debug.LogError(
            "[VictoryController] A narrativa da região " +
            $"{narrativeConfig.Region} está incompleta."
        );

        return false;
    }

    for (int j = i + 1;
         j < narrativeConfigs.Length;
         j++)
    {
        BossNarrativeConfig otherConfig =
            narrativeConfigs[j];

        if (otherConfig != null &&
            otherConfig.Region ==
            narrativeConfig.Region)
        {
            Debug.LogError(
                "[VictoryController] A região " +
                $"{narrativeConfig.Region} possui " +
                "mais de uma narrativa configurada."
            );

            return false;
        }
    }
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