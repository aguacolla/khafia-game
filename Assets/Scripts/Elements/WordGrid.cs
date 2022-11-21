using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
[DefaultExecutionOrder(-50)]
public class WordGrid : MonoBehaviour
{
    public static WordGrid instance;
    public WordGuessManager wordGuessManager => GameManager.Instance.wordGuessManager;

    public int childCount => transform.childCount;
    public List<Image> glowImages { get; set; } = new List<Image>();

    public int rowIndex => wordGuessManager.rowIndex;
    public int wordLen => wordGuessManager.wordLen;

    public Image hintGlow;
    private void Awake()
    {
        instance = this;
        foreach (Transform row in transform)
        {
            foreach (Transform letter in row)
            {
                letter.GetComponentInChildren<TextMeshProUGUI>().color = UIConfig.instance.gridLetterDefaultColor;
                Image glow = Instantiate(hintGlow, letter.position, Quaternion.identity, letter).GetComponent<Image>();
                glow.color = UIConfig.instance.hintColor;
                glow.gameObject.AddComponent<CanvasGroup>().alpha = 0;
                glowImages.Add(glow);
            }
        }
    }
    public void DisplayWord()
    {
        Transform row = transform.GetChild((rowIndex));
        for (int i = 0; i < wordLen; i++)
        {
            var eWord = wordGuessManager.enteredWord;
            var str = eWord.Length > i ? eWord[i].ToString() : "";
            if (str == "ي" && i != wordLen - 1)
            {
                str = "يـ";
            }
            else if (str == "ئ" && i != wordLen - 1)
            {
                str = "ئـ";
            }
            Transform letter = row.GetChild(i);
            if (letter.GetChild(1).childCount == 1 && letter.GetChild(1).GetChild(0).gameObject.activeInHierarchy && eWord.Length >= i/* && str.Equals(letter.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text, StringComparison.CurrentCulture)*/)
            {
                if (str.Equals(""))
                {
                    letter.GetChild(1).DOLocalMoveY(0, 0.2f);
                    letter.GetChild(1).GetComponent<CanvasGroup>().DOFade(1, 0.2f);
                }
                else
                {
                    letter.GetChild(1).DOLocalMoveY(250, 0.2f);
                    letter.GetChild(1).GetComponent<CanvasGroup>().DOFade(0, 0.2f);
                }
                //letter.GetChild(1).GetComponent<CanvasGroup>().DOFade(0, 0.1f);
                //letter.GetChild(1).GetComponent<Image>().DOFade(0, 0.1f);
                //letter.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 0.1f);
            }
            letter.GetComponentInChildren<TextMeshProUGUI>().text = str;
            //print(str);
        }
    }
    public Transform GetChild(int index)
    {
        return transform.GetChild(index);
    }

    public void Clean()
    {
        foreach (Transform row in transform)
        {
            foreach (Transform letter in row)
            {
                letter.GetComponent<Image>().sprite = UIConfig.instance.defaultWordImage;
                letter.GetComponentInChildren<TextMeshProUGUI>().color = UIConfig.instance.gridLetterDefaultColor;
                var tl = letter.GetComponent<TutorialElement>();
                if (tl)
                    tl.element = 0;
            }
        }
        foreach (Image glow in glowImages)
        {
            glow.rectTransform.localPosition = new Vector3(0, 0, 0);
            glow.color = UIConfig.instance.hintColor;
            glow.GetComponent<CanvasGroup>().alpha = 0;
            if (glow.transform.childCount > 0)
            {
                glow.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        TextMeshProUGUI[] gridTMPro = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI tmPro in gridTMPro) tmPro.text = "";

        Image[] images = GetComponentsInChildren<Image>();

        foreach (Image image in images) image.color = UIConfig.instance.defaultColor;

    }
    IEnumerator Shake(int row, float duration)
    {
        float startTime = Time.time;
        float time = Time.time - startTime;
        while (time <= duration)
        {
            float x = 50 * Mathf.Sin(40 * time) * Mathf.Exp(-5 * time);
            transform.GetChild(row).localPosition = new Vector3(x, transform.GetChild(row).localPosition.y, transform.GetChild(row).localPosition.z);
            yield return new WaitForSeconds(0.01f);
            time = Time.time - startTime;
        }
    }

    public void DoShake()
    {
        StartCoroutine(Shake(rowIndex, 1));
    }

    public void SetImageColor()
    {
        Transform row = transform.GetChild(rowIndex);
        for (int i = 0; i < wordLen; i++)
        {
            Image img = row.GetChild(i).GetComponent<Image>();
            img.color = UIConfig.instance.FulldefaultColor;
        }
    }

    public void SetLen(int len)
    {
        foreach (Transform row in transform)
        {
            for (int i = 0; i < row.childCount; i++)
            {
                var cell = row.GetChild(i);
                cell.gameObject.SetActive(i < len);
            }
        }
    }
}