using UnityEngine;

[CreateAssetMenu(
    fileName = "RegionPhaseConfig",
    menuName = "Elemental Ascension/Region Phase Config"
)]
public class RegionPhaseConfig : ScriptableObject
{
    private const int RequiredPhaseCount = 3;

    [Header("Região")]
    [SerializeField]
    private StageRegion region = StageRegion.Fire;

    [Header("Fases")]
    [SerializeField]
    private PhaseSpawnConfig[] phaseConfigs =
        new PhaseSpawnConfig[RequiredPhaseCount];

    public StageRegion Region => region;

    public bool TryGetPhaseConfig(
        int phaseNumber,
        out PhaseSpawnConfig phaseConfig
    )
    {
        phaseConfig = null;

        if (phaseConfigs == null ||
            phaseConfigs.Length != RequiredPhaseCount ||
            phaseNumber < 1 ||
            phaseNumber > RequiredPhaseCount)
        {
            return false;
        }

        phaseConfig = phaseConfigs[phaseNumber - 1];

        return phaseConfig != null;
    }

    public bool IsValid()
    {
        if (phaseConfigs == null ||
            phaseConfigs.Length != RequiredPhaseCount)
        {
            return false;
        }

        for (int i = 0; i < phaseConfigs.Length; i++)
        {
            if (phaseConfigs[i] == null)
            {
                return false;
            }
        }

        return true;
    }
}