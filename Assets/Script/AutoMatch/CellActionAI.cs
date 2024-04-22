using System;
using System.Collections;
using UnityEngine;
namespace Assets.Script.AutoMatch
{
    public class CellActionAI : CellAction
    {

        public float delayBetweenMatches = 0.5f;
        // Start is called before the first frame update
        void Start()
        {
            StartProcess();
            CheckFinalScore(score);
        }

        public override void StartProcess()
        {
            lineObject = new GameObject("line");
            lineObject.transform.SetParent(BaseAI.gridParent);
            score = 0;
            UpdateScoreText();
            // Thêm component LineRenderer vào game object mới
            lineRenderer = lineObject.AddComponent<LineRenderer>();
            StartCoroutine(AutoMatchAll());
            size = initialScript.getSize();
        }

        // public bool isPaused = false;

        public IEnumerator AutoMatchAll()
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
                            RestoreOriginalColor(obj.GetComponent<SpriteRenderer>());
                            yield return new WaitForSeconds(delayBetweenMatches);
                            CheckAndProcessMatrix3(obj);

                            if (BaseAI.MATRIX[i, j] != 0)
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