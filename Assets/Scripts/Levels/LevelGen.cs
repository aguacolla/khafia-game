using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
public static class LevelGen
{
    static LevelConfig config => LevelConfig.instance;
    static float beginTime;

    static Dictionary<int, LevelInfo> cached = new();
    public static LevelInfo Generate(int level, int? SEED = null)
    {
        if (GeneratedLevels.instance)
            if (level < GeneratedLevels.instance.levels.Count)
                return GeneratedLevels.instance.levels[level].Clone();
        int seed = level + (SEED.HasValue ? SEED.Value : LevelConfig.instance.genSeed);
        if (cached.ContainsKey(seed))
            return cached[seed].Clone();

        beginTime = Time.time + 4;
        var st = Random.state;
        Random.InitState(seed);
        LevelInfo info;

        int iteration = 0;

        GenerationInfo genInfo;
        if (level <= LevelConfig.instance.firstLevels)
            genInfo = LevelConfig.instance.genFirstLevels;
        else
            genInfo = LevelConfig.instance.genLevels.GetRandom();

        var wordLen = genInfo.wordLen;

        string GOAL = WordArray.GetWordRandom(wordLen);

    START:
        {
            iteration++;
            List<int> indexes = new List<int>();
            info = new LevelInfo();

            var dict = WordArray.GetDictionary(wordLen);
            var goal = GOAL;
            var goalOrigin = goal;
            goal = Simplify(goal);

            var solvedGuesses = genInfo.solves.GetRandom();
            var guessContent = genInfo.guesses.GetRandom();
            var inPlace = guessContent.inPlace;
            var inWord = guessContent.inWord;

            inWord = Mathf.Clamp(inWord, 0, wordLen);
            inPlace = Mathf.Clamp(inPlace, 0, wordLen);
            while (inPlace + inWord > wordLen)
                inPlace--;

            GuessContent[] guessContents = new GuessContent[solvedGuesses];
            for (int i = 0; i < solvedGuesses; i++)
            {
                inWord = Mathf.Max(inWord, 0);
                inPlace = Mathf.Max(inPlace, 0);

                var isLast = i == solvedGuesses - 1;

                // var inw = isLast ? inWord : Random.Range(1, 3);

                // var inp = isLast ? inPlace : inw > 1 ? 0 : Random.Range(1, 3);
                var inw = isLast ? inWord : Random.Range(1, inWord / 2 + 1);
                var inp = isLast ? inPlace : Random.Range(0, inPlace / 2 + 1);

                if (inp > inPlace) inp = inPlace;
                if (inw > inWord) inw = inWord;


                guessContents[i] = new GuessContent(inp, inw);

                inPlace -= inp;
                inWord -= inw;
            }
            foreach (var x in guessContents)
            {
                var s = DoSearch(goalOrigin, x, info.entered, indexes);
                if (searchFail)
                {
                    if (iteration >= LevelConfig.instance.maxIterations)
                    {
                        Debug.LogError($"Search fail: {seed}({level}) Reached max iterations");
                        s = GetRandomFallback(wordLen);
                        info.hasFailed = true;
                    }
                    else
                        goto START;
                }
                info.entered.Add(s);
            }
            if (iteration > 1)
            {
                if (iteration < LevelConfig.instance.maxIterations)
                    Debug.Log($"{seed}({level}): level generation iterations: " + iteration);
            }
            info.goalWord = goalOrigin;
            info.goalWordSimplified = goal;

        }

        Random.state = st;

        cached.Add(seed, info);
        return info;
    }

    public static string Simplify(string currentWord)
    {
        return WordGuessManager.Simplify(currentWord);
    }


