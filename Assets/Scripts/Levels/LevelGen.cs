using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
public static class LevelGen
{
    static LevelConfig config => LevelConfig.instance;

    public static LevelInfo Generate(int level)
    {
        var st = Random.state;
        Random.InitState(level);
        LevelInfo info = new LevelInfo();
        var dict = WordArray.AllWordsDict;
        {
            var goal = WordArray.WordList[Random.Range(0, WordArray.WordList.Length)];
            var goalOrigin = goal;
            goal = Simplify(goal);

            var solvedGuesses = config.solvedGuesses.random;
            var leftGuesses = config.leftGuesses.random;
            var inPlace = config.inPlaceCount.random;
            var inWord = config.inWordcount.random;

            var totalGuesses = solvedGuesses + leftGuesses;
            GuessContent[] guessContents = new GuessContent[solvedGuesses];
            for (int i = 0; i < solvedGuesses; i++)
            {
                inWord = Mathf.Max(inWord, 0);
                inPlace = Mathf.Max(inPlace, 0);

                var inp = Random.Range(0, inPlace + 1);
                var inw = Random.Range(0, inWord + 1);
                guessContents[i] = new GuessContent();

                inPlace -= inp;
                inWord -= inw;
            }
            foreach (var x in guessContents)
            {
                info.entered.Add(SearchWord(x, goalOrigin));
            }
            info.goalWord = goalOrigin;
            info.goalWordSimplified = goal;

        }

        Random.state = st;
        return info;
    }

    static string Simplify(string currentWord)
    {
        var currentWordSimplified = currentWord;
        currentWordSimplified = Regex.Replace(currentWordSimplified, @"[أ|إ|آ]", "ا");
        currentWordSimplified = Regex.Replace(currentWordSimplified, @"[ى]", "ي");
        return currentWordSimplified;

    }
    static GuessContent Compare(string goal, string other)
    {
        GuessContent content = default;
        // HashSet<char> inPlaceChars = new HashSet<char>();
        // HashSet<char> inWordChars = new HashSet<char>();

        for (int i = 0; i < other.Length; i++)
        {
            var g = goal[i];
            var o = other[i];
            if (g == o)
            {
                content.inPlace++;
                // inPlaceChars.Add(o);
            }
            else if (goal.Contains(o))
                content.inWord++;
        }
        return content;
    }
    static string SearchWord(GuessContent content, string goalOrigin)
    {
        var dict = WordArray.AllWordsDict;
        string any = null;
        GuessContent anyContent = default;
        foreach (var x in dict)
        {
            foreach (var word in x.Value)
            {
                if (word == goalOrigin)
                    continue;
                if (Compare(goalOrigin, word).Equals(content))
                    return word;
                else
                {
                    var
                        thisContent = Compare(goalOrigin, word);
                    if (any == null)
                    {
                        any = word;
                        anyContent = thisContent;
                    }
                    else
                    {
                        int anyDiff = Mathf.Abs(content.total - anyContent.total);
                        int thisDiff = Mathf.Abs(content.total - thisContent.total);

                        if (thisDiff < anyDiff)
                        {
                            any = word;
                            anyContent = thisContent;
                        }
                    }
                }
            }
        }
        return any;
    }
    struct GuessContent
    {
        public int inPlace;
        public int inWord;
        public int total => inPlace + inWord;
        public GuessContent(int inPlace, int inWord)
        {
            this.inPlace = inPlace;
            this.inWord = inWord;
        }
    }
}