using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryController : MonoBehaviour
{
    [Header("Imagens")]
    [SerializeField] private Image storyBackground;
    [SerializeField] private Image lumiPortrait;
    [SerializeField] private Sprite[] pageSprites = new Sprite[5];

    [Header("Animação do Lumi")]
    [SerializeField] private Sprite lumiIdleSprite;
    [SerializeField] private Sprite lumiTalkingSprite;
    [SerializeField] private float lumiFrameDelay = 0.14f;

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
        // Cena 1 — O equilíbrio elemental
        "No centro do reino repousava o Coração Primordial, fonte do equilíbrio entre todos os elementos.",
        "Ao seu redor, os Cristais de Fogo, Gelo, Planta e Pedra mantinham o mundo em perfeita harmonia.",

        // Cena 2 — O roubo dos cristais
        "Mas um antigo mago da Ordem, consumido por uma magia proibida, invadiu o santuário e roubou os quatro cristais.",
        "O Coração permaneceu no templo, mas sem eles sua luz começou a desaparecer.",

        // Cena 3 — A corrupção dos guardiões
        "O Vazio utilizou uma fagulha de cada cristal para corromper quatro antigos protetores do mundo.",
        "Fortalecidos e dominados por sua magia, os guardiões perderam o controle e suas terras mergulharam no caos.",

        // Cena 4 — A escolha de Lyren
        "Quando o Eclipse Elemental começou, o Coração Primordial chamou por um jovem aprendiz chamado Lyren.",
        "Por não estar ligado a apenas um elemento, somente ele poderia dominar os quatro poderes necessários para enfrentar o Vazio.",

        // Cena 5 — O início da jornada
        "Liberte os guardiões da corrupção. Como recompensa, cada um lhe entregará sua fagulha elemental.",
        "Dominando os quatro elementos, poderá enfrentar o Vazio, recuperar os cristais e restaurar o equilíbrio do mundo."
    };

    private int currentDialogueIndex;
    private Coroutine typingCoroutine;
    private Coroutine lumiAnimationCoroutine;
    private bool isTyping;
    private Vector2 swipeStartPosition;

    private void Start()
    {
        narratorName.text = "LUMI";
        SetLumiIdle();

        continueButton.onClick.AddListener(AdvanceDialogue);
        skipButton.onClick.AddListener(SkipStory);

        ShowCurrentDialogue();
    }

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

    private void OnDestroy()
    {
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(AdvanceDialogue);
        }

        if (skipButton != null)
        {
            skipButton.onClick.RemoveListener(SkipStory);
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

        StopTypingAndLumiAnimation();
        typingCoroutine =
            StartCoroutine(TypeDialogue(dialogues[currentDialogueIndex]));
    }

    private IEnumerator TypeDialogue(string text)
    {
        isTyping = true;
        StartLumiAnimation();

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
        StopLumiAnimation();
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
        StopLumiAnimation();
    }

    private void StopTypingAndLumiAnimation()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;
        StopLumiAnimation();
    }

    private void StartLumiAnimation()
    {
        StopLumiAnimation();
        lumiAnimationCoroutine = StartCoroutine(AnimateLumiTalking());
    }

    private IEnumerator AnimateLumiTalking()
    {
        bool showTalkingSprite = true;

        while (isTyping)
        {
            Sprite nextSprite =
                showTalkingSprite ? lumiTalkingSprite : lumiIdleSprite;

            if (lumiPortrait != null && nextSprite != null)
            {
                lumiPortrait.sprite = nextSprite;
            }

            showTalkingSprite = !showTalkingSprite;

            yield return new WaitForSecondsRealtime(
                Mathf.Max(0.05f, lumiFrameDelay)
            );
        }
    }

    private void StopLumiAnimation()
    {
        if (lumiAnimationCoroutine != null)
        {
            StopCoroutine(lumiAnimationCoroutine);
            lumiAnimationCoroutine = null;
        }

        SetLumiIdle();
    }

    private void SetLumiIdle()
    {
        if (lumiPortrait != null && lumiIdleSprite != null)
        {
            lumiPortrait.sprite = lumiIdleSprite;
        }
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