    static IEnumerable<string> AllWords(int len)
    {
        var dict = WordArray.GetDictionary(len);
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


    static void DoSearch(string goal, int count, GuessContent guessContent, List<string> target)
    {
        var goalSimple = Simplify(goal);

        var originContent = guessContent;


        foreach (var word in AllWords(goal.Length))
        {
            if (string.IsNullOrEmpty(word))
                continue;
            var wordSimple = Simplify(word);

            if (wordSimple == goalSimple)
                continue;
            if (guessContent.total == 0)
                break;

        }
    }

    static IEnumerable<KeyValuePair<int, bool>> DoCompare(string goal, string word)
    {
        List<int> checklist = new();
        if (goal.Length != word.Length)
        {
            Debug.LogError("Mismatch: " + goal + " -> " + word);
            yield break;
        }
        for (int i = 0; i < goal.Length; i++)
        {
            var g = goal[i];
            var w = word[i];

            if (g == w && !checklist.Contains(i))
            {
                checklist.Add(i);
                yield return new(i, true);
            }
            else
            {
                for (int j = 0; j < goal.Length; j++)
                {
                    g = goal[j];
                    if (w == g && !checklist.Contains(i))
                    {
                        checklist.Add(i);
                        if (word == "دءوبا")
                        {
                            var lll = 0;
                        }
                        yield return new KeyValuePair<int, bool>(j, false);
                    }
                }
            }
            if (word == "دؤوبا")
            {
                var lll = 0;
            }
        }
    }
    static Dictionary<int, List<string>> countOfLengths = new();
    static string GetRandomFallback(int len)
    {
        List<string> list;

        if (!countOfLengths.TryGetValue(len, out list))
        {
            list = new List<string>(10000);
            foreach (var x in AllWords(len))
            {
                if (string.IsNullOrEmpty(x))
                    continue;
                list.Add(x);
            }
            list.TrimExcess();
            countOfLengths.Add(len, list);
        }
        return list[Random.Range(0, list.Count)];
    }
    static string DoSearch(string goal, GuessContent LEFT, List<string> entered, List<int> indexes)
    {
        searchFail = false;
        var goalSimple = Simplify(goal);
        int indexesLen = indexes.Count;
        // string fallback = GetRandomFallback(goal.Length);
        foreach (var word in AllWords(goal.Length))
        {

            if (string.IsNullOrEmpty(word))
                continue;

            var wordSimple = Simplify(word);
            if (wordSimple == goal || entered.Contains(wordSimple))
                continue;

            var content = new GuessContent(LEFT.inPlace, LEFT.inWord);

            bool fail = false;
            IEnumerable<KeyValuePair<int, bool>> compResult = null;
            compResult = DoCompare(goalSimple, wordSimple);
            var s = 0;



            foreach (var x in compResult)
            {
                if (wordSimple == "دءوبا")
                {
                    var lll = 0;
                }
                if (indexes.Contains(x.Key))
                {
                    fail = true;
                    break;
                }
                if (x.Value)
                    content.inPlace--;
                else
                    content.inWord--;
                if (content.isNeg)
                {
                    fail = true;
                    break;
                }
                indexes.Add(x.Key);
            }
            if (content.total != 0)
                fail = true;
            if (fail)
            {
                if (indexesLen < indexes.Count)
                    indexes.RemoveRange(indexesLen, indexes.Count - indexesLen);
                continue;
            }
            return wordSimple;
        }
        searchFail = true;
        return null;

    }


    static bool searchFail = false;

    static string Search(string goal, GuessContent leftContent, List<string> entered, List<int> indexes)
    {
        searchFail = false;
        string found = null;
        var goalSimple = Simplify(goal);
        foreach (var word in AllWords(goal.Length))
        {
            if (word == goal || entered.Contains(word))
                continue;
            var wordSimple = Simplify(word);
            GuessContent content = default;
            try
            {
                content = Compare(goalSimple, wordSimple);
            }
            catch
            {
                var whatever = 0;
            }

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
            foreach (var word in AllWords(goal.Length))
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
    static List<int> matchIndexes = new List<int>();
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
        var dict = WordArray.GetDictionary(goalOrigin.Length);
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

}
[System.Serializable]
public struct GuessContent
{
    public int inPlace;
    public int inWord;
    public int total => inPlace + inWord;
    public bool isNeg => inPlace < 0 || inWord < 0;
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