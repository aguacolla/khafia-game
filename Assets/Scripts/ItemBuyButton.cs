using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Item
{
    Hint,
    Elimination,
    UNKNOWN,
}

public class ItemBuyButton : MonoBehaviour
{
    public Item item;
    public int multiplier = 1;
    public int price = 150;
    private Button button;
    private TextMeshProUGUI text;

    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        text.text = (price * multiplier).ToString();
        button.onClick.AddListener(Buy);
    }

    private void Buy()
    {
        if (GameManager.Instance.CoinsAvailable >= price * multiplier)
        {
            GameManager.Instance.CoinsAvailable -= price * multiplier;
            if (item == Item.Hint)
            {
                GameManager.Instance.HintsAvailable += multiplier;
            }
            else if (item == Item.Elimination)
            {
                GameManager.Instance.EliminationsAvailable += multiplier;
            }
            GameManager.Instance.OnItemBought?.Invoke();
            NotificationsManager.Instance.SpawnMessage(1);
        }
        else
        {
            NotificationsManager.Instance.SpawnMessage(2);
        }
    }
}
