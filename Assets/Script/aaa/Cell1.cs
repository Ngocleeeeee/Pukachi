using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.WallMode
{
    public class Cell1
    {
        public int i { get; set; }
        public int j { get; set; }
        public int val { get; set; }

        public Cell1(int i, int j)
        {
            this.i = i;
            this.j = j;
        }
        public Cell1(int i, int j, int val)
        {
            this.i = i;
            this.j = j;
            this.val = val;
        }
        public void SetCell(Cell1 cell2)
        {
            this.i = cell2.i;
            this.j = cell2.j;
        }

        public List<Cell1> Move(Cell1 cellFinal)
        {

            var lstNextState = new List<Cell1>();
            var lstCase = new List<Cell1>();

            if (j + 1 <= Base1.n+1) lstCase.Add(new Cell1(i, j + 1));
            if (i + 1 <= Base1.m+1) lstCase.Add(new Cell1(i + 1, j));
            if (j - 1 >= 0) lstCase.Add(new Cell1(i, j - 1));
            if (i - 1 >= 0) lstCase.Add(new Cell1(i - 1, j));
            foreach (var c in lstCase)
            {
                if (Base1.MATRIX[c.i, c.j] == 0|| c.Equals(cellFinal))
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
            return obj is Cell1 cell &&
                   i == cell.i &&
                   j == cell.j;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(i, j);
        }

        public static Cell1 operator -(Cell1 cell1, Cell1 cell2)
        {
            int resultX = cell1.i - cell2.i;
            int resultY = cell1.j - cell2.j;
            return new Cell1(resultX, resultY);
        }


    }
}

