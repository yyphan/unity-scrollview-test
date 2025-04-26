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
    
    private void OnEnable()
    {
        scrollRect.onValueChanged.AddListener(HandleScroll);
    }

    /// <summary>
    /// Unfortunately entry point need to be in this function to ensure ScrollRect's fully drawn before entry point
    /// Can remove if chose to hardcode num of rows
    /// </summary>
    void OnRectTransformDimensionsChange()
    {
        SetupPooling();
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

    private void SetupPooling()
    {
        // one-time call so...
        float viewportHeight = GetComponent<ScrollRect>().viewport.rect.height;
        float rowPlusSpacingHeight = InventoryManager.ROW_HEIGHT + InventoryManager.ROW_SPACING;
        int initalPoolCount = Mathf.CeilToInt(viewportHeight / rowPlusSpacingHeight) + 1;

        rowPool.Clear();
        for (int i = 0; i < initalPoolCount; i++)
        {
            var obj = Instantiate(GetRowPrefab(), inventoryManager.ContentHolder);
            obj.SetActive(false);
            
            var rowScript = obj.GetComponent<InventoryRow>();
            rowScript.Init(i);
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

    #endregion
}
