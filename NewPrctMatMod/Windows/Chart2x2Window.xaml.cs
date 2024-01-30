using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;

namespace PrctMatMod.Windows
{
    public partial class Chart2x2Window : Window
    {
        private static double[,] _matrix;
        public SeriesCollection SeriesCollection { get; set; }
        public  List <string> Labels { get; set; }
        private double[] temp;
        public Chart2x2Window(double[,] matrix)
        {
            InitializeComponent();
            _matrix = matrix;
            temp  = new double[] {_matrix[0,0],_matrix[0,1],_matrix[1,0],_matrix[1,1] };
        }
        

        private void Chart2x2Window_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Создание линейного графика
            LineSeries mylineseries = new LineSeries();
            // Установить заголовок полилинии
            mylineseries.Title = "График 1";
            // Линейная форма линейного графика
            mylineseries.LineSmoothness = 0;
            // Бессмысленный стиль линейного графика
            mylineseries.PointGeometry = null;
            
            LineSeries mylineseries1 = new LineSeries();
            // Установить заголовок полилинии
            mylineseries1.Title = "График 2";
            // Линейная форма линейного графика
            mylineseries1.LineSmoothness = 0;
            // Бессмысленный стиль линейного графика
            mylineseries1.PointGeometry = null;
            // Добавить абсциссу
            
            //Labels =new List<string>{ _matrix[0,0].ToString(),_matrix[0,1].ToString(), _matrix[1,0].ToString(),_matrix[1,1].ToString()};
            // Добавить данные линейного графика
            double[] one = new[] { temp[0], temp[2] };
            double[] two = new[] { temp[1], temp[3] };
            mylineseries.Values = new ChartValues<double>(one);
            mylineseries1.Values = new ChartValues<double>(two);
            SeriesCollection = new SeriesCollection { };
            SeriesCollection.Add(mylineseries);
            SeriesCollection.Add(mylineseries1);
            
            // var tooltip = new LiveCharts.Wpf.DefaultTooltip {
            //     SelectionMode = TooltipSelectionMode.SharedYValues
            // };
            //
            // Chart.DataTooltip = tooltip;
            
            
            DataContext = this;
        }
        
    }
}