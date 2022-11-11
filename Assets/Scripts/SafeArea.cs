using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SafeArea : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect safeArea;
    public float safeAreaFactor = 0.9f;
    private Vector2 minAnchor;
    private Vector2 maxAnchor;
    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        safeArea = Screen.safeArea;
        minAnchor = new Vector2(safeArea.xMin / Screen.width, safeArea.yMin / Screen.height);
        maxAnchor = new Vector2(safeArea.xMax / Screen.width, safeArea.yMax / Screen.height);
        rectTransform.anchorMin = minAnchor;
        rectTransform.anchorMax = maxAnchor;
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x * safeAreaFactor, rectTransform.offsetMin.y * safeAreaFactor);
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x * safeAreaFactor, rectTransform.offsetMax.y * safeAreaFactor);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
