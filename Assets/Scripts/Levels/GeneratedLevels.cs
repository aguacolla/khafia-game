
using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "GeneratedLevels", menuName = "GeneratedLevels", order = 0)]
public class GeneratedLevels : ScriptableObject
{
    public static GeneratedLevels instance
    {
        get
        {
            if (_instance)
                _instance = Resources.Load<GeneratedLevels>("GeneratedLevels");
            return _instance;
        }
    }

    static GeneratedLevels _instance;
    public int count = 1000;
    public List<LevelInfo> levels = new List<LevelInfo>();

#if UNITY_EDITOR

    [ContextMenu("Generate")]
    void Generate()
    {
        if (levels.Count == 0)
            levels.Add(new LevelInfo());
        int startPoint = levels.Count;
        for (int i = startPoint; i <= count; i++)
        {
            var cancel = UnityEditor.EditorUtility.DisplayCancelableProgressBar("Generating level", $"Level ({i}/{count})",
            (i - startPoint) / (float)(count - startPoint));
            if (cancel)
            {
                Debug.Log("Operation canceled");
                break;
            }
            levels.Add(LevelGen.Generate(i, 0));
        }
        UnityEditor.EditorUtility.ClearProgressBar();
    }
#endif


}