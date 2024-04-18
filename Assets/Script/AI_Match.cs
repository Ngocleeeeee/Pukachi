using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AI_Match : MonoBehaviour
{
    private SpriteRenderer selectedImage; // Biến để lưu trữ ảnh đã chọn
    private SpriteRenderer matchedImage;
    public float delayBetweenMatches = 1.0f; // Thời gian chờ giữa 2 lần
    List<Vector2Int[]> potentialMatches = new List<Vector2Int[]>();
    private Dictionary<int, List<Vector2Int>> imagePositions; // Dictionary lưu trữ vị trí của các giá trị trong ma trận
    bool isMatrixEmpty = false;
    void Start()
    {
        imagePositions = new Dictionary<int, List<Vector2Int>>();
        StartCoroutine(AutoMatchImages());
    }

    void Update()
    {

    }

    IEnumerator AutoMatchImages()
    {
        Main mainScript = FindObjectOfType<Main>();

        while (!IsMatrixEmpty(mainScript))
        {
            yield return new WaitForSeconds(delayBetweenMatches);

            if (selectedImage == null || matchedImage == null)
            {
                FindAndSelectMatchingImages(mainScript);
                //mainScript.PrintMatrix();
            }
            else
            {
                CheckAndProcessMatrix(selectedImage.gameObject, matchedImage.gameObject);
                //mainScript.PrintMatrix();
            }
        }
    }

    bool IsMatrixEmpty(Main mainScript)
    {
        int[,] matrix = mainScript.matrix;
        int rows = matrix.GetLength(0);
        int columns = matrix.GetLength(1);

        // Duyệt qua ma trận và kiểm tra giá trị khác 0
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (matrix[row, column] != 0)
                {
                    return false; // Ma trận không rỗng
                }
            }
        }

        return true; // Ma trận rỗng
    }

    void FindAndSelectMatchingImages(Main mainScript)
    {
        int[,] matrix = mainScript.matrix;
        int rows = matrix.GetLength(0);
        int columns = matrix.GetLength(1);

        Dictionary<int, List<Vector2Int>> imagePositions = new Dictionary<int, List<Vector2Int>>(); // Lưu trữ vị trí các ảnh trùng nhau

        // Lặp qua ma trận và lưu trữ vị trí các ảnh trùng nhau
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                int value = matrix[row, column];
                if (value != 0)
                {
                    Vector2Int position = new Vector2Int(column, row);
                    if (!imagePositions.ContainsKey(value))
                        imagePositions[value] = new List<Vector2Int>();
                    imagePositions[value].Add(position);
                }
            }
        }

            // Duyệt qua danh sách các ảnh trùng nhau và tìm kiếm và xử lý
            foreach (var pair in imagePositions)
        {
            List<Vector2Int> positions = pair.Value;

            // Duyệt qua các cặp vị trí có thể nối được và thực hiện kiểm tra và xử lý
            for (int i = 0; i < positions.Count - 1; i++)
            {
                for (int j = i + 1; j < positions.Count; j++)
                {
                    Vector2Int startPosition = positions[i];
                    Vector2Int endPosition = positions[j];

                    List<Vector2Int> path = FindPath(matrix, startPosition, endPosition);

                    if (path != null)
                    {
                        int directionChanges = CountDirectionChanges(path);

                        if (directionChanges <= 2)
                        {
                            // Thực hiện kiểm tra và xử lý
                            Vector3 selectedPosition = GetPositionFromMatrix(mainScript, startPosition);
                            Vector3 matchingPosition = GetPositionFromMatrix(mainScript, endPosition);

                            SpriteRenderer selectedSpriteRenderer = GetSpriteRendererAtPosition(selectedPosition);
                            SpriteRenderer matchingSpriteRenderer = GetSpriteRendererAtPosition(matchingPosition);

                            if (selectedSpriteRenderer.sprite == matchingSpriteRenderer.sprite)
                            {
                                selectedImage = selectedSpriteRenderer;
                                matchedImage = matchingSpriteRenderer;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    Vector3 GetPositionFromMatrix(Main mainScript, Vector2Int position)
    {
        return new Vector3(
            mainScript.firstSpriteX + position.x * mainScript.brickWidth,
            mainScript.firstSpriteY - position.y * mainScript.brickHeight,
            0f
        );
    }
    List<Vector2Int> FindPath(int[,] matrix, Vector2Int start, Vector2Int end)
    {
        int rows = matrix.GetLength(0);
        int columns = matrix.GetLength(1);

        // Kiểm tra điểm xuất phát và điểm đích có hợp lệ trong ma trận không
        if (start.x < 0 || start.x >= columns || start.y < 0 || start.y >= rows ||
            end.x < 0 || end.x >= columns || end.y < 0 || end.y >= rows ||
            matrix[start.y, start.x] == 0 || matrix[end.y, end.x] == 0)
        {
            return null;
        }

        // Định nghĩa các hướng di chuyển: lên, xuống, trái, phải
        Vector2Int[] directions = {
            new Vector2Int(0, -1), // Up
            new Vector2Int(0, 1),  // Down
            new Vector2Int(-1, 0), // Left
            new Vector2Int(1, 0)   // Right
        };

        // Khởi tạo hàng đợi để duyệt các đỉnh theo BFS
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);

        // Khởi tạo từ điển để lưu trữ các đỉnh trước đó
        Dictionary<Vector2Int, Vector2Int> previous = new Dictionary<Vector2Int, Vector2Int>();
        previous[start] = start;

        // Bắt đầu tìm kiếm đường đi
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // Kiểm tra nếu đã tìm thấy điểm đích
            if (current == end)
            {
                // Tạo danh sách đường đi từ vị trí hiện tại cho đến điểm xuất phát
                List<Vector2Int> path = new List<Vector2Int>();
                path.Add(current);

                // Lặp ngược từ điểm đích cho đến điểm xuất phát để lấy đường đi
                while (current != start)
                {
                    current = previous[current];
                    path.Add(current);
                }
                path.Reverse();

                return path;
            }

            // Kiểm tra các hướng di chuyển từ vị trí hiện tại
            for (int i = 0; i < directions.Length; i++)
            {
                Vector2Int next = current + directions[i];

                // Kiểm tra xem vị trí tiếp theo có hợp lệ không
                bool isValidPosition = next.x >= 0 && next.x < columns && next.y >= 0 && next.y < rows;
                bool isEndPosition = next == end;
                bool isBlankPosition = isValidPosition && matrix[next.y, next.x] == 0;

                if ((isEndPosition || isBlankPosition) && !previous.ContainsKey(next))
                {
                    queue.Enqueue(next);
                    previous[next] = current;
                }
            }
        }

        // Không tìm thấy đường đi
        return null;
    }

    SpriteRenderer GetSpriteRendererAtPosition(Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);
        if (hit.collider != null)
        {
            return hit.collider.GetComponent<SpriteRenderer>();
        }

        return null;
    }

    // Kiểm tra và xử lý ma trận
    void CheckAndProcessMatrix(GameObject obj1, GameObject obj2)
    {
        Main mainScript = FindObjectOfType<Main>(); // Tìm đối tượng chứa script Main

        if (mainScript != null)
        {
            // Lấy thông tin vị trí của đối tượng 1 và 2 trong ma trận
            Vector3 position1 = obj1.transform.position;
            Vector3 position2 = obj2.transform.position;

            // Chuyển đổi vị trí thành chỉ số hàng và cột trong ma trận
            int row1 = Mathf.RoundToInt((mainScript.firstSpriteY - position1.y) / mainScript.brickHeight);
            int column1 = Mathf.RoundToInt((position1.x - mainScript.firstSpriteX) / mainScript.brickWidth);

            int row2 = Mathf.RoundToInt((mainScript.firstSpriteY - position2.y) / mainScript.brickHeight);
            int column2 = Mathf.RoundToInt((position2.x - mainScript.firstSpriteX) / mainScript.brickWidth);

            // Kiểm tra nếu cả hai đối tượng có cùng số trong ma trận và không phải là cùng một đối tượng
            if (mainScript.matrix[row1, column1] == mainScript.matrix[row2, column2] && obj1 != obj2)
            {
                // Kiểm tra đường đi giữa hai vật thể
                List<Vector2Int> path = FindPath(mainScript.matrix, new Vector2Int(column1, row1), new Vector2Int(column2, row2));
                // Đếm số lần đổi hướng trái-phải-lên-xuống
                // Nếu tồn tại đường đi
                if (path != null)
                {
                    int directionChanges = CountDirectionChanges(path);
                    //Nếu đường đi khả thi(<=2_, xóa cả hai đối tượng khỏi ma trận và khỏi scene
                    if (directionChanges <= 2)
                    {// Thêm vị trí điểm bắt đầu và điểm cuối của nối vào danh sách vị trí để vẽ đường đỏ

                        // Tiếp tục kiểm tra và xử lý
                        FindAndSelectMatchingImages(mainScript);
                        Destroy(obj1);
                        Destroy(obj2);
                        mainScript.matrix[row1, column1] = 0;
                        mainScript.matrix[row2, column2] = 0;
                        //Debug.Log("OK");
                    }
                    else
                    {
                        //Debug.Log("NO");
                    }
                }



            }
        }
    }


    int CountDirectionChanges(List<Vector2Int> path)
    {
        int directionChanges = 0;
        if (path.Count > 2)
        {
            Vector2Int previousDirection = path[1] - path[0];
            for (int i = 2; i < path.Count; i++)
            {
                Vector2Int currentDirection = path[i] - path[i - 1];
                if (currentDirection != previousDirection)
                {
                    directionChanges++;
                }
                previousDirection = currentDirection;
            }
        }
        return directionChanges;


    }
}
