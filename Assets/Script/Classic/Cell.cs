using System;
using System.Collections.Generic;

namespace Assets.Script.Classic
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
        public void SetCell(Cell Cell2)
        {
            this.i = Cell2.i;
            this.j = Cell2.j;
        }

        public List<Cell> Move(Cell CellFinal)
        {

            var lstNextState = new List<Cell>();
            var lstCase = new List<Cell>();

            if (j + 1 <= BaseClassic.n+1) lstCase.Add(new Cell(i, j + 1));
            if (i + 1 <= BaseClassic.m+1) lstCase.Add(new Cell(i + 1, j));
            if (j - 1 >= 0) lstCase.Add(new Cell(i, j - 1));
            if (i - 1 >= 0) lstCase.Add(new Cell(i - 1, j));
            foreach (var c in lstCase)
            {
                if (BaseClassic.MATRIX[c.i, c.j] == 0|| c.Equals(CellFinal))
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
            return obj is Cell Cell &&
                   i == Cell.i &&
                   j == Cell.j;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(i, j);
        }

        public static Cell operator -(Cell Cell1, Cell Cell2)
        {
            int resultX = Cell1.i - Cell2.i;
            int resultY = Cell1.j - Cell2.j;
            return new Cell(resultX, resultY);
        }


    }
}

