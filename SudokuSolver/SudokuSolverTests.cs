using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace SudokuSolver
{
    [TestFixture]
    public class SudokuSolverTests
    {
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
        public void GetValuesFromCellsTest()
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
        public void IsPossibleTest()
        {
            var cells = "000580000500000900240079005005040710010000040082010500300120098001000006000036000".StringToCells();

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
            var cells = "000580000500000900240079005005040710010000040082010500300120098001000006000036000".StringToCells();

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

        //[Test]
        public void SolveTest()
        {
            // diabolical
            var initialCells = "000041000060000200000000000320600000000050041700000000000200300048000000501000000".StringToCells();

            var resultCells = new SudokuSolver().Solve(initialCells);

            //string result = resultCells.CellsToString();

            Assert.IsTrue(resultCells.CellsToString() == "872941563169573284453826197324617859986352741715498632697284315248135976531769428");
        }

        [Test]
        public void SolveMultipleTest()
        {
            var solver = new SudokuSolver();

            // input, expected output
            var games = new List<Tuple<string, string>>();
            games.Add(Tuple.Create<string, string>("000580000500000900240079005005040710010000040082010500300120098001000006000036000", "193584627578261934246379185635948712719652843482713569367125498821497356954836271"));
            games.Add(Tuple.Create<string, string>("400010000000309040070005009000060021004070600190050000900400070030608000000030006", "459716382612389745873245169387964521524173698196852437965421873731698254248537916")); // moderate
            games.Add(Tuple.Create<string, string>("309000400200709000087000000750060230600904008028050041000000590000106007006000104", "369218475215749863487635912754861239631924758928357641173482596542196387896573124")); // tough
            games.Add(Tuple.Create<string, string>("000704005020010070000080002090006250600070008053200010400090000030060090200407000", "981724365324615879765983142197836254642571938853249716476398521538162497219457683")); // diabolical
            games.Add(Tuple.Create<string, string>("000041000060000200000000000320600000000050041700000000000200300048000000501000000", "872941563169573284453826197324617859986352741715498632697284315248135976531769428"));

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