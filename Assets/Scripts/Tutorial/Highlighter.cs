using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class Highlighter : MonoBehaviour
{
    public static Highlighter instance;


    public float animationDur = 1;
    public Ease animationEase = Ease.InQuad;

    public event System.Action onClick;


    public bool hasFinishedAnimation { get; private set; }


    Image image;

    CanvasGroup canvasGroup;

    private void Awake()
    {
        instance = this;
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        gameObject.SetActive(false);
        GetComponent<Button>().onClick.AddListener(() =>
        {
            onClick?.Invoke();
        });
    }

    private void OnDisable()
    {
        hasFinishedAnimation = false;
        canvasGroup.alpha = 0;
    }
    private void OnEnable()
    {
        canvasGroup.DOFade(1, animationDur).SetEase(animationEase).OnComplete(() => hasFinishedAnimation = true);
    }

    public void DoHide()
    {
        canvasGroup.DOFade(0, animationDur).SetEase(animationEase).OnComplete(() => gameObject.SetActive(false));

    }
}