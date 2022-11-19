using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class TutorialElement : MonoBehaviour
{
    static List<TutorialElement> all = new List<TutorialElement>();

    public InstructionElement element;

    Canvas canvas;
    GraphicRaycaster graphicRaycaster;
    CanvasGroup canvasGroup;
    Button button;
    private void Awake()
    {

        canvasGroup = GetComponent<CanvasGroup>();
        button = GetComponent<Button>();
        all.Add(this);
    }

    public void SetHighlight(bool value)
    {
        if (value)
        {
            if (!canvas)
            {
                canvas = gameObject.AddComponent<Canvas>();
                graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
            }
            canvas.overrideSorting = true;
            canvas.sortingLayerName = "Highlight";
            canvas.enabled = value;
            graphicRaycaster.enabled = value;
        }
        else
        {
            if (canvas)
            {
                Destroy(graphicRaycaster);
                Destroy(canvas);
                graphicRaycaster = null;
                canvas = null;
            }
        }
    }

    public static IEnumerable<TutorialElement> SetAllHighlighted(params InstructionElement[] elements)
    {
        Highlighter.instance.gameObject.SetActive(true);
        foreach (var x in all)
        {
            var exist = elements.Contains(x.element);
            x.SetHighlight(exist);
            if (exist)
                yield return x;
        }
    }
    public static void ResetHighlights()
    {
        foreach (var x in all)
        {
            x.SetHighlight(false);
        }
        Highlighter.instance.gameObject.SetActive(value: false);
    }
    public static void Highlight(InstructionElement element)
    {
        Highlighter.instance.gameObject.SetActive(true);
        foreach (var x in all)
        {
            if (x.element == element)
                x.SetHighlight(true);
        }
    }


    public void SetInteractable(bool value)
    {
        if (canvasGroup)
            canvasGroup.interactable = value;
        if (button)
            button.interactable = value;
    }
}
