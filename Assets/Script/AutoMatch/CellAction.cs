using Assets.Script.WallMode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Script.AutoMatch
{
    public class CellAction : MonoBehaviour
    {
        public SpriteRenderer selectedImage; // Biến để lưu trữ ảnh đã chọn
        public Dictionary<Cell, Cell> PARENT;
        public Material lineMaterial;
        public GameObject lineObject;
        public LineRenderer lineRenderer;
        public Text scoreText, finalScoreText;
        public Cell CELL_FINAL, CELL_START;
        public CountdownTimer countdownTimer;
        public _InitialScriptAI initialScript;

        public int directionChanges = 0;
        public static int score, size;
        public float lineWidth = 0.16f;

        public UnityEngine.Color c1 = UnityEngine.Color.yellow;
        public UnityEngine.Color c2 = UnityEngine.Color.red;
        public UnityEngine.Color highlightColor = UnityEngine.Color.blue;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public virtual void StartProcess()
        {
            lineObject = new GameObject("line");
            lineObject.transform.SetParent(BaseAI.gridParent);
            score = 0;
            UpdateScoreText();
            // Thêm component LineRenderer vào game object mới
            lineRenderer = lineObject.AddComponent<LineRenderer>();
            size = initialScript.getSize();

        }

        public virtual void UpdateProcess()
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

        public virtual void CheckFinalScore(int score)
        {
            if (size == 0 || !countdownTimer.isCountingDown)
            {
                score = (score != 0) ? score * Mathf.CeilToInt(countdownTimer.getCurrentTime())
                    : (Mathf.CeilToInt(countdownTimer.getCurrentTime()) * CheckPoint() / 2);
                finalScoreText.text = score.ToString();

            }

        }

        public int CheckPoint()
        {
            int point = 0;
            for (int column = 1; column < BaseAI.MATRIX.GetLength(1) - 1; column++)
            {
                for (int row = 1; row < BaseAI.MATRIX.GetLength(0) - 1; row++)
                {
                    if (BaseAI.MATRIX[row, column] == 0)
                    {
                        point++;

                    }
                }
            }
            return point;
        }

        // Kiểm tra và xử lý ma trận
        public virtual void CheckAndProcessMatrix(GameObject obj1, GameObject obj2)
        {

            var cell1 = BaseAI.GetCell(obj1.name);
            var cell2 = BaseAI.GetCell(obj2.name);
            if (cell1 == null || cell2 == null) return;
            // Kiểm tra nếu cả hai đối tượng có cùng số trong ma trận và không phải là cùng một đối tượng
            if (BaseAI.MATRIX[cell1.i, cell1.j] == BaseAI.MATRIX[cell2.i, cell2.j] && obj1 != obj2)
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
                    size = size - 2;
                    IncreaseScore();
                    var val1 = BaseAI.MATRIX[cell1.i, cell1.j];

                    BaseAI.FREQUENCY[val1] -= 2;
                    if (!BaseAI.FREQUENCY.ContainsKey(0))
                    {
                        BaseAI.FREQUENCY[0] = 0;
                    }
                    BaseAI.FREQUENCY[0] += 2;

                    BaseAI.MATRIX[cell1.i, cell1.j] = 0;
                    BaseAI.MATRIX[cell2.i, cell2.j] = 0;
                    Debug.Log(DateTime.Now.Millisecond + "<color=green>SUCCESS</color>");

                }
                else
                {
                    // var pathRetry = FindPath(cell2, cell1);
                    Debug.Log(DateTime.Now.Millisecond + "<color=red>NO PATH FOUND</color>");

                }

            }
        }

        public virtual List<Cell> FindPath(Cell cellStart, Cell cellFinal)
        {
            if (!BaseAI.IsCellInMaxtrix(cellStart) || !BaseAI.IsCellInMaxtrix(cellFinal)) return null;

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
                        Debug.Log("------------ERROR-----------");
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

        public virtual List<Cell> DFS(Cell cellCurrent)
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

        public virtual List<Cell> GetPath(Cell cellCurrent1, Cell cellStart1, Dictionary<Cell, Cell> parent)
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

        public virtual bool IsValidPath(List<Cell> path1)
        {
            List<Cell> path = new List<Cell>();
            path.AddRange(path1);

            return GetChangeDirection(path) <= 2;
        }

        public virtual int GetChangeDirection(List<Cell> path)
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

            }

            return cnt;
        }

        public virtual void PrintPath(List<Cell> path)
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

        public virtual List<Vector3> GetListPosition(List<Cell> path)
        {
            var res = new List<Vector3>();
            Sprite firstSprite = BaseAI.lstSprites[0];
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

        public virtual void DrawPath(List<Vector3> positions)
        {
            if (lineRenderer != null) Destroy(lineRenderer.gameObject);
            DeleteObjectsWithName("line");
            // Nếu có ít nhất 2 GameObject được tìm thấy
            if (positions.Count >= 2)
            {
                // Tạo một game object mới để chứa đối tượng LineRenderer
                lineObject = new GameObject("line");
                lineObject.transform.SetParent(BaseAI.gridParent);

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

        public virtual IEnumerator DestroyLineAfterDelay(float delay, LineRenderer lineRenderer)
        {
            yield return new WaitForSeconds(delay);
            if (lineRenderer != null)
            {
                Destroy(lineRenderer.gameObject);
            }
        }

        public virtual void DeleteObjectsWithName(string objectName)
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

        public virtual void IncreaseScore()
        {
            if (countdownTimer.isCountingDown)
            {
                score += 1;

                UpdateScoreText();
            }
        }

        public virtual void UpdateScoreText()
        {
            scoreText.text = "Score: " + score.ToString();
        }

        public virtual void Hint()
        {
            bool allCellsProcessed = false;

            while (!allCellsProcessed)
            {
                allCellsProcessed = true;

                for (int i = 1; i <= BaseAI.m; i++)
                {
                    for (int j = 1; j <= BaseAI.n; j++)
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

        public virtual bool CheckAndProcessMatrix2(GameObject obj)
        {
            if (obj == null)
            {
                Debug.Log("GameObject is null.");
                return false;
            }



            Cell cell = BaseAI.GetCell(obj.name);
            if (cell == null)
            {
                Debug.Log("Cell not found for the GameObject.");
                return false;
            }

            for (int k = 1; k <= BaseAI.m; k++)
            {
                for (int l = 1; l <= BaseAI.n; l++)
                {
                    GameObject obj2 = GameObject.Find(k + "," + l);
                    if (obj2 != null && obj != null)
                    {
                        if (BaseAI.MATRIX[cell.i, cell.j] == BaseAI.MATRIX[k, l] && !(cell.i == k && cell.j == l))
                        {
                            var path = FindPath(cell, BaseAI.GetCell(obj2.name));
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

        public virtual void SetHighlightedColor(SpriteRenderer renderer)
        {
            renderer.color = highlightColor;
        }

        public virtual void RestoreOriginalColor(SpriteRenderer renderer)
        {
            // Đặt màu ban đầu của vật thể tại đây (ví dụ: màu trắng)
            renderer.color = UnityEngine.Color.white;
        }
    }
}