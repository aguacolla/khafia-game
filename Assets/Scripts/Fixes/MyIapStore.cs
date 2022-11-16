using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing;
using UnityEngine;
public class MyStore : IStore, IStoreListener
{
    public static bool isReady => callback != null;
    public static ProductCollection products => callback.products;
    private static IStoreCallback callback;
    public void Initialize(IStoreCallback c)
    {
        callback = c;
        Debug.Log("initialization done");
    }

    public void RetrieveProducts(System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Purchasing.ProductDefinition> products)
    {
        Debug.Log("Rretrieving products");

        // Fetch product information and invoke callback.OnProductsRetrieved();
    }

    public void Purchase(UnityEngine.Purchasing.ProductDefinition product, string developerPayload)
    {
        Debug.Log("purchase");

        // Start the purchase flow and call either callback.OnPurchaseSucceeded() or callback.OnPurchaseFailed()
    }

    public void FinishTransaction(UnityEngine.Purchasing.ProductDefinition product, string transactionId)
    {
        Debug.Log("finish transi");

        // Perform transaction related housekeeping 
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("1");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        Debug.Log("2");
        return default;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log("3");

    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("4");

    }
}