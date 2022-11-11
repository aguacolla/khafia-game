using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Store : Page
{
    private int tabActive = 0;
    public Button coinsButton;
    public Button boostersButton;
    public CanvasGroup coinsCanvasGroup;
    public CanvasGroup boostersCanvasGroup;
    
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI eliminationText;

    [SerializeField] GameObject PartclSystm;
    
    // Start is called before the first frame update
    void Start()
    {
        coinsButton.interactable = false;
        SetText();
        GameManager.Instance.OnTextChanged += SetText;
    }

    private void Update()
    {
        
    }

    // Update is called once per frame
    void SetText()
    {
        coinsText.DOText(GameManager.Instance.CoinsAvailable.ToString(), 0.25f);
        eliminationText.DOText(GameManager.Instance.EliminationsAvailable.ToString(), 0.25f);
        hintText.DOText(GameManager.Instance.HintsAvailable.ToString(), 0.25f);
    }

    public void SetActiveTab(int tab)
    {
        tabActive = tab;
        if(tab == 0)
        {
            PartclSystm.SetActive(true);
            coinsCanvasGroup.alpha = 1;
            boostersCanvasGroup.alpha = 0;
            coinsCanvasGroup.blocksRaycasts = true;
            boostersCanvasGroup.blocksRaycasts = false;
        }
        else
        {
            PartclSystm.SetActive(false);
            boostersCanvasGroup.alpha = 1;
            coinsCanvasGroup.alpha = 0;
            boostersCanvasGroup.blocksRaycasts = true;
            coinsCanvasGroup.blocksRaycasts = false;
        }
        //coinsCanvasGroup.alpha = tab == 0 ? 1 : 0;
        //boostersCanvasGroup.alpha = tab == 1 ? 1 : 0;
        coinsButton.interactable = tab == 1;
        boostersButton.interactable = tab == 0;
    }
}
