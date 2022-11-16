using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
public class AdRewardedButton : MonoBehaviour
{
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

}