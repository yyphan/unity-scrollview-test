using UnityEngine;

public class InventoryRow : MonoBehaviour
{
    [SerializeField] private InventoryItemDisplay[] rowItems;
    private int rowIdx;
    public int RowIdx => rowIdx;
    
    public void Init(int rowIndex)
    {
        rowIdx = rowIndex;
        name = $"Row {rowIdx}";
        
        var startIndex = rowIdx * rowItems.Length;
        for (var i = 0; i < rowItems.Length; i++)
        {
            rowItems[i].Init( startIndex+ i + 1);
            rowItems[i].name = $"item {startIndex + i + 1}";
        }
    }
}
