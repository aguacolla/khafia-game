using System.Collections.Generic;
[System.Serializable]
public class LevelInfo
{
    public string goalWord;
    public string goalWordSimplified;
    public bool hasFailed;
    public List<string> entered = new List<string>();



    public void ApplyInputs()
    {
        // UnityEngine.Object.FindObjectOfType<LevelsView>(true).testLevelInfo = this;
        var manager = GameManager.Instance.wordGuessManager;
        foreach (var x in entered)
        {
            var word = LevelGen.Simplify(x);
            foreach (var c in word)
            {
                manager.EnterLetter(c.ToString());
            }
            manager.EnterLetter("Enter");
            manager.CurrentState = InGameState.Typing;
        }
    }

    public LevelInfo Clone()
    {
        var clone = new LevelInfo();
        clone.goalWord = goalWord;
        clone.goalWordSimplified = goalWordSimplified;
        clone.hasFailed = hasFailed;
        clone.entered = new List<string>(entered);
        return clone;
    }
}