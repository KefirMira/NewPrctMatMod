using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Aspose.Cells;
using Microsoft.Win32;
using NewPrctMatMod.Pages;
using PrctMatMod.Windows;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Orientation = System.Windows.Controls.Orientation;

namespace PrctMatMod.Pages
{
    public partial class GameTheoryPage : Page
    {
        private double[,] matrix;
        private int rows = 0;
        private int columns = 0;
        private List<string> strat;
        private double[,] newmarr;
        private  double v1 = 0;
        private  double v2 = 0;
        private double[] ver1;
        private double[] ver2;
        public GameTheoryPage()
        {
            InitializeComponent();
        }

        public void ReadExcel()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    string fileName = openFileDialog.FileName;
                    // Загрузить файл Excel
                    Workbook wb = new Workbook(fileName);

                    // Получить все рабочие листы
                    WorksheetCollection collection = wb.Worksheets;
                    // Получить рабочий лист, используя его индекс
                    Worksheet worksheet = collection[0];
                    rows = worksheet.Cells.MaxDataRow + 1;
                    columns = worksheet.Cells.MaxDataColumn + 1;
                    GenerateMassive(rows, columns);
                    //matrix = new double[rows, columns];
                    // Цикл по строкам
                    for (int i = 0; i < rows; i++)
                    {
                        // Перебрать каждый столбец в выбранной строке
                        for (int j = 0; j < columns; j++)
                        {
                            matrix[i, j] = Convert.ToDouble(worksheet.Cells[i, j].Value);
                        }
                    }

                    List<TextBox> textBoxes = new List<TextBox>();
                    foreach (UIElement item in MatrixStackPanel.Children)
                    {
                        if (item is StackPanel)
                        {
                            foreach (UIElement item1 in (item as StackPanel).Children)
                            {
                                if (item1.GetType() == typeof(TextBox))
                                    textBoxes.Add(item1 as TextBox);
                            }
                        }
                    }

                    int g = 0;
                    for (int i = 0; i < matrix.GetUpperBound(0) + 1; i++)
                    {
                        for (int j = 0; j < matrix.GetUpperBound(1) + 1; j++)
                        {
                            textBoxes[g].Text = matrix[i, j].ToString();
                            g++;
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка чтения");
            }
        }

