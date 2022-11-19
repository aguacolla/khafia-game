using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class TutorialHelper : MonoBehaviour
{

    public float animationDuration = 0.5f;
    public static TutorialHelper instance;


    public event System.Action onClick;


    public TextMeshProUGUI text;
    public Button button;

    CanvasGroup canvasGroup;


    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);

        button.onClick.AddListener(OnClick);
        canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
    public void SetText(string text)
    {
        this.text.text = "";
        this.text.DOText(text, animationDuration);
    }
    public void SetButtonEnabled(bool value)
    {
        button.gameObject.SetActive(value);
    }

    void OnClick()
    {
        onClick?.Invoke();
    }

    private void OnEnable()
    {
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutQuad);
    }

}