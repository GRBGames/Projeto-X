using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldMapProgressBridge : MonoBehaviour
{
    private const string WorldMapSceneName = "WorldMap";
    private const int TotalStageCount = 15;

    [Header("Referências")]
    [SerializeField] private PhaseController phaseController;

    private bool isReturningToWorldMap;

    private void OnEnable()
    {
        if (phaseController != null)
        {
            phaseController.PhaseFinished +=
                HandlePhaseFinished;
        }
    }

    private void OnDisable()
    {
        if (phaseController != null)
        {
            phaseController.PhaseFinished -=
                HandlePhaseFinished;
        }
    }

    private void HandlePhaseFinished(
        int completedPhaseNumber
    )
    {
        if (!StageSelectionData.HasSelection)
        {
            Debug.Log(
                "[WorldMapProgressBridge] Game aberto diretamente. " +
                "O progresso do mapa não será alterado."
            );

            return;
        }

        if (completedPhaseNumber !=
            StageSelectionData.StageNumber)
        {
            Debug.LogWarning(
                "[WorldMapProgressBridge] A fase concluída não " +
                "corresponde à fase selecionada no WorldMap."
            );

            return;
        }

        if (completedPhaseNumber == 3)
        {
            Debug.Log(
                "[WorldMapProgressBridge] Ondas da fase 3 concluídas. " +
                "Aguardando a derrota do chefe."
            );

            return;
        }

        GameProgress.CompleteStage(
            StageSelectionData.GlobalStageIndex,
            TotalStageCount
        );

        if (!isReturningToWorldMap)
        {
            StartCoroutine(ReturnToWorldMap());
        }
    }

    private IEnumerator ReturnToWorldMap()
    {
        isReturningToWorldMap = true;

        // Aguarda o PhaseController terminar o evento atual.
        yield return null;

        if (Application.CanStreamedLevelBeLoaded(
                WorldMapSceneName
            ))
        {
            SceneManager.LoadScene(WorldMapSceneName);
        }
        else
        {
            isReturningToWorldMap = false;

            Debug.LogWarning(
                $"A cena '{WorldMapSceneName}' não foi adicionada " +
                "ao Build Profile."
            );
        }
    }
}