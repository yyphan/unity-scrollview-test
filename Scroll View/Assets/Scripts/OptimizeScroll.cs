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
    private float viewPortTopY;
    private float viewPortBtmY;
    private float RowHeightPlusSpacing => (InventoryManager.ROW_HEIGHT + InventoryManager.ROW_SPACING);
    
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
        // calling this to make sure ScrollRect is fully drawn, any alternatives?
        Canvas.ForceUpdateCanvases(); 
        
        // one-time setup
        Vector3[] worldCorners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(worldCorners);
        viewPortBtmY = worldCorners[0].y;
        viewPortTopY = worldCorners[1].y;
        float viewportHeight = GetComponent<ScrollRect>().viewport.rect.height;
        float rowPlusSpacingHeight = InventoryManager.ROW_HEIGHT + InventoryManager.ROW_SPACING;
        int visibleRowCount = Mathf.CeilToInt(viewportHeight / rowPlusSpacingHeight) + 1;
        
        SetupPooling(visibleRowCount);

        // layout the initial rows
        for (int i = 0; i < visibleRowCount; i++)
        {
            InventoryRow row = GetRowFromPool();
            row.Init(i);
            
            // assumed row anchoring top-left
            row.transform.localPosition = new Vector2(
                InventoryManager.LEFT_PADDING, 
                - InventoryManager.TOP_PADDING - i * RowHeightPlusSpacing
            );
            
            activeRows.AddLast(row);
        }
    }
    
    private void HandleScroll(Vector2 value)
    {
        UpdateVisibleItems();
    }

    /// <summary>
    /// Goal is to do really lazy update, only when
    /// 1. need to insert row to top
    /// 2. need to insert row to bottom
    /// 3. need to return row to pool
    ///
    /// Other time just let existing rows to flow with ScrollView's Content
    /// </summary>
    private void UpdateVisibleItems()
    {
        if (activeRows.Count == 0)
        {
            return;
        }
        
        InventoryRow topRow = activeRows.First.Value;
        if (RowIsFullyOut(topRow))
        {
            activeRows.RemoveFirst();
            ReturnRowToPool(topRow);
        }
        else if (topRow.RowIdx != 0 && RowIsFullyIn(topRow))
        {
            InsertTop();
        }
        
        InventoryRow btmRow = activeRows.Last.Value;
        if (RowIsFullyOut(btmRow))
        {
            activeRows.RemoveLast();
            ReturnRowToPool(btmRow);
        }
        else if (btmRow.RowIdx != inventoryManager.NumRows - 1 && RowIsFullyIn(btmRow))
        {
            InsertBottom();
        }
    }

    // assumed row anchoring top-left
    private bool RowIsFullyIn(InventoryRow row)
    {
        return row.transform.position.y <= viewPortTopY && 
               row.transform.position.y >= (viewPortBtmY + RowHeightPlusSpacing);
    }
    
    // assumed row anchoring top-left
    private bool RowIsFullyOut(InventoryRow row)
    {
        return row.transform.position.y <= viewPortBtmY || 
               row.transform.position.y >= (viewPortTopY + RowHeightPlusSpacing);
    }

    private void InsertTop()
    {
        var oriTopRow = activeRows.First.Value;
        
        var row = GetRowFromPool();
        row.transform.localPosition = oriTopRow.transform.localPosition + new Vector3(0, RowHeightPlusSpacing, 0); 
            
        var newTopIdx = oriTopRow.RowIdx - 1;
        row.Init(newTopIdx);
        
        activeRows.AddFirst(row);
    }

    private void InsertBottom()
    {
        var oriBtmRow = activeRows.Last.Value;
        
        var row = GetRowFromPool();
        row.transform.localPosition = oriBtmRow.transform.localPosition - new Vector3(0, RowHeightPlusSpacing, 0); 
            
        var newTopIdx = oriBtmRow.RowIdx + 1;
        row.Init(newTopIdx);
        
        activeRows.AddLast(row);
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
