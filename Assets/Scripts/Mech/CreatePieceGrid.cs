using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering.Canvas.ShaderGraph;

public class CreatePieceGrid : MonoBehaviour
{
    private int totalSize;
    private int pieceCount;
    public string[] pieceValues;
    [SerializeField] public Button Cell;
    [SerializeField] public GameObject PieceGrid;
    private Button[] CellArray;
    public void SetPieceCount(int count=0){pieceCount = count;}
    public void SetWidthSize(int size=0){totalSize = size;}

    // Grid'i ilk olusturma
    public void GenerateGrid()
    {
        CellArray = new Button[pieceCount];
        GridLayoutGroup PieceGridLayout = PieceGrid.GetComponent<GridLayoutGroup>();
        PieceGridLayout.cellSize = new UnityEngine.Vector2(560 / totalSize, 560 / totalSize);

        for(int i = 0; i < pieceCount; i++)
        {   
            if(pieceValues[i] !="")
            {
                var spawnedCell = Instantiate(Cell, PieceGrid.transform);
                spawnedCell.name = $"Piece{i}";
                spawnedCell.AddComponent<DraggableItem>();
                spawnedCell.GetComponent<GridLayoutGroup>().cellSize = new UnityEngine.Vector2((560 / totalSize) - 6, (560 / totalSize) - 6);
                CellArray[i] = spawnedCell;
                CellArray[i].GetComponentInChildren<TextMeshProUGUI>().text = pieceValues[i];
            }
        }
    }

    public void UpdateGrid()
    {
        foreach(Transform child in PieceGrid.transform)
        {
            Destroy(child.gameObject);
        }
        GenerateGrid();
    }

    public void SetChildOnGrid(Transform transformChild)
    {
        transformChild.SetParent(PieceGrid.transform);
    }
}
