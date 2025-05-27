using System.IO.Compression;
using System.Security;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using System;
using Unity.Burst.CompilerServices;

public class CreateBoard : MonoBehaviour
{
    // Tahta boyutu değerleri
    public int BOARD_SIZE_X = 7;
    public int BOARD_SIZE_Y = 7;
    
    // İşlem yön matrisi (l, d, r, u, h, v)
    private static readonly int[,] DIRECTIONS = new int[,]
    {
        { 0, 1 },  // Sağ
        { 1, 0 },  // Aşağı
        { 0, -1 }, // Sol
        { -1, 0 }, // Yukarı
        { 0, 1 },  // Sağ (tekrar)
        { 1, 0 }   // Aşağı (tekrar)
    };

    // İşlem matrisi
    private static readonly char[] OPERATIONS = { '+', '-', '*', '/' };

    // Maksimum bulmaca sayısı
    public int PIECE_COUNT = 9;

    // İşlem eleman sayısı
    private const int CHAR_COUNT = 5;

    // İşlem sayısını tutar
    private int OPERATION_COUNT = 0;

    // İşlemlerin sayı değerlerini tutan dizi
    private string[,,] OPERATION_VALUES;

    // Tahta array
    private string[,] board;

    public string[,] GetBoard() {
        return board;
    }

    public string[,,] GetOpValues() {
        return OPERATION_VALUES;
    }

    public void StartFunctions() {
        
        board = new string[BOARD_SIZE_X,BOARD_SIZE_Y];
        OPERATION_VALUES = new string[PIECE_COUNT + 1, 3, 3];
        StartFunction();
        
        /*
        PrintBoard();

        string operationsss= "";
        for (int i = 0; i < OPERATION_VALUES.GetLength(0); i++) // Satır sayısı
        {
            string row = "";
            for (int j = 0; j < OPERATION_VALUES.GetLength(1); j++) // Sütun sayısı
            {
                row += OPERATION_VALUES[i, j] + " "; // Her öğeyi boşlukla ayırarak birleştir
            }
            
            operationsss += row + "\n";
        }
        Debug.Log(operationsss);
        */

    }
    // 3 Boyutlu Dizi Konsola Yazdırma
    public void PrintOperationValues()
    {
        string output = ""; // Tüm verileri tek bir string'e ekle

        for (int i = 0; i < OPERATION_VALUES.GetLength(0); i++) // n (satır)
        {
            output += $"Row {i}:\n"; // Satır bilgisi ekle

            for (int j = 0; j < OPERATION_VALUES.GetLength(1); j++) // 3 nesne
            {
                output += $"  Object {j}: (";
                
                for (int k = 0; k < OPERATION_VALUES.GetLength(2); k++) // 3 değer (x, y, diğer)
                {
                    output += OPERATION_VALUES[i, j, k]; // Değeri ekle
                    if (k < OPERATION_VALUES.GetLength(2) - 1) output += ", "; // Virgül ekle
                }

                output += ")\n"; // Nesne bittiğinde yeni satıra geç
            }

            output += "\n"; // Satır bittiğinde ekstra boşluk bırak
        }

        Debug.Log(output); // Tek seferde konsola yazdır
    }

    public void PrintBoard()
    {
        // Tahta boyutları
        int boardSizeX = BOARD_SIZE_X;
        int boardSizeY = BOARD_SIZE_Y;

        // board matrisini düzgün şekilde formatla
        string boardOutput = "";

        for (int i = 0; i < boardSizeX; i++)
        {
            string row = "";

            for (int j = 0; j < boardSizeY; j++)
            {
                // Eğer hücre boşsa " " (boşluk) ekleyin
                string cell = string.IsNullOrEmpty(board[i, j]) ? " " : board[i, j];

                // Hücreleri | ile ayırarak satıra ekle
                row += cell + " | ";
            }

            // Satırın sonundaki fazla boşluğu temizle
            row = row.TrimEnd(' ', '|');

            // Tahtadaki her satırı birleştirerek boardOutput'a ekle
            boardOutput += row + "\n";

            // Satırlar arasında ayraç ekle
            boardOutput += new string('-', boardSizeY * 4) + "\n";
        }

        // Konsola tam tahta çıktısını yazdır
        Debug.Log(boardOutput);
    }

