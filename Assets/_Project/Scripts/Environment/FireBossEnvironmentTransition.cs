using UnityEngine;

public class FireBossEnvironmentTransition : MonoBehaviour
{
    [Header("Controle da fase")]
    [SerializeField] private PhaseController phaseController;

    [Header("Ambientes")]
    [SerializeField] private GameObject scrollingEnvironment;
    [SerializeField] private GameObject bossEnvironment;

    private void Awake()
    {
        ShowScrollingEnvironment();
    }

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

    private void HandleBossRequested()
    {
        if (phaseController == null)
        {
            return;
        }

        if (phaseController.CurrentRegion != StageRegion.Fire)
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
            "[FireBossEnvironmentTransition] Arena da Fênix ativada.");
    }

    private void ShowScrollingEnvironment()
    {
        if (scrollingEnvironment != null)
        {
            scrollingEnvironment.SetActive(true);
        }

        if (bossEnvironment != null)
        {
            bossEnvironment.SetActive(false);
        }
    }
}