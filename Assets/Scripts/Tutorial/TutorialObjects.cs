using System.Linq;
using UnityEngine;
using System.Collections.Generic;
[DefaultExecutionOrder(-1000)]
public class TutorialObjects : MonoBehaviour
{

    public static TutorialObjects instance;

    public Transform wordGrid;
    public Transform keyboard;
    public Transform hints;
    public Transform eliminations;

    public Transform levelMenu;
    public Transform[] playLevel;


    public EliminateButton eliminateButton { get; set; }
    public HintButton hintButton { get; set; }

    List<TutorialElement> keys = new List<TutorialElement>();
    List<TutorialGridRow> rows = new List<TutorialGridRow>();

    private void Awake()
    {
        instance = this;
        keyboard.gameObject.AddComponent<TutorialElement>().element = InstructionElement.Keyboard;
        wordGrid.gameObject.AddComponent<TutorialElement>().element = InstructionElement.WordGrid;
        //levelMenu.gameObject.AddComponent<TutorialElement>().element = InstructionElement.LevelMenu;
        //gameObject.AddComponent<TutorialElement>().element = InstructionElement.LevelMenu;
        foreach (Transform t in wordGrid)
        {
            rows.Add(t.gameObject.AddComponent<TutorialGridRow>());
        }

        foreach (Transform row in keyboard.GetChild(0))
        {
            foreach (Transform key in row)
            {
                if (key.name == "Hint")
                    this.hints = key;
                if (key.name == "Eliminate")
                    this.eliminations = key;
                if (key != hints && key != eliminations)
                    keys.Add(key.gameObject.AddComponent<TutorialElement>());
            }
        }
        hints.gameObject.AddComponent<TutorialElement>().element = InstructionElement.HintButton;
        eliminations.gameObject.AddComponent<TutorialElement>().element = InstructionElement.EliminationButton;
        eliminateButton = eliminations.GetComponent<EliminateButton>();
        hintButton = hints.GetComponent<HintButton>();
    }

    public void RefreshKeyboard()
    {
        var control = TutorialControl.instance;
        if (!control)
        {
            Debug.LogError("Control is not found");
            return;
        }

        var word = control.askedWord;
        var pointer = control.enteredLetters;

        int wordlen = word.Length;

        string letter = "";
        bool isFull = false;
        try
        {
            if (pointer >= wordlen)
                isFull = true;
            else
            if (!string.IsNullOrEmpty(word))
                letter = word[pointer].ToString();
        }
        catch
        {
            Debug.Log("Out of range " + word.Length + " : " + pointer);
        }


        foreach (var key in keys)
        {
            if (key.name == "Enter")
            {
                key.element = isFull ? InstructionElement.LettersOfWord : InstructionElement.None;
            }
            else if (key.name == letter)
            {
                key.element = InstructionElement.LettersOfWord;
            }
            else
            {
                key.element = InstructionElement.None;
            }
        }
    }

    public void RefreshGrid()
    {
        foreach (var x in rows)
            x.Refresh();
    }

}