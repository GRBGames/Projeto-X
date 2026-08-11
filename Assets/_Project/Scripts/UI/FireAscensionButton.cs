using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class FireAscensionButton : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField]
    private PlayerAscensionController
        ascensionController;

    [SerializeField]
    private GameObject elementIcon;

    private Button fireButton;

    private void Awake()
    {
        fireButton =
            GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (ascensionController == null)
        {
            return;
        }

        ascensionController.FireAvailabilityChanged +=
            HandleFireAvailabilityChanged;

        ascensionController.FireUnlockChanged +=
            HandleFireUnlockChanged;
    }

    private void Start()
    {
        if (!ValidateSetup())
        {
            enabled = false;
            return;
        }

        UpdateUnlockState(
            ascensionController.IsFireUnlocked
        );

        UpdateButtonState(
            ascensionController.CanActivateFire
        );
    }

    private void HandleFireAvailabilityChanged(
        bool isAvailable
    )
    {
        UpdateButtonState(isAvailable);
    }

    private void HandleFireUnlockChanged(
        bool isUnlocked
    )
    {
        UpdateUnlockState(isUnlocked);

        UpdateButtonState(
            ascensionController.CanActivateFire
        );
    }

    private void UpdateUnlockState(
        bool isUnlocked
    )
    {
        if (elementIcon == null)
        {
            return;
        }

        elementIcon.SetActive(isUnlocked);
    }

    private void UpdateButtonState(
        bool isAvailable
    )
    {
        if (fireButton == null)
        {
            return;
        }

        fireButton.interactable =
            isAvailable;
    }

    private bool ValidateSetup()
    {
        if (fireButton == null)
        {
            Debug.LogError(
                "[FireAscensionButton] " +
                "O componente Button não foi encontrado."
            );

            return false;
        }

        if (ascensionController == null)
        {
            Debug.LogError(
                "[FireAscensionButton] " +
                "Player Ascension Controller não foi atribuído."
            );

            return false;
        }

        if (elementIcon == null)
        {
            Debug.LogError(
                "[FireAscensionButton] " +
                "Element Icon não foi atribuído."
            );

            return false;
        }

        return true;
    }

    private void OnDisable()
    {
        if (ascensionController == null)
        {
            return;
        }

        ascensionController.FireAvailabilityChanged -=
            HandleFireAvailabilityChanged;

        ascensionController.FireUnlockChanged -=
            HandleFireUnlockChanged;
    }
}