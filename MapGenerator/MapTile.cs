using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Threading.Tasks;

namespace MapGenerator
{
    class MapTile
    {
        public int type {get; set; }//0-wall.1-path
        public MapTile[] neighbours;
        public int qual;
        public Brush backgrnd;//for test purposes
        public bool prefab;


        public MapTile()
            {
            prefab = false;
            type = 0;
            qual = -1;
            backgrnd = Brushes.Black;
            }

        public int countneigb()
        {
            int c = 0;
            foreach(MapTile m in neighbours)
            {
                if(m.type==1)
                {
                    c++;
                }
            }
            return c;
        }
    }
}
