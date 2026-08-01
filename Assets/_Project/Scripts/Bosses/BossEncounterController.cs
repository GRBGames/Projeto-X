using System;
using UnityEngine;

public class BossEncounterController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField]
    private PhaseController phaseController;

    [SerializeField]
    private GameObject bossObject;

    [SerializeField]
    private BossHealth bossHealth;

    public event Action BossBattleStarted;
    public event Action BossBattleCompleted;

    public bool IsBattleActive { get; private set; }

    private PlayerBarrier playerBarrier;
    private bool initialized;

    private void Awake()
    {
        if (bossObject != null)
        {
            bossObject.SetActive(false);
        }
    }

    private void Start()
    {
        playerBarrier = PlayerBarrier.Instance;

        if (!ValidateSetup())
        {
            enabled = false;
            return;
        }

        phaseController.BossRequested += StartBossBattle;
        bossHealth.BossDefeated += HandleBossDefeated;
        playerBarrier.BarrierBroken += HandleGameOver;

        initialized = true;
    }

    public void StartBossBattle()
    {
        if (!initialized ||
            IsBattleActive ||
            phaseController.IsBlocked)
        {
            return;
        }

        bossObject.SetActive(true);
        IsBattleActive = true;

        Debug.Log(
            $"[BossEncounterController] " +
            $"{bossObject.name} entrou na batalha."
        );

        BossBattleStarted?.Invoke();
    }

    private void HandleBossDefeated()
    {
        if (!IsBattleActive)
        {
            return;
        }

        IsBattleActive = false;

        Debug.Log(
            "[BossEncounterController] Boss derrotado. " +
            "Batalha concluída."
        );

        BossBattleCompleted?.Invoke();
    }

    private void HandleGameOver()
    {
        if (!IsBattleActive)
        {
            return;
        }

        IsBattleActive = false;
        bossObject.SetActive(false);

        Debug.Log(
            "[BossEncounterController] " +
            "Batalha interrompida pelo Game Over."
        );
    }

    private bool ValidateSetup()
    {
        if (phaseController == null)
        {
            Debug.LogError(
                "[BossEncounterController] " +
                "PhaseController não foi atribuído."
            );

            return false;
        }

        if (bossObject == null)
        {
            Debug.LogError(
                "[BossEncounterController] " +
                "Boss Object não foi atribuído."
            );

            return false;
        }

        if (bossHealth == null)
        {
            Debug.LogError(
                "[BossEncounterController] " +
                "BossHealth não foi atribuído."
            );

            return false;
        }

        if (playerBarrier == null)
        {
            Debug.LogError(
                "[BossEncounterController] " +
                "PlayerBarrier não foi encontrado."
            );

            return false;
        }

        return true;
    }

    private void OnDestroy()
    {
        if (!initialized)
        {
            return;
        }

        phaseController.BossRequested -= StartBossBattle;
        bossHealth.BossDefeated -= HandleBossDefeated;
        playerBarrier.BarrierBroken -= HandleGameOver;
    }
}