using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Script.WallMode
{
    public class CellActionWall : CellAction
    {

        public float delayBetweenMatches = 0.5f;
        // Start is called before the first frame update
        void Start()
        {
            StartProcess();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateProcess();
            CheckFinalScore(score);
            if (BaseWall._auto == true) { StartCoroutine(AutoMatchAll()); }
        }

        // Kiểm tra và xử lý ma trận
        public override void CheckAndProcessMatrix(GameObject obj1, GameObject obj2)
        {

            var cell1 = BaseWall.GetCell(obj1.name);
            var cell2 = BaseWall.GetCell(obj2.name);
            if (cell1 == null || cell2 == null) return;
            // Kiểm tra nếu cả hai đối tượng có cùng số trong ma trận và không phải là cùng một đối tượng
            if (BaseWall.MATRIX[cell1.i, cell1.j] == BaseWall.MATRIX[cell2.i, cell2.j] && obj1 != obj2)
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
                    var val1 = BaseWall.MATRIX[cell1.i, cell1.j];

                    BaseWall.FREQUENCY[val1] -= 2;
                    if (!BaseWall.FREQUENCY.ContainsKey(0))
                    {
                        BaseWall.FREQUENCY[0] = 0;
                    }
                    BaseWall.FREQUENCY[0] += 2;

                    BaseWall.MATRIX[cell1.i, cell1.j] = 0;
                    BaseWall.MATRIX[cell2.i, cell2.j] = 0;
                    Debug.Log(DateTime.Now.Millisecond + "<color=green>SUCCESS</color>");

                    //WALL MODE
                    Wall.RemoveABrick();

                }
                else
                {
                    // var pathRetry = FindPath(cell2, cell1);
                    Debug.Log(DateTime.Now.Millisecond + "<color=red>NO PATH FOUND</color>");
                }

            }
        }
        public override int GetChangeDirection(List<Cell> path)
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
                if (Wall.IsBlockByBrick(path[i].i, path[i].j))
                {
                    return 100;
                }

            }

            return cnt;
        }

        public IEnumerator AutoMatchAll()
        {
            bool allCellsProcessed = false;

            while (!allCellsProcessed)
            {

                allCellsProcessed = true;

                for (int i = 1; i <= BaseWall.m; i++)
                {
                    for (int j = 1; j <= BaseWall.n; j++)
                    {
                        GameObject obj = GameObject.Find(i + "," + j);

                        if (obj != null)
                        {
                            RestoreOriginalColor(obj.GetComponent<SpriteRenderer>());
                            yield return new WaitForSeconds(delayBetweenMatches);
                            CheckAndProcessMatrix3(obj);

                            if (BaseWall.MATRIX[i, j] != 0)
                            {
                                allCellsProcessed = false;
                            }
                        }
                    }
                }

                yield return null; // Yielding null allows other coroutine operations to execute.
            }

            if (size == 0 || countdownTimer.getCurrentTime() == 0f)
            {
                score *= Mathf.CeilToInt(countdownTimer.getCurrentTime());
                UpdateScoreText();
                Debug.Log("end game");
            }
            Debug.Log("All cells have been processed.");
        }

        // Function to pause the AutoMatchAll coroutine
        public void PauseAutoMatchAll()
        {
            StopCoroutine(AutoMatchAll());
        }

        // Function to resume the AutoMatchAll coroutine
        public void ResumeAutoMatchAll()
        {
            StartCoroutine(AutoMatchAll());
        }

        void CheckAndProcessMatrix3(GameObject obj)
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

            Cell cell = BaseWall.GetCell(obj.name);
            if (cell == null)
            {
                Debug.Log("Cell not found for the GameObject.");
                return;
            }

            for (int k = 1; k <= BaseWall.m; k++)
            {
                for (int l = 1; l <= BaseWall.n; l++)
                {
                    GameObject obj2 = GameObject.Find(k + "," + l);
                    if (obj2 != null && obj != null)
                    {
                        if (BaseWall.MATRIX[cell.i, cell.j] == BaseWall.MATRIX[k, l] && !(cell.i == k && cell.j == l))
                        {
                            var path = FindPath(cell, BaseWall.GetCell(obj2.name));
                            if (path != null)
                            {
                                var lstPos = GetListPosition(path);
                                DrawPath(lstPos);
                                Destroy(obj);
                                Destroy(obj2);
                                size = size - 2;
                                IncreaseScore();
                                var val1 = BaseWall.MATRIX[cell.i, cell.j];

                                BaseWall.FREQUENCY[val1] -= 2;
                                if (!BaseWall.FREQUENCY.ContainsKey(0))
                                {
                                    BaseWall.FREQUENCY[0] = 0;
                                }
                                BaseWall.FREQUENCY[0] += 2;

                                BaseWall.MATRIX[cell.i, cell.j] = 0;
                                BaseWall.MATRIX[BaseWall.GetCell(obj2.name).i, BaseWall.GetCell(obj2.name).j] = 0;
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
    }
}