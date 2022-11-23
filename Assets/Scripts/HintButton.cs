using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class HintButton : MonoBehaviour
{
    public TextMeshProUGUI countText;
    private Button button;
    private WordGuessManager wordGuessManager => GameManager.Instance.wordGuessManager;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    private bool limitReached => wordGuessManager.state.usedHints >= GameManager.Instance.hintLimit;

    public event Action onInputFinish;

    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        SetCounter();
        //countText.text = GameManager.Instance.HintsAvailable.ToString();
        GameManager.Instance.OnNewWord += ResetButton;
        GameManager.Instance.OnTextChanged += SetCounter;
    }

    void SetText()
    {
        countText.text = GameManager.Instance.HintsAvailable.ToString();
    }


    public void ResetButton()
    {
        SetCounter();
    }

    public void SetCounter()
    {
        int endValue = GameManager.Instance.HintsAvailable;
        countText.DOText(endValue.ToString(), 0.25f);
        button.GetComponent<Image>().sprite = (endValue == 0 || limitReached) ? inactiveSprite : activeSprite;
    }

    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable;
    }

    public void ShowHint()
    {
        if (GameManager.Instance.HintsAvailable >= 0 && limitReached)
        {
            NotificationsManager.Instance.SpawnMessage(0);
            return;
        }

        if ((!GameManager.Instance.devMode && GameManager.Instance.HintsAvailable <= 0))
        {
            //PopupManager.Instance.OpenPopup(3);
            PagesManager.Instance.FlipPage(2);
            GameManager.Instance.SwitchState("store");
            return;
        }



        if (wordGuessManager.lettersHinted.Count <= 0)
        {
            return;
        }



        GameManager.Instance.HintsAvailable--;
        wordGuessManager.hintCalled = true;
        wordGuessManager.state.usedHints++;
        GameManager.Instance.timesHintUsed++;

        ShowHintInstant();
    }

    public void ShowHintInstant()
    {
        string word = GameManager.Instance.CurrentWordSimplified;
        Transform currentRow = wordGuessManager.wordGrid.GetChild(wordGuessManager.rowIndex);
        List<TextMeshProUGUI> letters = new List<TextMeshProUGUI>();

        for (int j = 0; j < wordGuessManager.wordLen; j++)
            letters.Add(currentRow.GetChild(j).GetComponentInChildren<TextMeshProUGUI>());

        var st = Random.state;
        Random.InitState(wordGuessManager.hintSeed);
        int i = Random.Range(0, wordGuessManager.lettersHinted.Count);
        Random.state = st;

        int index = wordGuessManager.lettersHinted[i];
        wordGuessManager.lettersHinted.RemoveAt(i);
        string hint = word[index].ToString();
        TextMeshProUGUI hintLetter;

        var targetIndex = index;
        if (letters[targetIndex].transform.parent.GetChild(1).childCount == 1)
        {
            letters[targetIndex].transform.parent.GetChild(1).GetChild(0).gameObject.SetActive(true);

            hintLetter = letters[targetIndex].transform.parent.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
            hintLetter.transform.position = transform.position;
        }
        else
        {
            hintLetter = Instantiate(letters[targetIndex].gameObject, transform.position, Quaternion.identity, letters[targetIndex].transform.parent.GetChild(1)).GetComponent<TextMeshProUGUI>();
            hintLetter.color = wordGuessManager.hintTextColor;
        }
        hintLetter.text = hint;
        Sequence seq = DOTween.Sequence();
        seq.Append(hintLetter.rectTransform.DOMove(letters[targetIndex].rectTransform.position, 0.5f).SetEase(Ease.InOutSine));
        seq.Join(letters[targetIndex].transform.parent.GetChild(1).GetComponent<CanvasGroup>().DOFade(1, 0.1f));
        seq.Append(hintLetter.rectTransform.DOShakeScale(0.05f, 0.5f, 1, 20));
        seq.onComplete += () =>
        {
            hintLetter.rectTransform.sizeDelta = hintLetter.rectTransform.anchoredPosition = new();
        };
        //seq.Join(letters[targetIndex].transform.parent.GetChild(1).GetComponent<Image>().DOColor(wordGuessManager.hintColor, 0.1f));
        print(letters[targetIndex].transform.parent.childCount);
        SetCounter();
        if (limitReached)
        {
            button.GetComponent<Image>().sprite = inactiveSprite;
        }
        onInputFinish?.Invoke();
    }
}
