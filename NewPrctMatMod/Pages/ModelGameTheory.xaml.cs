using System.Windows;
using System.Windows.Controls;
using NewPrctMatMod.Models;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace NewPrctMatMod.Pages;

public partial class ModelGameTheory : Page
{
    private double[,] matrix;
    private List<ForModelGameTheory> _forModel;
    private int rows = 0;
    private int columns = 0;
    private  double v1 = 0;
    private  double v2 = 0;
    private double[] ver1 ;
    private double[] ver2 ;
    public ModelGameTheory()
    {
        InitializeComponent();
    }
    public ModelGameTheory( double v1_, double v2_, double[] ver1_, double[] ver2_ ,double[,] matrix1)
    {
        InitializeComponent();
        try
        {
            v1 = v1_;
            v2 = v2_;
            int lenghtVer = 0;
            for (int i = 0; i < ver1_.Length; i++)
            {
                if (ver1_[i] != 0)
                    lenghtVer++;
            }
            int lenghtVer1 = 0;
            for (int i = 0; i < ver2_.Length; i++)
            {
                if (ver2_[i] != 0)
                    lenghtVer1++;
            }
        
            ver1 = new double[lenghtVer];
            ver2 = new double[lenghtVer1];
        
            lenghtVer = 0;
            lenghtVer1 = 0;
        
            for(int i = 0; i < ver1_.Length; i++)
            {
                if (ver1_[i] != 0)
                {
                    ver1[lenghtVer] = ver1_[i];
                    lenghtVer++;
                }
            }
        
            for (int i = 0; i < ver2_.Length; i++)
            {
                if (ver2_[i] != 0)
                {
                    ver2[lenghtVer1] = ver2_[i];
                    lenghtVer1++;
                }
            }
        
            matrix = matrix1;

        }
        catch
        {
            MessageBox.Show("Ошибка считывания данных");
        }
        
    }

    public double Select(double SlNumA, double SlNumB)
    {
        int k = 0;
        int l = 0;

        try
        {
            for (int i = 0; i < ver1.Length; i++)
            {
                if (SlNumA <= ver1[i])
                    k = i;
            }
            for (int i = 0; i < ver2.Length; i++)
            {
                if (SlNumB <= ver2[i])
                    l = i;
            }
        }
        catch
        {
            MessageBox.Show("Ошибка");
        }
       
        return matrix[k, l];
    }
    public double SelectA(double SlNumA)
    {
        int k = 0;
        try
        {
            for (int i = 0; i < ver1.Length; i++)
            {
                if (SlNumA <= ver1[i])
                    k = i;
            }    
        }
        catch
        {
            MessageBox.Show("Ошибка");
        }
        
        return k+1;
    }
    public double SelectB( double SlNumB)
    {
        int l = 0;
        try
        {
            for (int i = 0; i < ver2.Length; i++)
            {
                if (SlNumB <= ver2[i])
                    l = i;
            }
        }
        catch
        {
            MessageBox.Show("Ошибка");
        }
        
        return  l+1;
    }
    
