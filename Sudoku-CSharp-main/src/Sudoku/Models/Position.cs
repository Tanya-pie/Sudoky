namespace Sudoku.Models
{

    /// Класс Position для удержания положения строки и столбца в ячейке сетки

    public class Position
    {

        /// Свойство строки

        public int Row { get; private set; }

        /// Свойство столбца

        public int Column { get; private set; }


        /// Конструктор позиций


        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
