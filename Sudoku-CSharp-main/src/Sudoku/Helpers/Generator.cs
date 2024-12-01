using Sudoku.Constants;
using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Helpers
{
    ///Generator Class для создания игральной сетки
    public class Generator
    {

        /// Экземпляр сетки

        readonly Grid grid;
        
        /// Режим игры
        
        private readonly string mode;

        ///Экземпляр решателя

        private readonly Solver solver;

        ///Случайный объект для использования при создании случайных чисел 

        private readonly Random random = new Random();

        /// Конструктор генератора

        /// name="gridInstance">Экземпляр сетки
        public Generator(Grid gridInstance, string gridMode)
        {
            grid = gridInstance ?? new Grid(4, 4);
            mode = gridMode;
            solver = new Solver(grid);
        }


        /// Генерация сетки судоку

        public bool Generate()
        {
            solver.Solve();
            GenerateGrid();

            return true;
        }


        /// Создание сетки судоку с несколькими пустыми ячейками. И их определения, в зависимости от сложности (по общепринятым правилам)

        private void GenerateGrid()
        {
            var cellValueIndexes = (mode, grid.GridSize) switch
            {
                (FormConstants.Hard, 9) => GenerateRandomIndexes(random.Next(16, 24)),
                (FormConstants.Medium, 9) => GenerateRandomIndexes(random.Next(24, 31)),
                (FormConstants.Easy, 9) => GenerateRandomIndexes(random.Next(31, 39)),
                (FormConstants.Hard, 4) => GenerateRandomIndexes(random.Next(1, 4)),
                (FormConstants.Medium, 4) => GenerateRandomIndexes(random.Next(4, 7)),
                _ => GenerateRandomIndexes(random.Next(5, 9))
            };

            grid.Cells.ForEach(cell => cell.Value = !cellValueIndexes.Contains(cell.Index) ? -1 : cell.Value);
        }

        /// Генерация случайных индексов, которые будут содержать значения ячеек

        ///  name="requiredNumbers"
        private List<int> GenerateRandomIndexes(int requiredNumbers)
        {
            return Enumerable.Range(0, requiredNumbers).Select(x => random.Next(0, grid.TotalCells)).ToList();
        }
    }
}
