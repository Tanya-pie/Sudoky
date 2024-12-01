using Sudoku.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Models
{

    /// Сетка для игры в судоку

    public class Grid
    {

        /// Свойство Total Rows (Общее количество строк)

        public int TotalRows { get; private set; }

        /// Свойство Итоговых столбцов

        public int TotalColumns { get; private set; }

        /// Свойство Размера сетки

        public int GridSize { get; private set; }

        /// Свойство размера подсети

        public int SubGridSize { get; private set; }

        /// Свойство Total Cells (Общие ячейки)

        public int TotalCells { get => TotalRows * TotalColumns; }

        /// Свойство Cells (Ячейки)

        public List<Cell> Cells { get; set; }

        /// Решатель

        public Solver Solver { get; }


        /// Конструктор сетки

        public Grid(int totalRows, int totalColumns)
        {
            TotalRows = totalRows;
            TotalColumns = totalColumns;
            GridSize = Convert.ToInt16(Math.Sqrt(totalRows * totalColumns));
            SubGridSize = Convert.ToInt16(Math.Sqrt(totalRows));
            Cells = new List<Cell>(TotalCells);
            Solver = new Solver(this);
            InitializeCells();
        }


        ///Проверяет, полностью ли заполнена сетка или нет

        public bool IsGridFilled() => Cells.FirstOrDefault(cell => cell.Value == -1) == null;


        /// Проверяет, пуста ли сетка или нет

        public bool IsGridEmpty() => Cells.FirstOrDefault(cell => cell.Value != -1) == null;


        /// Сброс значения ячеек на -1

        public void Clear() => Cells.ForEach(cell => SetCellValue(-1, cell.Index));

        
        /// Get the Cell
        
        public Cell GetCell(int cellIndex) => Cells[cellIndex];


        /// Установление значения ячейки

        public void SetCellValue(int cellIndex, int value) => Cells[cellIndex].Value = value;


        /// Инициализируйте ячейки

        private void InitializeCells()
        {
            for (var x = 0; x < TotalRows; x++)
            {
                for (var y = 0; y < TotalColumns; y++)
                {
                    var groupDivider = Convert.ToInt32(Math.Sqrt(TotalRows));
                    Cells.Add(new Cell(
                        index: x * TotalRows + y,
                        value: -1,
                        groupNumber: (x / groupDivider) + groupDivider * (y / groupDivider) + 1,
                        position: new Position(row: x + 1, column: y + 1)
                    ));
                }
            }
        }
    }
}
