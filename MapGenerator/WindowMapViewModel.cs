using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MapGenerator
{
    class WindowMapViewModel
    {
        public Map currentmap { get; set; }
        public Grid mg;
        public Canvas canvas;

        public WindowMapViewModel(Grid ng, Grid t)
        {
            currentmap = new Map(0);
            canvas = new Canvas();
            currentmap.tst = new Test(t);
            mg = ng;
            currentmap.canvpaint(canvas);
            mg.Children.Add(canvas);
            paintm();
        }

        public void paintm()
        {
            canvas.HorizontalAlignment = HorizontalAlignment.Left;
            canvas.VerticalAlignment= VerticalAlignment.Top;
            canvas.ClipToBounds = true;
            canvas.SnapsToDevicePixels = true;
            canvas.MaxHeight = mg.ActualHeight;
            canvas.MaxWidth = mg.ActualWidth;
            double mult = (double)currentmap.height / (double)currentmap.width;
            if(currentmap.height>currentmap.width)
            {
                canvas.Width = mg.ActualWidth / mult;
                canvas.Height = mg.ActualHeight;
            }
            else if(currentmap.height < currentmap.width)
            {
                canvas.Height = mg.ActualHeight * mult;
                canvas.Width = mg.ActualWidth;
            }
            else
            {
                canvas.Width = mg.ActualWidth;
                canvas.Height = mg.ActualHeight;
            }
            canvas = currentmap.canvpaint(canvas);
        }

        public void movecommand(string contrl)
        {
            switch (contrl)
            {
                case "Left":
                    if (Account.maptile.y - 1 < 0 || Account.maptile.y - 1 > currentmap.tiles.Length)
                    {
                        break;
                    }
                    if (movecheck(currentmap.tiles[Account.maptile.x, Account.maptile.y - 1]) == 1)
                    {
                        Account.maptile.y -= 1;
                        //paintmap();
                    }

                    break;
                case "Right":
                    if (Account.maptile.y + 1 < 0 || Account.maptile.y + 1 >= currentmap.height)
                    {
                        break;
                    }
                    if (movecheck(currentmap.tiles[Account.maptile.x, Account.maptile.y + 1]) == 1)
                    {
                        Account.maptile.y += 1;
                        //paintmap();
                    }
                    break;
                case "Forward":
                    if (Account.maptile.x - 1 < 0 || Account.maptile.x - 1 > currentmap.tiles.Length)
                    {
                        break;
                    }
                    if (movecheck(currentmap.tiles[Account.maptile.x - 1, Account.maptile.y]) == 1)
                    {
                        Account.maptile.x -= 1;
//paintmap();
                    }
                    break;
                case "Backward":
                    if (Account.maptile.x + 1 < 0 || Account.maptile.x + 1 >= currentmap.width)
                    {
                        break;
                    }
                    if (movecheck(currentmap.tiles[Account.maptile.x + 1, Account.maptile.y]) == 1)
                    {
                        Account.maptile.x += 1;
                        //paintmap();
                    }
                    break;
            }
        }
   

        public int movecheck(MapTile mt)
        {
            if (mt != null&&mt.type!=0)
            {
                return 1;
            }
            else { return 0; }
        }



    }
}
