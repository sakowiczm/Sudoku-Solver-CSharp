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
        // lista znanych wartosci
        public List<Cell> Data { get; private set; }

        public SudokuSolver(List<Cell> data)
        {
            Data = data;
        }

        public void SolveCurrent()
        {
            var emptyCells = GetEmptyCells(Data);

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
                        }
                        else
                        {
                            var knownValues = Data.Where(c => c.Block == block).Select(c => c.Value.Value);
                            List<int> notKnownValues = Enumerable.Range(1, 9).Except(knownValues).ToList();

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

                            int cont2 = 0;
                            while (cont2 != notKnownValues.Count || cont2 == 0)
                            {
                                cont2 = notKnownValues.Count;
                                var values = notKnownValues;

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
                                        Data.Add(cell);
                                    }

                                    // reduce horizontally
                                    var cells = cellsInBlock.Where(c => c.PossibleValues.Count() == 1);

                                    foreach (Cell cell in cells)
                                    {
                                        cell.Value = cell.PossibleValues[0];
                                        cell.PossibleValues.Clear();
                                        notKnownValues.Remove(value);

                                        cellsInBlock.Remove(cell);
                                        emptyCells.Remove(cell);
                                        Data.Add(cell);

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
        }

        private List<Cell> GetEmptyCells(List<Cell> data)
        {
            throw new NotImplementedException();
        }

        private bool IsPossible(Cell cell, int n)
        {
            throw new NotImplementedException();
        }


    }
}
