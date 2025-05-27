using System;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreatePuzzleGrid : MonoBehaviour
{    
    [SerializeField] public bool TEST = false;
    [SerializeField] public bool activeted = false;
    [SerializeField] public int[] TotalSize;
    [SerializeField] public int PieceCount;
    [SerializeField] public Button Cell;
    [SerializeField] public GameObject PuzzleGrid;
    private Button[,] CellArray;
    private CreateBoard CBS; //CreateBoardScript
    private CreatePieceGrid CPGS; //CreatePieceGridScript
    private string[,] board;
    private string[,,] opValues;
    private string[,] coDeletedValues;
    void Awake()
    {       
        // ilk defa board'u oluşturur ve Grid'i olusturur.
        CellArray = new Button[TotalSize[0],TotalSize[1]];
        CBS = GetComponent<CreateBoard>();
        CPGS = GetComponent<CreatePieceGrid>();
        CBS.BOARD_SIZE_X = TotalSize[0];
        CBS.BOARD_SIZE_Y = TotalSize[1];
        CBS.PIECE_COUNT = PieceCount;
        CBS.StartFunctions();
        board = CBS.GetBoard();
        opValues = CBS.GetOpValues();
        coDeletedValues = new string[opValues.GetLength(0),3];
        // CBS.PrintBoard();
        // CBS.PrintOperationValues();
        
        GenerateGrid();
        SelectDeletedValues();
        CPGS.GenerateGrid();

    }
    void Update()
    {
        // Tekrar olusturma aktif edilir.
        if (activeted)
        {
            activeted = false;
            CBS.BOARD_SIZE_X = TotalSize[0];
            CBS.BOARD_SIZE_Y = TotalSize[1];
            CBS.StartFunctions();
            board = CBS.GetBoard();
            opValues = CBS.GetOpValues();

            CBS.PrintBoard();
            CBS.PrintOperationValues();

            UpdateGrid();
            SelectDeletedValues();
            CPGS.UpdateGrid();

            // PrintValues();
        }

        if(TEST)
        {
            TEST = false;
            CPGS.SetChildOnGrid(CellArray[2,2].transform.GetChild(0));
        }
    }

    void PrintValues(string[] deletedValues)
    {
        string text = "";
        for(int i=0; i<deletedValues.Length;i++)
        {
            text += deletedValues[i] + ", ";
        }
        Debug.Log(text);
    }

    // Grid'i ilk olusturma
    private void GenerateGrid()
    {
        GridLayoutGroup PuzzleGridLayout = PuzzleGrid.GetComponent<GridLayoutGroup>();
        PuzzleGridLayout.cellSize = new UnityEngine.Vector2(560 / TotalSize[0], 560 / TotalSize[1]);

        for(int x = 0; x < TotalSize[0]; x++)
        {   
            for(int y = 0; y < TotalSize[1];y++)
            {
                var spawnedCell = Instantiate(Cell, PuzzleGrid.transform);
                spawnedCell.name = $"Tile{x}{y}";
                spawnedCell.GetComponent<GridLayoutGroup>().cellSize = new UnityEngine.Vector2((560 / TotalSize[0]) - 6, (560 / TotalSize[1]) - 6);
                CellArray[x,y] = spawnedCell;

                // board uzerinde bos ise o hucre gorunmez yapilir. Degilse deger yazilir.
                if(board[x,y] == "rolled" || board[x,y] == "None" || board[x,y] == "")
                {
                    Color disableColor = CellArray[x,y].GetComponent<Image>().color;
                    disableColor.a = 0;
                    CellArray[x,y].GetComponent<Image>().color = disableColor;
                    disableColor = CellArray[x,y].GetComponentInChildren<TextMeshProUGUI>().color;
                    disableColor.a = 0;
                    CellArray[x,y].GetComponentInChildren<TextMeshProUGUI>().color = disableColor;
                } 
                else
                {
                    CellArray[x,y].GetComponentInChildren<TextMeshProUGUI>().text = board[x,y];
                } 
            }
        }
    }

    // Grid'i yeniden olusturur.
    private void UpdateGrid() 
    {
        for(int x = 0; x < TotalSize[0]; x++)
        {   
            for(int y = 0; y < TotalSize[1];y++)
            {
                
                if(CellArray[x,y].transform.childCount > 1) 
                {
                    CPGS.SetChildOnGrid(CellArray[x,y].transform.GetChild(0));
                }
                
                Color enableColor = CellArray[x,y].GetComponent<Image>().color;
                enableColor.a = 1f;
                CellArray[x,y].GetComponent<Image>().color = enableColor;
                enableColor = CellArray[x,y].GetComponentInChildren<TextMeshProUGUI>().color;
                enableColor.a = 1f;
                CellArray[x,y].GetComponentInChildren<TextMeshProUGUI>().color = enableColor;
                CellArray[x,y].GetComponentInChildren<TextMeshProUGUI>().text = "";

                if(board[x,y] == "rolled" || board[x,y] == "None" || board[x,y] == "")
                {
                    Color disableColor = CellArray[x,y].GetComponent<Image>().color;
                    disableColor.a = 0;
                    CellArray[x,y].GetComponent<Image>().color = disableColor;
                    disableColor = CellArray[x,y].GetComponentInChildren<TextMeshProUGUI>().color;
                    disableColor.a = 0;
                    CellArray[x,y].GetComponentInChildren<TextMeshProUGUI>().color = disableColor;
                } 
                else
                {
                    CellArray[x,y].GetComponentInChildren<TextMeshProUGUI>().text = board[x,y];
                }
            }
        }
    }

    // Silinecek degerleri bulma.
    private void SelectDeletedValues()
    {
        string[] deletedValues = new string[opValues.GetLength(0)];

        for(int i=0; i<deletedValues.Length;i++)
        {
            coDeletedValues[i,0] = "";
            coDeletedValues[i,1] = "";
            coDeletedValues[i,2] = "";
        }

        for(int i=0; i<opValues.GetLength(0);i++)
        {
            int selectedRND = UnityEngine.Random.Range(0, 3);
            if(opValues[i,selectedRND,0] != "")
            {
                // Silinecek deger daha once secildiyse yeni bir deger sec.
                bool isBeen = false;
                for(int j=0; j<deletedValues.GetLength(0); j++)
                {
                    if( coDeletedValues[j,0] == opValues[i,selectedRND,0] && 
                        coDeletedValues[j,1] == opValues[i,selectedRND,1] && 
                        coDeletedValues[j,2] == opValues[i,selectedRND,2])
                    {
                        isBeen = true;
                        break;
                    }
                }
                if(isBeen)
                {
                    i--;
                    continue;
                }
                else
                {
                    coDeletedValues[i,0] = opValues[i,selectedRND,0];
                    coDeletedValues[i,1] = opValues[i,selectedRND,1];
                    coDeletedValues[i,2] = opValues[i,selectedRND,2];
                }
            }
        }

        for(int i=0; i<coDeletedValues.GetLength(0);i++)
        {
            deletedValues[i] = coDeletedValues[i,0];
        }
        RemoveDeletedValues();

        CPGS.SetPieceCount(deletedValues.GetLength(0));
        CPGS.SetWidthSize(TotalSize[0]);
        
        // Integer'a cevirip siraliyor.
        deletedValues = deletedValues
            .Where(x => !string.IsNullOrEmpty(x))  // Boş ve null elemanları filtrele
            .OrderBy(x => Convert.ToInt32(x))      // Sayısal sıralama
            .Concat(deletedValues.Where(x => string.IsNullOrEmpty(x))) // Boş elemanları sona ekle
            .ToArray();

        CPGS.pieceValues = deletedValues;
    }
    void PrintaValues(string[,] coDeletedaValues)
    {
        string text = "";
        for(int i=0; i<coDeletedaValues.GetLength(0);i++)
        {
            text += coDeletedaValues[i,0] + ", " + coDeletedaValues[i,1] + ", " + coDeletedaValues[i,2] + "\n";
        }
        Debug.Log(text);
    }
    // Silinecek degerleri PuzzleGrid uzerinde degistiriyor.
    private void RemoveDeletedValues()
    {
        int cnt=0;
        PrintaValues(coDeletedValues);
        for(int i=0; i<coDeletedValues.GetLength(0);i++)
        {
            if(coDeletedValues[i,0] != "") {cnt++;}
        }
        for(int i=0; i<cnt;i++)
        {
            CellArray[int.Parse(coDeletedValues[i,1]),int.Parse(coDeletedValues[i,2])].GetComponentInChildren<TextMeshProUGUI>().text = "";
            CellArray[int.Parse(coDeletedValues[i,1]),int.Parse(coDeletedValues[i,2])].AddComponent<InventorySlot>();
        }
    }
}