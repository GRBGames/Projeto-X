using System;
using UnityEngine;

public enum StageRegion
{
    Fire = 0,
    Ice = 1,
    Plant = 2,
    Stone = 3,
    Void = 4
}

public static class StageSelectionData
{

    public static bool HasSelection { get; private set; }
    
    public static string StageId { get; private set; } =
        "Fire_1";

    public static StageRegion Region { get; private set; } =
        StageRegion.Fire;

    public static int StageNumber { get; private set; } = 1;

    public static int GlobalStageIndex { get; private set; } = 0;

    public static bool IsBossStage
    {
        get
        {
            return StageNumber == 3;
        }
    }

    public static bool TrySelectStage(string stageId)
    {
        if (string.IsNullOrWhiteSpace(stageId))
        {
            Debug.LogError(
                "[StageSelectionData] O ID da fase está vazio."
            );

            return false;
        }

        string[] idParts = stageId.Split('_');

        if (idParts.Length != 2)
        {
            Debug.LogError(
                $"[StageSelectionData] ID inválido: {stageId}."
            );

            return false;
        }

        if (!Enum.TryParse(
                idParts[0],
                true,
                out StageRegion selectedRegion
            ))
        {
            Debug.LogError(
                $"[StageSelectionData] Região inválida: {idParts[0]}."
            );

            return false;
        }

        if (!int.TryParse(
                idParts[1],
                out int selectedStageNumber
            ))
        {
            Debug.LogError(
                $"[StageSelectionData] Número inválido: {idParts[1]}."
            );

            return false;
        }

        if (selectedStageNumber < 1 ||
            selectedStageNumber > 3)
        {
            Debug.LogError(
                "[StageSelectionData] A fase deve estar entre 1 e 3."
            );

            return false;
        }

        HasSelection = true;
        StageId = stageId;
        Region = selectedRegion;
        StageNumber = selectedStageNumber;

        GlobalStageIndex =
            ((int)selectedRegion * 3) +
            (selectedStageNumber - 1);

        Debug.Log(
            $"Fase preparada: {StageId} | " +
            $"Região: {Region} | " +
            $"Número: {StageNumber} | " +
            $"Chefe: {IsBossStage} | " +
            $"Índice global: {GlobalStageIndex}"
        );

        return true;
    }
}