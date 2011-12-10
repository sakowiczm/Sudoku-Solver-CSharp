using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace SudokuSolver
{
    [TestFixture]
    public class SudokuSolverTests
    {
        private int?[,] GetBoard()
        {
            var board = new int?[9, 9];

            #region Fill with data

            // cell 00
            board[0, 1] = 5;
            board[0, 2] = 2;
            board[1, 2] = 4;

            // test one:
            //board[1, 0] = 3;

            // cell 10
            board[3, 0] = 5;
            board[4, 0] = 8;
            board[4, 2] = 7;
            board[5, 2] = 9;

            // cell 20
            board[6, 1] = 9;
            board[8, 2] = 5;

            // cell 01
            board[2, 3] = 5;
            board[1, 4] = 1;
            board[1, 5] = 8;
            board[2, 5] = 2;

            // cell 11
            board[4, 3] = 4;
            board[4, 5] = 1;

            // cell 21
            board[6, 3] = 7;
            board[6, 5] = 5;
            board[7, 3] = 1;
            board[7, 4] = 4;

            // cell 02
            board[0, 6] = 3;
            board[2, 7] = 1;

            // cell 12
            board[3, 6] = 1;
            board[4, 6] = 2;
            board[4, 8] = 3;
            board[5, 8] = 6;

            // cell 22
            board[7, 6] = 9;
            board[8, 6] = 8;
            board[8, 7] = 6;

            #endregion

            return board;
        }

        [Test]
        public void GetValuesTest()
        {
            var cells = "000580000500000900240079005005040710010000040082010500300120098001000006000036000".StringToCells();

            List<int> a = SudokuSolver.GetValues(cells, new Cell(0, 0), Orientation.Vertical);
            Assert.IsTrue(a.Where(i => (new List<int>() { 5, 2, 3 }).Contains(i)).Count() == 3);

            a = SudokuSolver.GetValues(cells, new Cell(6, 1), Orientation.Horizontal);
            Assert.IsTrue(a.Where(i => (new List<int>() { 5, 9 }).Contains(i)).Count() == 2);

            a = SudokuSolver.GetValues(cells, new Cell(4, 3), Orientation.Vertical);
            Assert.IsTrue(a.Where(i => (new List<int>() { 8, 7, 4, 1, 2, 3 }).Contains(i)).Count() == 6);            
        }
        [Test]
        public void GetValuesFromArrayTest()
        {
            var board = GetBoard();

            List<int> a = SudokuSolver.GetValues(board, 0, 0, Orientation.Vertical);
            Assert.IsTrue(a.Where(i => (new List<int>() { 5, 2, 3 }).Contains(i)).Count() == 3);

            a = SudokuSolver.GetValues(board, 6, 1, Orientation.Horizontal);
            Assert.IsTrue(a.Where(i => (new List<int>() { 5, 9 }).Contains(i)).Count() == 2);

            a = SudokuSolver.GetValues(board, 4, 3, Orientation.Vertical);
            Assert.IsTrue(a.Where(i => (new List<int>() { 8, 7, 4, 1, 2, 3 }).Contains(i)).Count() == 6);
        }

        [Test]
        public void GetValuesFromCellsTest()
        {
            var board = GetBoard();
            var cells = SudokuSolver.GetCells(board);

            List<int> a = SudokuSolver.GetValues(cells, new Cell(0, 0), Orientation.Vertical);
            Assert.IsTrue(a.Where(i => (new List<int>() { 5, 2, 3 }).Contains(i)).Count() == 3);

            a = SudokuSolver.GetValues(cells, new Cell(6, 1), Orientation.Horizontal);
            Assert.IsTrue(a.Where(i => (new List<int>() { 5, 9 }).Contains(i)).Count() == 2);

            a = SudokuSolver.GetValues(cells, new Cell(4, 3), Orientation.Vertical);
            Assert.IsTrue(a.Where(i => (new List<int>() { 8, 7, 4, 1, 2, 3 }).Contains(i)).Count() == 6);            
        }

        [Test]
        public void IsPossibleTest()
        {
            var board = GetBoard();
            var cells = SudokuSolver.GetCells(board);
            //var cells = "000580000500000900240079005005040710010000040082010500300120098001000006000036000".StringToCells();

            bool isPossible = SudokuSolver.IsPossible(cells, new Cell(0, 0), 1);
            Assert.IsTrue(isPossible);

            isPossible = SudokuSolver.IsPossible(cells, new Cell(0, 0), 2);
            Assert.IsFalse(isPossible);

            isPossible = SudokuSolver.IsPossible(cells, new Cell(0, 0), 3);
            Assert.IsFalse(isPossible);

            isPossible = SudokuSolver.IsPossible(cells, new Cell(5, 5), 5);
            Assert.IsFalse(isPossible);

            isPossible = SudokuSolver.IsPossible(cells, new Cell(8, 4), 9);
            Assert.IsTrue(isPossible);
        }

        [Test]
        public void GetBlockTest()
        {
            var block = SudokuSolver.GetBlock(1, 1);
            Assert.IsTrue(block == Block.B00);

            block = SudokuSolver.GetBlock(5, 2);
            Assert.IsTrue(block == Block.B10);

            block = SudokuSolver.GetBlock(8, 2);
            Assert.IsTrue(block == Block.B20);

            block = SudokuSolver.GetBlock(8, 0);
            Assert.IsTrue(block == Block.B20);

            block = SudokuSolver.GetBlock(2, 3);
            Assert.IsTrue(block == Block.B01);

            block = SudokuSolver.GetBlock(4, 5);
            Assert.IsTrue(block == Block.B11);

            block = SudokuSolver.GetBlock(7, 4);
            Assert.IsTrue(block == Block.B21);

            block = SudokuSolver.GetBlock(1, 8);
            Assert.IsTrue(block == Block.B02);

            block = SudokuSolver.GetBlock(4, 6);
            Assert.IsTrue(block == Block.B12);

            block = SudokuSolver.GetBlock(8, 2);
            Assert.IsTrue(block == Block.B20);

            block = SudokuSolver.GetBlock(8, 8);
            Assert.IsTrue(block == Block.B22);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetBlockInvalidValuesTest()
        {
            var block = SudokuSolver.GetBlock(1, 9);
            Assert.IsTrue(block == Block.B00);
        }

        [Test]
        public void GetCellsTest()
        {
            //var cells = "000580000500000900240079005005040710010000040082010500300120098001000006000036000".StringToCells();
            var board = GetBoard();

            var cells = SudokuSolver.GetCells(board);

            Assert.IsTrue(cells.Count == 81);
            Assert.IsTrue(cells.Count(c => c.Value.HasValue) == 28);
            Assert.IsTrue(cells.Count(c => !c.Value.HasValue) == 81 - 28);
        }

        [Test]
        public void ImportExportTest()
        {
            string input = "000580000500060900240079005005040710010000040082010500300120498001000006000036000";
                            
            string output = input.StringToCells().CellsToString();

            Assert.IsTrue(input == output);

            input = "193584627578261934246379185635948712719652843482713569367125498821497356954836271";
            output = input.StringToCells().CellsToString();

            Assert.IsTrue(input == output);
        }

        [Test]
        public void AAA()
        {
            var board = GetBoard();

            var initialCells1 = SudokuSolver.GetCells(board);
            var initialCells2 = "000580000500000900240079005005040710010000040082010500300120098001000006000036000".StringToCells();

            Assert.IsTrue(initialCells1.CellsToString() == initialCells2.CellsToString());
            //Assert.IsTrue(new CollectionComparer<Cell>().Equals(initialCells1, initialCells2));
        }
		
        [Test]
        public void SolveTest()
        {
            var board = GetBoard();
            var ss = new SudokuSolver();

            // 0,0,0,5,8,0,0,0,0,5,0,0,0,0,0,9,0,0,2,4,0,0,7,9,0,0,5,0,0,5,0,4,0,7,1,0,0,1,0,0,0,0,0,4,0,0,8,2,0,1,0,5,0,0,3,0,0,1,2,0,0,9,8,0,0,1,0,0,0,0,0,6,0,0,0,0,3,6,0,0,0
            var initialCells = SudokuSolver.GetCells(board);


            var resultCells = ss.Solve(initialCells);

            var abc = resultCells.Where(c => !c.Initial).ToList();

            Assert.IsTrue(resultCells != null);
            Assert.IsTrue(resultCells.Count == 81);
            Assert.IsTrue(resultCells.Count(c => c.Value.HasValue) == 81);

            Assert.IsTrue(resultCells.Count(c => c.X == 0 && c.Y == 0 && c.Value.Value == 1) == 1);
            Assert.IsTrue(resultCells.Count(c => c.X == 1 && c.Y == 0 && c.Value.Value == 9) == 1);
            Assert.IsTrue(resultCells.Count(c => c.X == 2 && c.Y == 0 && c.Value.Value == 3) == 1);

            Assert.IsTrue(resultCells.Count(c => c.X == 1 && c.Y == 1 && c.Value.Value == 7) == 1);
            Assert.IsTrue(resultCells.Count(c => c.X == 2 && c.Y == 1 && c.Value.Value == 8) == 1);
            Assert.IsTrue(resultCells.Count(c => c.X == 2 && c.Y == 2 && c.Value.Value == 6) == 1);
        }

        [Test]
        public void STest()
        {
            var board = GetBoard();
            var ss = new SudokuSolver();

            // 0,0,0,5,8,0,0,0,0,5,0,0,0,0,0,9,0,0,2,4,0,0,7,9,0,0,5,0,0,5,0,4,0,7,1,0,0,1,0,0,0,0,0,4,0,0,8,2,0,1,0,5,0,0,3,0,0,1,2,0,0,9,8,0,0,1,0,0,0,0,0,6,0,0,0,0,3,6,0,0,0
            //var initialCells = SudokuSolver.GetCells(board);
            var initialCells = "000580000500000900240079005005040710010000040082010500300120098001000006000036000".StringToCells();

            var resultCells = ss.Solve(initialCells);

            Console.WriteLine(resultCells.ToString());
        }

        [Test]
        public void SolveTest1()
        {
            //var board = GetBoard();
            //var ss = new SudokuSolver();

            //// 0,0,0,5,8,0,0,0,0,5,0,0,0,0,0,9,0,0,2,4,0,0,7,9,0,0,5,0,0,5,0,4,0,7,1,0,0,1,0,0,0,0,0,4,0,0,8,2,0,1,0,5,0,0,3,0,0,1,2,0,0,9,8,0,0,1,0,0,0,0,0,6,0,0,0,0,3,6,0,0,0
            //var initialCells = SudokuSolver.GetCells(board);


            //var esultCells = ss.S(initialCells);

            //var abc = resultCells.Where(c => !c.Initial).ToList();

            //Assert.IsTrue(resultCells != null);
            //Assert.IsTrue(resultCells.Count == 81);
            //Assert.IsTrue(resultCells.Count(c => c.Value.HasValue) == 81);

            //Assert.IsTrue(resultCells.Count(c => c.X == 0 && c.Y == 0 && c.Value.Value == 1) == 1);
            //Assert.IsTrue(resultCells.Count(c => c.X == 1 && c.Y == 0 && c.Value.Value == 9) == 1);
            //Assert.IsTrue(resultCells.Count(c => c.X == 2 && c.Y == 0 && c.Value.Value == 3) == 1);

            //Assert.IsTrue(resultCells.Count(c => c.X == 1 && c.Y == 1 && c.Value.Value == 7) == 1);
            //Assert.IsTrue(resultCells.Count(c => c.X == 2 && c.Y == 1 && c.Value.Value == 8) == 1);
            //Assert.IsTrue(resultCells.Count(c => c.X == 2 && c.Y == 2 && c.Value.Value == 6) == 1);
        }   
		     
        [Test]
        public void SolveTest2()
        {
            var solver = new SudokuSolver();

            // input, expected output
            var games = new List<Tuple<string, string>>();
            games.Add(Tuple.Create<string, string>("000580000500000900240079005005040710010000040082010500300120098001000006000036000", 
                                                    "193584627578261934246379185635948712719652843482713569367125498821497356954836271"));

            foreach (var game in games)
            {
                var cells = solver.Solve(game.Item1.StringToCells());
                string output = cells.CellsToString();
                Console.WriteLine(game.Item1 + " --> " + output);
                Assert.IsTrue(output == game.Item2);
            }

        }


    }
}