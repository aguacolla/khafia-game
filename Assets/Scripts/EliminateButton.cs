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
    private WordGuessManager wordGuessManager => GameManager.Instance.wordGuessManager;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    public Keyboard keyboard => Keyboard.instance;
    public GameObject arrow;
    public Ease ease;
    public float duration;
    public List<string> eliminatedLetters => wordGuessManager.state.eliminatedLetters;

    private bool limitReached => wordGuessManager.state.usedEliminations >= GameManager.Instance.eliminationLimit;

    public event System.Action onInputFinish;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetCounter();
        GameManager.Instance.OnNewWord += ResetButton;
        GameManager.Instance.OnTextChanged += SetCounter;
    }

    public void ResetButton()
    {
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
    public void DoEliminate()
    {
        EliminateLetters(GameManager.Instance.eliminateLetterCount);
    }

    public void EliminateLetters(int numberOfLetters)
    {

        List<string> keys = keyboard.GetLetterList();

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



        GameManager.Instance.EliminationsAvailable--;
        GameManager.Instance.timesEliminationUsed++;
        wordGuessManager.state.usedEliminations++;

        EliminateLettersInstant(numberOfLetters);


    }
    public void EliminateLettersInstant(int numberOfLetters)
    {
        int count = keyboard.keyCount;
        int index = 0;
        List<string> keys = keyboard.GetLetterList();

        var st = Random.state;
        Random.InitState(wordGuessManager.eliminationSeed);
        for (int i = 0; i < numberOfLetters; i++)
        {

            while (true)
            {
                index = Random.Range(0, count);
                if (!GameManager.Instance.CurrentWordSimplified.Contains(keys[index]) && !eliminatedLetters.Contains(keys[index]))
                {
                    if (keyboard.GetKeyImage(keys[index]).color == wordGuessManager.keyboardDefaultColor)
                    {
                        wordGuessManager.EliminationCount++;
                        eliminatedLetters.Add(keys[index]);
                        EliminateKey(keys[index]);
                        break;
                    }
                }
            }
        }
        Random.state = st;
        SetCounter();
        if (limitReached)
        {
            button.GetComponent<Image>().sprite = inactiveSprite;
        }
        onInputFinish?.Invoke();
    }

    private void EliminateKey(string letter)
    {
        print(letter);
        var key = keyboard.GetKeyImage(letter);
        Sequence seq = DOTween.Sequence();
        GameObject arrow = Instantiate(this.arrow, transform);
        Vector2 diff = (keyboard[letter].transform.position - transform.position).normalized;
        arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
        seq.Append(arrow.GetComponent<RectTransform>().DOMove(keyboard[letter].transform.position, duration).SetEase(ease));
        seq.Append(keyboard[letter].image.DOColor(wordGuessManager.notInWordColor, 0.25f));
        seq.Join(keyboard[letter].text.DOColor(Color.white, 0.25f));
        seq.Join(arrow.GetComponent<RectTransform>().DOShakeRotation(0.08f, 50, 10, 10));
        seq.Join(keyboard[letter].transform.GetComponent<RectTransform>().DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.1f, 10, 10));
        seq.AppendInterval(0.25f);
        seq.Append(arrow.GetComponent<Image>().DOFade(0, 0.25f));
        seq.onComplete += () => Destroy(arrow);
    }
}
