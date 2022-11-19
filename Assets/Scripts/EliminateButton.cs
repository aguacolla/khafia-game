using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EliminateButton : MonoBehaviour
{
    public TextMeshProUGUI countText;
    private Button button;
    private WordGuessManager wordGuessManager;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    private Dictionary<string, Button> keyboard;
    public GameObject arrow;
    public Ease ease;
    public float duration;
    public List<string> eliminatedLetters;

    private bool limitReached;

    public event System.Action onInputFinish;

    void Awake()
    {
        button = GetComponent<Button>();
        eliminatedLetters = new List<string>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetCounter();
        wordGuessManager = GameManager.Instance.wordGuessManager;
        keyboard = wordGuessManager.KeyboardButtons;
        GameManager.Instance.OnNewWord += ResetButton;
        GameManager.Instance.OnTextChanged += SetCounter;
    }

    public void ResetButton()
    {
        limitReached = false;
        SetCounter();
    }

    public void SetCounter()
    {
        int endValue = GameManager.Instance.EliminationsAvailable;
        countText.DOText(endValue.ToString(), 0.25f);
        button.GetComponent<Image>().sprite = (endValue == 0 || limitReached) ? inactiveSprite : activeSprite;
    }

    void SetText()
    {
        countText.text = GameManager.Instance.EliminationsAvailable.ToString();
    }

    public void EliminateLetters(int numberOfLetters)
    {

        int count = keyboard.Count;
        int index = 0;
        List<string> keys = keyboard.Keys.ToList();

        if (GameManager.Instance.EliminationsAvailable >= 0 && limitReached)
        {
            NotificationsManager.Instance.SpawnMessage(0);
            return;
        }

        if ((!GameManager.Instance.devMode && GameManager.Instance.EliminationsAvailable <= 0))
        {
            //PopupManager.Instance.OpenPopup(3);
            PagesManager.Instance.FlipPage(2);
            GameManager.Instance.SwitchState("store");
            return;
        }

        if (wordGuessManager.EliminationCount + numberOfLetters + 5 >= keys.Count)
        {
            print("not enough letters " + wordGuessManager.EliminationCount + " " + keys.Count);
            return;
        }
        for (int i = 0; i < numberOfLetters; i++)
        {

            while (true)
            {
                index = Random.Range(0, count);
                if (!GameManager.Instance.CurrentWordSimplified.Contains(keys[index]) && !eliminatedLetters.Contains(keys[index]))
                {
                    if (keyboard[keys[index]].GetComponent<Image>().color == wordGuessManager.keyboardDefaultColor)
                    {
                        wordGuessManager.EliminationCount++;
                        eliminatedLetters.Add(keys[index]);
                        EliminateKey(keys[index]);
                        break;
                    }
                }
            }
        }
        GameManager.Instance.EliminationsAvailable--;
        GameManager.Instance.timesEliminationUsed++;

        SetCounter();
        if (GameManager.Instance.timesEliminationUsed >= GameManager.Instance.eliminationLimit)
        {
            limitReached = true;
            button.GetComponent<Image>().sprite = inactiveSprite;
        }
        onInputFinish?.Invoke();
    }

    private void EliminateKey(string key)
    {
        print(key);
        Sequence seq = DOTween.Sequence();
        GameObject arrow = Instantiate(this.arrow, transform);
        Vector2 diff = (keyboard[key].transform.position - transform.position).normalized;
        arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
        seq.Append(arrow.GetComponent<RectTransform>().DOMove(keyboard[key].transform.position, duration).SetEase(ease));
        seq.Append(keyboard[key].GetComponent<Image>().DOColor(wordGuessManager.notInWordColor, 0.25f));
        seq.Join(keyboard[key].GetComponentInChildren<TextMeshProUGUI>().DOColor(Color.white, 0.25f));
        seq.Join(arrow.GetComponent<RectTransform>().DOShakeRotation(0.08f, 50, 10, 10));
        seq.Join(keyboard[key].GetComponent<RectTransform>().DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.1f, 10, 10));
        seq.AppendInterval(0.25f);
        seq.Append(arrow.GetComponent<Image>().DOFade(0, 0.25f));
        seq.onComplete += () => Destroy(arrow);
    }
}
