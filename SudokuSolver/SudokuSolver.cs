using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
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
        public int X { get; set; }
        public int Y { get; set; }

        public int? Value { get; set; }
        public bool Initial { get; set; }

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

        // lista pol pustych - algorytm dziala tak dlugo az ta lista jest pusta - jesli po kolejnej iteracji - lista pol pustych sie nie zmniejszyla, znaczy ze pozostale elementy 'trzeba' zgadywac
        // lista pol wypelnionych -

        public SudokuSolver(Cell[,] sudoku)
        {
            _sudoku = sudoku;
            _empty = GetEmptyCells(_sudoku);
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


        //public void Solve()
        //{
        //    // may need to repeat below loop x many times until all cells are filled or all cells left are in 'possoble' state

        //    foreach (var block in _blocks)
        //    {
        //        var cells = GetEmptySpaces(block);

        //        if (cells.Count == 0)
        //            continue;

        //        var missingNumber = GetMissingNumbers(block);

        //        foreach (var cell in cells)
        //        {
        //            foreach (var number in missingNumber)
        //            {
        //                IsValid()
        //            }
        //        }

        //    }
        //}
    }
}
