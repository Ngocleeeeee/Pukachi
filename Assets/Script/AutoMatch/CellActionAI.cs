using System;
using System.Collections;
using UnityEngine;
namespace Assets.Script.AutoMatch
{
    public class CellActionAI : CellAction
    {
        public static bool isAutoMatching = true;
        public float delayBetweenMatches = 0.5f;
        // Start is called before the first frame update
        void Start()
        {
            StartProcess();
            
        }
        void Update()
        {
            CheckFinalScore(score);
        }

        public override void StartProcess()
        {
            lineObject = new GameObject("line");
            lineObject.transform.SetParent(BaseAI.gridParent);
            // Thêm component LineRenderer vào game object mới
            lineRenderer = lineObject.AddComponent<LineRenderer>();
            StartCoroutine(AutoMatchAll());
            size = initialScript.getSize();
        }

        // public bool isPaused = false;

        //----------------------------
        public void SetAutoMatching(bool value)
        {
            isAutoMatching = value;
            Debug.Log("SET" + value);
        }
        IEnumerator AutoMatchAll()
        {
            while (true) // Vòng lặp vô hạn
            {
                Debug.Log("isauto matching" + IsAutoMatching());

                // Kiểm tra trạng thái của isAutoMatching trước mỗi lần lặp


                bool allCellsProcessed = true; // Gán allCellsProcessed = true mặc định

                // Kiểm tra mỗi ô trong ma trận
                for (int i = 1; i <= BaseAI.m; i++)
                {
                    for (int j = 1; j <= BaseAI.n; j++)
                    {
                        if (BaseAI.MATRIX[i, j] == 0) continue;
                        GameObject obj = GameObject.Find(i + "," + j);
                        if (obj != null)
                        {
                            RestoreOriginalColor(obj.GetComponent<SpriteRenderer>());
                            yield return new WaitForSeconds(delayBetweenMatches);
                            CheckAndProcessMatrix3(obj);
                            if (IsAutoMatching() == false)
                            {
                                //Hien panel 
                                yield break; // Nếu isAutoMatching là false, thoát coroutine

                            }
                            if (BaseAI.MATRIX[i, j] != 0)
                            {
                                allCellsProcessed = false; // Nếu còn ô chưa xử lý, đặt allCellsProcessed = false
                            }
                        }
                    }
                }

                // Nếu tất cả các ô đã được xử lý hoặc hết thời gian đếm ngược
                /*if (allCellsProcessed || size == 0 || countdownTimer.getCurrentTime() <= 0f)
                {
                    //hien panel end game
                    score *= Mathf.CeilToInt(countdownTimer.getCurrentTime());
                    UpdateScoreText();
                    Debug.Log("end game");
                    yield break; // Kết thúc coroutine
                }*/

                yield return null; // Yielding null allows other coroutine operations to execute.
            }
        }


        private bool IsAutoMatching()
        {
            
            Debug.Log("end game" + isAutoMatching);
            return isAutoMatching;
        }

        // Gọi phương thức này từ nơi bạn muốn cập nhật giá trị của isAutoMatching
        public void PauseAutoMatching()
        {
            SetAutoMatching(false);
            //muon pause thi se sett false
        }

        public void ResumeAutoMatching()
        {
            SetAutoMatching(true);
            StartCoroutine(AutoMatchAll());
        }

        //--------------------------

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

            Cell cell = BaseAI.GetCell(obj.name);
            if (cell == null)
            {
                Debug.Log("Cell not found for the GameObject.");
                return;
            }

            for (int k = 1; k <= BaseAI.m; k++)
            {
                for (int l = 1; l <= BaseAI.n; l++)
                {
                    if (BaseAI.MATRIX[k, l] == 0) continue;
                    GameObject obj2 = GameObject.Find(k + "," + l);
                    if (obj2 != null && obj != null)
                    {
                        if (BaseAI.MATRIX[cell.i, cell.j] == BaseAI.MATRIX[k, l] && !(cell.i == k && cell.j == l))
                        {
                            var path = FindPath(cell, BaseAI.GetCell(obj2.name));
                            if (path != null)
                            {
                                var lstPos = GetListPosition(path);
                                DrawPath(lstPos);
                                Destroy(obj);
                                Destroy(obj2);
                                size = size - 2;
                                IncreaseScore();
                                var val1 = BaseAI.MATRIX[cell.i, cell.j];

                                BaseAI.FREQUENCY[val1] -= 2;
                                if (!BaseAI.FREQUENCY.ContainsKey(0))
                                {
                                    BaseAI.FREQUENCY[0] = 0;
                                }
                                BaseAI.FREQUENCY[0] += 2;

                                BaseAI.MATRIX[cell.i, cell.j] = 0;
                                BaseAI.MATRIX[BaseAI.GetCell(obj2.name).i, BaseAI.GetCell(obj2.name).j] = 0;
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