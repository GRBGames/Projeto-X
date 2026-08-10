using UnityEngine;

public static class GameProgress
{
    private const string HighestUnlockedStageKey =
        "HighestUnlockedStage";

    private const string HasProgressKey =
        "HasGameProgress";

    private const string CampaignCompletedKey =
        "CampaignCompleted";

    public static int HighestUnlockedStage
    {
        get
        {
            return PlayerPrefs.GetInt(
                HighestUnlockedStageKey,
                0
            );
        }
    }

    public static bool HasProgress
    {
        get
        {
            return PlayerPrefs.GetInt(
                HasProgressKey,
                0
            ) == 1;
        }
    }

    public static bool CampaignCompleted
    {
        get
        {
            return PlayerPrefs.GetInt(
                CampaignCompletedKey,
                0
            ) == 1;
        }
    }

    public static bool CompleteStage(
        int completedStageIndex,
        int totalStageCount
    )
    {
        if (totalStageCount <= 0)
        {
            Debug.LogError(
                "Não existem fases cadastradas."
            );

            return false;
        }

        int highestUnlockedStage = Mathf.Clamp(
            HighestUnlockedStage,
            0,
            totalStageCount - 1
        );

        if (completedStageIndex != highestUnlockedStage)
        {
            Debug.Log(
                $"A fase de índice {completedStageIndex} já foi concluída " +
                "ou não é a fase atual."
            );

            return false;
        }

        PlayerPrefs.SetInt(HasProgressKey, 1);

        if (highestUnlockedStage < totalStageCount - 1)
        {
            int nextStageIndex = highestUnlockedStage + 1;

            PlayerPrefs.SetInt(
                HighestUnlockedStageKey,
                nextStageIndex
            );

            Debug.Log(
                $"Nova fase liberada. Índice atual: {nextStageIndex}"
            );
        }
        else
        {
            PlayerPrefs.SetInt(CampaignCompletedKey, 1);

            Debug.Log(
                "Campanha concluída. Todas as fases estão liberadas."
            );
        }

        PlayerPrefs.Save();
        return true;
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(HighestUnlockedStageKey);
        PlayerPrefs.DeleteKey(HasProgressKey);
        PlayerPrefs.DeleteKey(CampaignCompletedKey);
        PlayerPrefs.Save();

        Debug.Log("Progresso apagado.");
    }
}