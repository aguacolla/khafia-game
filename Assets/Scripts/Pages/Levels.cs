using UnityEngine;

public class Levels : Page
{
    public LevelsView levelsView;


    private void OnEnable()
    {

        for (int i = 1; i < GameManager.Instance.UnlockedLevel; i++)
        {
            levelsView.SetStars(i, GameManager.Instance.GetLevelStars(i));
        }
        levelsView.SetUnlocked(GameManager.Instance.UnlockedLevel);

    }
}