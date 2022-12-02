using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : Singleton<PopupManager>
{
    public List<Popup> popups = new List<Popup>();
    [SerializeField] private Popup currentPopup;

    private void Start()
    {
        foreach (Popup popup in popups)
        {
            popup.gameObject.SetActive(false);
        }
    }

    public void OpenPopup(Popup popup)
    {
        popup.Open();
        currentPopup = popup;
    }

    public void OpenPopup(int index)
    {
        currentPopup = popups[index];
        popups[index].Open();
    }

    public void ClosePopup(Popup popup)
    {
        popup.Close();
    }

    public void ClosePopup(int index)
    {
        popups[index].Close();
    }

    public void CloseCurrentPopup()
    {
        if (currentPopup)
            currentPopup.Close();
    }
}
