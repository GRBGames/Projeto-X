using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldMapController : MonoBehaviour
{
    private const string GameSceneName = "Game";

    [Header("Botões das fases em ordem")]
    [SerializeField] private Button[] stageButtons;

    private int highestUnlockedStage;

    private void Start()
    {
        LoadProgress();
        UpdateStageButtons();
    }

    public void SelectStage(string stageId)
    {
        bool stageSelected =
            StageSelectionData.TrySelectStage(stageId);

        if (!stageSelected)
        {
            return;
        }

        LoadGameScene();
    }

    public void CompleteStageForTest(int completedStageIndex)
    {
        bool stageCompleted = GameProgress.CompleteStage(
            completedStageIndex,
            stageButtons.Length
        );

        if (!stageCompleted)
        {
            return;
        }

        LoadProgress();
        UpdateStageButtons();
    }

    private void LoadProgress()
    {
        highestUnlockedStage =
            GameProgress.HighestUnlockedStage;
    }

    private void UpdateStageButtons()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            if (stageButtons[i] == null)
            {
                Debug.LogWarning(
                    $"O botão do índice {i} não foi configurado."
                );

                continue;
            }

            stageButtons[i].interactable =
                i <= highestUnlockedStage;
        }
    }

    private void LoadGameScene()
    {
        if (Application.CanStreamedLevelBeLoaded(GameSceneName))
        {
            SceneManager.LoadScene(GameSceneName);
        }
        else
        {
            Debug.LogWarning(
                $"A cena '{GameSceneName}' não foi adicionada " +
                "ao Build Profile."
            );
        }
    }

    [ContextMenu("TESTE - Concluir fase atual")]
    private void CompleteCurrentStageForTest()
    {
        CompleteStageForTest(highestUnlockedStage);
    }

    [ContextMenu("TESTE - Apagar progresso")]
    private void ResetProgressForTest()
    {
        GameProgress.ResetProgress();

        LoadProgress();
        UpdateStageButtons();
    }
}