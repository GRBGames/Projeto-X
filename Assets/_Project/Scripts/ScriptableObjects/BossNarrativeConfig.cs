using System;
using UnityEngine;

[Serializable]
public class BossMemoryPage
{
    [Header("Diálogo")]
    [SerializeField]
    private string speakerName;

    [SerializeField, TextArea(3, 6)]
    private string dialogue;

    [Header("Ilustração")]
    [SerializeField]
    private Sprite illustration;

    public string SpeakerName => speakerName;
    public string Dialogue => dialogue;
    public Sprite Illustration => illustration;

    public string FormattedDialogue
    {
        get
        {
            return
                $"<b>{speakerName.ToUpperInvariant()}</b>\n\n" +
                dialogue;
        }
    }

    public bool IsValid()
    {
        return
            !string.IsNullOrWhiteSpace(speakerName) &&
            !string.IsNullOrWhiteSpace(dialogue) &&
            illustration != null;
    }
}

[CreateAssetMenu(
    fileName = "BossNarrativeConfig",
    menuName = "Elemental Ascension/Boss Narrative Config"
)]
public class BossNarrativeConfig : ScriptableObject
{
    [Header("Região")]
    [SerializeField]
    private StageRegion region = StageRegion.Fire;

    [Header("Recompensa")]
    [SerializeField]
    private string rewardMessage;

    [SerializeField]
    private Sprite rewardIllustration;

    [Header("Memória do Vazio")]
    [SerializeField]
    private BossMemoryPage[] memoryPages =
        new BossMemoryPage[3];

    public StageRegion Region => region;
    public string RewardMessage => rewardMessage;
    public Sprite RewardIllustration => rewardIllustration;
    public int MemoryPageCount => memoryPages.Length;

    public bool TryGetMemoryPage(
        int pageIndex,
        out BossMemoryPage memoryPage
    )
    {
        memoryPage = null;

        if (memoryPages == null ||
            pageIndex < 0 ||
            pageIndex >= memoryPages.Length)
        {
            return false;
        }

        memoryPage = memoryPages[pageIndex];

        return memoryPage != null &&
               memoryPage.IsValid();
    }

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(rewardMessage) ||
            rewardIllustration == null ||
            memoryPages == null ||
            memoryPages.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < memoryPages.Length; i++)
        {
            if (memoryPages[i] == null ||
                !memoryPages[i].IsValid())
            {
                return false;
            }
        }

        return true;
    }
}
