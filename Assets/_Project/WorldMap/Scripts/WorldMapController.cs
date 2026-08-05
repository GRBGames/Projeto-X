using UnityEngine;
using UnityEngine.UI;

public class WorldMapController : MonoBehaviour
{
    [Header("Botões das fases em ordem")]
    [SerializeField] private Button[] stageButtons;

    [Header("Progressão temporária")]
    [SerializeField, Min(0)] private int highestUnlockedStage = 0;

    private void Start()
    {
        UpdateStageButtons();
    }

    public void SelectStage(string stageId)
    {
        Debug.Log($"Fase selecionada: {stageId}");
    }

    public void CompleteStageForTest(int completedStageIndex)
    {
        // Impede que uma fase antiga libere novas fases repetidamente.
        if (completedStageIndex != highestUnlockedStage)
        {
            Debug.Log(
                $"A fase de índice {completedStageIndex} já foi concluída " +
                "ou não é a fase atual."
            );

            return;
        }

        // Verifica se a última fase já foi alcançada.
        if (highestUnlockedStage >= stageButtons.Length - 1)
        {
            Debug.Log("Campanha concluída. Todas as fases estão liberadas.");
            return;
        }

        highestUnlockedStage++;

        UpdateStageButtons();

        Debug.Log($"Nova fase liberada. Índice atual: {highestUnlockedStage}");
    }

    private void UpdateStageButtons()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            stageButtons[i].interactable = i <= highestUnlockedStage;
        }
    }
}