    public void CreateValues() 
    {
        board = new string[BOARD_SIZE_X, BOARD_SIZE_Y];

        // Tahtayi ve islem degerlerini bos string ile doldur
        for (int i = 0; i < OPERATION_VALUES.GetLength(0); i++)
        {
            for (int j = 0; j < OPERATION_VALUES.GetLength(1); j++)
            {
                OPERATION_VALUES[i, j, 0] = "";
                OPERATION_VALUES[i, j, 1] = "";
                OPERATION_VALUES[i, j, 2] = "";
            }
        }
        
        for (int i = 0; i < BOARD_SIZE_X; i++)
        {
            for (int j = 0; j < BOARD_SIZE_Y; j++)
            {
                board[i, j] = "";
            }
        }
    }

    private bool IsWithinBounds(int x, int y) 
    {
        return x >= 0 && x < BOARD_SIZE_X && y >= 0 && y < BOARD_SIZE_Y; 
    }

    private bool IsInteger(object value) 
    {
        if (value == null) return false;
        return int.TryParse(value.ToString(), out _);
    }

    private bool CanPlaceInDirection(int x, int y, int dx, int dy, bool m = false)
    {
        for (int step = 0; step < CHAR_COUNT; step++)
        {
            int nx = m ? x + (step - 2) * dx : x + step * dx;
            int ny = m ? y + (step - 2) * dy : y + step * dy;

            if(!IsWithinBounds(nx,ny) || board[nx,ny] == "None")
            {
                return false;
            }
        }
        return true;
    }

    private bool[] CheckAvailableDirections(int x, int y)
    {
        bool[] availability = new bool[6];

        if (CanPlaceInDirection(x, y, DIRECTIONS[0, 0], DIRECTIONS[0, 1]))
            availability[0] = true; // Sag
        if (CanPlaceInDirection(x, y, DIRECTIONS[1, 0], DIRECTIONS[1, 1]))
            availability[1] = true; // Asagi
        if (CanPlaceInDirection(x, y, DIRECTIONS[2, 0], DIRECTIONS[2, 1]))
            availability[2] = true; // Sol
        if (CanPlaceInDirection(x, y, DIRECTIONS[3, 0], DIRECTIONS[3, 1]))
            availability[3] = true; // Yukari
        if (CanPlaceInDirection(x, y, DIRECTIONS[4, 0], DIRECTIONS[4, 1], true))
            availability[4] = true; // YatayOrta
        if (CanPlaceInDirection(x, y, DIRECTIONS[5, 0], DIRECTIONS[5, 1], true))
            availability[5] = true; // DikeyOrta

        return availability;
    }

