using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MapGenerator
{
    class MapCreator
    {
        static int floornum=0;
        static int floorcur = 0;
        static MapTile[,] mtiles = new MapTile[1,1];
        static int h;
        static int w;
        public static MapTile[,] createmap(MapTile[,] smap, int he, int wi)
        {
            h = he;
            w = wi;
            mtiles = smap;
            Random r = new Random();
            floornum = r.Next((int)((smap.Length*0.6)-(smap.Length*0.1)), (int)((smap.Length * 0.6) + (smap.Length * 0.1))) ;
            int[] dir = new int[4] { 0, 0, 0, 0 };
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if(mtiles[x, y].type==1)
                    {
                        tmove(x, y);
                        break;
                    }
                }
            }


            return mtiles;
        }

        private static void tmove(int i, int j)
        {
            Random r = new Random();
            bool e = false;
            int c = 0;
            while (!e&&c<8)
            {
                c++;
                if(floorcur==floornum||c==8)
                {
                    return;
                }
                e = true;
                foreach(int t in mtiles[i, j].neighbours)
                {
                    if(t==0)
                    {
                        e = false;
                    }
                }
                int ra = r.Next(0, 4);
                switch (ra)
                {
                    case 0:
                        if (mtiles[i, j].neighbours[0] == 0 && checkcell(i, j + 1))
                        {
                            mtiles[i, j + 1].type = 1;
                            Map.celltchange(i, j + 1, mtiles);
                            floorcur++;
                            tmove(i, j + 1);
                            mtiles[i, j].neighbours[0] = 1;
                        }
                        break;
                    case 1:
                        if (mtiles[i, j].neighbours[1] == 0 && checkcell(i + 1, j))
                        {
                            mtiles[i + 1, j].type = 1;
                            Map.celltchange(i+1, j, mtiles);
                            floorcur++;
                            tmove(i + 1, j);
                            mtiles[i, j].neighbours[1] = 1;
                        }
                        break;
                    case 2:
                        if (mtiles[i, j].neighbours[2] == 0 && checkcell(i, j - 1))
                        {
                            mtiles[i, j - 1].type = 1;
                            Map.celltchange(i, j - 1, mtiles);
                            floorcur++;
                            tmove(i, j - 1);
                            mtiles[i, j].neighbours[2] = 1;
                        }
                        break;
                    case 3:
                        if (mtiles[i, j].neighbours[3] == 0 && checkcell(i - 1, j))
                        {
                            mtiles[i - 1, j].type = 1;
                            Map.celltchange(i-1, j, mtiles);
                            floorcur++;
                            tmove(i - 1, j);
                            mtiles[i, j].neighbours[3] = 1;
                        }
                        break;
                }
            }
        }

        private static bool checkcell(int i, int j)
        {
            bool t = false;
            if (mtiles[i, j].type == 0 && i < w && i > 0 && j < h && j > 0)
            {
                t = true;
                int ver = 0;
                foreach (int tt in mtiles[i, j].neighbours)
                {
                    ver = ver + tt;
                }
                if (ver == 4)
                {
                    t = false;
                }
                if (ver == 3 && (i == 1 || j == 1 || j == h - 1||i==w-1))
                {
                    t = false;
                }
                if (ver == 4 && ((i == 1 && j == 1) || (i == 1 && j == h-1) || (i == w-1 && j == 1) || (i == w-1 && j == h-1)))
                {
                    t = false;
                }
            }

            return t;
        }
    }
}
