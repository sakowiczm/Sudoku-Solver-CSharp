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

    public class Cell
    {
        public Cell()
        {
            PossibleValues = new List<int>(9);
        }

        public int X { get; set; }
        public int Y { get; set; }

        public int? Value { get; set; }
        public bool Initial { get; set; }

        public List<int> PossibleValues { get; set; }

        public Block Block
        {
            get { return GetBlock(X, Y); }
        }

        private Block GetBlock(int x, int y)
        {
            return Block.B00; // todo:
        }
    }

    public class SudokuSolver
    {
        public Cell[,] _sudoku = new Cell[8,8];

        public List<Cell> _empty;
        public List<Cell> _filled;

        // lista pol pustych - algorytm dziala tak dlugo az ta lista jest pusta - jesli po kolejnej iteracji - lista pol pustych sie nie zmniejszyla, znaczy ze pozostale elementy 'trzeba' zgadywac
        // lista pol wypelnionych -

        public SudokuSolver(Cell[,] sudoku)
        {
            _sudoku = sudoku;
            _empty = GetEmptyCells(_sudoku);
        }

        public void SolveCurrent()
        {
            var emptyCells = GetEmptyCells(_sudoku);

            int initialEmptyCellCount = emptyCells.Count;
            bool proceed = true;

            while (proceed)
            {
                foreach (var block in Block) // todo: for each block
                {
                    var cellsInBlock = emptyCells.Where(c => c.Block == block).ToList();

                    int count = cellsInBlock.Count();

                    if(count == 0)
                        continue;

                    if (count == 0)
                    {
                        // just fill this single value
                    }

                    if (count > 0)
                    {
                        if (count == 1)
                        {
                            // just fill this single value
                        }
                        else
                        {
                            var knownValues = _filled.Where(c => c.Block == block).Select(c => c.Value);
                            List<int> notKnownValues = null; // todo: create based on knowValues;

                            // build list of possible values
                            foreach (var cell in cellsInBlock)
                            {
                                foreach (var value in notKnownValues)
                                {
                                    bool isPossible = IsPossible(cell, value);

                                    if (isPossible)
                                        cell.PossibleValues.Add(value);
                                }
                            }

                            // ponizszy foreach zamienic na while  i tak samo untill notKnowValues nie sa = 0 badz wartosc jest taka sama jak na poczatku

                            // if we have only on occurence of possible values for a cell fill the value
                            foreach (var value in notKnownValues)
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
                                    _filled.Add(cell);
                                }

                                var cells = cellsInBlock.Where(c => c.PossibleValues.Count() == 1);

                                foreach (Cell cell in cells)
                                {
                                    cell.Value = cell.PossibleValues[0];
                                    cell.PossibleValues.Clear();
                                    notKnownValues.Remove(value);

                                    cellsInBlock.Remove(cell);
                                    emptyCells.Remove(cell);
                                    _filled.Add(cell);

                                    cellsInBlock.ForEach(c => c.PossibleValues.Remove(cell.Value.Value));
                                }
                            }

                            // clear PossibleValues for a block
                            foreach (var cell in cellsInBlock)
                                cell.PossibleValues.Clear();
                        }
                    }
                }

                // check if there is no more possibilities for this alghorithm
                if (emptyCells.Count == initialEmptyCellCount || emptyCells.Count == 0)
                    proceed = false;

                initialEmptyCellCount = emptyCells.Count;
            }

            // check if sudoku is solved
            if (emptyCells.Count == 0) 
                return;

            // todo: here another more complex attempt
        }

        public void Solve()
        {
            var emptyCells = GetEmptyCells(_sudoku);

            foreach (var cell in emptyCells)
            {
                var missing = GetBlockMissingNumbers(_sudoku, cell);

                // if missing.Count() == 1 then cell.Value = missing[0];

                foreach (int n in missing)
                {
                    bool success = IsValid(cell, n);

                    if (success)
                    {
                        _empty.Remove(cell);
                        _sudoku[cell.X, cell.Y].Value = n; // todo: here add or update

                        break;
                    }
                }
            }
        }

        private List<Cell> GetEmptyCells(Cell[,] sudoku)
        {
            throw new NotImplementedException();
        }

        private List<int> GetBlockMissingNumbers(Cell[,] sudoku, Cell cell)
        {
            throw new NotImplementedException();
        }

        private bool IsValid(Cell cell, int n)
        {
            throw new NotImplementedException();
        }

        private bool IsPossible(Cell cell, int n)
        {
            throw new NotImplementedException();
        }


    }
}
