using System;
using System.Collections;
using UnityEngine;

namespace Assets.Script.Gravity
{
    public class CellActionGravity : CellAction
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
        }

        private void ReupdateMatrix()
        {
            GravityType gravity = new GravityType();

            /*if(randMode) gravity.ApplyGravity(true);        
            else gravity.ApplyGravity2(true);*/
            gravity.ApplyGravity(false);
            //Debug.Log(score + "x" + size);
            UpdateScoreText();
            if(BaseGravity._auto == true) { StartCoroutine(AutoMatchAll()); }
            //Debug.Log(scoreText.text);

        }

        // Kiểm tra và xử lý ma trận
        public override void CheckAndProcessMatrix(GameObject obj1, GameObject obj2)
        {

            var cell1 = BaseGravity.GetCell(obj1.name);
            var cell2 = BaseGravity.GetCell(obj2.name);
            if (cell1 == null || cell2 == null) return;
            // Kiểm tra nếu cả hai đối tượng có cùng số trong ma trận và không phải là cùng một đối tượng
            if (BaseGravity.MATRIX[cell1.i, cell1.j] == BaseGravity.MATRIX[cell2.i, cell2.j] && obj1 != obj2)
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
                    var val1 = BaseGravity.MATRIX[cell1.i, cell1.j];

                    BaseGravity.FREQUENCY[val1] -= 2;
                    if (!BaseGravity.FREQUENCY.ContainsKey(0))
                    {
                        BaseGravity.FREQUENCY[0] = 0;
                    }
                    BaseGravity.FREQUENCY[0] += 2;

                    BaseGravity.MATRIX[cell1.i, cell1.j] = 0;
                    BaseGravity.MATRIX[cell2.i, cell2.j] = 0;
                    Debug.Log(DateTime.Now.Millisecond + "<color=green>SUCCESS</color>");

                    ReupdateMatrix();

                }
                else
                {
                    // var pathRetry = FindPath(cell2, cell1);
                    Debug.Log(DateTime.Now.Millisecond + "<color=red>NO PATH FOUND</color>");
                }

            }
        }

        
        public IEnumerator AutoMatchAll()
        {
            bool allCellsProcessed = false;

            while (!allCellsProcessed)
            {

                allCellsProcessed = true;

                for (int i = 1; i <= BaseGravity.m; i++)
                {
                    for (int j = 1; j <= BaseGravity.n; j++)
                    {
                        GameObject obj = GameObject.Find(i + "," + j);

                        if (obj != null)
                        {
                            RestoreOriginalColor(obj.GetComponent<SpriteRenderer>());
                            yield return new WaitForSeconds(delayBetweenMatches);
                            CheckAndProcessMatrix3(obj);

                            if (BaseGravity.MATRIX[i, j] != 0)
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

            Cell cell = BaseGravity.GetCell(obj.name);
            if (cell == null)
            {
                Debug.Log("Cell not found for the GameObject.");
                return;
            }

            for (int k = 1; k <= BaseGravity.m; k++)
            {
                for (int l = 1; l <= BaseGravity.n; l++)
                {
                    GameObject obj2 = GameObject.Find(k + "," + l);
                    if (obj2 != null && obj != null)
                    {
                        if (BaseGravity.MATRIX[cell.i, cell.j] == BaseGravity.MATRIX[k, l] && !(cell.i == k && cell.j == l))
                        {
                            var path = FindPath(cell, BaseGravity.GetCell(obj2.name));
                            if (path != null)
                            {
                                var lstPos = GetListPosition(path);
                                DrawPath(lstPos);
                                Destroy(obj);
                                Destroy(obj2);
                                size = size - 2;
                                IncreaseScore();
                                var val1 = BaseGravity.MATRIX[cell.i, cell.j];

                                BaseGravity.FREQUENCY[val1] -= 2;
                                if (!BaseGravity.FREQUENCY.ContainsKey(0))
                                {
                                    BaseGravity.FREQUENCY[0] = 0;
                                }
                                BaseGravity.FREQUENCY[0] += 2;

                                BaseGravity.MATRIX[cell.i, cell.j] = 0;
                                BaseGravity.MATRIX[BaseGravity.GetCell(obj2.name).i, BaseGravity.GetCell(obj2.name).j] = 0;
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