    private void UpdateBoard(int x, int y, int dx, int dy, int d, string num1, string num2, string result, string operation)
    {
        
        OPERATION_VALUES[OPERATION_COUNT, 0, 0] = num1.ToString();
        OPERATION_VALUES[OPERATION_COUNT, 1, 0] = num2.ToString();
        OPERATION_VALUES[OPERATION_COUNT, 2, 0] = result.ToString();
        
        if (d == 2)
        {
            board[x, y - 4] = num1;
            board[x, y - 3] = operation;
            board[x, y - 2] = num2;
            board[x, y - 1] = "=";
            board[x, y] = result;

            OPERATION_VALUES[OPERATION_COUNT, 0, 1] = x.ToString();
            OPERATION_VALUES[OPERATION_COUNT, 1, 1] = x.ToString();
            OPERATION_VALUES[OPERATION_COUNT, 2, 1] = x.ToString();
            OPERATION_VALUES[OPERATION_COUNT, 0, 2] = (y-4).ToString();
            OPERATION_VALUES[OPERATION_COUNT, 1, 2] = (y-2).ToString();
            OPERATION_VALUES[OPERATION_COUNT, 2, 2] = y.ToString();

            if (IsWithinBounds(x, y + 1)) board[x, y + 1] = "None";
            if (IsWithinBounds(x, y - 5)) board[x, y - 5] = "None";
        }
        else if (d == 3)
        {
            board[x - 4, y] = num1;
            board[x - 3, y] = operation;
            board[x - 2, y] = num2;
            board[x - 1, y] = "=";
            board[x, y] = result;

            OPERATION_VALUES[OPERATION_COUNT, 0, 1] = (x-4).ToString();
            OPERATION_VALUES[OPERATION_COUNT, 1, 1] = (x-2).ToString();
            OPERATION_VALUES[OPERATION_COUNT, 2, 1] = x.ToString();
            OPERATION_VALUES[OPERATION_COUNT, 0, 2] = y.ToString();
            OPERATION_VALUES[OPERATION_COUNT, 1, 2] = y.ToString();
            OPERATION_VALUES[OPERATION_COUNT, 2, 2] = y.ToString();

            if (IsWithinBounds(x + 1, y)) board[x + 1, y] = "None";
            if (IsWithinBounds(x - 5, y)) board[x - 5, y] = "None";
        }
        else if (d == 4)
        {
            board[x, y - 2] = num1;
            board[x, y - 1] = operation;
            board[x, y] = num2;
            board[x, y + 1] = "=";
            board[x, y + 2] = result;

            OPERATION_VALUES[OPERATION_COUNT, 0, 1] = x.ToString();
            OPERATION_VALUES[OPERATION_COUNT, 1, 1] = x.ToString();
            OPERATION_VALUES[OPERATION_COUNT, 2, 1] = x.ToString();
            OPERATION_VALUES[OPERATION_COUNT, 0, 2] = (y-2).ToString();
            OPERATION_VALUES[OPERATION_COUNT, 1, 2] = y.ToString();
            OPERATION_VALUES[OPERATION_COUNT, 2, 2] = (y+2).ToString();

            if (IsWithinBounds(x, y - 3)) board[x, y - 3] = "None";
            if (IsWithinBounds(x, y + 3)) board[x, y + 3] = "None";
        }
        else if (d == 5)
        {
            board[x - 2, y] = num1;
            board[x - 1, y] = operation;
            board[x, y] = num2;
            board[x + 1, y] = "=";
            board[x + 2, y] = result;

            OPERATION_VALUES[OPERATION_COUNT, 0, 1] = (x-2).ToString();
            OPERATION_VALUES[OPERATION_COUNT, 1, 1] = x.ToString();
            OPERATION_VALUES[OPERATION_COUNT, 2, 1] = (x+2).ToString();
            OPERATION_VALUES[OPERATION_COUNT, 0, 2] = y.ToString();
            OPERATION_VALUES[OPERATION_COUNT, 1, 2] = y.ToString();
            OPERATION_VALUES[OPERATION_COUNT, 2, 2] = y.ToString();

            if (IsWithinBounds(x - 3, y)) board[x - 3, y] = "None";
            if (IsWithinBounds(x + 3, y)) board[x + 3, y] = "None";
        }
        else
        {
            board[x, y] = num1;
            board[x + dx, y + dy] = operation;
            board[x + 2 * dx, y + 2 * dy] = num2;
            board[x + 3 * dx, y + 3 * dy] = "=";
            board[x + 4 * dx, y + 4 * dy] = result;

            OPERATION_VALUES[OPERATION_COUNT, 0, 1] = x.ToString();
            OPERATION_VALUES[OPERATION_COUNT, 1, 1] = (x + 2 * dx).ToString();
            OPERATION_VALUES[OPERATION_COUNT, 2, 1] = (x + 4 * dx).ToString();
            OPERATION_VALUES[OPERATION_COUNT, 0, 2] = y.ToString();
            OPERATION_VALUES[OPERATION_COUNT, 1, 2] = (y + 2 * dy).ToString();
            OPERATION_VALUES[OPERATION_COUNT, 2, 2] = (y + 4 * dy).ToString();

            if (IsWithinBounds(x - dx, y - dy)) board[x - dx, y - dy] = "None";
            if (IsWithinBounds(x + 5 * dx, y + 5 * dy)) board[x + 5 * dx, y + 5 * dy] = "None";
        }
        
        OPERATION_COUNT++;
    }
    private (int num1, int num2, int result, char operation) SetOperationValues(int x, int y, int d, (int, int)? firstN, (int, int)? secondN, (int, int)? resultN, int placeCounter)
    {
        int num1 = 0, num2 = 0, result = 0;
        char operation = ' ';

        while (true)
        {
            if (placeCounter == 2)
            {
                operation = OPERATIONS[UnityEngine.Random.Range(0, OPERATIONS.Length)];

                if (firstN == null)
                {
                    num2 = int.Parse(board[secondN.Value.Item1, secondN.Value.Item2]);
                    result = int.Parse(board[resultN.Value.Item1, resultN.Value.Item2]);

                    num1 = operation switch
                    {
                        '/' => num2 * result,
                        '-' => num2 + result,
                        '+' => result - num2,
                        '*' => result / num2,
                        _ => num1
                    };
                }
                else if (secondN == null)
                {
                    num1 = int.Parse(board[firstN.Value.Item1, firstN.Value.Item2]);
                    result = int.Parse(board[resultN.Value.Item1, resultN.Value.Item2]);

                    num2 = operation switch
                    {
                        '/' => num1 / result,
                        '-' => num1 - result,
                        '+' => result - num1,
                        '*' => result / num1,
                        _ => num2
                    };
                }
                else if (resultN == null)
                {
                    num1 = int.Parse(board[firstN.Value.Item1, firstN.Value.Item2]);
                    num2 = int.Parse(board[secondN.Value.Item1, secondN.Value.Item2]);

                    result = operation switch
                    {
                        '/' => num1 / num2,
                        '-' => num1 - num2,
                        '+' => num1 + num2,
                        '*' => num1 * num2,
                        _ => result
                    };
                }
                break;
            }

            if (string.IsNullOrEmpty(board[x, y]) || board[x, y] == "rolled")
            {
                num1 = UnityEngine.Random.Range(1, 16);
                num2 = UnityEngine.Random.Range(1, 16);
                operation = OPERATIONS[UnityEngine.Random.Range(0, OPERATIONS.Length)];

                if (operation == '/' && num1 % num2 != 0) continue;
                if (operation == '-')
                {
                    if (num1 == num2) num1 += 1;
                    else (num1, num2) = (Mathf.Max(num1, num2), Mathf.Min(num1, num2));
                }

                result = operation switch
                {
                    '/' => num1 / num2,
                    '-' => num1 - num2,
                    '+' => num1 + num2,
                    '*' => num1 * num2,
                    _ => result
                };
                break;
            }
            else
            {
                if (d == 0 || d == 1)
                {
                    num1 = int.Parse(board[x, y]);
                    num2 = UnityEngine.Random.Range(1, 16);
                    operation = OPERATIONS[UnityEngine.Random.Range(0, OPERATIONS.Length)];

                    if (operation == '/' && num1 % num2 != 0) continue;
                    if (operation == '-') num2 = UnityEngine.Random.Range(1, num1 + 1);

                    result = operation switch
                    {
                        '/' => num1 / num2,
                        '-' => num1 - num2,
                        '+' => num1 + num2,
                        '*' => num1 * num2,
                        _ => result
                    };
                    break;
                }
                else if (d == 2 || d == 3)
                {
                    result = int.Parse(board[x, y]);
                    num1 = UnityEngine.Random.Range(1, 16);
                    operation = OPERATIONS[UnityEngine.Random.Range(0, OPERATIONS.Length-2)];

                    if (operation == '/' && (result == 0 || num1 % result != 0)) continue;
                    if (operation == '-') num1 = UnityEngine.Random.Range(result + 1, result + 16);

                    num2 = operation switch
                    {
                        '/' => num1 / result,
                        '-' => num1 - result,
                        '+' => result - num1,
                        '*' => result / num1,
                        _ => num2
                    };
                    if(num2 < 0) 
                    {
                        num2 *= -1;
                        operation = '-';
                    }

                    break;
                }
                else if (d == 4 || d == 5)
                {
                    num2 = int.Parse(board[x, y]);
                    num1 = UnityEngine.Random.Range(num2 + 1, num2 + 16);
                    operation = OPERATIONS[UnityEngine.Random.Range(0, OPERATIONS.Length)];

                    if (operation == '/' && num1 % num2 != 0) continue;

                    result = operation switch
                    {
                        '/' => num1 / num2,
                        '-' => num1 - num2,
                        '+' => num1 + num2,
                        '*' => num1 * num2,
                        _ => result
                    };
                    break;
                }
            }
        }
        return (num1, num2, result, operation);
    }


