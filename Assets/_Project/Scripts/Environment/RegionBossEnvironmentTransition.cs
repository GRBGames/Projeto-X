using System.Collections;
using UnityEngine;

public class RegionBossEnvironmentTransition : MonoBehaviour
{
    [Header("Controle da fase")]
    [SerializeField] private PhaseController phaseController;

    [Header("Região")]
    [SerializeField] private StageRegion targetRegion;

    [Header("Ambientes")]
    [SerializeField] private GameObject scrollingEnvironment;
    [SerializeField] private GameObject bossEnvironment;

    private void OnEnable()
    {
        if (phaseController != null)
        {
            phaseController.BossRequested += HandleBossRequested;
        }
    }

    private void OnDisable()
    {
        if (phaseController != null)
        {
            phaseController.BossRequested -= HandleBossRequested;
        }
    }

    private IEnumerator Start()
    {
        if (phaseController == null)
        {
            yield break;
        }

        // O PhaseController define a região após iniciar a fase.
        while (phaseController.CurrentPhaseNumber == 0)
        {
            yield return null;
        }

        bool isCurrentRegion =
            phaseController.CurrentRegion == targetRegion;

        if (scrollingEnvironment != null)
        {
            scrollingEnvironment.SetActive(isCurrentRegion);
        }

        if (bossEnvironment != null)
        {
            bossEnvironment.SetActive(false);
        }
    }

    private void HandleBossRequested()
    {
        if (phaseController == null ||
            phaseController.CurrentRegion != targetRegion)
        {
            return;
        }

        if (scrollingEnvironment != null)
        {
            scrollingEnvironment.SetActive(false);
        }

        if (bossEnvironment != null)
        {
            bossEnvironment.SetActive(true);
        }

        Debug.Log(
            $"[RegionBossEnvironmentTransition] " +
            $"Arena do chefe de {targetRegion} ativada."
        );
    }
}
