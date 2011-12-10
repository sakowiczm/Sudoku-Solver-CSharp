using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    public static class Extentions
    {
         public static string CellsToString(this List<Cell> cells)
         {
             var values = new List<int>(81);

             for (int i = 0; i < 9; i++)
             {
                 for (int j = 0; j < 9; j++)
                 {
                     var cell = cells.FirstOrDefault(c => c.X == j && c.Y == i && c.Value.HasValue);
                     values.Add(cell != null ? cell.Value.Value : 0);
                 }
             }

            return string.Join("", values);
        }

         public static List<Cell> StringToCells(this string values)
         {
             var cells = new List<Cell>(81);

             for (int i = 0; i < 9; i++)
             {
                 for (int j = 0; j < 9; j++)
                 {
                     var cell = new Cell();
                     cell.X = i;
                     cell.Y = j;
                     cell.Initial = true;

                     int value = Convert.ToInt32(values[(9 * j) + i].ToString());

                     cell.Value = value == 0 ? (int?)null : value;
                     cell.Block = SudokuSolver.GetBlock(cell.X, cell.Y);

                     cells.Add(cell);
                 }
             }

             return cells;
         }

         public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
         {
             var list = new List<T>(listToClone.Count);

             list.AddRange(listToClone.Select(i => i.Clone()).Cast<T>());

             return list;
         }
    }
}