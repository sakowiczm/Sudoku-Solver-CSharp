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

        public List<Cell> S(List<Cell> cells)
        {
            int emptyCellCount = 81;

            while (cells.Count(c => !c.Value.HasValue) < emptyCellCount)
            {
                emptyCellCount = cells.Count(c => !c.Value.HasValue);

                foreach (Block block in Enum.GetValues(typeof(Block)))
                {
                    bool contBlockCheck = true;
                    while (contBlockCheck)
                    {
                        var ecs = cells.Where(c => !c.Value.HasValue && c.Block == block);

                        if (ecs.Count() == 0)
                        {
                            contBlockCheck = false;
                            continue;
                        }

                        CalculatePossibleValues(cells, ecs);

                        var hr = CheckHorizontally(ecs);

                        CalculatePossibleValues(cells, ecs);

                        var vr = CheckVertically(ecs);

                        contBlockCheck = hr || vr;
                    }
                }
            }

            return cells;
        }

        private bool CheckHorizontally(IEnumerable<Cell> ecs)
        {
            bool result = false;
            var hcs = ecs.Where(c => c.PossibleValues.Count() == 1);

            foreach (Cell cell in hcs)
            {
                cell.Value = cell.PossibleValues[0];
                result = true;
            }

            return result;
        }

        private bool CheckVertically(IEnumerable<Cell> ecs)
        {
            bool result = false;

            for (int i = 1; i < 10; i++)
            {
                if (ecs.Count(c => c.PossibleValues.Contains(i)) == 1)
                {
                    var cell = ecs.First(c => c.PossibleValues.Contains(i));
                    cell.Value = i;

                    result = true;
                }
            }

            return result;
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

        public List<Cell> Solve1(List<Cell> cells, int emptyCellsCount)
        {
            bool stop = true;
            Cell empty = null;

            while ((empty = cells.First(c => !c.Value.HasValue)) != null && stop)
            {
                stop = false;
                //var emptyBlockCells = cells.Where(c => !c.Value.HasValue && c.Block == empty.Block).ToList();

                
                var knownValues = cells.Where(c => c.Block == empty.Block && c.Value.HasValue).Select(c => c.Value.Value);
                List<int> notKnownValues = Enumerable.Range(1, 9).Except(knownValues).ToList();

                var ec1 = cells.Where(c => !c.Value.HasValue && c.Block == empty.Block);
                foreach (var cell in ec1)
                {
                    foreach (var value in notKnownValues)
                    {
                        bool isPossible = IsPossible(cells, cell, value);

                        if (isPossible)
                            cell.PossibleValues.Add(value);
                    }
                }

                bool cont = true;
                while (notKnownValues.Count > 0 && cont)
                {
                    stop = true;
                    cont = false;

                    // reduce horizontally
                    var ec2 = cells.Where(c => !c.Value.HasValue && c.Block == empty.Block).ToList();

                    //foreach (Cell cell in ec2.Where(c => c.PossibleValues.Count() == 1))
                    //{
                    //    cell.Value = cell.PossibleValues[0];
                    //    cell.PossibleValues.Clear();
                    //    notKnownValues.Remove(cell.Value.Value);

                    //    // ?
                    //    ec2.ForEach(c => c.PossibleValues.Remove(cell.Value.Value));
                    //    cont = true;
                    //}

                    for (int i = 1; i < 10; i++)
                    {
                        if (ec2.Count(c => c.PossibleValues.Contains(i)) == 1)
                        {
                            var cell = ec2.First(c => c.PossibleValues.Contains(i));

                            cell.Value = i;
                            cell.PossibleValues.Clear();
                            notKnownValues.Remove(i);

                            // ?
                            ec2.ForEach(c => c.PossibleValues.Remove(i));
                            cont = true;
                        }
                    }
                }


            }

            return cells;
        }

        //public List<Cell> Solve2(List<Cell> cells, int emptyCellsCount)
        //{
        //    var emptyCells = cells.Where(c => !c.Value.HasValue).ToList();

        //    int cont1 = -1;
        //    while (cont1 != emptyCells.Count || cont1 == -1)
        //    {
        //        cont1 = emptyCells.Count;

        //        foreach (Block block in Enum.GetValues(typeof(Block)))
        //        {
        //            if (emptyCells.Count(c => c.Block == block) > 0)
        //            {
        //                // move to GetPossibleValues (?)
        //                var knownValues = cells.Where(c => c.Block == block && c.Value.HasValue).Select(c => c.Value.Value);
        //                List<int> notKnownValues = Enumerable.Range(1, 9).Except(knownValues).ToList();

        //                // build list of possible values
        //                foreach (var cell in emptyCells.Where(c => c.Block == block))
        //                {
        //                    foreach (var value in notKnownValues)
        //                    {
        //                        bool isPossible = IsPossible(cells, cell, value);

        //                        if (isPossible)
        //                            cell.PossibleValues.Add(value);
        //                    }
        //                }
        //                //end of move to GetPossibleValues

        //                bool cont2 = true;
        //                while (notKnownValues.Count > 0 && cont2)
        //                {
        //                    cont2 = false;


        //                    //// reduce horizontally
        //                    //var ccc = cells.Where(c => !c.Value.HasValue && c.Block == block && c.PossibleValues.Count() == 1);

        //                    //foreach (Cell cell in ccc)
        //                    //{
        //                    //    //if (cell.PossibleValues.Count > 0)
        //                    //    {
        //                    //        cell.Value = cell.PossibleValues[0];
        //                    //        cell.PossibleValues.Clear();
        //                    //        notKnownValues.Remove(cell.Value.Value);

        //                    //        emptyCells.Remove(cell);

        //                    //        ccc.ForEach(c => c.PossibleValues.Remove(cell.Value.Value));
        //                    //        cont2 = true;
        //                    //    }
        //                    //}


        //                    foreach (var value in Enumerable.Range(1, 9))
        //                    {
        //                        // reduce vertically
        //                        if (cellsInBlock.Count(c => c.PossibleValues.Contains(value)) == 1)
        //                        {
        //                            var cell = cellsInBlock.First(c => c.PossibleValues.Contains(value));

        //                            cell.Value = value;
        //                            cell.PossibleValues.Clear();
        //                            notKnownValues.Remove(value);
        //                            cellsInBlock.ForEach(c => c.PossibleValues.Remove(value));

        //                            cellsInBlock.Remove(cell);
        //                            emptyCells.Remove(cell);

        //                            cont2 = true;
        //                        }

        //                    }
        //                }

        //                // clear PossibleValues for a block
        //                foreach (var cell in cellsInBlock)
        //                    cell.PossibleValues.Clear();
        //            }
        //        }
        //    }
        //}

        //private List<Cell> Solve(List<Cell> cells, int emptyCellsCount)
        //{
        //    var emptyCells = cells.Where(c => !c.Value.HasValue).ToList();

        //    int cont1 = -1;
        //    while (cont1 != emptyCells.Count || cont1 == -1)
        //    {
        //        cont1 = emptyCells.Count;

        //        foreach (Block block in Enum.GetValues(typeof(Block)))
        //        {
        //            if (emptyCells.Count(c => c.Block == block) > 0)
        //            {
        //                // move to GetPossibleValues (?)
        //                var knownValues = cells.Where(c => c.Block == block && c.Value.HasValue).Select(c => c.Value.Value);
        //                List<int> notKnownValues = Enumerable.Range(1, 9).Except(knownValues).ToList();

        //                // build list of possible values
        //                foreach (var cell in emptyCells.Where(c => c.Block == block))
        //                {
        //                    foreach (var value in notKnownValues)
        //                    {
        //                        bool isPossible = IsPossible(cells, cell, value);

        //                        if (isPossible)
        //                            cell.PossibleValues.Add(value);
        //                    }
        //                }
        //                //end of move to GetPossibleValues

        //                bool cont2 = true;
        //                while (notKnownValues.Count > 0 && cont2)
        //                {
        //                    cont2 = false;


        //                    // reduce horizontally
        //                    var ccc = cells.Where(c => !c.Value.HasValue && c.Block == block && c.PossibleValues.Count() == 1);

        //                    foreach (Cell cell in ccc)
        //                    {
        //                        //if (cell.PossibleValues.Count > 0)
        //                        {
        //                            cell.Value = cell.PossibleValues[0];
        //                            cell.PossibleValues.Clear();
        //                            notKnownValues.Remove(cell.Value.Value);

        //                            emptyCells.Remove(cell);

        //                            ccc.ForEach(c => c.PossibleValues.Remove(cell.Value.Value));
        //                            cont2 = true;
        //                        }
        //                    }


        //                    foreach (var value in Enumerable.Range(1,9))
        //                    {
        //                        // reduce vertically
        //                        if (cellsInBlock.Count(c => c.PossibleValues.Contains(value)) == 1)
        //                        {
        //                            var cell = cellsInBlock.First(c => c.PossibleValues.Contains(value));

        //                            cell.Value = value;
        //                            cell.PossibleValues.Clear();
        //                            notKnownValues.Remove(value);
        //                            cellsInBlock.ForEach(c => c.PossibleValues.Remove(value));

        //                            cellsInBlock.Remove(cell);
        //                            emptyCells.Remove(cell);

        //                            cont2 = true;
        //                        }

        //                    }
        //                }

        //                // clear PossibleValues for a block
        //                foreach (var cell in cellsInBlock)
        //                    cell.PossibleValues.Clear();
        //            }
        //        }
        //    }

        //    // check if sudoku is solved
        //    if(cells.FirstOrDefault(c => !c.Value.HasValue) == null)
        //        return cells;

        //    _operationLog.Add("First pass ended. Missing cells count " + emptyCells.Count + ".");
        //    _operationLog.Add("Status: " + cells.Export());

        //    // not solveable
        //    if (emptyCells.Count == emptyCellsCount)
        //        return null;

        //    // todo: check if this is a solvable sudoku if not return null
        //    // else get first empty cell calculate possible values for it and foreach value

        //    var emptyCell = cells.First(c => !c.Value.HasValue);

        //    // todo: check if I need to do this - most likely not
        //    emptyCell.PossibleValues.Clear();

        //    // move to GetPossibleValues (?)
        //    var kv = cells.Where(c => c.Block == emptyCell.Block && c.Value.HasValue).Select(c => c.Value.Value);
        //    var nkv = Enumerable.Range(1, 9).Except(kv).ToList();

        //    // build list of possible values
        //    foreach (var value in nkv)
        //    {
        //        bool isPossible = IsPossible(cells, emptyCell, value);

        //        if (isPossible)
        //            emptyCell.PossibleValues.Add(value);
        //    }
        //    //end of move to GetPossibleValues

        //    //foreach (var c in emptyCell.PossibleValues)
        //    foreach (var c in emptyCell.PossibleValues)
        //    {
        //        _operationLog.Add(string.Format("Trying cell X: {0}, Y: {1}, Possible Values: {2}, Trying Value: {3}", emptyCell.X, emptyCell.Y, string.Join(",", emptyCell.PossibleValues), c));

        //        emptyCell.Value = c;

        //        var result = Solve(new List<Cell>(cells), emptyCells.Count - 1);

        //        if(result != null)
        //            return result;
        //    }

        //    return null;
        //}

        public List<Cell> Solve(List<Cell> cells)
        {
            //return Solve(cells, 0);
            return null;
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
