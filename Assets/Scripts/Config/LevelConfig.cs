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
    public int genSeed = 0;
    public int maxIterations = 10;
}