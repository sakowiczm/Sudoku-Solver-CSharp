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

    public class Cell
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
    }

    public class SudokuSolver
    {
        public List<Cell> Cells { get; private set; }

        public SudokuSolver(int?[,] data)
        {
            Cells = GetCells(data);
        }

        public bool Validate()
        {
            throw new NotImplementedException();
        }

        public void Solve()
        {
            var emptyCells = Cells.Where(c => !c.Value.HasValue).ToList();

            int cont1 = 0;
            while (cont1 != emptyCells.Count || cont1 == 0)
            {
                cont1 = emptyCells.Count;

                foreach (Block block in Enum.GetValues(typeof(Block)))
                {
                    var cellsInBlock = emptyCells.Where(c => c.Block == block).ToList();

                    int count = cellsInBlock.Count();

                    if(count == 0)
                        continue;

                    if (count > 0)
                    {
                        if (count == 1)
                        {
                            // just fill this single value
                            var knownValues = Cells.Where(c => c.Block == block && c.Value.HasValue).Select(c => c.Value.Value);
                            int value = Enumerable.Range(1, 9).Except(knownValues).First();

                            var cell = cellsInBlock[0];
                            cell.Value = value;
                            cell.PossibleValues.Clear();
                            cellsInBlock.ForEach(c => c.PossibleValues.Remove(value));

                            cellsInBlock.Remove(cell);
                            emptyCells.Remove(cell);
                            //Cells.Add(cell);
                        }
                        else
                        {
                            var knownValues = Cells.Where(c => c.Block == block && c.Value.HasValue).Select(c => c.Value.Value);
                            List<int> notKnownValues = Enumerable.Range(1, 9).Except(knownValues).ToList();

                            // build list of possible values
                            foreach (var cell in cellsInBlock)
                            {
                                foreach (var value in notKnownValues)
                                {
                                    bool isPossible = IsPossible(Cells, cell, value);

                                    if (isPossible)
                                        cell.PossibleValues.Add(value);
                                }
                            }

                            int cont2 = 0;
                            while (cont2 != notKnownValues.Count || cont2 == 0)
                            {
                                cont2 = notKnownValues.Count;
                                var values = new List<int>(notKnownValues);

                                foreach (var value in values)
                                {
                                    // reduce vertically
                                    if (cellsInBlock.Count(c => c.PossibleValues.Contains(value)) == 1)
                                    {
                                        var cell = cellsInBlock.First(c => c.PossibleValues.Contains(value));

                                        cell.Value = value;
                                        cell.PossibleValues.Clear();
                                        notKnownValues.Remove(value);
                                        cellsInBlock.ForEach(c => c.PossibleValues.Remove(value));

                                        cellsInBlock.Remove(cell);
                                        emptyCells.Remove(cell);
                                        //Cells.Add(cell); // todo: here update
                                    }

                                    // reduce horizontally
                                    var cells = new List<Cell>(cellsInBlock.Where(c => c.PossibleValues.Count() == 1));

                                    foreach (Cell cell in cells)
                                    {
                                        cell.Value = cell.PossibleValues[0];
                                        cell.PossibleValues.Clear();
                                        notKnownValues.Remove(value);

                                        cellsInBlock.Remove(cell);
                                        emptyCells.Remove(cell);
                                        //Cells.Add(cell);

                                        cellsInBlock.ForEach(c => c.PossibleValues.Remove(cell.Value.Value));
                                    }
                                }
                            }

                            // clear PossibleValues for a block
                            foreach (var cell in cellsInBlock)
                                cell.PossibleValues.Clear();
                        }
                    }
                }
            }

            // check if sudoku is solved
            if (emptyCells.Count == 0) 
                return;

            // todo: here another more complex attempt
            Console.WriteLine("You dont want to be here.");
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

        public static List<int> GetValues(List<Cell> cells, Cell cell, Orientation orientation)
        {
            var values =
                    cells.Where(
                        c => (orientation == Orientation.Horizontal ? c.Y == cell.Y : c.X == cell.X) && c.Value.HasValue)
                        .Select(c => c.Value.Value).ToList();

            return values;
        }

        public static bool IsPossible(List<Cell> cells, Cell cell, int value)
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
