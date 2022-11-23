using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Keyboard : MonoBehaviour
{

    [System.Serializable]
    public class Key
    {
        public Button button;
        public TextMeshProUGUI text;
        public Image image => button.image;
        public Transform transform => button.transform;
        public string name => transform.name;
    }
    public static Keyboard instance;
    public EliminateButton eliminateButton { get; private set; }
    public HintButton hintButton { get; private set; }
    public EnterButton enterButton { get; private set; }
    List<Key> keys = new List<Key>();
    private void Awake()
    {
        instance = this;
        foreach (Button but in GetComponentsInChildren<Button>())
        {
            if (but.name == "Enter")
                enterButton = but.GetComponent<EnterButton>();
            if (but.name == "Eliminate")
                eliminateButton = but.GetComponent<EliminateButton>();
            if (but.name == "Hint")
                hintButton = but.GetComponent<HintButton>();
            but.onClick = new Button.ButtonClickedEvent();
            but.onClick.AddListener(() =>
            {
                OnClick(but.name);
            });

            if (but.name == "Enter" || but.name == "Back" || but.name == "Hint" || but.name == "Eliminate") continue;

            var key = new Key
            {
                button = but,
                text = but.GetComponentInChildren<TextMeshProUGUI>()
            };
            key.text.color = UIConfig.instance.keyboardDefaultTextColor;
            key.button.image.color = UIConfig.instance.keyboardDefaultColor;
            keys.Add(key);
        }
    }


    void OnClick(string btnName)
    {
        switch (btnName)
        {
            case "Eliminate":
                eliminateButton.EliminateLetters(GameManager.Instance.eliminateLetterCount);
                break;
            case "Hint":
                hintButton.ShowHint();
                break;
            default:
                GameManager.Instance.wordGuessManager.EnterLetter(btnName);
                break;

        }
    }

    public void Clean()
    {
        foreach (var key in keys)
        {
            key.text.color = UIConfig.instance.keyboardDefaultTextColor;
            key.button.image.color = UIConfig.instance.keyboardDefaultColor;
        }
        enterButton.SetInteractable(false);
        enterButton.SetIncorrectWord(false);
        hintButton.ResetButton();
        eliminateButton.ResetButton();
    }

    public int keyCount => keys.Count;
    public Key GetKey(string letter) => this.keys.Find(x => x.name == letter);
    public Image GetKeyImage(string letter)
    {
        return this.keys.Find(x => x.button.name == letter).image;
    }
    public List<string> GetLetterList()
    {
        List<string> list = new List<string>(keys.Count);
        foreach (var x in keys)
            list.Add(x.button.name);

        return list;

    }
    public Key this[string letter]
    {
        get => GetKey(letter);
    }







}