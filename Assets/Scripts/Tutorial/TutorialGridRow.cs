using UnityEngine;
using UnityEngine.UI;
public class TutorialGridRow : TutorialElement
{
    private void Reset()
    {
    }
    private void Awake()
    {
        foreach (Transform t in transform)
        {
            var element = t.gameObject.AddComponent<TutorialElement>();
            element.element = InstructionElement.None;
        }
    }

    WordGuessManager gm => GameManager.Instance.wordGuessManager;

    public void Refresh()
    {
        foreach (Transform t in transform)
        {
            var element = t.GetComponent<TutorialElement>();
            var image = t.GetComponentInChildren<Image>();
            if (image)
            {
                if (image.color == gm.inPlaceColor)
                {
                    element.element = InstructionElement.InPlaceCells;
                }
                else if (image.color == gm.inWordColor)
                {
                    element.element = InstructionElement.InWordCells;

                }
                else
                {
                    element.element = InstructionElement.NotInWordCells;
                }
            }

        }
    }
}