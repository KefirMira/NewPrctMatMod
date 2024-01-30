using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PrctMatMod.Pages;

namespace NewPrctMatMod;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Navigate(new GameTheoryPage());
    }

    private void MainFrame_OnContentRendered(object? sender, EventArgs e)
    {
        if (MainFrame.CanGoBack)
        {
            BackButton.Visibility = Visibility.Visible;
        }
        else
        {
            BackButton.Visibility = Visibility.Collapsed;
        }    }

    private void BackButton_OnClick(object sender, RoutedEventArgs e)
    {
        MainFrame.GoBack();
    }
}