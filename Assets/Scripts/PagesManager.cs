using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PagesManager : Singleton<PagesManager>
{
    public List<Page> pages = new List<Page>();
    public Page currentPage;
    public Page previousPage;
    // Start is called before the first frame update
    
    void Start()
    {
        pages.ForEach(page => page.gameObject.SetActive(false));
        currentPage = pages[3];
        currentPage.OpenPage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void FlipPage(Page page)
    {
        if (currentPage != null)
        {
            previousPage = currentPage;
        }
        currentPage = page;
        currentPage.OpenPage();
        previousPage?.ClosePage();
    }
    
    public void FlipPage(int pageIndex)
    {
        FlipPage(pages[pageIndex]);
    }
    
    public void GoBack()
    {
        if(GameManager.Instance.CurrentState == GameManager.Instance.States["game"])
        {
            GameManager.Instance.SwitchState("menu");
        }
        else if (GameManager.Instance.CurrentState == GameManager.Instance.States["store"])
        {
            if(previousPage == pages[0])
            {
                GameManager.Instance.SwitchState("menu");
            }
            else if(previousPage == pages[1])
            {
                GameManager.Instance.SwitchState("game");
            }
        }
        FlipPage(currentPage != pages[1] ? previousPage : pages[0]);
    }
}
