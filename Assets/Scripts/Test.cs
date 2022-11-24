using UnityEngine;

public class Test : MonoBehaviour
{
    public int levelToGenerate = 18;
    public LevelInfo levelInfo;
    void OnEnable()
    {
        levelInfo = LevelGen.Generate(levelToGenerate, null, true);
    }
}