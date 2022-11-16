using UnityEngine;
using UnityEngine.Purchasing;
using TMPro;
using UnityEngine.UI;
public class FixIapButtonPrice : MonoBehaviour
{
    TextMeshProUGUI text;
    IAPButton button;

    Text mtext;

    private void Awake()
    {
        mtext = new GameObject("mtext").AddComponent<Text>();
        mtext.transform.SetParent(transform);

        text = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<IAPButton>();

        button.priceText = mtext;
        button.enabled = false;
        button.enabled = true;
        Invoke("UpdateText", 1);
    }

    void UpdateText()
    {
        text.text = mtext.text;
        mtext.gameObject.SetActive(false);
    }
}