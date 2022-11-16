using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
public class DailyRewardButton : MonoBehaviour
{
    public float duration = 2;
    public Color animationColor = Color.cyan;

    private void OnEnable()
    {
        GetComponent<Image>().DOColor(animationColor, duration).SetLoops(-1, LoopType.Yoyo);
    }
    private void Start()
    {
        GameManager.Instance.OnTextChanged += UpdateContent;
    }

    public void UpdateContent()
    {
        gameObject.SetActive(GameManager.Instance.DailyRewardsAvailable > 0);

    }
}