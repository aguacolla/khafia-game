using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RTLTMPro;

public class LossLevelPopup : Popup
{
    public TextMeshProUGUI level;

    private void OnEnable()
    {
        level.text = GameManager.Instance.LevelGame.ToString();
    }
}
