using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver
{
    public class SudokuSolver
    {
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

            if (cells.Count(c => !c.Value.HasValue) == 0)
                return cells;

            // to nie jest potrzebne wystarczy pierwszy element
            foreach (var cell in cells.Where(c => !c.Value.HasValue))
            {
                CalculatePossibleValues(cells, new List<Cell>() { cell } );

                if (cell.PossibleValues.Count == 0)
                    continue;

                foreach (var v in cell.PossibleValues)
                {
                    cell.Value = v;
                    var result = Solve((List<Cell>)cells.Clone());

                    if (result != null && result.Count(c => !c.Value.HasValue) == 0)
                        return result;
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

            return Block.B22;
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
