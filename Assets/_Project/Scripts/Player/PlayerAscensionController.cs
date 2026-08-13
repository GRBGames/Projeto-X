using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerWeapon))]
public class PlayerAscensionController : MonoBehaviour
{
    [Header("Desbloqueio")]
    [SerializeField]
    private bool fireUnlocked;

    [Header("Ascensão de Fogo")]
    [SerializeField]
    [Min(0.1f)]
    private float fireDuration = 10f;

    [SerializeField]
    [Min(0f)]
    private float fireCooldown = 5f;

    public event Action<DamageElement> ElementChanged;

    public event Action<bool>
        FireAvailabilityChanged;

    public event Action<bool>
        FireUnlockChanged;

    public DamageElement CurrentElement
    {
        get;
        private set;
    } = DamageElement.Neutral;

    public bool IsAscended =>
        CurrentElement != DamageElement.Neutral;

    public bool IsFireUnlocked =>
        fireUnlocked;

    public bool IsFireOnCooldown
    {
        get;
        private set;
    }

    public bool CanActivateFire =>
        IsFireUnlocked &&
        !IsAscended &&
        !IsFireOnCooldown;

    private PlayerWeapon playerWeapon;
    private Coroutine fireAscensionRoutine;

    private void Awake()
    {
        playerWeapon =
            GetComponent<PlayerWeapon>();

        if (playerWeapon == null)
        {
            Debug.LogError(
                "[PlayerAscensionController] " +
                "PlayerWeapon não foi encontrado."
            );

            enabled = false;
        }

        if (StageSelectionData.HasSelection)
        {
            fireUnlocked =
            GameProgress.IsFireUnlocked;

            Debug.Log(
                "[PlayerAscensionController] " +
                $"Desbloqueio de Fogo carregado: {fireUnlocked}."
            );
        }
        else
        {
            Debug.Log(
                "[PlayerAscensionController] Game aberto diretamente. " +
                 "Mantendo a configuração de teste do Inspector."
            );
        }
    }

    public void UnlockFire()
    {
        if (fireUnlocked)
        {
            return;
        }

        fireUnlocked = true;

        FireUnlockChanged?.Invoke(true);

        FireAvailabilityChanged?.Invoke(
            CanActivateFire
        );

        Debug.Log(
            "[PlayerAscensionController] " +
            "Ascensão de Fogo desbloqueada."
        );
    }

    public void ActivateFire()
    {
        if (!IsFireUnlocked)
        {
            Debug.Log(
                "[PlayerAscensionController] " +
                "A Ascensão de Fogo ainda não foi desbloqueada."
            );

            return;
        }

        if (!CanActivateFire)
        {
            Debug.Log(
                "[PlayerAscensionController] " +
                "A Ascensão de Fogo não está disponível."
            );

            return;
        }

        FireAvailabilityChanged?.Invoke(false);

        fireAscensionRoutine = StartCoroutine(
            FireAscensionRoutine()
        );
    }

    public void ReturnToNeutral()
    {
        if (fireAscensionRoutine != null)
        {
            StopCoroutine(fireAscensionRoutine);
            fireAscensionRoutine = null;
        }

        IsFireOnCooldown = false;

        SetElement(DamageElement.Neutral);

        FireAvailabilityChanged?.Invoke(
            CanActivateFire
        );
    }

    private IEnumerator FireAscensionRoutine()
    {
        SetElement(DamageElement.Fire);

        // O especial acontece apenas uma vez,
        // no início de cada ativação.
        playerWeapon.FireTripleBurst();

        Debug.Log(
            "[PlayerAscensionController] " +
            $"Ascensão de Fogo ativa por {fireDuration} segundos."
        );

        yield return new WaitForSeconds(
            fireDuration
        );

        IsFireOnCooldown =
            fireCooldown > 0f;

        SetElement(DamageElement.Neutral);

        if (!IsFireOnCooldown)
        {
            fireAscensionRoutine = null;

            FireAvailabilityChanged?.Invoke(
                CanActivateFire
            );

            yield break;
        }

        Debug.Log(
            "[PlayerAscensionController] " +
            $"Cooldown de Fogo iniciado: {fireCooldown} segundos."
        );

        yield return new WaitForSeconds(
            fireCooldown
        );

        IsFireOnCooldown = false;
        fireAscensionRoutine = null;

        FireAvailabilityChanged?.Invoke(
            CanActivateFire
        );

        Debug.Log(
            "[PlayerAscensionController] " +
            "Ascensão de Fogo disponível novamente."
        );
    }

    private void SetElement(
        DamageElement newElement
    )
    {
        if (CurrentElement == newElement)
        {
            return;
        }

        CurrentElement = newElement;

        Debug.Log(
            "[PlayerAscensionController] " +
            $"Elemento ativo: {CurrentElement}."
        );

        ElementChanged?.Invoke(CurrentElement);
    }

    private void OnDisable()
    {
        if (fireAscensionRoutine != null)
        {
            StopCoroutine(fireAscensionRoutine);
            fireAscensionRoutine = null;
        }

        IsFireOnCooldown = false;
        SetElement(DamageElement.Neutral);
    }
}