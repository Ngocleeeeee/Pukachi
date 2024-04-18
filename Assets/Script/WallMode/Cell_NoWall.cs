using Assets.Script.WallMode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;

public class Cell_NoWall : MonoBehaviour
{
    private SpriteRenderer selectedImage; // Biến để lưu trữ ảnh đã chọn
    public int directionChanges = 0;
    Dictionary<Cell, Cell> PARENT;
    Cell CELL_FINAL, CELL_START;
    public Material lineMaterial;
    public float lineWidth = 0.16f;
    UnityEngine.Color c1 = UnityEngine.Color.yellow;
    UnityEngine.Color c2 = UnityEngine.Color.red;
    GameObject lineObject;
    LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
       lineObject = new GameObject("line");
       lineObject.transform.SetParent(Base.gridParent);

        // Thêm component LineRenderer vào game object mới
       lineRenderer = lineObject.AddComponent<LineRenderer>();
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
                            //clickedImage = null;
                        }
                        else // Nếu hai ảnh không giống nhau
                        {
                            selectedImage = null; // Chọn ảnh mới
                            //clickedImage = null;
                        }
                    }
                }
            }
        }
    }



    // Kiểm tra và xử lý ma trận
    void CheckAndProcessMatrix(GameObject obj1, GameObject obj2)
    {

        var cell1 = Base.GetCell(obj1.name);
        var cell2 = Base.GetCell(obj2.name);
        if (cell1 == null || cell2 == null) return;
        // Kiểm tra nếu cả hai đối tượng có cùng số trong ma trận và không phải là cùng một đối tượng
        if (Base.MATRIX[cell1.i, cell1.j] == Base.MATRIX[cell2.i, cell2.j] && obj1 != obj2)
        {
            // Kiểm tra đường đi giữa hai vật thể
            var path = FindPath(cell1, cell2);
            // Đếm số lần đổi hướng trái-phải-lên-xuống

            // Nếu tồn tại đường đi
            if (path != null)
            {
                // Vẽ đường thẳng đỏ giữa các vị trí
                var lstPos = GetListPosition(path);
                DrawPath(lstPos);
                Destroy(obj1);
                Destroy(obj2);
                var val1 = Base.MATRIX[cell1.i, cell1.j];

                Base.FREQUENCY[val1] -= 2;
                if (!Base.FREQUENCY.ContainsKey(0))
                {
                    Base.FREQUENCY[0] = 0;
                }
                Base.FREQUENCY[0] += 2;

                Base.MATRIX[cell1.i, cell1.j] = 0;
                Base.MATRIX[cell2.i, cell2.j] = 0;
                Debug.Log(DateTime.Now.Millisecond + "<color=green>SUCCESS</color>");

                //WALL MODE
               // Wall.RemoveABrick();
            
            }
            else
            {
               // var pathRetry = FindPath(cell2, cell1);
                Debug.Log(DateTime.Now.Millisecond + "<color=red>NO PATH FOUND</color>");
            }

        }
    }
    List<Cell> FindPath(Cell cellStart, Cell cellFinal)
    {
        if (!Base.IsCellInMaxtrix(cellStart) || !Base.IsCellInMaxtrix(cellFinal)) return null;

        // Khởi tạo hàng đợi để duyệt các đỉnh theo BFS
        Queue<Cell> queue = new Queue<Cell>();
        queue.Enqueue(cellStart);
       
        // Khởi tạo từ điển để lưu trữ các đỉnh trước đó
        Dictionary<Cell, Cell> parent = new Dictionary<Cell, Cell>();
        parent[cellStart] = cellStart;

        // Bắt đầu tìm kiếm đường đi
        //BFS
        while (queue.Count > 0)
        {
            Cell cellCurrent = queue.Dequeue();
            if (cellCurrent.Equals(cellFinal))
            {
                Debug.Log(DateTime.Now.Millisecond + "<color=green>MATCH</color>");
                // Tạo danh sách đường đi từ vị trí hiện tại cho đến điểm xuất phát
                var path = GetPath(cellCurrent, cellStart, parent);
                if (IsValidPath(path)) return path;
                else
                {
                    Debug.Log(DateTime.Now.Millisecond + "<color=yellow>PATH > 2</color>");
                    continue;
                }
            }

            foreach (var adj in cellCurrent.Move(cellFinal))
            {
                if (!parent.ContainsKey(adj))
                {

                    parent[adj] = cellCurrent;
                    var pathTMP = GetPath(adj, cellStart, parent);
                    if (GetChangeDirection(pathTMP) >= 3)
                    {
                        parent.Remove(adj);
                        continue;
                    }
                    queue.Enqueue(adj);

                }
            }

        }

        // Không tìm thấy đường đi
        //Tiếp tục chơi sang DFS
        PARENT = new Dictionary<Cell, Cell>();
        CELL_FINAL = cellFinal;
        CELL_START = cellStart;
        PARENT[CELL_START] = CELL_START;

        return DFS(cellStart);

    }

    
    private List<Cell> DFS(Cell cellCurrent)
    {
        var path = GetPath(cellCurrent, CELL_START, PARENT);
        if (cellCurrent.Equals(CELL_FINAL))
        {
            if (IsValidPath(path))
            {
                Debug.Log(DateTime.Now.Millisecond + "<color=green>DFS FOUNDED PATH</color>");
                return path;
            }
            return null;
        }
        if (GetChangeDirection(path) >= 3) return null;
        foreach (var adj in cellCurrent.Move(CELL_FINAL))
        {
            if (!PARENT.ContainsKey(adj))
            {
                PARENT[adj] = cellCurrent;
                var res = DFS(adj);
                if (res == null) { 
                    PARENT.Remove(adj);
                }
                else return res;
            }
        }
        return null;
    }
    
    private List<Cell> GetPath(Cell cellCurrent1, Cell cellStart1, Dictionary<Cell, Cell> parent)
    {

        List<Cell> path = new List<Cell>();
        var cellCurrent = new Cell(cellCurrent1.i, cellCurrent1.j);
        var cellStart = new Cell(cellStart1.i, cellStart1.j);
        path.Add(cellCurrent);

        // Lặp ngược từ điểm đích cho đến điểm xuất phát để lấy đường đi
        while (!cellCurrent.Equals(cellStart))
        {
            cellCurrent = parent[cellCurrent];
            path.Add(cellCurrent);
        }
        path.Reverse();
        PrintPath(path);
        
        return path;
    }
  
    private bool IsValidPath(List<Cell> path1) 
    {
        List<Cell> path = new List<Cell>();
        path.AddRange(path1);
        return GetChangeDirection(path) <= 2;
    }

    private int GetChangeDirection(List<Cell> path)
    {
        int cnt = 0;
        var baseCell = new Cell(path[0].i, path[0].j);
        for (int i = 1; i < path.Count; i++)
        {
            var moveCell = new Cell(path[i].i, path[i].j);
            if (baseCell.i != moveCell.i && baseCell.j != moveCell.j)
            {
                baseCell.SetCell(path[i-1]);
                cnt++;
            }

            //WALL MODE
            //if (Wall.IsBlockByBrick(path[i].i, path[i].j))
            //{
            //    return 100;
            //}

        }

        return cnt;
    }

    void PrintPath(List<Cell> path)
    {
        if (path == null)
        {
            Debug.Log("Không tìm thấy đường đi.");
            return;
        }

        Debug.Log("Đường đi từ điểm xuất phát đến điểm đích:");

        foreach (var point in path)
        {
            Debug.Log(DateTime.Now.Millisecond + "path (" + point.i + ", " + point.j + ")");
        }
    }

    private List<Vector3> GetListPosition(List<Cell> path)
    {
        var res = new List<Vector3>();
        Sprite firstSprite = Base.lstSprites[0];
        var brickWidth = firstSprite.bounds.size.x;
        var brickHeight = firstSprite.bounds.size.y;

        var brickPrefab = GameObject.FindWithTag("firstOBJ");
        var firstSpriteX = brickPrefab.transform.position.x + brickWidth/2f - lineWidth - 0.1f;
        var firstSpriteY = brickPrefab.transform.position.y - brickHeight/2f + lineWidth +0.2f;

        var startPos = new Vector3 (firstSpriteX, firstSpriteY);
        foreach (var point in path)
        {
            float xPosition = firstSpriteX + (point.j * brickWidth);
            float yPosition = firstSpriteY - (point.i * brickHeight);
            res.Add(new Vector3(xPosition, yPosition, 0));
        }

        return res;
    }

    private void DrawPath(List<Vector3> positions)
    {
        if(lineRenderer!=null) Destroy(lineRenderer.gameObject);
        DeleteObjectsWithName("line");
        // Nếu có ít nhất 2 GameObject được tìm thấy
        if (positions.Count >= 2)
        {
            // Tạo một game object mới để chứa đối tượng LineRenderer
            lineObject = new GameObject("line");
            lineObject.transform.SetParent(Base.gridParent);

            // Thêm component LineRenderer vào game object mới
            lineRenderer = lineObject.AddComponent<LineRenderer>();
            lineRenderer.material = lineMaterial;  // Gán vật liệu cho LineRenderer
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
           

            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.SetColors(c1, c2);

            // Đặt các điểm kết nối cho LineRenderer
            lineRenderer.positionCount = positions.Count;
            for (int i = 0; i < positions.Count; i++)
            {
                lineRenderer.SetPosition(i, positions[i]);
            }
            lineRenderer.sortingLayerName = "Default";
            lineRenderer.sortingOrder = 1;
            StartCoroutine(DestroyLineAfterDelay(1f, lineRenderer));
        }
    }

    private IEnumerator DestroyLineAfterDelay(float delay, LineRenderer lineRenderer)
    {
        yield return new WaitForSeconds(delay);
        Destroy(lineRenderer.gameObject);
    }
    private void DeleteObjectsWithName(string objectName)
    {
        GameObject[] objectsToDelete = GameObject.FindObjectsOfType<GameObject>();

        for (int i = 0; i < objectsToDelete.Length; i++)
        {
            if (objectsToDelete[i].name == objectName)
            {
                // Sử dụng Destroy hoặc DestroyImmediate để xóa game object
                // Nếu bạn không cần xóa ngay lập tức, hãy sử dụng Destroy
                // Nếu bạn cần xóa ngay lập tức, hãy sử dụng DestroyImmediate
                DestroyImmediate(objectsToDelete[i]);
                // DestroyImmediate(objectsToDelete[i]);
            }
        }
    }
}