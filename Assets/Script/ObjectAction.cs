using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class ObjectAction : MonoBehaviour
{
    private SpriteRenderer selectedImage; // Biến để lưu trữ ảnh đã chọn
    public int directionChanges = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Kiểm tra khi nhấn chuột trái
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                SpriteRenderer clickedImage = hit.collider.GetComponent<SpriteRenderer>();

                if (clickedImage != null && clickedImage != selectedImage) // Kiểm tra xem ảnh được chọn có hợp lệ và không phải là ảnh đã được chọn trước đó
                {
                    if (selectedImage == null) // Nếu đây là ảnh đầu tiên được chọn
                    {
                        selectedImage = clickedImage;
                    }
                    else // Nếu đã có ảnh được chọn trước đó
                    {
                        if (selectedImage.sprite == clickedImage.sprite) // Kiểm tra nếu hai ảnh giống nhau
                        {
                            // Kiểm tra và xử lý ma trận
                            CheckAndProcessMatrix(selectedImage.gameObject, clickedImage.gameObject);

                            // Đặt selectedImage về null để chuẩn bị cho việc chọn ảnh mới
                            selectedImage = null;
                        }
                        else // Nếu hai ảnh không giống nhau
                        {
                            selectedImage = null; // Chọn ảnh mới
                        }
                    }
                }
            }
        }
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
                    if ( directionChanges <= 2)
                    {
                        Destroy(obj1);
                        Destroy(obj2);
                        mainScript.matrix[row1, column1] = 0;
                        mainScript.matrix[row2, column2] = 0;
                        Debug.Log("OK");
                    }
                    else
                    {
                        Debug.Log("NO");
                    }
                }
                    
                
                
            }
        }
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
    // biến `directionChanges` để lưu trữ số lần đổi hướng. Đối với mỗi đường đi tìm thấy,gọi hàm `CountDirectionChanges` để đếm số lần đổi hướng trái-phải-lên-xuống.
    int CountDirectionChanges(List<Vector2Int> path)
    {
        int directionChanges = 0;
        if (path.Count > 1)
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