    private bool PlaceNumber(int x, int y)
    {
        int placeCounter = 0;
        int dx = 0;
        int dy = 0;
        int d = 0;
        bool directionCheck = false;
        
        // HUCREDE ISLEM YAPILAN YONDEKI BUTUN ELEMANLARIN TAHTANIN ICINDE MI KONTROLU YAPILIYOR
        bool[] directions = CheckAvailableDirections(x, y);
        
        bool allFalse = true;
        foreach (var direction in directions)
        {
            if (direction)
            {
                allFalse = false;
                break;
            }
        }

        if (allFalse)
            return false;

        // ISLEMIN YAPILACAGI SAYILARIN DEGISKENLERI
       
        (int, int)? firstN = null;
        (int, int)? secondN = null;
        (int, int)? resultN = null;

        for (int i = 0; i < directions.Length; i++)
        {
            placeCounter = 0;
            if (directions[i])
            {
                dx = DIRECTIONS[i, 0];
                dy = DIRECTIONS[i, 1];
                bool m = (i >= 4);
                d = i;

                // ONCEDEN ISLEM EKLENEN BIR YOL OLUP OLMADIGINI KONTROL EDIYOR
                for (int n = 0; n < CHAR_COUNT; n++)
                {
                    if (!(d > 3) && IsWithinBounds(x + (n * dx), y + (n * dy)))
                    {
                        if (board[x + (n * dx), y + (n * dy)] != "" && board[x + (n * dx), y + (n * dy)] != "rolled" && board[x + (n * dx), y + (n * dy)] != "None")
                        {
                            placeCounter++;
                        }
                    }
                    else if (IsWithinBounds(x + ((n - 2) * dx), y + ((n - 2) * dy)))
                    {
                        if (board[x + ((n - 2) * dx), y + ((n - 2) * dy)] != "" && board[x + ((n - 2) * dx), y + ((n - 2) * dy)] != "rolled" && board[x + ((n - 2) * dx), y + ((n - 2) * dy)] != "None")
                        {
                            placeCounter++;
                        }
                    }
                }

                // placeCounter 2'DEN BUYUK ISE ONCEDEN ISLEM EKLENMISTIR. BASKA BIR YON BULUNUYOR.
                if (placeCounter > 2)
                    continue;

                // placeCounter 2 ISE ONCEDEN ISLEM EKLENMEMISTIR ANCAK ISLEM UZERINDE 2 ADET SAYI VARDIR. UCUNCU SAYI BULUNMAK ICIN BU IKI SAYI ALINIR.
                else if (placeCounter == 2)
                {
                    for (int n = 0; n < CHAR_COUNT; n++)
                    {
                        if (!(d > 3))
                        {
                            if (IsWithinBounds(x + (n * dx), y + (n * dx)) && IsInteger(board[x + (n * dx), y + (n * dx)]))
                            {
                                if (n == 0)
                                    firstN = (x + (n * dx), y + (n * dx));
                                else if (n == 2)
                                    secondN = (x + (n * dx), y + (n * dx));
                                else if (n == 4)
                                    resultN = (x + (n * dx), y + (n * dx));
                            }
                        }
                        else
                        {
                            if (IsInteger(board[x + ((n - 2) * dx), y + ((n - 2) * dy)]))
                            {
                                if (n == 0)
                                    firstN = (x + ((n - 2) * dx), y + ((n - 2) * dy));
                                else if (n == 2)
                                    secondN = (x + ((n - 2) * dx), y + ((n - 2) * dy));
                                else if (n == 4)
                                    resultN = (x + ((n - 2) * dx), y + ((n - 2) * dy));
                            }
                        }
                    }
                }
                else
                {
                    directionCheck = true;
                }
                break;
            }
        }

        if (!directionCheck)
            return false;

        var (num1, num2, result, operation) = SetOperationValues(x, y, d, firstN, secondN, resultN, placeCounter);
        
        if (OPERATION_COUNT >= PIECE_COUNT)
            return false;

        UpdateBoard(x, y, dx, dy, d, num1.ToString(), num2.ToString(), result.ToString(), operation.ToString());
        return true;
    }
    