        private void GenerateButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int a = Convert.ToInt32(HeightTextBox.Text);
                int b = Convert.ToInt32(WidthTextBox.Text);
                GenerateMassive(a,b);
            }
            catch
            {
                MessageBox.Show("Ошибка ввода данных! Убедитесь в правильности введённых данных!", "Ошибка!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }

            private void ReadButton_OnClick(object sender, RoutedEventArgs e)
            {
                try
                {
                    List<TextBox> textBoxes = new List<TextBox>();
                    foreach (UIElement item in MatrixStackPanel.Children)
                    {
                        if (item is StackPanel)
                        {
                            foreach (UIElement item1 in (item as StackPanel).Children)
                            {
                                if (item1.GetType() == typeof(TextBox))
                                    textBoxes.Add(item1 as TextBox);
                            }
                        }
                    }

                    int k = 0;
                    int l = 0;
                    for (int i = 0; i < textBoxes.Count; i++)
                    {
                        if (k < rows)
                        {

                            matrix[k, l] = Convert.ToInt32(textBoxes[i].Text);
                            l++;
                            if (l == columns)
                            {
                                l = 0;
                                k++;
                            }
                        }
                        else
                        {
                            k = 0;
                        }
                    }

                    double[] masRows = new double[rows];
                    double[] masColumns = new double[columns];
                    //ищем минимумы в строках
                    for (int i = 0; i < matrix.GetUpperBound(0) + 1; i++)
                    {
                        double minStr = matrix[i, 0];
                        for (int j = 0; j < matrix.GetUpperBound(1) + 1; j++)
                        {
                            if (minStr > matrix[i, j])
                                minStr = matrix[i, j];
                        }

                        masRows[i] = minStr;
                    }

                    //ищем масимумы в столбцах
                    for (int j = 0; j < matrix.GetUpperBound(1) + 1; j++)
                    {
                        double maxStr = matrix[0, j];
                        for (int i = 0; i < matrix.GetUpperBound(0) + 1; i++)
                        {
                            if (maxStr < matrix[i, j])
                                maxStr = matrix[i, j];
                        }

                        masColumns[j] = maxStr;
                    }

                    string str = "";
                    str += "Минимальные элементы в строках соответственно равны -\n";
                    for (int i = 0; i < masRows.Length; i++)
                    {
                        str += $"Строка №{i} - {masRows[i]}\n";
                    }

                    str += "Максимальные элементы в столбцах соответственно равны -\n";
                    for (int i = 0; i < masColumns.Length; i++)
                    {
                        str += $"Столбец №{i} - {masColumns[i]}\n";
                    }

                    str +=
                        $"Седловые точки равны\nСреди строк - {masRows.Max()} - максимум среди минимумов по строкам\nСреди столбцов - {masColumns.Min()} - минимум среди максимум по колонкам";
                    if (masRows.Max() == masColumns.Min())
                    {
                        str += "\nСедловые точки равны - чистые стратегии";
                    }
                    else
                    {
                        NextButton.Visibility = Visibility.Visible;
                        ChartButton.Visibility = Visibility.Visible;
                    }

                    ResultSedlTextBox.Text = str;
                }
                catch
                {
                    MessageBox.Show("Ошибка считывания данных");
                }

            }

        public void Result(double[,] mas)
        {
            try
            {
                double[] p = new double[rows];
                double[] q = new double[columns];
                
                double a = mas[0, 0];
                double b = mas[0, 1];
                double c = mas[1, 0];
                double d = mas[1, 1];
                
                double f =((d*1.0 - c) / (a + d - c - b));
                p[0] = (d*1.0  - c) / (a + d - c - b);
                p[1] = (a  -b*1.0) / (a + d - c - b);
                q[0] = (d*1.0  - b) / ((a + d) -(b + c) );
                q[1] = 1-q[0];
                
                double v = (a*1.0 *d - b*c)/(a+d-c-b);
                
                FullResTextBlock.Text = $"Оптимальной смешанной стратегией игрока А является стратегия Sa = |{p[0]};{p[1]}|, игрока B - Sb = |{q[0]},{q[1]}|. Цена игры v = {v}";
            }
            catch
            {
                MessageBox.Show("Ошибка вычислений");
            }
        }

        public void Simplex(double[,] matrix1,int st,DocX document = null)
        {
            try
            {
                
                strat = new List<string>();

                double[,] newmatr = new double[matrix1.GetUpperBound(0) + 2,
                    matrix1.GetUpperBound(1) + matrix1.GetUpperBound(0) + 3];
                if (st == 1)
                {
                    for (int i = 0; i < newmatr.GetUpperBound(0) + 1; i++)
                    {
                        for (int j = 0; j < newmatr.GetUpperBound(1) + 1; j++)
                        {
                            if (i < matrix1.GetUpperBound(0) + 1 && j < matrix1.GetUpperBound(1) + 1)
                            {
                                newmatr[i, j] = matrix1[i, j];
                            }
                            else if (i == newmatr.GetUpperBound(0) && j < matrix1.GetLength(1))
                            {
                                newmatr[i, j] = -1.0;

                            }
                            else if (j - matrix1.GetUpperBound(1) - 1 == i && j != newmatr.GetUpperBound(1))
                            {
                                newmatr[i, j] = 1.0;
                                strat.Add($"{j + 1}");
                            }
                            else if (j == newmatr.GetUpperBound(1) && i != newmatr.GetUpperBound(0))
                            {
                                newmatr[i, j] = 1.0;
                            }
                        }
                    }
                }
                else if (st == 2)
                {
                    for (int i = 0; i < newmatr.GetUpperBound(0) + 1; i++)
                    {
                        for (int j = 0; j < newmatr.GetUpperBound(1) + 1; j++)
                        {
                            if (i < matrix1.GetUpperBound(0) + 1 && j < matrix1.GetUpperBound(1) + 1)
                            {
                                newmatr[i, j] = -1 * matrix1[i, j];
                            }
                            else if (i == newmatr.GetUpperBound(0) && j < matrix1.GetLength(1))
                            {
                                newmatr[i, j] = 1.0;
                            }
                            else if (j - matrix1.GetUpperBound(1) - 1 == i && j != newmatr.GetUpperBound(1))
                            {
                                newmatr[i, j] = 1.0;
                                strat.Add($"{j + 1}");
                            }
                            else if (j == newmatr.GetUpperBound(1) && i != newmatr.GetUpperBound(0))
                            {
                                newmatr[i, j] = -1.0;
                            }
                        }
                    }
                }

                for (int i = 0; i < newmatr.GetUpperBound(0) + 1; i++)
                {
                    for (int j = 0; j < newmatr.GetUpperBound(1) + 1; j++)
                    {
                        Console.Write($"{newmatr[i, j]}\t");
                    }

                    Console.WriteLine();
                }

                foreach (var item in strat)
                {
                    Console.WriteLine(item);
                }

                if (document != null)
                {
                    Paragraph paragraph1 = document.InsertParagraph();
                    paragraph1.AppendLine($"Преобразованная матрица с дополнительными(свободными) переменными")
                        .Font("Times New Roman")
                        .FontSize(14)
                        .Alignment = Alignment.left;

                    Table table = document.AddTable(newmatr.GetUpperBound(0) + 1, newmatr.GetUpperBound(1) + 1);

                    for (int i = 0; i < newmatr.GetUpperBound(0) + 1; i++)
                    {
                        for (int j = 0; j < newmatr.GetUpperBound(1) + 1; j++)
                        {
                            table.Rows[i]
                                .Cells[j]
                                .Paragraphs
                                .First()
                                .Append(newmatr[i, j].ToString())
                                .Font("Times New Roman")
                                .FontSize(12)
                                .Bold();
                        }
                    }

                    table.Alignment = Alignment.center;
                    document.InsertTable(table);


                    Paragraph paragraph2 = document.InsertParagraph();
                    paragraph2.AppendLine($"Изначальные выигрышные стратегии:")
                        .Font("Times New Roman")
                        .FontSize(12)
                        .Alignment = Alignment.left;

                    foreach (var item in strat)
                    {
                        Paragraph paragraph3 = document.InsertParagraph();
                        paragraph3.AppendLine($"{item}")
                            .Font("Times New Roman")
                            .FontSize(12)
                            .Alignment = Alignment.left;
                    }
                }

                if (st == 1)
                {
                    while (CheckForMax(newmatr))
                    {
                        int indexColumn = SearchIndexMinColumn(newmatr);
                        int indexRow = SearchIndexMinRow(newmatr, indexColumn);
                        strat[indexRow] = $"{indexColumn + 1}";
                        double[,] newnewmatr = NullRow(newmatr, indexColumn, indexRow);
                        newmatr = Res(newmatr, indexColumn, indexRow, newnewmatr);

                        if (document != null)
                        {
                            Paragraph paragraph3 = document.InsertParagraph();
                            paragraph3.AppendLine(
                                    $"Доминирующий столбец - {indexColumn + 1}\nДоминирующая строка - {indexRow + 1}\nДоминирующий столбец определяется поиском наименьшего среди всех отрицательных элементов решений\nДоминирующая строка ищется по следующей формуле:\nЭлемент решения/элемент текущей строки доминирующего столбца")
                                .Font("Times New Roman")
                                .FontSize(12)
                                .Alignment = Alignment.left;

                            Paragraph paragraph5 = document.InsertParagraph();
                            paragraph5.AppendLine(
                                    $"Преобразование матрицы:\n Для решения матрицы был выбран метод прямоугольника. Его суть заключчается в том, что текущий элемент вычитает из себя резульатты решения, полученные при перемножении членов матрицы расположенных на пересечении доминирующего элемента с текущим, делёных на доминирующий элемент")
                                .Font("Times New Roman")
                                .FontSize(12)
                                .Alignment = Alignment.left;

                            Table table1 = document.AddTable(newmatr.GetUpperBound(0) + 1,
                                newmatr.GetUpperBound(1) + 1);

                            for (int i = 0; i < newmatr.GetUpperBound(0) + 1; i++)
                            {
                                for (int j = 0; j < newmatr.GetUpperBound(1) + 1; j++)
                                {
                                    table1.Rows[i]
                                        .Cells[j]
                                        .Paragraphs
                                        .First()
                                        .Append(newmatr[i, j].ToString())
                                        .Font("Times New Roman")
                                        .FontSize(12)
                                        .Bold();
                                }
                            }

                            table1.Alignment = Alignment.center;
                            document.InsertTable(table1);
                            if (CheckForMax(newmatr))
                            {
                                Paragraph paragraph7 = document.InsertParagraph();
                                paragraph7.AppendLine(
                                        $"Последныы строка(строка с решениями) всё ещё имеет отрицательные элементы, решение продолжается")
                                    .Font("Times New Roman")
                                    .FontSize(12)
                                    .Alignment = Alignment.left;
                            }
                        }

                        for (int i = 0; i < newmatr.GetUpperBound(0) + 1; i++)
                        {
                            for (int j = 0; j < newmatr.GetUpperBound(1) + 1; j++)
                            {
                                Console.Write($"{newmatr[i, j]}\t");
                            }

                            Console.WriteLine();
                        }

                        foreach (var item in strat)
                        {
                            Console.WriteLine(item);
                        }

                    }
                }
                else if (st == 2)
                {
                    while (CheckForMin(newmatr))
                    {
                        int indexRow = SearchIndexMinColumnForMin(newmatr);
                        int indexColumn = SearchIndexMinRowForMin(newmatr, indexRow);
                        strat[indexRow] = $"{indexColumn + 1}";
                        double[,] newnewmatr = NullRow(newmatr, indexColumn, indexRow);
                        newmatr = Res(newmatr, indexColumn, indexRow, newnewmatr);

                        if (document != null)
                        {
                            Paragraph paragraph3 = document.InsertParagraph();
                            paragraph3.AppendLine(
                                    $"Доминирующий столбец - {indexColumn + 1}\nДоминирующая строка - {indexRow + 1}\nДоминирующий столбец определяется поиском наименьшего среди всех отрицательных элементов решений\nДоминирующая строка ищется по следующей формуле:\nЭлемент решения/элемент текущей строки доминирующего столбца")
                                .Font("Times New Roman")
                                .FontSize(12)
                                .Alignment = Alignment.left;

                            Paragraph paragraph5 = document.InsertParagraph();
                            paragraph5.AppendLine(
                                    $"Преобразование матрицы:\n Для решения матрицы был выбран метод прямоугольника. Его суть заключчается в том, что текущий элемент вычитает из себя резульатты решения, полученные при перемножении членов матрицы расположенных на пересечении доминирующего элемента с текущим, делёных на доминирующий элемент")
                                .Font("Times New Roman")
                                .FontSize(12)
                                .Alignment = Alignment.left;

                            Table table1 = document.AddTable(newmatr.GetUpperBound(0) + 1,
                                newmatr.GetUpperBound(1) + 1);

                            for (int i = 0; i < newmatr.GetUpperBound(0) + 1; i++)
                            {
                                for (int j = 0; j < newmatr.GetUpperBound(1) + 1; j++)
                                {
                                    table1.Rows[i]
                                        .Cells[j]
                                        .Paragraphs
                                        .First()
                                        .Append(newmatr[i, j].ToString())
                                        .Font("Times New Roman")
                                        .FontSize(12)
                                        .Bold();
                                }
                            }

                            table1.Alignment = Alignment.center;
                            document.InsertTable(table1);
                            if (CheckForMin(newmatr))
                            {
                                Paragraph paragraph7 = document.InsertParagraph();
                                paragraph7.AppendLine(
                                        $"Последныы строка(строка с решениями) всё ещё имеет отрицательные элементы, решение продолжается")
                                    .Font("Times New Roman")
                                    .FontSize(12)
                                    .Alignment = Alignment.left;
                            }
                        }

                        for (int i = 0; i < newmatr.GetUpperBound(0) + 1; i++)
                        {
                            for (int j = 0; j < newmatr.GetUpperBound(1) + 1; j++)
                            {
                                Console.Write($"{newmatr[i, j]}\t");
                            }

                            Console.WriteLine();
                        }

                        foreach (var item in strat)
                        {
                            Console.WriteLine(item);
                        }

                    }
                }

                if (st == 1)
                {
                    ver1 = new double[newmatr.GetUpperBound(0)];
                }
                else
                {
                    ver2 = new double[newmatr.GetUpperBound(0)];
                }

                FullResTextBlock.Text += $"Вероятности игрока {st}\n";
                for (int i = 0; i < newmatr.GetUpperBound(0); i++)
                {
                    if (st == 1)
                    {
                        FullResTextBlock.Text +=
                            $"Вероятность использования стратегии {strat[i]}={Math.Round(newmatr[i, newmatr.GetUpperBound(1)] / newmatr[newmatr.GetUpperBound(0), newmatr.GetUpperBound(1)], 2)}\n";
                        ver1[i] = newmatr[i, newmatr.GetUpperBound(1)] /
                                  newmatr[newmatr.GetUpperBound(0), newmatr.GetUpperBound(1)];
                    }
                    else
                    {
                        if (Convert.ToInt32(strat[i]) < matrix.GetUpperBound(1) + 1)
                        {
                            FullResTextBlock.Text +=
                                $"Вероятность использования стратегии {strat[i]}={Math.Round(newmatr[i, newmatr.GetUpperBound(1)] / newmatr[newmatr.GetUpperBound(0), newmatr.GetUpperBound(1)] * -1, 2)}\n";
                            ver2[i] = newmatr[i, newmatr.GetUpperBound(1)] /
                                newmatr[newmatr.GetUpperBound(0), newmatr.GetUpperBound(1)] * -1;
                        }
                    }
                }

                if (st == 1)
                {
                    newmarr = new double[ver1.Length, ver1.Length];
                    int k = 0;
                    for (int i = 0; i < newmarr.GetUpperBound(0) + 1; i++)
                    {
                        for (int j = 0; j < newmarr.GetUpperBound(1) + 1; j++)
                        {
                            if (Convert.ToInt32(strat[k]) < matrix.GetUpperBound(1) + 1)
                            {
                                newmarr[i, j] = matrix[j, (Convert.ToInt32(strat[i]) - 1)];
                            }
                        }
                    }
                }


                if (newmatr[newmatr.GetUpperBound(0), newmatr.GetUpperBound(1)] > 0)
                {
                    FullResTextBlock.Text +=
                        $"Цена игры = {newmatr[newmatr.GetUpperBound(0), newmatr.GetUpperBound(1)]}\n";
                    v1 = newmatr[newmatr.GetUpperBound(0), newmatr.GetUpperBound(1)];
                }
                else
                {
                    v2 = -1 * newmatr[newmatr.GetUpperBound(0), newmatr.GetUpperBound(1)];
                    FullResTextBlock.Text +=
                        $"Цена игры = {-1 * newmatr[newmatr.GetUpperBound(0), newmatr.GetUpperBound(1)]}\n";
                }

                if (document != null)
                {
                    Paragraph paragraph4 = document.InsertParagraph();
                    paragraph4.AppendLine($"Результат решения: {FullResTextBlock.Text}")
                        .Font("Times New Roman")
                        .FontSize(12)
                        .Alignment = Alignment.left;
                }

            }
            catch
            {
                MessageBox.Show("Ошибка вычислений");
            }

        }

        public double[,] NullRow(double[,] newmatr,int indexColumn,int indexRow)
        {
                double[,] newnewmatr = new double[newmatr.GetUpperBound(0)+1,newmatr.GetUpperBound(1)+1];
            try
            {
                for (int i = 0; i < newmatr.GetUpperBound(1)+1; i++)
                {
                    newnewmatr[indexRow, i] = newmatr[indexRow, i] / newmatr[indexRow, indexColumn];
                }

                for (int i = 0; i < newnewmatr.GetUpperBound(0)+1; i++)
                {
                    if(i==indexRow)
                        continue;
                    newnewmatr[i, indexColumn] = 0;
                }

            }
            catch
            {
                MessageBox.Show("Ошибка преобразования");
            }
                return newnewmatr;
        }
        
        

        public double[,] Res(double[,] newmatr, int indexColumn, int indexRow, double[,] newnewmatr)
        {
            try
            {
                for (int i = 0; i < newnewmatr.GetUpperBound(0)+1; i++)
                {
                    for (int j = 0; j < newnewmatr.GetUpperBound(1)+1; j++)
                    {
                        if(i==indexRow||j==indexColumn)
                            continue;
                        newnewmatr[i, j] = newmatr[i, j] - (newmatr[i,indexColumn]*newmatr[indexRow,j]) / newmatr[indexRow, indexColumn];
                    }
                }

            }
            catch
            {
                MessageBox.Show("Ошибка вычислений");
            }
            
            return newnewmatr;
        }
        
        

        public bool CheckForMax(double[,] newmatr)
        {
            try
            {
                for (int i = 0; i < newmatr.GetUpperBound(1)+1; i++)
                {
                    if (newmatr[newmatr.GetUpperBound(0) , i]<0)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
            
            return false;
        }
        
        
        public bool CheckForMin(double[,] newmatr)
        {
            try
            {
                for (int i = 0; i < newmatr.GetUpperBound(0); i++)
                {
                    if (newmatr[ i,newmatr.GetUpperBound(1) ]<0)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
            
            return false;
        }
        

        public int SearchIndexMinColumn(double[,] newmatr)
        {
            int index = 0;
            try
            {
                double min = newmatr[newmatr.GetUpperBound(0) , 0];
            
                for (int i = 0; i < newmatr.GetUpperBound(1)+1; i++)
                {
                    if (newmatr[newmatr.GetUpperBound(0) , i] < min)
                    {
                        min = newmatr[newmatr.GetUpperBound(0) , i];
                        index = i;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
            
            return index;
        }

        public int SearchIndexMinRow(double[,] newmatr,int j)
        {
                int index = 0;
            try
            {
                double[] min = new double[newmatr.GetUpperBound(0)];

                for (int i = 0; i < newmatr.GetUpperBound(0); i++)
                {
                    min[i] = newmatr[i, newmatr.GetUpperBound(1) ] / newmatr[i, j];
                }
                for (int i = 0; i < min.Length; i++)
                {
                    if (min[index] > min[i])
                        index = i;
                }


            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
                        return index;
        }
        
        
        
        public int SearchIndexMinColumnForMin(double[,] newmatr)
        {
                int index = 0;
            try
            {
                double min = newmatr[newmatr.GetUpperBound(0) , 0];
            
                for (int i = 0; i < newmatr.GetUpperBound(0); i++)
                {
                    if (newmatr[ i,newmatr.GetUpperBound(1) ] < min)
                    {
                        min = newmatr[ i,newmatr.GetUpperBound(1) ];
                        index = i;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
            
            return index;
        }

        public int SearchIndexMinRowForMin(double[,] newmatr,int j)
        {
                int index = 0;
            try
            {
                double min = newmatr[j , 0];
                for (int i = 0; i < newmatr.GetUpperBound(1)+1; i++)
                {
                    if (min > newmatr[j, i])
                    {
                        min = newmatr[j, i];
                        index = i;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
            
            return index;
        }

        
        private void NextButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (rows == 2 && columns == 2)
            {
                Result(matrix);
            }
            else
            {
                try
                {
                    FullResTextBlock.Text = "";
                    Simplex(matrix,1);
                    double[,] newmatr = new double[matrix.GetUpperBound(1)+1, matrix.GetUpperBound(0)+1];
                    for (int i = 0; i < newmatr.GetUpperBound(0)+1; i++)
                    {
                        for (int j = 0; j < newmatr.GetUpperBound(1)+1; j++)
                        {
                            newmatr[i, j] = matrix[j, i];
                        }
                    }
                    Simplex(newmatr,2);
                    DopInfoStackPanel.Visibility = Visibility.Visible;
                }
                catch
                {
                    MessageBox.Show("Ошибка");
                }
            }
        }

        private void ChartButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Chart2x2Window window = new Chart2x2Window(matrix);
                window.ShowDialog();
                this.IsEnabled = false;
                window.Close();
                this.IsEnabled = true;
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
            
        }

        


        private void GenerateMassive(int a, int b)
        {
            try
            {
                MatrixStackPanel.Children.Clear();

                columns = b;
                rows = a;
                if (a > 0 && b > 0)
                {
                    // MatrixStackPanel.Width = b * 50 + 100;
                    // MatrixStackPanel.Height = a * 50 + 100;
                    matrix = new double[a, b];
                    for (int i = 0; i < matrix.GetUpperBound(0)+1; i++)
                    {
                        StackPanel stackPanel = new StackPanel()
                        {
                            Orientation = Orientation.Horizontal,
                            Width = b * 40 + 100,
                            Height = 40
                        };
                        for (int j = 0; j < matrix.GetUpperBound(1)+1; j++)
                        {
                            TextBox addTextBox = new TextBox()
                            {
                                Width = 30,
                                Height = 30,
                                Margin = new Thickness(5),
                                Name = "textBox"+i+j
                            };
                            stackPanel.Children.Add(addTextBox);
                        }
                        MatrixStackPanel.Children.Add(stackPanel);
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка ввода данных! Убедитесь в правильности введённых данных!", "Ошибка!",
                        MessageBoxButton.OK, MessageBoxImage.Error);    
                }

                ReadButton.Visibility = Visibility.Visible;
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
            
        }
        
        
        
        

        private void LoadDataButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    string[] inputData = File.ReadAllLines(openFileDialog.FileName);
                    string[] inputData1 = new string[inputData.Length-1];
                    string[] numbersColRowCount = inputData[0].Split(' ');
                    this.columns = Convert.ToInt32(numbersColRowCount[1]);
                    this.rows = Convert.ToInt32(numbersColRowCount[0]);
                    GenerateMassive(rows,columns);
                    matrix = new double[rows, columns];
                    int g = 0;
                    for (int i = 1; i < inputData.Length; i++)
                    {
                        inputData1[g] = inputData[i];
                        g++;
                    }
                    List<TextBox> textBoxes = new List<TextBox>();
                    foreach (UIElement item in MatrixStackPanel.Children)
                    {
                        if (item is StackPanel)
                        {
                            foreach (UIElement item1 in (item as StackPanel).Children)
                            {
                                if (item1.GetType() == typeof(TextBox))
                                    textBoxes.Add(item1 as TextBox);
                            }
                        }
                    }
                    g = 0;
                    string[,] dataMatrix = new string[rows,columns];
                    for (int i = 0; i < inputData1.Length; i++)
                    {
                        string [] data = inputData1[i].Split(' ');
                        for (int j = 0; j < data.Length; j++)
                        {
                            dataMatrix[i, j] = data[j];
                            textBoxes[g].Text = dataMatrix[i,j].ToString();
                            g++;
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
            
        }

        private void NewLoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            ReadExcel();
        }

        private void RaandomMatrixButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int a = Convert.ToInt32(HeightTextBox.Text);
                int b = Convert.ToInt32(WidthTextBox.Text);
                GenerateMassive(a,b);
                List<TextBox> textBoxes = new List<TextBox>();
                foreach (UIElement item in MatrixStackPanel.Children)
                {
                    if (item is StackPanel)
                    {
                        foreach (UIElement item1 in (item as StackPanel).Children)
                        {
                            if (item1.GetType() == typeof(TextBox))
                                textBoxes.Add(item1 as TextBox);
                        }
                    }
                }
                int g = 0;
                for (int i = 0; i < matrix.GetUpperBound(0)+1; i++)
                {
                    for (int j = 0; j < matrix.GetUpperBound(1)+1; j++)
                    {
                        Random rnd = new Random();
                        matrix[i, j] = rnd.NextDouble();
                        textBoxes[g].Text = matrix[i,j].ToString();
                        g++;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка ввода данных! Убедитесь в правильности введённых данных!", "Ошибка!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }  
        }

        private void PrintButton_OnClick(object sender, RoutedEventArgs e)
        {
                try
                {
                    FullResTextBlock.Text = "";
                    Random rnd = new Random();
                string path = @"..\..\..\Documents\"+ DateTime.Now.ToString("dd.MM.yyyy")+$"Решение{rnd.Next(0,9999)}"+".docx";
                DocX document = DocX.Create(path);
                
                Paragraph paragraph = document.InsertParagraph();
                paragraph.AppendLine($"Изначальная матрица")
                    .Font("Times New Roman")
                    .FontSize(14)
                    .Alignment = Alignment.left;
                
                Table table = document.AddTable(matrix.GetUpperBound(0)+1,matrix.GetUpperBound(1)+1);

                for (int i = 0; i < matrix.GetUpperBound(0)+1; i++)
                {
                    for (int j = 0; j < matrix.GetUpperBound(1)+1; j++)
                    {
                        table.Rows[i]
                            .Cells[j]
                            .Paragraphs
                            .First()
                            .Append(matrix[i,j].ToString())
                            .Font("Times New Roman")
                            .FontSize(12)
                            .Bold();
                    }
                }
                
                table.Alignment = Alignment.center;
                document.InsertTable(table);
                
                Paragraph paragraph1 = document.InsertParagraph();
                paragraph1.AppendLine($"Поиск седловых точек")
                    .Font("Times New Roman")
                    .FontSize(14)
                    .Alignment = Alignment.left;
                
                
                Paragraph paragraph2 = document.InsertParagraph();
                paragraph2.AppendLine(ResultSedlTextBox.Text)
                    .Font("Times New Roman")
                    .FontSize(12)
                    .Alignment = Alignment.left;
                
                Paragraph paragraph3 = document.InsertParagraph();
                paragraph3.AppendLine($"Решение симплекс-методов в смешанных стратегиях для первого игрока")
                    .Font("Times New Roman")
                    .FontSize(14)
                    .Alignment = Alignment.left;
                
                Simplex(matrix,1,document);
                
                Paragraph paragraph4 = document.InsertParagraph();
                paragraph4.AppendLine($"Решение симплекс-методов в смешанных стратегиях для второго игрока")
                    .Font("Times New Roman")
                    .FontSize(14)
                    .Alignment = Alignment.left;

                
                double[,] newmatr = new double[matrix.GetUpperBound(1)+1, matrix.GetUpperBound(0)+1];
                for (int i = 0; i < newmatr.GetUpperBound(0)+1; i++)
                {
                    for (int j = 0; j < newmatr.GetUpperBound(1)+1; j++)
                    {
                        newmatr[i, j] = matrix[j, i];
                    }
                }
                Simplex(newmatr,2,document);
                
                
                document.Save();
                MessageBox.Show($"Документ успешно сформирован! Путь до файла - {path}");
                }
                catch
                {
                    MessageBox.Show("Ошибка формирования документа");
                }
            
        }

        private void ModelGameTheoryButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ModelGameTheory(v1,v2,ver1,ver2,newmarr));
        }
    }
}