using UnityEngine;

[CreateAssetMenu(fileName = "UIConfig", menuName = "Config/UI", order = 0)]
public class UIConfig : Config<UIConfig>
{
    public Sprite[] backgroundPatterns;
    public Color defaultColor;
    public Color FulldefaultColor;
    public Color outlineColor = new Color32(63, 63, 63, 255);
    [Header("Letter place colors")]
    public Color inPlaceColor = new Color32(83, 141, 78, 255);
    public Color inWordColor = new Color32(181, 159, 59, 255);
    public Color notInWordColor = new Color32(42, 42, 42, 255);
    [Header("Hint Colors")]
    public GameObject hintGlow;
    public Color hintColor;
    public Color hintTextColor;

    [Header("Word grid Settings")]
    public Sprite defaultWordImage;
    public Sprite wordImage;
    public Color gridLetterDefaultColor;
    public Color gridLetterCheckedColor;
    [Header("Keyboard Settings")]

    public Color keyboardDefaultColor = new Color(129, 131, 132, 255);
    public Color keyboardDefaultTextColor;


    [Header("Levels View")]
    public GameObject[] levelsSeason;
    public GameObject firstSeason;


}