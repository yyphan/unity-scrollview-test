using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private RectTransform contentHolder;
    public RectTransform ContentHolder => contentHolder;
    
    [SerializeField] private int numRows = 10;
    public int NumRows => numRows;
    
    // Manual VerticalLayoutGroup
    public static float LEFT_PADDING = 10f;
    public static float RIGHT_PADDING = 10f;
    public static float BTM_PADDING = 10f;
    public static float TOP_PADDING = 0f;
    public static float ROW_SPACING = 10f;
    public static float ROW_HEIGHT = 100f;

    private void Start()
    {
        SetContentHeight();
    }

    /// <summary>
    /// Content's Transform will have full height for ScrollBar to display correctly
    /// </summary>
    private void SetContentHeight()
    {
        var contentHeight = TOP_PADDING + ROW_HEIGHT * numRows + ROW_SPACING * (numRows - 1) + BTM_PADDING;
        contentHolder.sizeDelta = new Vector2(0f, contentHeight);
    }
}
