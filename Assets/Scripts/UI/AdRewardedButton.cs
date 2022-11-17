using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AdRewardedButton : MonoBehaviour
{
    public TextMeshProUGUI text;
    public string prefix = "";
    public Button button
    {
        get
        {
            if (_button) return _button;
            Init();
            return _button;
        }
    }
    Button _button;

    private void Awake()
    {
        Init();
    }
    void Init()
    {
        if (_button)
            return;
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (text)
        {
            text.text = prefix + AdsManager.Instance.rewardCoins;
        }
    }

}