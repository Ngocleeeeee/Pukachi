using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Script.WallMode
{
    public class Wall
    {
        public int NumberOfWall { get; set; }
        public List<int> Rows { get; set; }
        public List<int> Cols { get; set; }
        private static List<GameObject> lstWallObject;
        private static List<Cell> lstBrick;

        public Wall(int[] rows, int[] cols)
        {
            Rows = getRandomWall(rows, false);
            Cols = getRandomWall(cols);
        }

        private List<int> getRandomWall(int[] numbers, bool full = true)
        {
            var res = new List<int>();
            System.Random random = new System.Random();
            int randomNumber1 = numbers[random.Next(numbers.Length)];
            int randomNumber2;
            do
            {
                randomNumber2 = numbers[random.Next(numbers.Length)];
            } while (randomNumber2 == randomNumber1);
            res.Add(randomNumber1);
            if (full) res.Add(randomNumber2);

            return res;
        }

        public void GenerateWall()
        {
            DrawWall();
        }

        private List<Cell> GetListCell()
        {

            lstBrick = new List<Cell>();
            for (int j = 0; j < BaseWall.n + 2; j++)
            {
                foreach (var row in Rows)
                {
                    lstBrick.Add(new Cell(row, j));
                }

            }

            for (int i = 0; i < BaseWall.m + 2; i++)
            {
                foreach (var col in Cols)
                {
                    var newCell = new Cell(i, col);
                    if (!lstBrick.Any(c => c.Equals(newCell)))
                    {
                        lstBrick.Add(newCell);
                    }

                }

            }
            return lstBrick;
        }

        private void DrawWall()
        {
            Sprite firstSprite = BaseWall.lstSprites[0];
            var brickWidth = firstSprite.bounds.size.x;
            var brickHeight = firstSprite.bounds.size.y;

            var brickPrefab = GameObject.FindWithTag("firstOBJ");
            var firstSpriteX = brickPrefab.transform.position.x;
            var firstSpriteY = brickPrefab.transform.position.y;

            var wallOBJ = GameObject.FindWithTag("wallObj");
            var lstBrick = GetListCell();
            lstWallObject = new List<GameObject>();
            foreach (var cell in lstBrick)
            {
                GameObject brick;
                brick = GameObject.Instantiate(wallOBJ, BaseWall.gridParent);
                brick.name = $"!{cell.i}:{cell.j}";
                SpriteRenderer spriteRenderer = brick.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder = 100;

                float xPosition = firstSpriteX + (cell.j * brickWidth);
                float yPosition = firstSpriteY - (cell.i * brickHeight);
                brick.transform.position = new Vector3(xPosition, yPosition, 0);

                lstWallObject.Add(brick);
            }
        }

        public static void RemoveABrick()
        {
            if (lstWallObject.Count == 0) return;
            System.Random random = new System.Random();
            var randomNumber = random.Next(0, lstWallObject.Count - 1);
            var selectedBrick = lstWallObject[randomNumber];
            if (selectedBrick != null)
            {
                GameObject.Destroy(selectedBrick.gameObject);
                lstWallObject.Remove(selectedBrick);
                //delete position
                var nameBrick = selectedBrick.gameObject.name;

                var nameBrickByCell = nameBrick.Split(":");
                var cellX = int.Parse(nameBrickByCell[0].Substring(1));
                var cellY = int.Parse(nameBrickByCell[1]);
                var selectedPos = lstBrick.Find(br => br.i == cellX && br.j == cellY);
                if (selectedPos != null)
                {
                    lstBrick.Remove(selectedPos);
                }
            }
        }

        public static bool IsBlockByBrick(int i, int j)
        {
            return lstBrick.Any(br => br.i == i && br.j == j);
        }


    }
}
