using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Page : MonoBehaviour
{
    public void OpenPage()
    {
        gameObject.SetActive(true);
    }
    
    public void ClosePage()
    {
        gameObject.SetActive(false);
    }
}
