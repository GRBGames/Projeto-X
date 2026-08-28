using UnityEngine;

public static class GameProgress
{
    private const string HighestUnlockedStageKey =
        "HighestUnlockedStage";

    private const string HasProgressKey =
        "HasGameProgress";

    private const string CampaignCompletedKey =
        "CampaignCompleted";

    private const string FireUnlockedKey =
        "FireAscensionUnlocked";

    private const string IceUnlockedKey =
        "IceAscensionUnlocked";

    private const string PlantUnlockedKey =
        "PlantAscensionUnlocked";

    private const string StoneUnlockedKey =
        "StoneAscensionUnlocked";

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

    public static bool IsFireUnlocked
    {
        get
        {
            return PlayerPrefs.GetInt(
                FireUnlockedKey,
                0
            ) == 1;
        }
    }

    public static bool IsIceUnlocked
{
        get
        {
            return PlayerPrefs.GetInt(
                IceUnlockedKey,
                0
            ) == 1;
        }
}

    public static bool IsPlantUnlocked
{
        get
        {
            return PlayerPrefs.GetInt(
                PlantUnlockedKey,
                0
            ) == 1;
        }
}

    public static bool IsStoneUnlocked
{
        get
        {
            return PlayerPrefs.GetInt(
                StoneUnlockedKey,
                0
            ) == 1;
        }
}

    public static bool IsAscensionUnlocked(
    StageRegion region
)
{
    if (!TryGetAscensionKey(
            region,
            out string ascensionKey
        ))
    {
        return false;
    }

    return PlayerPrefs.GetInt(
        ascensionKey,
        0
    ) == 1;
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

    public static void UnlockAscension(
    StageRegion region
)
{
    if (!TryGetAscensionKey(
            region,
            out string ascensionKey
        ))
    {
        Debug.Log(
            $"[GameProgress] A região {region} " +
            "não possui uma Ascensão para desbloquear."
        );

        return;
    }

        if (IsAscensionUnlocked(region))
    {
        return;
    }

    PlayerPrefs.SetInt(ascensionKey, 1);
    PlayerPrefs.Save();

    Debug.Log(
        $"[GameProgress] Ascensão de {region} " +
        "desbloqueada e salva."
    );
}

public static void UnlockFire()
{
    UnlockAscension(StageRegion.Fire);
}

private static bool TryGetAscensionKey(
    StageRegion region,
    out string ascensionKey
)
{
    switch (region)
    {
        case StageRegion.Fire:
            ascensionKey = FireUnlockedKey;
            return true;

        case StageRegion.Ice:
            ascensionKey = IceUnlockedKey;
            return true;

        case StageRegion.Plant:
            ascensionKey = PlantUnlockedKey;
            return true;

        case StageRegion.Stone:
            ascensionKey = StoneUnlockedKey;
            return true;

        default:
            ascensionKey = null;
            return false;
    }
}

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(HighestUnlockedStageKey);
        PlayerPrefs.DeleteKey(HasProgressKey);
        PlayerPrefs.DeleteKey(CampaignCompletedKey);
        PlayerPrefs.DeleteKey(FireUnlockedKey);
        PlayerPrefs.DeleteKey(IceUnlockedKey);
        PlayerPrefs.DeleteKey(PlantUnlockedKey);
        PlayerPrefs.DeleteKey(StoneUnlockedKey);
        PlayerPrefs.Save();

        Debug.Log("Progresso apagado.");
    }
}