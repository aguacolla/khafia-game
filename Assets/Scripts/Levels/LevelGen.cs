using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
public static class LevelGen
{
    static LevelConfig config => LevelConfig.instance;

    public static LevelInfo Generate(int level)
    {
        var st = Random.state;
        Random.InitState(level + LevelConfig.instance.genSeed);
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

                var isLast = i == solvedGuesses - 1;

                var inp = isLast ? inPlace : Random.Range(0, inPlace + 1);
                var inw = isLast ? inWord : Random.Range(0, inWord + 1);
                guessContents[i] = new GuessContent(inp, inw);

                inPlace -= inp;
                inWord -= inw;
            }
            foreach (var x in guessContents)
            {
                info.entered.Add(SearchWord(x, goalOrigin, info.entered));
            }
            info.goalWord = goalOrigin;
            info.goalWordSimplified = goal;

        }

        Random.state = st;
        return info;
    }

    public static string Simplify(string currentWord)
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
    static KeyValuePair<K, V> GetAt<K, V>(this Dictionary<K, V> dict, int index)
    {
        int counter = 0;
        foreach (var x in dict)
        {
            if (counter == index)
                return x;
            counter++;
        }
        return default;
    }
    static string SearchWord(GuessContent content, string goalOrigin, List<string> current)
    {
        var dict = WordArray.AllWordsDict;
        string any = null;
        GuessContent anyContent = default;

        int startIndex = Random.Range(0, dict.Count);
        int count = dict.Count;

        // int i = startIndex + 1;
        // if (startIndex == dict.Count - 1)
        int i = startIndex;
        int loopSafe = 0;
        do
        {
            loopSafe++;
            if (loopSafe > 1000)
            {
                throw new System.Exception("Something is not correct");
            }

            var x = dict.GetAt(i);

            foreach (var word in x.Value)
            {
                if (word == goalOrigin || current.Contains(word))
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
            i++;
            i %= count;
        } while (i != startIndex);
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