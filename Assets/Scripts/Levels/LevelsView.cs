using UnityEngine.UI;
using UnityEngine;
using Mkey;
using System.Collections.Generic;
using TMPro;
public class LevelsView : MonoBehaviour
{
    public int minLevels = 100;
    public float lockedAlpha = 0.9f;
    public Vector2 scrollOffset = new Vector2(0, 2000);
    public Color outlineColorCurrent;
    public Color outlineColorDefault;
    List<LevelButton> levelButtons = new List<LevelButton>();

    int seasonIndex = 0;

    public int levelCount => levelButtons.Count;
    ScrollRect _scrollRect;
    ScrollRect scrollRect => _scrollRect ? _scrollRect : _scrollRect = GetComponent<ScrollRect>();
    RectTransform contentPanel => scrollRect.content;




    void PrepareButton(LevelButton button)
    {
        button.outline = button.gameObject.AddComponent<Outline>();
        button.canvasGroup = button.gameObject.AddComponent<CanvasGroup>();
        button.outline.effectColor = outlineColorDefault;
        button.button.interactable = false;
    }
    void Expand()
    {
        GameObject prefab;
        GameObject go;
        if (levelButtons.Count == 0)
        {
            prefab = UIConfig.instance.firstSeason;
            go = Instantiate(prefab);
            go.transform.SetParent(contentPanel, false);
            levelButtons.Add(null);

            foreach (Transform t in go.transform)
                Destroy(t.gameObject);
        }


        seasonIndex %= UIConfig.instance.levelsSeason.Length;

        prefab = UIConfig.instance.levelsSeason[seasonIndex];
        go = Instantiate(prefab);
        go.transform.SetParent(contentPanel, false);
        foreach (Transform x in go.transform)
        {
            var button = x.GetComponent<LevelButton>();
            button.button.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(2);
            });
            button.numberText.text = levelCount.ToString();

            var goText = button.numberText.gameObject;
            DestroyImmediate(button.numberText);

            var text = goText.gameObject.AddComponent<TextMeshProUGUI>();
            text.text = levelCount.ToString();
            text.enableAutoSizing = true;
            text.verticalAlignment = VerticalAlignmentOptions.Middle;
            text.horizontalAlignment = HorizontalAlignmentOptions.Center;
            text.fontSizeMax = 1000;
            text.rectTransform.sizeDelta = (new Vector2(84, 94));
            text.fontStyle = FontStyles.Bold;

            PrepareButton(button);


            int mLevel = levelCount;
            button.button.onClick.AddListener(() =>
            {
                HandleClick(mLevel);
            });

            UpdateButton(button, 0, true);

            this.levelButtons.Add(button);
        }
        seasonIndex++;

        if (levelCount < minLevels)
            Expand();
    }

    void HandleClick(int index)
    {

    }

    LevelButton GetButton(int level)
    {
        while (level >= levelButtons.Count)
        {
            Expand();
        }
        return levelButtons[level];
    }


    void UpdateButton(LevelButton button, int stars, bool isLocked)
    {
        button.LeftStar.SetActive(stars >= 2);
        button.RightStar.SetActive(stars >= 2);
        button.MiddleStar.SetActive(stars == 1 || stars == 3);
        button.Lock.SetActive(stars == 0 && isLocked);
        button.button.interactable = false;

        button.canvasGroup.alpha = isLocked ? lockedAlpha : 1;
        button.outline.effectColor = stars == 0 && !isLocked ? outlineColorCurrent : outlineColorDefault;

    }
    public void SetUnlocked(int level)
    {
        GetButton(level);
        for (int i = level; i < levelCount; i++)
        {
            var btn = GetButton(level);
            if (!btn)
            {
                throw new System.Exception("No button " + i);

            }
            UpdateButton(btn, 0, i == level);
            if (i == level)
                SnapTo(btn.GetComponent<RectTransform>());
        }
    }
    public void SetStars(int level, int stars)
    {
        var btn = GetButton(level);
        UpdateButton(btn, stars, false);
    }


    void SnapTo(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();
        var delta = (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
                        - (Vector2)scrollRect.transform.InverseTransformPoint(target.position)
                        + scrollOffset;
        delta.x = contentPanel.anchoredPosition.x;
        contentPanel.anchoredPosition = delta;

        ;
    }


}