using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapGenerator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WindowMapViewModel temp;
        public MainWindow()
        {
            InitializeComponent();
            temp = new WindowMapViewModel(MapGrid, test);
            temp.paintm();
        }
        private void testt_Click(object sender, RoutedEventArgs e)
        {
            DateTime start = DateTime.Now;
            int d = 0;
            if (testtb.Text!="")
            {
                d = Int32.Parse(testtb.Text);
            }
            temp.currentmap.generatemap(d);
            DateTime end = DateTime.Now;
            temp.paintm();
            MessageBox.Show(start+" - "+end+" = "+(end-start));
            
        }

        private void Moving_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            //temp.movecommand(b.Name);
        }

        private void MapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            temp.paintm();
        }

        private void testtb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
    }
}
