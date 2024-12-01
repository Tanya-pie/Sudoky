namespace Sudoku.Models
{

    /// Ячейка для хранения сведений об индексе, значении, номере группы, позиции

    public class Cell
    {

        /// Свойство индекса

        public int Index { get; private set; }

        /// Свойство Value (значения)

        public int Value { get; set; }

        /// Свойство номера группы (подсетки)


        public int GroupNumber { get; private set; }

        /// Свойство Position (положение)

        public Position Position { get; private set; }



        /// Конструктор ячеек

        public Cell(int index, int value, int groupNumber, Position position)
        {
            Index = index;
            Value = value;
            GroupNumber = groupNumber;
            Position = position;
        }
    }
}
