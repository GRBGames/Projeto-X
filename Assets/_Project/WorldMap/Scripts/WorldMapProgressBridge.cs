using UnityEngine;

public class WorldMapProgressBridge : MonoBehaviour
{
    private const int TotalStageCount = 15;

    [Header("Referências")]
    [SerializeField]
    private PhaseController phaseController;

    [SerializeField]
    private BossEncounterController bossEncounterController;

    private void OnEnable()
    {
        if (phaseController != null)
        {
            phaseController.PhaseFinished +=
                HandlePhaseFinished;
        }

        if (bossEncounterController != null)
        {
            bossEncounterController.BossBattleCompleted +=
                HandleBossBattleCompleted;
        }
    }

    private void OnDisable()
    {
        if (phaseController != null)
        {
            phaseController.PhaseFinished -=
                HandlePhaseFinished;
        }

        if (bossEncounterController != null)
        {
            bossEncounterController.BossBattleCompleted -=
                HandleBossBattleCompleted;
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

        CompleteSelectedStage();

        Debug.Log(
            "[WorldMapProgressBridge] Fase comum concluída. " +
            "Progresso salvo; aguardando o painel de vitória."
        );
    }

    private void HandleBossBattleCompleted()
    {
        if (!StageSelectionData.HasSelection)
        {
            Debug.Log(
                "[WorldMapProgressBridge] Boss derrotado em teste direto. " +
                "O progresso do mapa não será alterado."
            );

            return;
        }

        if (!StageSelectionData.IsBossStage)
        {
            Debug.LogWarning(
                "[WorldMapProgressBridge] Uma batalha de chefe terminou, " +
                "mas a fase selecionada não é uma fase de chefe."
            );

            return;
        }

        CompleteSelectedStage();

        GameProgress.UnlockAscension(
            StageSelectionData.Region
        );

        Debug.Log(
            "[WorldMapProgressBridge] Fase de chefe concluída. " +
            "Progresso e elemento salvos; " +
            "aguardando o botão do painel de vitória."
        );
    }

    private void CompleteSelectedStage()
    {
        GameProgress.CompleteStage(
            StageSelectionData.GlobalStageIndex,
            TotalStageCount
        );
    }
}