    private bool ChooseNewNumber()
    {
        // Sayı olan elemanların konumlarını bulma
        var numericPositions = GetNumericPositions();  // Bu metodda sayılara sahip konumları almak gerek

        // Sayı olan elemanları karıştırma
        for (int i = numericPositions.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            var temp = numericPositions[i];
            numericPositions[i] = numericPositions[j];
            numericPositions[j] = temp;
        }

        // Sayı olan elemanların her birine yeni işlem ekleyebilmek için yapılan döngü
        foreach (var pos in numericPositions)
        {
            int row = pos.Item1; 
            int col = pos.Item2;

            // Sayıya işlem uygulanabilir mi anlamak için
            bool isPlaced = PlaceNumber(row, col);
            if (!isPlaced)
            {
                continue;
            }
            else
            {
                return true;
            }
        }

        return false;
    }
    private (int, int)[] GetNumericPositions()
    {
        // board matrisinde sayıları arayacağız
        List<(int, int)> numericPositions = new List<(int, int)>();

        for (int row = 0; row < board.GetLength(0); row++) // board'un satır sayısı kadar döngü
        {
            for (int col = 0; col < board.GetLength(1); col++) // board'un sütun sayısı kadar döngü
            {
                // board hücresindeki eleman bir sayı ise (yani sayıya eşit oluyorsa)
                if (IsInteger(board[row, col]))
                {
                    numericPositions.Add((row, col)); // Sayı olan pozisyonu listeye ekle
                }
            }
        }

        return numericPositions.ToArray(); // Listeyi array'e çevirip döndür
    }

