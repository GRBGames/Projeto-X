using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class StoryController : MonoBehaviour
{
    [Header("Imagens")]
    [SerializeField] private Image storyBackground;
    [SerializeField] private Image lumiPortrait;
    [SerializeField] private Sprite[] pageSprites = new Sprite[5];

    [Header("Textos")]
    [SerializeField] private TMP_Text narratorName;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text continueLabel;

    [Header("Botões")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button skipButton;

    [Header("Indicadores")]
    [SerializeField] private Image[] pageDots = new Image[5];
    [SerializeField] private Color activeDotColor =
        new Color32(217, 168, 62, 255);
    [SerializeField] private Color inactiveDotColor =
        new Color32(75, 64, 94, 255);

    [Header("Efeito de escrita")]
    [SerializeField] private float characterDelay = 0.025f;

    [Header("Deslizar")]
    [SerializeField] private float minimumSwipeDistance = 120f;

    [Header("Próxima cena")]
    [SerializeField] private string worldMapSceneName = "WorldMap";

    private readonly string[] dialogues =
    {
        "Olá! Eu sou Lumi. Eu preferia conhecê-lo em um dia menos... apocalíptico.",
        "Durante séculos, o Núcleo Primordial manteve os quatro elementos em equilíbrio.",

        "Até que um antigo mago da Ordem invadiu o castelo e quebrou o Núcleo.",
        "Agora, corrompido por uma força proibida, ele é conhecido apenas como O Vazio.",

        "Ele tomou o Coração Primordial e entregou os quatro cristais aos antigos guardiões.",
        "Corrompidos por seu poder, eles passaram a proteger os caminhos até o Castelo do Vazio.",

        "Os maiores magos tentaram recuperar os cristais, mas nenhum conseguia suportar mais de um elemento.",
        "Então o Núcleo revelou algo inesperado: você, Lyren.",

        "Sua magia não pertence ao fogo, ao gelo, à planta ou à pedra. Ela pode se unir a todos eles.",
        "Recupere os quatro cristais, atravesse o castelo e impeça o Eclipse Elemental."
    };

    private int currentDialogueIndex;
    private Coroutine typingCoroutine;
    private bool isTyping;

    private Vector2 swipeStartPosition;

    private void Update()
{
    if (Touchscreen.current != null)
    {
        HandleTouchSwipe();
    }
    else if (Mouse.current != null)
    {
        HandleMouseSwipe();
    }
}

private void HandleTouchSwipe()
{
    var touch = Touchscreen.current.primaryTouch;

    if (touch.press.wasPressedThisFrame)
    {
        swipeStartPosition = touch.position.ReadValue();
    }

    if (touch.press.wasReleasedThisFrame)
    {
        TrySwipe(touch.position.ReadValue());
    }
}

private void HandleMouseSwipe()
{
    if (Mouse.current.leftButton.wasPressedThisFrame)
    {
        swipeStartPosition = Mouse.current.position.ReadValue();
    }

    if (Mouse.current.leftButton.wasReleasedThisFrame)
    {
        TrySwipe(Mouse.current.position.ReadValue());
    }
}

private void TrySwipe(Vector2 endPosition)
{
    float horizontalDistance = endPosition.x - swipeStartPosition.x;

    if (Mathf.Abs(horizontalDistance) < minimumSwipeDistance)
    {
        return;
    }

    if (horizontalDistance < 0f)
    {
        AdvanceDialogue();
    }
    else
    {
        PreviousDialogue();
    }
}

private void PreviousDialogue()
{
    if (isTyping)
    {
        CompleteTyping();
        return;
    }

    if (currentDialogueIndex <= 0)
    {
        return;
    }

    currentDialogueIndex--;
    ShowCurrentDialogue();
}

    private void Start()
    {
        narratorName.text = "LUMI";

        continueButton.onClick.AddListener(AdvanceDialogue);
        skipButton.onClick.AddListener(SkipStory);

        ShowCurrentDialogue();
    }

    private void OnDestroy()
    {
        continueButton.onClick.RemoveListener(AdvanceDialogue);
        skipButton.onClick.RemoveListener(SkipStory);
    }

    public void AdvanceDialogue()
    {
        if (isTyping)
        {
            CompleteTyping();
            return;
        }

        if (currentDialogueIndex >= dialogues.Length - 1)
        {
            OpenWorldMap();
            return;
        }

        currentDialogueIndex++;
        ShowCurrentDialogue();
    }

    public void SkipStory()
    {
        OpenWorldMap();
    }

    private void ShowCurrentDialogue()
    {
        int currentPage = currentDialogueIndex / 2;

        UpdateBackground(currentPage);
        UpdateProgressDots(currentPage);

        continueLabel.text =
            currentDialogueIndex == dialogues.Length - 1
            ? "INICIAR JORNADA"
            : "CONTINUAR";

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine =
            StartCoroutine(TypeDialogue(dialogues[currentDialogueIndex]));
    }

    private IEnumerator TypeDialogue(string text)
    {
        isTyping = true;

        dialogueText.text = text;
        dialogueText.maxVisibleCharacters = 0;
        dialogueText.ForceMeshUpdate();

        int totalCharacters = dialogueText.textInfo.characterCount;

        for (int i = 0; i <= totalCharacters; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(characterDelay);
        }

        dialogueText.maxVisibleCharacters = int.MaxValue;
        isTyping = false;
        typingCoroutine = null;
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.maxVisibleCharacters = int.MaxValue;
        isTyping = false;
    }

    private void UpdateBackground(int pageIndex)
    {
        if (pageSprites == null || pageIndex >= pageSprites.Length)
        {
            return;
        }

        if (pageSprites[pageIndex] != null)
        {
            storyBackground.sprite = pageSprites[pageIndex];
        }
    }

    private void UpdateProgressDots(int pageIndex)
    {
        for (int i = 0; i < pageDots.Length; i++)
        {
            if (pageDots[i] != null)
            {
                pageDots[i].color =
                    i == pageIndex ? activeDotColor : inactiveDotColor;
            }
        }
    }

    private void OpenWorldMap()
    {
        if (Application.CanStreamedLevelBeLoaded(worldMapSceneName))
        {
            SceneManager.LoadScene(worldMapSceneName);
        }
        else
        {
            Debug.LogWarning(
                $"A cena '{worldMapSceneName}' ainda não foi adicionada ao Build Profile."
            );
        }
    }
}