    private void GenerateMassive(int a, int b)
    {
        try
        {
            _forModel = new List<ForModelGameTheory>();
            columns = b;
            rows = a;
            Random rnd = new Random();
            int sum = 0;
            if (a > 0 && b > 0)
            {
                int winA = 0;
                int winB = 0;
                for (int i = 0; i < rows; i++)
                {
                    ForModelGameTheory forModelGameTheory = new ForModelGameTheory();
                    forModelGameTheory.Num = i + 1;
                    forModelGameTheory.SlNumA = rnd.NextDouble();
                    forModelGameTheory.StA = $"A" + SelectA(forModelGameTheory.SlNumA);
                    forModelGameTheory.SlNumB = rnd.NextDouble();
                    forModelGameTheory.StB = "B" + SelectB(forModelGameTheory.SlNumB);
                    forModelGameTheory.Win = Convert.ToInt32(Select(forModelGameTheory.SlNumA,forModelGameTheory.SlNumB));
                    sum += forModelGameTheory.Win;
                    forModelGameTheory.NakoplWin = sum;
                    forModelGameTheory.SrWin = forModelGameTheory.NakoplWin*1.0 / forModelGameTheory.Num;
                    _forModel.Add(forModelGameTheory);
                }

                RexTextBlock.Text = $"Соотношение выбора стратегий игрока А:\n Первая стратегия выбрана {_forModel.Where(c=>c.StA=="A1").Count()} раз(а)\n Вторая стратегия выбрана  {_forModel.Where(c=>c.StA=="A2").Count()}" +
                                    $"\n Соотношение выбора стратегий игрока B:\n Первая стратегия выбрана {_forModel.Where(c=>c.StB=="B1").Count()} раз(а)\n Вторая стратегия выбрана  {_forModel.Where(c=>c.StB=="B2").Count()}";
            }
            else
            {
                MessageBox.Show("Ошибка ввода данных! Убедитесь в правильности введённых данных!", "Ошибка!",
                    MessageBoxButton.OK, MessageBoxImage.Error);    
            }
        }
        catch
        {
            MessageBox.Show("Ошибка генерации массива");
        }
        
    }


