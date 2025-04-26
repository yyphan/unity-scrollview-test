using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptimizeScroll : MonoBehaviour
{
    [SerializeField] private RectTransform viewPort;
    [SerializeField] private ScrollRect scrollRect;
    
    //private const float ItemHeight = 110f; // includes spacing of 10
    //private const float ItemSpacing = 10f;

    [SerializeField] private InventoryManager inventoryManager;

    private Queue<InventoryRow> rowPool = new Queue<InventoryRow>();
    private GameObject rowPfb;
    
    private LinkedList<InventoryRow> activeRows = new LinkedList<InventoryRow>();
    
    private void OnEnable()
    {
        scrollRect.onValueChanged.AddListener(HandleScroll);
        InitRows();
    }

    /// <summary>
    /// Entry point
    /// </summary>
    private void InitRows()
    {
        // TODO
        // calling this to make sure viewportHeight is correct, any alternatives?
        Canvas.ForceUpdateCanvases(); 
        
        // one-time call
        float viewportHeight = GetComponent<ScrollRect>().viewport.rect.height;
        float rowPlusSpacingHeight = InventoryManager.ROW_HEIGHT + InventoryManager.ROW_SPACING;
        int visibleRowCount = Mathf.CeilToInt(viewportHeight / rowPlusSpacingHeight) + 1;
        
        SetupPooling(visibleRowCount);

        // layout the initial rows
        for (int i = 0; i < visibleRowCount; i++)
        {
            InventoryRow row = GetRowFromPool();
            row.Init(i);
            
            // assumed anchoring top-left
            row.transform.localPosition = new Vector2(
                InventoryManager.LEFT_PADDING, 
                - InventoryManager.TOP_PADDING - i * (InventoryManager.ROW_HEIGHT + InventoryManager.ROW_SPACING)
            );
            
            activeRows.AddLast(row);
        }
    }
    
    private void HandleScroll(Vector2 value)
    {
        UpdateVisibleItems();
    }

    private void UpdateVisibleItems()
    {
        // Implement your solution here
        // Access the array of inventory rows as needed: inventoryManager.inventoryRows
        
        
    }

    #region [ Pooling ]

    private void SetupPooling(int initalPoolCount)
    {
        rowPool.Clear();
        for (int i = 0; i < initalPoolCount; i++)
        {
            var obj = Instantiate(GetRowPrefab(), inventoryManager.ContentHolder);
            obj.SetActive(false);
            
            var rowScript = obj.GetComponent<InventoryRow>();
            rowPool.Enqueue(rowScript);
        }
    }

    private GameObject GetRowPrefab()
    {
        if (rowPfb is null)
        {
            rowPfb = ResourceHelper.LoadPrefab("Inventory Row.prefab");
            if (rowPfb is null)
            {
                Debug.LogError("Error loading Inventory Row.prefab");
            }
        }
        
        return rowPfb;
    }

    private InventoryRow GetRowFromPool()
    {
        if (rowPool.Count == 0)
        {
            var obj = Instantiate(GetRowPrefab(), inventoryManager.ContentHolder);
            obj.SetActive(false);
            
            var rowScript = obj.GetComponent<InventoryRow>();
            rowPool.Enqueue(rowScript);
        }

        var row = rowPool.Dequeue();
        row.gameObject.SetActive(true);
        
        return row;
    }

    private void ReturnRowToPool(InventoryRow row)
    {
        row.gameObject.SetActive(false);
        rowPool.Enqueue(row);
    }

    #endregion // [ Pooling ]
}
