namespace Sudoku.Constants
{

    /// Константы, используемые во всех файлах приложения

    public class FormConstants
    {
        // режимы сетки
        public const string Easy = "Easy";
        public const string Medium = "Medium";  
        public const string Hard = "Hard";

        // сообщения
        public const string PuzzleGenerated = "Сгенерированная головоломка судоку!";
        public const string PuzzleSolved = "Головоломка судоку решена!";
        public const string PuzzleCleared = "Сетка головоломок судоку очищена";
        public const string CongratulationsMessage = "Поздравляем, головоломка судоку решена!";
        public const string PuzzleGridEmpty = "Сетка для головоломки судоку пуста";
        public const string PuzzleInvalidSolve = "К сожалению, головоломка судоку решена неправильно";
        public const string PuzzleValidButNotCompleted = "Текущее состояние головоломки судоку правильное, но она еще не завершена";
        public const string PuzzleInvalidSolveState = "Извините, текущее состояние головоломки судоку неверно";
        public const string PuzzleNoSolution = "У этой головоломки судоку нет решения";

        // фон
        public const string FontFamily = "Maiandra GD";
    }
}
