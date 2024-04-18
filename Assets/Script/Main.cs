using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Main : MonoBehaviour
{
    public int rows = 8; // Số hàng trong lưới
    public int columns = 16; // Số cột trong lưới
    public Sprite[] pikachuSprites; // Mảng các Sprite cho viên gạch giống Pikachu
    public Transform gridParent; // Transform của đối tượng Grid
    public GameObject brickPrefab; // Khai báo biến brickPrefab
    public GameObject brickHelp; // Khai báo biến brickParent
    public float brickWidth;
    public float brickHeight;
    public float firstSpriteX;
    public float firstSpriteY;
    public int[,] matrix;

    void Start()
    {
        GameObject gridParentObject = GameObject.FindWithTag("Grid");
        brickPrefab = GameObject.FindWithTag("firstOBJ");
        brickHelp = GameObject.FindWithTag("zeroOBJ");
        if (gridParentObject != null)
        {
            gridParent = gridParentObject.transform;
        }

        // Lấy kích thước của Sprite đầu tiên trong mảng pikachuSprites
        Sprite firstSprite = pikachuSprites[0];
        brickWidth = firstSprite.bounds.size.x;
        brickHeight = firstSprite.bounds.size.y;

        // Lấy vị trí của Sprite đầu tiên
        firstSpriteX = brickPrefab.transform.position.x;
        firstSpriteY = brickPrefab.transform.position.y;

        // Tạo ma trận mới với kích thước lớn hơn
        int newRows = rows + 2;
        int newColumns = columns + 2;
        matrix = new int[newRows, newColumns];
        // Bao viền ma trận bằng các giá trị 0
        for (int row = 1; row <= rows; row++)
        {
            for (int column = 1; column <= columns; column++)
            {
                matrix[row, column] = 1;
            }
        }
        for (int i = 0; i < newRows; i++)
        {
            matrix[i, 0] = 0; // Cột đầu tiên
            matrix[i, newColumns - 1] = 0; // Cột cuối cùng
        }

        for (int j = 0; j < newColumns; j++)
        {
            matrix[0, j] = 0; // Hàng đầu tiên
            matrix[newRows - 1, j] = 0; // Hàng cuối cùng
        }

        for (int row = 0; row <= rows + 1; row++)
        {
            for (int column = 0; column <= columns + 1; column++)
            {
                // Kiểm tra giá trị tại vị trí trong ma trận
                int value = matrix[row, column];

                // Chọn ngẫu nhiên một Sprite từ mảng pikachuSprites
                GameObject brick;
                if (value == 0)
                {
                    brick = Instantiate(brickHelp, gridParent);
                }
                else
                {
                    int a = Random.Range(0, pikachuSprites.Length);
                    Sprite randomSprite = pikachuSprites[a];
                    brick = Instantiate(brickPrefab, gridParent);
                    SpriteRenderer spriteRenderer = brick.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = randomSprite;
                    matrix[row, column] = a+1;
                }

                // Đặt vị trí của viên gạch dựa trên hàng và cột, sử dụng kích thước của Sprite và vị trí của Sprite đầu tiên
                float xPosition = firstSpriteX + (column * brickWidth);
                float yPosition = firstSpriteY - (row * brickHeight);
                brick.transform.position = new Vector3(xPosition, yPosition, 0);

                // Sao chép script từ đối tượng gốc và gán cho bản sao
                ObjectAction originalObjectAction = brickPrefab.GetComponent<ObjectAction>();
                if (originalObjectAction != null)
                {
                    ObjectAction clonedObjectAction = brick.AddComponent<ObjectAction>();
                    // Gán các giá trị từ script gốc cho script trên bản sao (nếu có)
                    clonedObjectAction.enabled = originalObjectAction.enabled;
                    // Sao chép các thuộc tính khác của script (nếu có)
                }

                // Sao chép collider từ đối tượng gốc và gán cho bản sao
                Collider2D originalCollider = brickPrefab.GetComponent<Collider2D>();
                if (originalCollider != null)
                {
                    Collider2D clonedCollider = brick.GetComponent<Collider2D>();
                    // Kích hoạt collider trên bản sao
                    clonedCollider.enabled = true;
                    // Sao chép các thuộc tính khác của collider (nếu có)
                }
            }
        }
        //PrintMatrix();
    }
    public void PrintMatrix()
    {
        string matrixString = "";

        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            for (int column = 0; column < matrix.GetLength(1); column++)
            {
                matrixString += matrix[row, column] + " ";
            }

            matrixString += "\n";
        }

        Debug.Log("Matrix:\n" + matrixString);
    }

        //public void ReloadImages()
        //{
        //    // Thực hiện tải lại hình ảnh tương ứng với giá trị mới trong ma trận
        //    for (int row = 0; row < matrix.GetLength(0); row++)
        //    {
        //        for (int column = 0; column < matrix.GetLength(1); column++)
        //        {
        //            int value = matrix[row, column];
        //            GameObject brick = GetGameObjectAtPosition(row, column);
        //            SpriteRenderer spriteRenderer = brick.GetComponent<SpriteRenderer>();
        //            Sprite newSprite = GetSpriteByValue(value); // Hàm GetSpriteByValue là hàm của bạn để lấy sprite dựa trên giá trị trong ma trận
        //            spriteRenderer.sprite = newSprite;
        //        }
        //    }
        //}

        Sprite GetSpriteByValue(int value)
    {
        if (value >= 1 && value <= pikachuSprites.Length)
        {
            return pikachuSprites[value - 1];
        }
        else
        {
            Debug.LogError("Invalid value: " + value);
            return null;
        }
    }
    public GameObject GetGameObjectAtPosition(int row, int column)
    {
        int index = row * columns + column;
        if (index >= 0 && index < gridParent.childCount)
        {
            Transform cell = gridParent.GetChild(index);
            return cell.gameObject;
        }
        else
        {
            //Debug.LogError("Invalid position: (" + row + ", " + column + ")");
            return null;
        }
    }
    public int[,] GetMatrix()
    {
        return matrix;
    }
    // Update is called once per frame

    void Update()
    {

    }
}