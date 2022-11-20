using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using RTLTMPro;
using TMPro;
[DefaultExecutionOrder(-100)]
public class EnterButton : MonoBehaviour
{
    private Button _button;
    private Image _image;
    public TextMeshProUGUI text;
    public Color normalColor;
    public string normalText = "أدخل";

    public Color incorrectWordColor;
    public string incorrectText = "كلمة خاطئة";

    public Color inactiveColor;
    public string inactiveText = "كلمة قصيرة";

    private ColorBlock _block;
    // Start is called before the first frame update
    void Awake()
    {
        _block = ColorBlock.defaultColorBlock;
        _block.normalColor = normalColor;
        _block.disabledColor = inactiveColor;
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Start()
    {
        SetInteractable(false);
    }

    public void SetInteractable(bool interactable)
    {
        _button.interactable = interactable;
        _image.color = interactable ? normalColor : inactiveColor;
        text.text = interactable ? normalText : inactiveText;
    }

    public void SetIncorrectWord(bool incorrectWord)
    {
        if (incorrectWord)
        {
            text.text = incorrectText;
            _image.color = incorrectWordColor;
        }
        else
        {
            text.text = normalText;
            _image.color = normalColor;
        }
        //_button.interactable = !incorrectWord;
    }
}
