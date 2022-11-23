using UnityEngine;

[CreateAssetMenu(fileName = "TutorialConfig", menuName = "Config/Tutorial", order = 0)]
public class TutorialConfig : Config<TutorialConfig>
{
    public string goalWord = "مرحبا";
    public Instruct[] instructs;
    public Instruct[] postInstructs;
    // [Space]
    // public Instruction[] insrtuctions;





    private void Reset()
    {
        instructs = new Instruct[] {
            new Instruct ("في هذه اللعبة التدريبية سيتم شرح كيفية اللعب"),
            new Instruct("هناك كلمة مخفية والهدف هو ايجاد تلك الكلمة!"),
            new Instruct("فلتجرب ادخال كلمة ما ").Word("مدرسة"),
            new Instruct("لون الاحرف هو الفتاح لايجاد الكلمة!").SetHighlights(InstructionElement.WordGrid),
            new Instruct("هذه الحروف ضمن الكلمة وفي مكانها الصحيح").SetHighlights(InstructionElement.InPlaceCells),
            new Instruct("هذه الحروف ضمن الكلمة ولكن ليست في مكانها").SetHighlights(InstructionElement.InWordCells),
            new Instruct("هذه الحروف ليست ضمن الكلمة").SetHighlights(InstructionElement.NotInWordCells),
            new Instruct("استعمل اداة السهم لازالة عدد من الحروف التي ليست ضمن الكلمة", Item.Elimination),
            new Instruct("رائع!"),
            new Instruct("بازالة الحروف غير الموجودة يصبح منن السهل العثور على الكلمة"),
            new Instruct("استعمل اداة العدسة لكشف حرف من الكلمة", Item.Hint),
            new Instruct("رائع!"),
            new Instruct("تكشف اداة العدسة عن احد الحروف من الكلمة"),
            new Instruct("فلتدخل كلمة اخرى").Word("مركبة"),
            new Instruct(" اقتربت هذه المرة. فلتدخل كلمة مشابهة ").Word("مرحبا"),
    };

        postInstructs = new Instruct[] {
    //   new Instruct("يمكنك استعمال النقود لشراء الادوات من المتجر").SetHighlights(InstructionElement.Coins),
    //   new Instruct("قي المتجر يمكن شراء النقود وغيرها من الاغراض").SetHighlights(InstructionElement.Coins),
    //   new Instruct("").SetHighlights(InstructionElement.Coins),
      new Instruct("انقر هنا لعرض جميع المستويات ").SetHighlights(InstructionElement.Coins),
      new Instruct("انقر هنا للعب المستوى الحالي ").SetHighlights(InstructionElement.Coins),
    };
        //     insrtuctions = new Instruction[]
        // {
        //     new (InstructionType.ShowText, "مرحبا بك في اللعبة التدريبية", time: 2, waitFor: WaitFor.Time),
        //     new (InstructionType.ShowText, "هناك كلمة مخفية والهدف هو ايجاد تلك الكلمة!", waitFor: WaitFor.ClickingNext),
        //     new( InstructionType.ASkForInput, "فلتجرب ادخال كلمة ما ",
        //         inputWord: "مدرسة",
        //          highlights: new InstructionElement[]{InstructionElement.LettersOfWord},
        //          freezeNonHighlighted: true),
        //     new Instruction (InstructionType.ShowText, helpText: "لاحظ الحروف الملونة! الكلمة الخفية تحوي هذه الحروف الملونة").WaitForNext().Highlights(InstructionElement.InWordCells, InstructionElement.InPlaceCells),
        //     new Instruction (InstructionType.ShowText, helpText: "هذه الحروف ضمن الكلمة وفي مكانها الصحيح").WaitForNext().Highlights(InstructionElement.InPlaceCells),
        //     new Instruction (InstructionType.ShowText, helpText: "هذه الحروف ضمن الكلمة ولكن ليست في مكانها").WaitForNext().Highlights(InstructionElement.InWordCells),
        //     new Instruction (InstructionType.ShowText, helpText: "هذه الحروف ليست ضمن الكلمة").WaitForNext().Highlights(InstructionElement.NotInWordCells),
        //     new Instruction (InstructionType.ShowText, helpText: "استعمل اداة السهم لازالة عدد من الحروف التي ليست ضمن الكلمة").WaitForClick(InstructionElement.EliminationButton).Highlights(InstructionElement.EliminationButton).FreezeOthers(),
        //     new Instruction (InstructionType.ShowText, helpText: "استعمل اداة العدسة لكشف حرف من الكلمة").WaitForClick(InstructionElement.EliminationButton).Highlights(InstructionElement.HintButton).FreezeOthers(),
        //     new Instruction (InstructionType.ASkForInput, helpText: "فلتدخل كلمة اخرى").AskForInput("مركبة").Highlights(InstructionElement.Keyboard).FreezeOthers(),
        //     new Instruction(InstructionType.ShowText, " اقتربت هذه المرة. فلتدخل كلمة مشابهة ").AskForInputGoal(),
        // };
    }

}
[System.Serializable]
public class Instruct
{
    [TextArea]
    public string text;
    public string inputWord = "";
    public bool useTimeout;
    public float timeout = 3;
    public Item item = Item.UNKNOWN;
    public InstructionElement[] highlights;
    public InstructionElement[] noclickHighlights;
    public bool requireWord => !string.IsNullOrEmpty(inputWord);
    public bool byClick => item == Item.UNKNOWN && !requireWord;

    public Instruct(string helpText, Item item = Item.UNKNOWN)
    {
        this.item = item;
        if (item == Item.Hint)
            SetHighlights(InstructionElement.HintButton);
        if (item == Item.Elimination)
            SetHighlights(InstructionElement.EliminationButton);
        this.text = helpText;
    }
    public Instruct SetHighlights(params InstructionElement[] highlights)
    {
        this.highlights = highlights;
        return this;
    }
    public Instruct SetTimeout(float timeout)
    {
        this.useTimeout = true;
        this.timeout = timeout;
        return this;
    }
    public Instruct Word(string inputWord)
    {
        this.inputWord = inputWord;
        SetHighlights(InstructionElement.LettersOfWord, InstructionElement.WordGrid);
        return this;
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
    [Header("Highlights")]
    public bool freezeNonHighlighted;
    public InstructionElement[] highlights;



    public Instruction(InstructionType type, string helpText,
    string inputWord = default, WaitFor waitFor = default,
    float time = default,
     bool freezeNonHighlighted = default,
     InstructionElement[] highlights = default)
    {
        this.type = type;
        this.helpText = helpText;
        this.inputWord = inputWord;
        this.waitFor = waitFor;
        this.time = time;
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
    InWordCells,
    InPlaceCells,
    NotInWordCells,
    EliminationButton,
    HintButton,
    TutorialHelper,
    WordGrid,

    PlayClassic,
    LevelMenu,
    PlayLevel,
    Store,
    Coins,

}

public enum InstructionType
{
    ShowText,
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