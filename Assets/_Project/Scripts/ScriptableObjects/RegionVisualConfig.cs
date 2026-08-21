using UnityEngine;

[CreateAssetMenu(
    fileName = "RegionVisualConfig",
    menuName = "Elemental Ascension/Region Visual Config"
)]
public class RegionVisualConfig : ScriptableObject
{
    [Header("Região")]
    [SerializeField]
    private StageRegion region = StageRegion.Fire;

    [Header("Cores da interface")]
    [SerializeField]
    private Color accentColor = Color.white;

    [SerializeField]
    private Color buttonColor = Color.white;

    public StageRegion Region => region;
    public Color AccentColor => accentColor;
    public Color ButtonColor => buttonColor;
}