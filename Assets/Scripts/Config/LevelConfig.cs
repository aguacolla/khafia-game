using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Config/Level", order = 0)]
public class LevelConfig : Config<LevelConfig>
{
    public float levelDuration = 60;
    public float[] starsDiv = new float[]{
        1/3f,
        2/3f,
        1
    };
    public MinMax solvedGuesses = new MinMax(3, 4);
    public MinMax leftGuesses = new MinMax(3, 4);
    public MinMax inWordcount = new MinMax(3, 4);
    public MinMax inPlaceCount = new MinMax(1, 2);
    public int firstLevels = 10;
    public GenerationInfo genFirstLevels = new GenerationInfo(4, new GuessContent[]{
        new (1,3),
    }, new int[] { 3, 4 });
    public GenerationInfo[] genLevels = {
        new GenerationInfo(5, new GuessContent[] {new GuessContent(1,4), new GuessContent(0,5)},new int[]{3,4}),
        new GenerationInfo(4, new GuessContent[] {new GuessContent(1,2), new GuessContent(0,4)},new int[]{3,4}),
    };
    public int genSeed = 0;
    public int maxIterations = 10;
}
[System.Serializable]
public struct GenerationInfo
{
    public int wordLen;
    public GuessContent[] guesses;
    public int[] solves;

    public GenerationInfo(int wordLen, GuessContent[] guesses, int[] solves)
    {
        this.wordLen = wordLen;
        this.guesses = guesses;
        this.solves = solves;
    }
}