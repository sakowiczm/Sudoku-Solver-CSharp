using System;
using System.Collections.Generic;

namespace SudokuSolver
{
    public class Cell : ICloneable, IComparable<Cell>
    {
        public Cell()
        {
            PossibleValues = new List<int>(9);
        }

        public Cell(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public int? Value { get; set; }
        public bool Initial { get; set; }

        public Block Block { get; set; }

        public List<int> PossibleValues { get; set; }

        #region ICloneable Members

        public object Clone()
        {
            var cell = new Cell();
            cell.X = this.X;
            cell.Y = this.Y;
            cell.Value = this.Value;
            cell.Initial = this.Initial;
            cell.Block = this.Block;
            cell.PossibleValues = this.PossibleValues;

            return cell;
        }

        #endregion

        public int CompareTo(Cell other)
        {
            return (this.X == other.X && this.Y == other.Y && this.Value == other.Value && this.Block == other.Block) ? 0 : -1;
        }
    }
}