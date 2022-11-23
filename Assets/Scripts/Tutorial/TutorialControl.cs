using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class TutorialControl : MonoBehaviour
{

    public static TutorialControl instance;




    public int HintsAvail { get; set; } = 3;
    public int EliminationsAvail { get; set; } = 3;

    public string askedWord => instruction.inputWord;

    public int enteredLetters { get; set; }


    public bool launchedByNewUser { get; set; }



    public Instruct instruction;
    // public InstructionType instructionType;

    TutorialConfig config => TutorialConfig.instance;

    WordGuessManager guessManager => GameManager.Instance.wordGuessManager;

    TutorialHelper helper => TutorialHelper.instance;
    TutorialObjects objects => TutorialObjects.instance;


    private void OnEnable()
    {
        Highlighter.instance.gameObject.SetActive(true);
        helper.onClick += OnHelperClick;
        Highlighter.instance.onClick += OnHelperClick;
        guessManager.onInputFinish += OnInput;
        helper.baseTransform.gameObject.SetActive(true);
        instance = this;

        objects.hintButton.onInputFinish += OnInputHint;
        objects.eliminateButton.onInputFinish += OnInputEliminate;

        guessManager.wordGuessedEvent.AddListener(Finish);



        StartCoroutine(TutorialCycle());
    }
    private void OnDisable()
    {
        Highlighter.instance.gameObject.SetActive(false);
        helper.onClick -= OnHelperClick;
        Highlighter.instance.onClick -= OnHelperClick;
        guessManager.onInputFinish -= OnInput;
        helper.baseTransform.gameObject.SetActive(false);
        instance = null;


        objects.hintButton.onInputFinish -= OnInputHint;
        objects.eliminateButton.onInputFinish -= OnInputEliminate;

        guessManager.wordGuessedEvent.RemoveListener(Finish);


        StopAllCoroutines();
        print("has disabled");
    }


    private void Update()
    {
        if (useTimer)
        {
            mTimer += Time.deltaTime;
            if (mTimer > maxTime)
            {
                mTimer = 0;
                useTimer = false;
                moveNext = true;
            }
        }
    }

    bool useTimer;
    float maxTime;
    float mTimer;

    bool moveNext = false;

    void Finish()
    {
        moveNext = true;
    }
    bool MoveNext()
    {
        if (moveNext)
        {
            moveNext = false;
            return true;
        }
        return false;
    }
    IEnumerator TutorialCycle()
    {
        yield return new WaitForSeconds(.3f);
        yield return new WaitUntil(() => Highlighter.instance.hasFinishedAnimation);
        foreach (var x in config.instructs)
        {
            instruction = x;
            if (x.useTimeout)
            {
                useTimer = true;
                mTimer = 0;
                maxTime = x.timeout;
            }

            objects.RefreshGrid();
            objects.RefreshKeyboard();
            TutorialElement.SetAllHighlighted(x.highlights);
            // TutorialElement.SetAllHighlightedNoClick(x.noclickHighlights);

            helper.SetButtonEnabled(x.byClick);
            helper.SetText(x.text);

            yield return new WaitUntil(MoveNext);
        }
        TutorialElement.ResetHighlights();
        Highlighter.instance.gameObject.SetActive(false);
        helper.baseTransform.gameObject.SetActive(false);
    }





    void SetHelpText(string helpText)
    {
        helper.SetText(helpText);
    }
    void OnHelperClick()
    {
        if (instruction == null)
            return;
        if (instruction.byClick)
            moveNext = true;
    }

    void OnInput(string str)
    {
        var isEnter = str == "Enter";
        var isBack = str == "Back";

        if (isEnter)
        {
            if (this.instruction.requireWord)
                moveNext = true;
            enteredLetters = 0;
        }
        else if (isBack)
        {
            enteredLetters = Mathf.Max(enteredLetters - 1, 0);
        }
        else
        {
            enteredLetters++;
        }
        objects.RefreshKeyboard();
        TutorialElement.SetAllHighlighted(instruction.highlights);
    }
    void OnInputEliminate()
    {
        if (instruction.item == Item.Elimination)
            moveNext = true;
    }
    void OnInputHint()
    {
        if (instruction.item == Item.Hint)
            moveNext = true;
    }

}