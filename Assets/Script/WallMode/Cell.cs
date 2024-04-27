using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Script.WallMode
{
    public class Cell
    {
        public int i { get; set; }
        public int j { get; set; }
        public int val { get; set; }

        public Cell(int i, int j)
        {
            this.i = i;
            this.j = j;
        }
        public Cell(int i, int j, int val)
        {
            this.i = i;
            this.j = j;
            this.val = val;
        }
        public void SetCell(Cell cell2)
        {
            this.i = cell2.i;
            this.j = cell2.j;
        }

        public List<Cell> Move(Cell cellFinal)
        {

            var lstNextState = new List<Cell>();
            var lstCase = new List<Cell>();

            if (j + 1 <= BaseWall.n + 1) lstCase.Add(new Cell(i, j + 1));
            if (i + 1 <= BaseWall.m + 1) lstCase.Add(new Cell(i + 1, j));

            if (j - 1 >= 0) lstCase.Add(new Cell(i, j - 1));
            if (i - 1 >= 0) lstCase.Add(new Cell(i - 1, j));
            foreach (var c in lstCase)
            {

                if (BaseWall.MATRIX[c.i, c.j] == 0 || c.Equals(cellFinal))
 
                {
                    lstNextState.Add(c);
                }
            }
            //string s = "( "+i+", "+j+"): ";
            //foreach (var c in lstNextState)
            //{
            //    s += "[" + c.i + ", " + c.j + "], ";
            //}
            //Debug.Log(DateTime.Now.Millisecond + s);
            return lstNextState;

        }

        public override bool Equals(object obj)
        {
            return obj is Cell cell &&
                   i == cell.i &&
                   j == cell.j;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(i, j);
        }

        public static Cell operator -(Cell cell1, Cell cell2)
        {
            int resultX = cell1.i - cell2.i;
            int resultY = cell1.j - cell2.j;
            return new Cell(resultX, resultY);
        }


        public static GameObject GetGameObject(Cell cell)
        {
            string objName = cell.i + "," + cell.j;
            GameObject gridParentObject = GameObject.FindWithTag("Grid");

            // Kiểm tra nếu đối tượng cha tồn tại
            if (gridParentObject != null)
            {
                // Lấy danh sách các đối tượng con
                Transform parentTransform = gridParentObject.transform;
                int childCount = parentTransform.childCount;

                for (int i = 0; i < childCount; i++)
                {
                    var childObj = parentTransform.GetChild(i).gameObject;
                    if (childObj.name == objName) return childObj;
                }

            }
            return null;
        }


    }
}