    private void ResetBoard()
    {
        for (int i = 0; i < BOARD_SIZE_X; i++) // BOARD_SIZE_X kadar döngü
        {
            for (int j = 0; j < BOARD_SIZE_Y; j++) // BOARD_SIZE_Y kadar döngü
            {
                board[i, j] = ""; // board matrisindeki her bir elemanı boş string ile sıfırla
            }
        }
    }
    private void ResetOperationValues()
    {
        OPERATION_COUNT = 0;
        for (int i = 0; i < OPERATION_VALUES.GetLength(0); i++)
        {
            for (int j = 0; j < OPERATION_VALUES.GetLength(1); j++)
            {
                OPERATION_VALUES[i, j, 0] = ""; // Her hücreyi boş (""), yani sıfırla
                OPERATION_VALUES[i, j, 1] = ""; // Her hücreyi boş (""), yani sıfırla
                OPERATION_VALUES[i, j, 2] = ""; // Her hücreyi boş (""), yani sıfırla
            }
        }
    }

    private void StartFunction()
    {
        ResetBoard(); // Board'u sıfırlıyoruz
        ResetOperationValues();
        
        // Başlangıç noktası bulma
        bool isSuccess = false;

        for (int i = 0; i < BOARD_SIZE_X; i++)
        {
            for (int j = 0; j < BOARD_SIZE_Y; j++)
            {
                if (board[i, j] == "" && UnityEngine.Random.value < 0.33f) // 1/3 oranında başlama
                {
                    isSuccess = PlaceNumber(i, j);
                    if (!isSuccess)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                else if (board[i, j] == "")
                {
                    board[i, j] = "rolled"; // "rolled" olarak işaretliyoruz
                }
            }
            if (isSuccess)
            {
                break;
            }
        }

        int REPEATE_COUNTER = 0;
        while (REPEATE_COUNTER < PIECE_COUNT + 10)
        {
            if (ChooseNewNumber())
            {
                continue;
            }
            else
            {
                REPEATE_COUNTER += 1;
            }
        }

        for (int i = 0; i < PIECE_COUNT; i++)
        {
            int zeroCounter = 0;
            for (int j = 0; j < 3; j++)
            {
                if (OPERATION_VALUES[i, j, 0] == "0") // string "0" ile karşılaştırıyoruz
                {
                    zeroCounter += 1;
                }
            }
            if (zeroCounter == 3)
            {
                StartFunction(); // Rekürsif çağrı
            }
        }
    }

}
