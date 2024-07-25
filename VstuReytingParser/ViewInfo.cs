namespace VstuReytingParser
{
    public class ViewInfo
    {
        public static void PrintTable(List<List<string>> table, int maxLineLength = 50)
        {
            if (table == null || table.Count == 0)
                return;

            // Определение максимальной ширины каждого столбца
            int[] columnWidths = GetColumnWidths(table, maxLineLength);

            // Печать заголовка
            PrintSeparator(columnWidths);
            foreach (var row in table)
            {
                PrintRow(row, columnWidths, maxLineLength);
                PrintSeparator(columnWidths);
            }
        }

        private static int[] GetColumnWidths(List<List<string>> table, int maxLineLength)
        {
            int columnCount = table.Max(row => row.Count);
            int[] widths = new int[columnCount];

            foreach (var row in table)
            {
                for (int i = 0; i < row.Count; i++)
                {
                    // Вычисление максимальной ширины с учетом переносов
                    var cellLines = SplitString(row[i], maxLineLength);
                    widths[i] = Math.Max(widths[i], cellLines.Max(line => line.Length));
                }
            }

            return widths;
        }

        private static void PrintSeparator(int[] columnWidths)
        {
            string separator = "+" + string.Join("+", columnWidths.Select(w => new string('-', w + 2))) + "+";
            Console.WriteLine(separator);
        }

        private static void PrintRow(List<string> row, int[] columnWidths, int maxLineLength)
        {
            var rowsToPrint = SplitRow(row, columnWidths, maxLineLength);
            foreach (var r in rowsToPrint)
            {
                Console.WriteLine("|" + string.Join("|", r.Select((cell, index) => $" {cell.PadRight(columnWidths[index])} ")) + "|");
            }
        }

        private static List<List<string>> SplitRow(List<string> row, int[] columnWidths, int maxLineLength)
        {
            // Создаем список строк для печати
            List<List<string>> rowsToPrint = new List<List<string>>();

            // Разбиваем каждую ячейку строки на подстроки
            List<List<string>> splitCells = row.Select(cell => SplitString(cell, maxLineLength).ToList()).ToList();

            // Находим максимальное количество подстрок в одной из ячеек
            int maxSubLines = splitCells.Max(cell => cell.Count);

            // Формируем строки для печати
            for (int lineIndex = 0; lineIndex < maxSubLines; lineIndex++)
            {
                List<string> newRow = new List<string>();
                for (int cellIndex = 0; cellIndex < splitCells.Count; cellIndex++)
                {
                    if (lineIndex < splitCells[cellIndex].Count)
                    {
                        newRow.Add(splitCells[cellIndex][lineIndex]);
                    }
                    else
                    {
                        newRow.Add(string.Empty); // Добавляем пустую строку, если подстроки закончились
                    }
                }
                rowsToPrint.Add(newRow);
            }

            return rowsToPrint;
        }

        private static string[] SplitString(string text, int maxLineLength)
        {
            List<string> lines = new List<string>();
            for (int i = 0; i < text.Length; i += maxLineLength)
            {
                lines.Add(text.Substring(i, Math.Min(maxLineLength, text.Length - i)));
            }
            return lines.ToArray();
        }
    }
}
