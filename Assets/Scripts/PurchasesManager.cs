using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class PurchasesManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnPurchaseComplete(Product product)
    {
        print(product.definition.id);
        switch (product.definition.id)
        {
            case "com.kabakebstudio.wordlearabic.noads":
                GameManager.Instance.DisableAds();
                break;
            case "com.kabakebstudio.wordlearabic.starterbundle":
                GameManager.Instance.CoinsAvailable += 1000;
                GameManager.Instance.HintsAvailable += 3;
                GameManager.Instance.EliminationsAvailable += 3;
                print("bought bundle");
                break;
            case "com.kabakebstudio.wordlearabic.250coins":
                GameManager.Instance.CoinsAvailable += 250;
                break;
            case "com.kabakebstudio.wordlearabic.800coins":
                GameManager.Instance.CoinsAvailable += 800;
                break;
            case "com.kabakebstudio.wordlearabic.1700coins":
                GameManager.Instance.CoinsAvailable += 1700;
                break;
            case "com.kabakebstudio.wordlearabic.7200coins":
                GameManager.Instance.CoinsAvailable += 7200;
                break;
        }

        NotificationsManager.Instance.SpawnMessage(1);
        print($"coins added {GameManager.Instance.CoinsAvailable}");
    }
    
    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        NotificationsManager.Instance.SpawnMessage(2);
    }
}
