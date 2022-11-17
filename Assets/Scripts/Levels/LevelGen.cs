using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
public static class LevelGen
{
    static LevelConfig config => LevelConfig.instance;
    static float beginTime;
    public static LevelInfo Generate(int level)
    {
        beginTime = Time.time + 4;
        var st = Random.state;
        Random.InitState(level + LevelConfig.instance.genSeed);
        LevelInfo info = new LevelInfo();
        List<int> indexes = new List<int>();

        var wordLen = 5;
        var dict = WordArray.AllWordsDict;
        int iteration = 0;
    START:
        {
            var goal = WordArray.WordList[Random.Range(0, WordArray.WordList.Length)];
            var goalOrigin = goal;
            goal = Simplify(goal);

            var solvedGuesses = config.solvedGuesses.random;
            var leftGuesses = config.leftGuesses.random;
            var inPlace = config.inPlaceCount.random;
            var inWord = config.inWordcount.random;

            inWord = Mathf.Clamp(inWord, 0, wordLen);
            inPlace = Mathf.Clamp(inPlace, 0, wordLen);
            while (inPlace + inWord > wordLen)
                inPlace--;
            while (inPlace + inWord < wordLen)
                inWord++;

            GuessContent gc = new GuessContent(inPlace, inWord);



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
                var s = Search(goalOrigin, x, info.entered, indexes);
                if (searchFail)
                {
                    if (iteration >= LevelConfig.instance.maxIterations)
                    {
                        Debug.LogError("Search fail");
                    }
                    else
                        goto START;
                }
                info.entered.Add(s);
            }
            if (iteration > 1)
            {
                Debug.Log("level generation iterations: " + iteration);
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


    static IEnumerable<string> AllWords()
    {
        var dict = WordArray.AllWordsDict;
        int startIndex = Random.Range(0, dict.Count);
        int count = dict.Count;
        int i = startIndex;
        int loopSafe = 0;
        do
        {
            loopSafe++;
            if (loopSafe > 1000)
                throw new System.Exception("Something is not correct");
            var array = dict.GetAt(i).Value;
            foreach (var word in array)
                yield return word;
            i++;
            i %= count;
        } while (i != startIndex);
    }
    static bool searchFail = false;

    static string Search(string goal, GuessContent leftContent, List<string> entered, List<int> indexes)
    {
        searchFail = false;
        string found = null;
        var goalSimple = Simplify(goal);
        foreach (var word in AllWords())
        {
            if (word == goal || entered.Contains(word))
                continue;
            var wordSimple = Simplify(word);
            var content = Compare(goalSimple, wordSimple);

            if (content.Equals(leftContent))
            {
                var temp = CompareIndexes(goal, word, indexes);
                if (temp == null)
                    continue;
                var foundContent = content;
                leftContent = leftContent.Sub(foundContent);
                indexes.AddRange(temp);
                return word;
            }
        }
        if (found == null)
            foreach (var word in AllWords())
            {
                // Debug.LogError("word not found");
                searchFail = true;
                return word;
            }
        return found;
    }
    static List<int> temp = new List<int>();
    static List<int> CompareIndexes(string goal, string other, List<int> indexes)
    {
        var oldCount = indexes.Count;

        temp.Clear();

        for (int i = 0; i < goal.Length; i++)
        {
            var g = goal[i];
            var o = other[i];

            if (g == o)
            {
                if (indexes.Contains(i) || temp.Contains(i))
                    return null;
                temp.Add(i);
            }
            else
            {
                for (int j = 0; j < goal.Length; j++)
                {
                    var G = goal[j];
                    if (o == G)
                    {
                        if (indexes.Contains(j) || temp.Contains(j))
                            return null;
                        temp.Add(j);
                    }
                }
            }
        }
        return temp;

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
    static string SearchWord(GuessContent content, string goalOrigin, List<string> current, List<int> indexes)
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

        string randomWord = "";

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
                if (Time.time > beginTime)
                    throw new System.Exception("Took to much time");
                if (word == goalOrigin || current.Contains(word))
                    continue;
                // if (Compare(goalOrigin, word).Equals(content))
                //     return word;
                else
                {
                    var
                        thisContent = Compare(goalOrigin, word);
                    if (randomWord == null)
                    {
                        randomWord = word;
                    }
                    {
                        int anyDiff = Mathf.Abs(content.total - anyContent.total);
                        int thisDiff = Mathf.Abs(content.total - thisContent.total);

                        if (thisDiff < anyDiff)
                        {
                            var goalSimplify = Simplify(goalOrigin);
                            var wordSimplify = Simplify(word);
                            var tempList = CompareIndexes(goalSimplify, wordSimplify, indexes);
                            if (tempList == null)
                                continue;
                            indexes.AddRange(tempList);
                            any = word;
                            anyContent = thisContent;
                        }
                    }
                }
            }
            i++;
            i %= count;
        } while (i != startIndex);

        if (any == null)
        {
            Debug.LogError("Word not found for goal: " + goalOrigin);
            any = randomWord;
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
        public GuessContent Sub(GuessContent other)
        {
            return new GuessContent(inPlace - other.inPlace, inWord - other.inWord);
        }
        public override string ToString()
        {
            return inPlace + "," + inWord;
        }
    }
}