    private void EnterButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            rows = Convert.ToInt32(RazmTextBox.Text);
            columns = 8;
            GenerateMassive(rows,columns);
            RezDataGrid.ItemsSource = _forModel;
        }
        catch
        {
            MessageBox.Show("Ошибка");
        }
        
    }

    public void Print()
    {
        try
        {
            
        Random rnd = new Random();
            string path = @"..\..\..\Documents\"+ DateTime.Now.ToString("dd.MM.yyyy")+$"РешениеСПомощьюТаблицы{rnd.Next(0,9999)}"+".docx";
            DocX document = DocX.Create(path);
            
            Paragraph paragraph = document.InsertParagraph();
            paragraph.AppendLine($"Количество сыгранных партий = {rows}\n" +
                                 $"Будем выбирать стратегии игроков, используя геометрическое определение вероятности. Так как все случайные числа из отрезка [0; 1], то чтобы стратегия А1 появлялась примерно в половине случаев, будем ее выбирать если случайное число меньше {ver1[0]}; в остальных случаях выбирается стратегия А2. Аналогично для игрока В. Стратегию В1 будем выбирать, если соответствующее случайное число меньше {ver2[0]}, в противном случае выбираем стратегию В1.\nЗаполним расчетную таблицу:")
                .Font("Times New Roman")
                .FontSize(14)
                .Alignment = Alignment.left;
            
            Table table = document.AddTable(_forModel.Count+1,columns);

            table.Rows[0]
                    .Cells[0]
                    .Paragraphs
                    .First()
                    .Append("Номер партии")
                    .Font("Times New Roman")
                    .FontSize(12)
                    .Bold();   
                table.Rows[0]
                    .Cells[1]
                    .Paragraphs
                    .First()
                    .Append("Случайное число А")
                    .Font("Times New Roman")
                    .FontSize(12)
                    .Bold();   
                table.Rows[0]
                    .Cells[2]
                    .Paragraphs
                    .First()
                    .Append("Стратегия А")
                    .Font("Times New Roman")
                    .FontSize(12)
                    .Bold();   
                table.Rows[0]
                    .Cells[3]
                    .Paragraphs
                    .First()
                    .Append("Случайное число B")
                    .Font("Times New Roman")
                    .FontSize(12)
                    .Bold();
                table.Rows[0]
                    .Cells[4]
                    .Paragraphs
                    .First()
                    .Append("Стратегия B")
                    .Font("Times New Roman")
                    .FontSize(12)
                    .Bold();   
                table.Rows[0]
                    .Cells[5]
                    .Paragraphs
                    .First()
                    .Append("Выигрыш")
                    .Font("Times New Roman")
                    .FontSize(12)
                    .Bold();   
                table.Rows[0]
                    .Cells[6]
                    .Paragraphs
                    .First()
                    .Append("Накопленный выигрыш")
                    .Font("Times New Roman")
                    .FontSize(12)
                    .Bold();   
                table.Rows[0]
                    .Cells[7]
                    .Paragraphs
                    .First()
                    .Append("Средний выигрыш")
                    .Font("Times New Roman")
                    .FontSize(12)
                    .Bold();
            
            
            for (int i = 0; i < _forModel.Count; i++)
            {
                table.Rows[i+1]
                    .Cells[0]
                    .Paragraphs
                    .First()
                    .Append(_forModel[i].Num.ToString())
                    .Font("Times New Roman")
                    .FontSize(12)
                    .Bold();   
                table.Rows[i+1]
                    .Cells[1]
                    .Paragraphs
                    .First()
                    .Append(_forModel[i].StA.ToString())
                    .Font("Times New Roman")
                    .FontSize(12)
                    .Bold();   
                table.Rows[i+1]
                    .Cells[2]
                    .Paragraphs
                    .First()
                    .Append(_forModel[i].SlNumA.ToString())
                    .Font("Times New Roman")
                    .FontSize(12)
                    .Bold();   
                table.Rows[i+1]
                    .Cells[3]
                    .Paragraphs
                    .First()
                    .Append(_forModel[i].StB.ToString())
                    .Font("Times New Roman")
                    .FontSize(12)
                    .Bold();
                table.Rows[i+1]
                    .Cells[4]
                    .Paragraphs
                    .First()
                    .Append(_forModel[i].SlNumB.ToString())
                    .Font("Times New Roman")
                    .FontSize(12)
                    .Bold();   
                table.Rows[i+1]
                    .Cells[5]
                    .Paragraphs
                    .First()
                    .Append(_forModel[i].Win.ToString())
                    .Font("Times New Roman")
                    .FontSize(12)
                    .Bold();   
                table.Rows[i+1]
                    .Cells[6]
                    .Paragraphs
                    .First()
                    .Append(_forModel[i].NakoplWin.ToString())
                    .Font("Times New Roman")
                    .FontSize(12)
                    .Bold();   
                table.Rows[i+1]
                    .Cells[7]
                    .Paragraphs
                    .First()
                    .Append(_forModel[i].SrWin.ToString())
                    .Font("Times New Roman")
                    .FontSize(12)
                    .Bold();   
            }
            
            table.Alignment = Alignment.center;
            document.InsertTable(table);
            
            Paragraph paragraph1 = document.InsertParagraph();
            paragraph1.AppendLine($"Результаты")
                .Font("Times New Roman")
                .FontSize(14)
                .Alignment = Alignment.left;
            
            
            Paragraph paragraph2 = document.InsertParagraph();
            paragraph2.AppendLine($"Таким образом, в результате моделирования в {rows} партиях цена игры (средний выигрыш) равен {_forModel.Last().SrWin}. Этот результат согласуется с теоретической ценой игры {v1}.\nЧастоты использования игроками своих чистых стратегий соответственно равны:\n\nХ({_forModel.Where(c=>c.StA=="A1").Count()}/{rows};{_forModel.Where(c=>c.StA=="A2").Count()}/{rows}), Y({_forModel.Where(c=>c.StB=="B1").Count()}/{rows}; {_forModel.Where(c=>c.StB=="B2").Count()}/{rows}) или\n\nХ({_forModel.Where(c=>c.StA=="A1").Count()*1.0/rows}; {_forModel.Where(c=>c.StA=="A2").Count()*1.0/rows}), Y({_forModel.Where(c=>c.StA=="B1").Count()/rows}; {_forModel.Where(c=>c.StA=="B2").Count()/rows})")
                .Font("Times New Roman")
                .FontSize(12)
                .Alignment = Alignment.left;
            
            
            document.Save();
            MessageBox.Show($"Документ успешно сформирован! Путь до файла - {path}");
        
        }
        catch
        {
            MessageBox.Show("Ошибка генерации документа");
        }
    }

    private void PrintButton_OnClick(object sender, RoutedEventArgs e)
    {
        Print();
    }
}