using System;
using System.Windows.Forms;

namespace Sudoku
{

    /// Программный запуск приложения Sudoku

    static class Program
    {

        ///  Основная точка входа в приложение

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SudokuForm());
        }
    }
}
