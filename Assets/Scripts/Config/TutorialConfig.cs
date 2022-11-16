using UnityEngine;

[CreateAssetMenu(fileName = "TutorialConfig", menuName = "Config/Tutorial", order = 0)]
public class TutorialConfig : Config<TutorialConfig>
{
    public string goalWord = "مرحبا";
    public Instruction[] insrtuctions;




    private void Reset()
    {
        insrtuctions = new Instruction[]
    {
        new (InstructionType.ShowText, "مرحبا بك في اللعبة التدريبية", time: 2, waitFor: WaitFor.Time),
        new (InstructionType.ShowText, "هناك كلمة مخفية والهدف هو ايجاد تلك الكلمة!", waitFor: WaitFor.ClickingNext),
        new( InstructionType.ASkForInput, "فلتجرب ادخال كلمة ما ",
            inputWord: "مدرسة",
             highlights: new InstructionElement[]{InstructionElement.LettersOfWord},
             freezeNonHighlighted: true),
        new Instruction (InstructionType.ShowText, helpText: "لاحظ الحروف الملونة! الكلمة الخفية تحوي هذه الحروف الملونة").WaitForNext().Highlights(InstructionElement.InWordCells, InstructionElement.InPlaceCells),
        new Instruction (InstructionType.ShowText, helpText: "هذه الحروف ضمن الكلمة وفي مكانها الصحيح").WaitForNext().Highlights(InstructionElement.InPlaceCells),
        new Instruction (InstructionType.ShowText, helpText: "هذه الحروف ضمن الكلمة ولكن ليست في مكانها").WaitForNext().Highlights(InstructionElement.InWordCells),
        new Instruction (InstructionType.ShowText, helpText: "هذه الحروف ليست ضمن الكلمة").WaitForNext().Highlights(InstructionElement.NotInWordCells),
        new Instruction (InstructionType.ShowText, helpText: "استعمل اداة السهم لازالة عدد من الحروف التي ليست ضمن الكلمة").WaitForClick(InstructionElement.EliminationButton).Highlights(InstructionElement.EliminationButton).FreezeOthers(),
        new Instruction (InstructionType.ShowText, helpText: "استعمل اداة العدسة لكشف حرف من الكلمة").WaitForClick(InstructionElement.EliminationButton).Highlights(InstructionElement.HintButton).FreezeOthers(),
        new Instruction (InstructionType.ASkForInput, helpText: "فلتدخل كلمة اخرى").AskForInput("مركبة").Highlights(InstructionElement.Keyboard).FreezeOthers(),
        new Instruction(InstructionType.ShowText, " اقتربت هذه المرة. فلتدخل كلمة مشابهة ").AskForInputGoal(),
    };
    }

}

[System.Serializable]
public struct Instruction
{
    public InstructionType type;
    [TextArea]
    public string helpText;
    [Tooltip("This word will be asked to enter")]
    public string inputWord;
    [Header("Waiting")]
    public WaitFor waitFor;
    public float time;
    public InstructionElement buttonClick;
    [Header("Highlights")]
    public bool freezeNonHighlighted;
    public InstructionElement[] highlights;

    public Instruction(InstructionType type, string helpText,
    string inputWord = default, WaitFor waitFor = default,
    float time = default,
     InstructionElement buttonClick = default,
     bool freezeNonHighlighted = default,
     InstructionElement[] highlights = default)
    {
        this.type = type;
        this.helpText = helpText;
        this.inputWord = inputWord;
        this.waitFor = waitFor;
        this.time = time;
        this.buttonClick = buttonClick;
        this.freezeNonHighlighted = freezeNonHighlighted;
        this.highlights = highlights;
    }



    public Instruction WaitForTime(float time)
    {
        this.time = time;
        this.waitFor |= WaitFor.Time;
        return this;
    }
    public Instruction WaitForNext()
    {
        this.waitFor |= WaitFor.ClickingNext;
        return this;
    }
    public Instruction Highlights(params InstructionElement[] elements)
    {
        this.highlights = elements;
        return this;
    }
    public Instruction FreezeOthers()
    {
        freezeNonHighlighted = true;
        return this;
    }
    public Instruction WaitForClick(InstructionElement element)
    {
        waitFor |= WaitFor.ClickingNext;
        return this;
    }
    public Instruction AskForInput(string word)
    {
        waitFor |= WaitFor.EnterCorrectWord;
        inputWord = word;
        type = InstructionType.ASkForInput;
        return this;
    }
    public Instruction AskForInputGoal()
    {
        type = InstructionType.AskForInputGoal;
        return this;
    }

}

public enum InstructionElement
{
    None,
    Keyboard,
    LettersOfWord,
    LettersOfGoalWord,
    LastRowOfGrid,
    InWordCells,
    InPlaceCells,
    NotInWordCells,
    EliminationButton,
    HintButton,
}

public enum InstructionType
{
    ShowText,
    WaitFor,
    ASkForInput,
    AskForInputGoal,
}
[System.Flags]
public enum WaitFor
{
    None,
    Time = 1,
    ButtonClick = 2,
    ClickingNext = 4,
    EnterCorrectWord = 16,
}