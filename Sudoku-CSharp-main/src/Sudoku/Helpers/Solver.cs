using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Helpers
{

    /// Класс Solver будет использоваться для решения сетки судоку с использованием процесса обратного отслеживания

    public class Solver
    {

        /// Экземпляр сетки

        readonly Grid grid;

        /// Индексы ячеек, исключающие возможность обратного отслеживания

        private readonly List<int> filledCells = new List<int>();

        /// Индексы ячеек с недопустимыми значениями, используемыми в процессе обратного отслеживания

        private List<List<int>> blackListCells;

        /// Случайный объект для использования при создании случайных чисел

        private readonly Random random = new Random();


        /// Конструктор решателя

        public Solver(Grid gridInstance)
        {
            grid = gridInstance ?? new Grid(4, 4);
            InitializeBlackList();
        }


        /// Решает сетку судоку


        public bool Solve()
        {
            // Возвращает значение false, если текущая таблица недействительна
            if (!ValidateGrid()) return false;

            // Инициализируйет заполненные ячейки, чтобы сохранить те, которые будут использоваться при обратном отслеживании
            IntializeFilledCells();

            // Очистите черный список
            ClearBlackList();

            int currentCellIndex = 0;

            // Выполните итерацию по всем ячейкам сетки
            while (currentCellIndex < grid.TotalCells)
            {
                // Если текущий индекс ячейки уже сохранен в заполненных ячейках, передайте его
                if (filledCells.Contains(currentCellIndex))
                {
                    ++currentCellIndex;
                    continue;
                }

                // Очистите черные списки индексов после текущего индекса
                ClearBlackList(cleaningStartIndex: currentCellIndex + 1);

                Cell currentCell = grid.GetCell(cellIndex: currentCellIndex);

                int foundNumber = GetValidNumberForTheCell(currentCellIndex);

                // Не найден действительный ячейки, вернемся назад
                if (foundNumber == 0)
                    currentCellIndex = BackTrackTo(currentCellIndex);
                else
                {
                    // Установите найденное допустимое значение в текущую ячейку
                    grid.SetCellValue(currentCell.Index, foundNumber);
                    ++currentCellIndex;
                }
            }

            return true;
        }


        /// Проверьте, является ли сетка действительной

        public bool ValidateGrid(bool ignoreEmptyCells = false) =>
            grid.Cells.Where(cell => cell.Value != -1)
            .FirstOrDefault(cell => cell.Value != -1 && !IsValidValueForTheCell(cell.Value, cell)) == null;


        /// Проверяет, является ли указанное значение ячейки значением для ячейки

        public bool IsValidValueForTheCell(int value, Cell cell)
        {
            var matchedCell = grid.Cells
                .Where(cellItem => cellItem.Index != cell.Index && (cellItem.GroupNumber == cell.GroupNumber
                || cellItem.Position.Row == cell.Position.Row || cellItem.Position.Column == cell.Position.Column))
                .FirstOrDefault(prop => prop.Value == value);

            return matchedCell == null;
        }


        /// Инициализируйте заполненные ячейки, чтобы сохранить те, которые будут использоваться при обратном отслеживании

        private void IntializeFilledCells()
        {
            filledCells.Clear();
            filledCells.AddRange(grid.Cells.FindAll(cell => cell.Value != -1).Select(cell => cell.Index));
        }


        /// Инициализируйте черный список

        private void InitializeBlackList()
        {
            blackListCells = new List<List<int>>(grid.TotalCells);
            for (int index = 0; index < blackListCells.Capacity; index++)
                blackListCells.Add(new List<int>());
        }


        /// Операция обратного отслеживания для ячейки, указанной с помощью индекса

        private int BackTrackTo(int index)
        {
            // Пройдите мимо защищенных ячеек
            while (filledCells.Contains(--index)) ;

            // Найди отследенный номер сотового
            Cell backTrackedCell = grid.GetCell(index);

            // Добавьте значение в черный список ячейки, в которой был выполнен обратный поиск
            AddToBlacklist(backTrackedCell.Value, cellIndex: index);

            // Сбросьте значение ячейки, отслеживаемое в обратном порядке
            backTrackedCell.Value = -1;

            // Сбросьте черный список, начиная со следующей ячейки текущего отслеживания
            ClearBlackList(cleaningStartIndex: index + 1);

            return index;
        }


        /// Возвращает действительное число для указанного индекса ячейки


        private int GetValidNumberForTheCell(int cellIndex)
        {
            int foundNumber = 0;
            var possibleNumbers = Enumerable.Range(1, grid.GridSize).ToList();

            // Найдите действительные номера для этой ячейки
            var validNumbers = possibleNumbers.Where(val => !blackListCells[cellIndex].Contains(val)).ToArray();

            if (validNumbers.Length > 0)
            {
                // Возвращает (случайное) действительное число из имеющихся действительных чисел
                int choosenIndex = random.Next(validNumbers.Length);
                foundNumber = validNumbers[choosenIndex];
            }

            // Попробуйте получить допустимое (случайное) значение для текущей ячейки, если ни одно допустимое значение не разорвет цикл
            do
            {
                Cell currentCell = grid.GetCell(cellIndex);

                // Проверьте найденный номер, действителен ли он для ячейки, если это неверный номер для ячейки, то добавьте это значение в черный список ячейки
                if (foundNumber != 0 && !grid.Solver.IsValidValueForTheCell(foundNumber, currentCell))
                    AddToBlacklist(foundNumber, cellIndex);
                else
                    break;

                // Получите действительное (случайное) значение из действительных чисел
                foundNumber = GetValidNumberForTheCell(cellIndex: cellIndex);
            } while (foundNumber != 0);

            return foundNumber;
        }


        /// Добавьте значение в указанный индекс черного списка

        private void AddToBlacklist(int value, int cellIndex) => blackListCells[cellIndex].Add(value);


        /// Очищает черный список после определенного индекса ячейки

        private void ClearBlackList(int cleaningStartIndex = 0)
        {
            for (int index = cleaningStartIndex; index < blackListCells.Count; index++)
                blackListCells[index].Clear();
        }
    }
}
