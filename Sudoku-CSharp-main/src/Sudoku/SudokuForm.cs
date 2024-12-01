using Sudoku.Constants;
using Sudoku.Helpers;
using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{

    /// Класс реализации событий формы Судоку

    public partial class SudokuForm : Form
    {
        int ticks = 0;
        bool timerStarted = false;
        bool gridCleared = false;
        int previousGridSize = 4;
        string gridMode = FormConstants.Easy;
        Grid grid;
        readonly List<Label> cellControls = new List<Label>();


        /// Конструктор форм судоку

        public SudokuForm() => InitializeComponent();

        #region Form Loading Event


        /// Событие загрузки формы судоку

        private void SudokuForm_Load(object sender, EventArgs e)
        {
            cmbBoxMode.SelectedIndex = 0;
            cmbBoxGrid.SelectedIndex = 0;
        }
        
        /// обновление интерфейса

        #endregion

        #region Timer tick Event


        /// Timer Tick (таймер) 

        private void Timer_Tick(object sender, EventArgs e)
        {
            ticks = timerStarted ? ticks + 1 : 0;
            lblTimer.Text = TimeSpan.FromSeconds(ticks).ToString(@"hh\:mm\:ss");

            if (gridCleared)
            {
                gridCleared = false;
                timer.Stop();
            }
        }


        /// сообщение Timer Tick 

        private void MessageTimer_Tick(object sender, EventArgs e)
        {
            ResetStatus();
            messageTimer.Stop();
        }
        /// обновление
        #endregion

        #region Click Events


        /// Сгенерировать событие нажатия кнопки

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            timerStarted = false;
            ticks = 0;
            timer.Stop();

            timer.Start();
            timerStarted = true;

            ResetTheGrid();
            var generator = new Generator(grid, gridMode);
            generator.Generate();
            RefreshTheGrid();

            lblStatus.ForeColor = Color.DeepSkyBlue;
            lblStatus.Text = FormConstants.PuzzleGenerated;

            messageTimer.Start();
        }


        /// Событие подтверждения нажатия кнопки

        private void BtnValidate_Click(object sender, EventArgs e)
        {
            string message;
            var messageColor = Color.DodgerBlue;

            if (grid.IsGridEmpty())
            {
                messageColor = Color.Orange;
                message = FormConstants.PuzzleGridEmpty;
            }
            else if (grid.IsGridFilled() && grid.Solver.ValidateGrid())
            {
                messageColor = Color.LawnGreen;
                message = FormConstants.CongratulationsMessage;
                timer.Stop();
            }
            else if (grid.IsGridFilled() && !grid.Solver.ValidateGrid())
            {
                messageColor = Color.Red;
                message = FormConstants.PuzzleInvalidSolve;
            }
            else if (!grid.IsGridFilled() && grid.Solver.ValidateGrid(ignoreEmptyCells: true))
                message = FormConstants.PuzzleValidButNotCompleted;
            else
            {
                messageColor = Color.Red;
                message = FormConstants.PuzzleInvalidSolveState;
            }

            lblStatus.ForeColor = messageColor;
            lblStatus.Text = message;

            messageTimer.Interval = 10000;
            messageTimer.Start();
        }


        /// нажатие кнопки "Решить"

        private void BtnSolve_Click(object sender, EventArgs e)
        {
            var solver = new Solver(grid);
            var solved = solver.Solve();

            if (solved)
            {
                RefreshTheGrid();

                lblStatus.ForeColor = Color.LawnGreen;
                lblStatus.Text = FormConstants.PuzzleSolved;

                timer.Stop();
            }
            else
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = FormConstants.PuzzleNoSolution;
            }

            messageTimer.Interval = 7000;
            messageTimer.Start();
        }


        /// Событие нажатия кнопки сброса

        private void BtnReset_Click(object sender, EventArgs e)
        {
            timer.Start();
            timerStarted = false;
            gridCleared = true;

            ResetTheGrid();

            lblStatus.ForeColor = Color.White;
            lblStatus.Text = FormConstants.PuzzleCleared;

            messageTimer.Start();
        }

        #endregion

        #region Options Selection Events


        /// Событие изменения индекса выбора выпадающего списка режима

        private void CmbBoxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridMode = cmbBoxMode.SelectedIndex switch
            {
                2 => FormConstants.Hard,
                1 => FormConstants.Medium,
                _ => FormConstants.Easy
            };

        }


        /// Событие изменения индекса выбора выпадающего списка сетки

        private void CmbBoxGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            int columns;
            int rows = columns = cmbBoxGrid.SelectedIndex == 1 ? 9 : 4;
            grid = new Grid(rows, columns);
            timerStarted = false;
            ResetStatus();
            CreateTheGrid();
        }
        /// обновление
        #endregion

        #region Grid Events


        /// Создание сетки судоку в пользовательском интерфейсе оконной формы

        private void CreateTheGrid()
        {
            ClearTheGrid();
            previousGridSize = grid.GridSize + 1;

            var cellTopLocation = 6;
            var cellWidth = grid.GridSize == 9 ? 39 : 91;
            var cellHeight = grid.GridSize == 9 ? 39 : 91;
            var cellFontFamily = FormConstants.FontFamily;
            var cellFontSize = grid.GridSize == 9 ? 16 : 22;

            // Выполняет итерацию по строкам
            for (var x = 0; x < grid.TotalRows; x++)
            {
                var cellLeftLocation = 5;

                // Выполняет итерацию по столбцам и помещает каждую ячейку рядом с текущей строкой
                for (var y = 0; y < grid.TotalColumns; y++)
                {
                    // Контрольная метка внутри ячейки
                    var cell = new Label
                    {
                        // индекс ячейки
                        Tag = x * grid.TotalRows + y,

                        // Свойства пользовательского интерфейса
                        Width = cellWidth,
                        Height = cellHeight,
                        Left = cellLeftLocation,
                        Top = cellTopLocation,
                        Cursor = Cursors.Hand,
                        ForeColor = Color.Black,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font(cellFontFamily, cellFontSize),
                        BackColor = Color.GhostWhite,
                    };

                    // Щелкните Событие для ячейки
                    cell.MouseClick += Cell_Click;
                    // Событие наведения курсора мыши на ячейку
                    cell.MouseHover += Cell_Hover;
                    // Событие выхода мыши из ячейки
                    cell.MouseLeave += Cell_Leave;

                    // Измените "расположение слева от ячейки", добавив левое заполнение для других ячеек в текущем столбце
                    cellLeftLocation += cellWidth + ((y + 1) % grid.SubGridSize == 0 ? 5 : 1);

                    // Добавьте ячейку в список "Элементы управления ячейками" и в таблицу
                    cellControls.Add(cell);
                    gridView.Controls.Add(cell);
                }

                // Измените "расположение ячейки в верхней части" с помощью верхнего отступа для других ячеек в текущей строке
                cellTopLocation += cellHeight + ((x + 1) % grid.SubGridSize == 0 ? 5 : 2);
            }
        }


        /// Событие наведения курсора мыши на ячейку

        private void Cell_Hover(object sender, EventArgs e)
        {
            Label cellControl = (sender as Label);
            cellControl.BackColor = Color.ForestGreen;
            cellControl.ForeColor = Color.GhostWhite;
        }


        /// Оставьте событие для ячейки

        private void Cell_Leave(object sender, EventArgs e)
        {
            Label cellControl = (sender as Label);
            cellControl.BackColor = Color.GhostWhite;
            cellControl.ForeColor = Color.Black;
        }


        /// Щелкните Событие для ячейки


        private void Cell_Click(object sender, MouseEventArgs e)
        {
            Label clickedCellControl = (sender as Label);
            int cellIndex = (int)clickedCellControl.Tag;

            if (grid.GridSize == 9)
            {
                var numpadGrid9Dialog = new NumpadGrid9Dialog();

                #region Cell Click Location Calcutaion to display Numpad Grid 9 Dialog

                int numpadLocationX = clickedCellControl.Location.X - (numpadGrid9Dialog.Width / 4) + this.Location.X;
                int numpadLocationY = clickedCellControl.Location.Y - (numpadGrid9Dialog.Height / 4) + this.Location.Y;

                if (numpadLocationX < 0) numpadLocationX = 0;
                if (numpadLocationY < 0) numpadLocationY = 0;

                if (Screen.PrimaryScreen.WorkingArea.Width < numpadGrid9Dialog.Width + numpadLocationX)
                    numpadLocationX = Screen.PrimaryScreen.WorkingArea.Width - numpadGrid9Dialog.Width;

                if (Screen.PrimaryScreen.WorkingArea.Height < numpadGrid9Dialog.Height + numpadLocationY)
                    numpadLocationY = Screen.PrimaryScreen.WorkingArea.Height - numpadGrid9Dialog.Height;

                Point numpadLocation = new Point(numpadLocationX, numpadLocationY);
                numpadGrid9Dialog.Location = numpadLocation;

                #endregion

                // Откройтие диалогового окна с цифровой клавиатурой
                numpadGrid9Dialog.Show();

                // Обработайтка события закрытия диалогового окна с цифровой клавиатурой, чтобы получить результат
                numpadGrid9Dialog.FormClosed += (object s, FormClosedEventArgs ea) =>
                {
                    if (numpadGrid9Dialog.Value != -1 && numpadGrid9Dialog.Value != 0)
                    {
                        grid.SetCellValue(cellIndex, numpadGrid9Dialog.Value);
                        RefreshTheGrid();
                    }
                    else if (numpadGrid9Dialog.Value == 0)
                    {
                        grid.SetCellValue(cellIndex, -1);
                        RefreshTheGrid();
                    }

                    // Закройте диалоговое окно цифровой клавиатуры
                    numpadGrid9Dialog.Dispose();
                };
            }
            else
            {
                var numpadGrid4Dialog = new NumpadGrid4Dialog();

                #region Cell Click Location Calcutaion to display Numpad Grid 9 Dialog

                int numpadLocationX = clickedCellControl.Location.X - (numpadGrid4Dialog.Width / 4) + this.Location.X;
                int numpadLocationY = clickedCellControl.Location.Y - (numpadGrid4Dialog.Height / 4) + this.Location.Y;

                if (numpadLocationX < 0) numpadLocationX = 0;
                if (numpadLocationY < 0) numpadLocationY = 0;

                if (Screen.PrimaryScreen.WorkingArea.Width < numpadGrid4Dialog.Width + numpadLocationX)
                    numpadLocationX = Screen.PrimaryScreen.WorkingArea.Width - numpadGrid4Dialog.Width;

                if (Screen.PrimaryScreen.WorkingArea.Height < numpadGrid4Dialog.Height + numpadLocationY)
                    numpadLocationY = Screen.PrimaryScreen.WorkingArea.Height - numpadGrid4Dialog.Height;

                Point numpadLocation = new Point(numpadLocationX, numpadLocationY);
                numpadGrid4Dialog.Location = numpadLocation;

                #endregion

                // Откройте диалоговое окно с цифровой клавиатурой
                numpadGrid4Dialog.Show();

                // Обработайте событие закрытия диалогового окна с цифровой клавиатурой, чтобы получить результат
                numpadGrid4Dialog.FormClosed += (object s, FormClosedEventArgs ea) =>
                {
                    if (numpadGrid4Dialog.Value != -1 && numpadGrid4Dialog.Value != 0)
                    {
                        grid.SetCellValue(cellIndex, numpadGrid4Dialog.Value);
                        RefreshTheGrid();
                    }
                    else if (numpadGrid4Dialog.Value == 0)
                    {
                        grid.SetCellValue(cellIndex, -1);
                        RefreshTheGrid();
                    }

                    // Закройте диалоговое окно цифровой клавиатуры
                    numpadGrid4Dialog.Dispose();
                };

            }
        }


        /// Сбросьте метку состояния
        private void ResetStatus() => lblStatus.Text = string.Empty;


        /// Обновление сетки судоку в пользовательском интерфейсе оконной формы

        private void RefreshTheGrid()
            => cellControls.ForEach(cell =>
                {
                    var cellIndex = (int)cell.Tag;
                    var cellValue = grid.GetCell(cellIndex).Value;
                    cell.Text = cellValue != -1 ? cellValue.ToString() : string.Empty;
                });


        /// Сброс сетки судоку в пользовательском интерфейсе оконной формы

        private void ResetTheGrid()
            => Parallel.Invoke(
                () => cellControls.ForEach(cell => cell.Text = string.Empty),
                () => grid.Cells.ForEach(prop => prop.Value = -1));


        /// Очистите сетку судоку

        private void ClearTheGrid()
        {
            cellControls.Clear();
            for (int i = 0; i < previousGridSize; i++) gridView.Controls.Clear();
            gridView.BackgroundColor = Color.FromArgb(47, 47, 47);
            gridView.Refresh();
        }

        #endregion

        private void lblStatus_Click(object sender, EventArgs e)
        {

        }
    }
}
