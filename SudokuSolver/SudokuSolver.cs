using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    // todo: try to use TPL
    // todo: try to fit here strategy pattern
    // todo: heavy on interfaces and unit tests

    public enum Block
    {
        B00,
        B10,
        B20,
        B01,
        B11,
        B21,
        B02,
        B12,
        B22
    }

    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    public class Cell : ICloneable
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
            cell.PossibleValues = this.PossibleValues; // this can be shared

            return cell;
        }

        #endregion
    }

    public class SudokuSolver
    {
        //public List<Cell> Cells { get; private set; }

        //public SudokuSolver(int?[,] data)
        //{
        //    Cells = GetCells(data);
        //}

        //public bool Validate()
        //{
        //    throw new NotImplementedException();
        //}

        List<string> _operationLog = new List<string>();

        public List<Cell> Solve(List<Cell> cells)
        {
            int emptyCellCount = 81;

            while (cells.Count(c => !c.Value.HasValue) < emptyCellCount)
            {
                emptyCellCount = cells.Count(c => !c.Value.HasValue);

                foreach (Block block in Enum.GetValues(typeof(Block)))
                {
                    int cc = 9;
                    while (cells.Count(c => !c.Value.HasValue && c.Block == block) < cc)
                    {
                        var ecs = cells.Where(c => !c.Value.HasValue && c.Block == block);
                        cc = ecs.Count();

                        CalculatePossibleValues(cells, ecs);

                        CheckHorizontally(ecs);

                        CalculatePossibleValues(cells, ecs);

                        CheckVertically(ecs);
                    }
                }
            }

            // todo: here backtrack algorithm

            if (cells.Count(c => !c.Value.HasValue) == 0)
                return cells;

            // to nie jest potrzebne wystarczy pierwszy element
            foreach (var cell in cells.Where(c => !c.Value.HasValue))
            {
                CalculatePossibleValues(cells, new List<Cell>() { cell } );

                if (cell.PossibleValues.Count == 0)
                    continue;

                var step = (List<Cell>)cells.Clone();

                foreach (var v in cell.PossibleValues)
                {
                    cell.Value = v;
                    var result = Solve(cells);

                    if (result != null && result.Count(c => !c.Value.HasValue) == 0)
                        return cells;

                    cells = step;
                }
            }

            return null;
        }

        private void CheckHorizontally(IEnumerable<Cell> ecs)
        {
            var hcs = ecs.Where(c => c.PossibleValues.Count() == 1);

            foreach (Cell cell in hcs)
            {
                cell.Value = cell.PossibleValues[0];

                //foreach (var item in ecs)
                //{
                //    item.PossibleValues.Remove(cell.Value.Value);
                //}
            }
        }

        private void CheckVertically(IEnumerable<Cell> ecs)
        {
            for (int i = 1; i < 10; i++)
            {
                if (ecs.Count(c => c.PossibleValues.Contains(i)) == 1)
                {
                    var cell = ecs.First(c => c.PossibleValues.Contains(i));
                    cell.Value = i;

                    //foreach (var item in ecs)
                    //{
                    //    item.PossibleValues.Remove(cell.Value.Value);
                    //}
                }
            }
        }

        private void CalculatePossibleValues(IEnumerable<Cell> allCells, IEnumerable<Cell> cells)
        {
            foreach (var cell in cells)
            {
                cell.PossibleValues.Clear();

                for (int i = 1; i < 10; i++)
                {
                    bool isPossible = IsPossible(allCells, cell, i);

                    if (isPossible)
                        cell.PossibleValues.Add(i);
                }
            }
        }

        public static List<Cell> GetCells(int?[,] board)
        {
            var cells = new List<Cell>();

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    int? value = board[i, j];

                    var cell = new Cell();
                    cell.Initial = value.HasValue;
                    cell.Value = value;
                    cell.X = i;
                    cell.Y = j;
                    cell.Block = GetBlock(i, j);

                    cells.Add(cell);
                }

            return cells;
        }

        public static Block GetBlock(int x, int y)
        {
            if (x < 0 || x > 8 || y < 0 || y > 8)
                throw new ArgumentOutOfRangeException();

            if (x < 3 && y < 3)
                return Block.B00;

            if (x > 2 && x < 6 && y < 3)
                return Block.B10;

            if (x > 5 && y < 3)
                return Block.B20;

            if (x < 3 && y > 2 && y < 6)
                return Block.B01;

            if (x > 2 && x < 6 && y > 2 && y < 6)
                return Block.B11;

            if (x > 5 && y > 2 && y < 6)
                return Block.B21;

            if (x < 3 && y > 5)
                return Block.B02;

            if (x > 2 && x < 6 && y > 5)
                return Block.B12;

            //if (x > 5 && y > 5)
            return Block.B22;
        }

        public static List<int> GetValues(int?[,] board, int x, int y, Orientation orientation)
        {
            List<int> values = new List<int>();

            for (int i = 0; i < 9; i++)
            {
                int? value = orientation == Orientation.Horizontal ? board[i, y] : board[x, i];

                if (value.HasValue)
                    values.Add(value.Value);
            }

            return values;
        }

        public static List<int> GetValues(IEnumerable<Cell> cells, Cell cell, Orientation orientation)
        {
            var values =
                    cells.Where(
                        c => (orientation == Orientation.Horizontal ? c.Y == cell.Y : c.X == cell.X) && c.Value.HasValue)
                        .Select(c => c.Value.Value).ToList();

            return values;
        }

        public static bool IsPossible(IEnumerable<Cell> cells, Cell cell, int value)
        {
            if(cell == null || cell.Value.HasValue)
                throw new ArgumentException("cell");

            // value exist in block
            if (cells.Count(c => c.Block == cell.Block && c.Value.HasValue && c.Value.Value == value) > 0)
                return false;

            // horizontal check
            if(GetValues(cells, cell, Orientation.Horizontal).Contains(value))
                return false;

            // vertical check
            if (GetValues(cells, cell, Orientation.Vertical).Contains(value))
                return false;

            return true;
        }
    }
}
