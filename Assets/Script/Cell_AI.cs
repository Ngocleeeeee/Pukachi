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

public class Cell_AI : MonoBehaviour
{
    public int directionChanges = 0;
    Dictionary<Cell, Cell> PARENT;
    Cell CELL_FINAL, CELL_START;
    public Material lineMaterial;
    public float lineWidth = 0.16f;
    UnityEngine.Color c1 = UnityEngine.Color.yellow;
    UnityEngine.Color c2 = UnityEngine.Color.red;
    GameObject lineObject;
    LineRenderer lineRenderer;
    public float delayBetweenMatches = 0.5f;
    // Start is called before the first frame update
    public UnityEngine.Color highlightColor = UnityEngine.Color.blue;
    void Start()
    {
        lineObject = new GameObject("line");
        lineObject.transform.SetParent(Base.gridParent);

        // Thêm component LineRenderer vào game object mới
        lineRenderer = lineObject.AddComponent<LineRenderer>();
        StartCoroutine(AutoMatchAll());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator AutoMatchAll()
    {
        bool allCellsProcessed = false;

        while (!allCellsProcessed)
        {
            allCellsProcessed = true;

            for (int i = 1; i <= Base.m; i++)
            {
                for (int j = 1; j <= Base.n; j++)
                {
                    GameObject obj = GameObject.Find(i + "," + j);

                    if (obj != null)
                    {
                        RestoreOriginalColor(obj.GetComponent<SpriteRenderer>());
                        yield return new WaitForSeconds(delayBetweenMatches);
                        CheckAndProcessMatrix(obj);

                        if (Base.MATRIX[i, j] != 0)
                        {
                            allCellsProcessed = false;
                        }
                    }
                }
            }
        }

        Debug.Log("All cells have been processed.");
    }
    public void Hint()
    {
        bool allCellsProcessed = false;

        while (!allCellsProcessed)
        {
            allCellsProcessed = true;

            for (int i = 1; i <= Base.m; i++)
            {
                for (int j = 1; j <= Base.n; j++)
                {
                    GameObject obj = GameObject.Find(i + "," + j);
                    
                    if (obj != null)
                    {

                        if (CheckAndProcessMatrix2(obj) == true)
                        {
                            var renderer = obj.GetComponent<SpriteRenderer>();
                            SetHighlightedColor(renderer);
                            return;
                        }

                    }
                }
            }
        }
    }




    // Biến để lưu trữ màu bôi đậm cho vật thể được chọn


    void CheckAndProcessMatrix(GameObject obj)
    {
        if (obj == null)
        {
            Debug.Log("GameObject is null.");
            return;
        }

        var renderer = obj.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            Debug.Log("SpriteRenderer component not found on the GameObject.");
            return;
        }

        SetHighlightedColor(renderer);

        Cell cell = Base.GetCell(obj.name);
        if (cell == null)
        {
            Debug.Log("Cell not found for the GameObject.");
            return;
        }

        for (int k = 1; k <= Base.m; k++)
        {
            for (int l = 1; l <= Base.n; l++)
            {
                GameObject obj2 = GameObject.Find(k + "," + l);
                if (obj2 != null && obj != null)
                {
                    if (Base.MATRIX[cell.i, cell.j] == Base.MATRIX[k, l] && !(cell.i == k && cell.j == l))
                    {
                        var path = FindPath(cell, Base.GetCell(obj2.name));
                        if (path != null)
                        {
                            var lstPos = GetListPosition(path);
                            DrawPath(lstPos);
                            Destroy(obj);
                            Destroy(obj2);
                            var val1 = Base.MATRIX[cell.i, cell.j];

                            Base.FREQUENCY[val1] -= 2;
                            if (!Base.FREQUENCY.ContainsKey(0))
                            {
                                Base.FREQUENCY[0] = 0;
                            }
                            Base.FREQUENCY[0] += 2;

                            Base.MATRIX[cell.i, cell.j] = 0;
                            Base.MATRIX[Base.GetCell(obj2.name).i, Base.GetCell(obj2.name).j] = 0;
                            RestoreOriginalColor(renderer);
                            Debug.Log(DateTime.Now.Millisecond + "<color=green>SUCCESS</color>");
                            return;
                        }
                        else
                        {
                            Debug.Log(DateTime.Now.Millisecond + "<color=red>NO PATH FOUND</color>");
                        }
                    }
                }
            }
        }
    }
    bool CheckAndProcessMatrix2(GameObject obj)
    {
        if (obj == null)
        {
            Debug.Log("GameObject is null.");
            return false;
        }

        

        Cell cell = Base.GetCell(obj.name);
        if (cell == null)
        {
            Debug.Log("Cell not found for the GameObject.");
            return false;
        }

        for (int k = 1; k <= Base.m; k++)
        {
            for (int l = 1; l <= Base.n; l++)
            {
                GameObject obj2 = GameObject.Find(k + "," + l);
                if (obj2 != null && obj != null)
                {
                    if (Base.MATRIX[cell.i, cell.j] == Base.MATRIX[k, l] && !(cell.i == k && cell.j == l))
                    {
                        var path = FindPath(cell, Base.GetCell(obj2.name));
                        if (path != null)
                        {
                            var renderer = obj2.GetComponent<SpriteRenderer>();
                            //DrawPath(lstPos);
                            SetHighlightedColor(renderer);

                            return true;
                        }
                        else
                        {
                            Debug.Log(DateTime.Now.Millisecond + "<color=red>NO PATH FOUND</color>");
                        }
                    }
                }
            }

        }
        return false;
    }


    // Phương thức để đặt màu bôi đậm cho vật thể được chọn
    void SetHighlightedColor(SpriteRenderer renderer)
    {
        renderer.color = highlightColor;
    }

    // Phương thức để trả lại màu ban đầu của vật thể
    void RestoreOriginalColor(SpriteRenderer renderer)
    {
        // Đặt màu ban đầu của vật thể tại đây (ví dụ: màu trắng)
        renderer.color = UnityEngine.Color.white;
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
                if (res == null)
                {
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
                baseCell.SetCell(path[i - 1]);
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
        var firstSpriteX = brickPrefab.transform.position.x + brickWidth / 2f - lineWidth - 0.1f;
        var firstSpriteY = brickPrefab.transform.position.y - brickHeight / 2f + lineWidth + 0.2f;

        var startPos = new Vector3(firstSpriteX, firstSpriteY);
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
        if (lineRenderer != null) Destroy(lineRenderer.gameObject);
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
        if (lineRenderer != null)
        {
            Destroy(lineRenderer.gameObject);
        }
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