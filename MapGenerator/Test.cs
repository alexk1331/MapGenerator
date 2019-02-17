using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace MapGenerator
{
    public class Test
    {
        Grid main = new Grid();
        StackPanel stp = new StackPanel();
        TextBlock size = new TextBlock();
        TextBlock floorc = new TextBlock();
        TextBlock floorn = new TextBlock();
        TextBlock floorreal = new TextBlock();

        public Test(Grid g)
        {
            main = g;
            stp.Children.Add(size);
            stp.Children.Add(floorc);
            stp.Children.Add(floorn);
            stp.Children.Add(floorreal);
            main.Children.Add(stp);
        }

        public void updt(int h, int w, int fc, int fn, int fr)
        {
            size.Text = "height: "+h+ "width: " + w;
            floorc.Text = "curcount: " + fc;
            floorn.Text = "needcount: " + fn;
            floorreal.Text = "realcount: " + fr;
        }
    }
}
