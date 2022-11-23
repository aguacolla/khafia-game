using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class State
{
    public int wordLen => goalWord.Length;
    public string goalWord;
    public string goalWordSimple;
    public string enteredWord = "";
    public int rowIndex;
    public bool wordGuessed;
    public bool outOftrials;
    public InGameState currentState;
    public InGameState pastState;
    public bool incorrectWord;
    public List<int> lettersHinted = new List<int>();
    public bool hintCalled;
    public int eliminationCount;

    public int seed;

    public List<string> tries = new List<string>();
    public int usedHints;
    public int usedEliminations;

    public bool isOver;
    public List<string> eliminatedLetters = new();

}

public enum GameMode
{
    Classic,
    Level,
    Tutorial,
}