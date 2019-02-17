using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace MapGenerator
{
    class Map
    {
        public int deep { get; }//lvl depth. deeper=larger maps
        public int height;
        public int width;
        public MapTile[,] tiles;
        public int plst;//player position. 0-current ppos. 1-fromupper. 2-from down//this is needed only for ingame mapgen. can be deleted safely
        int floornum;
        int floorcur;
        int widtun;
        int cavesize;
        public Test tst;//view for test info
        struct Tunneler
        {
            public int lenght;
            public int wide;
            public MapTile prev;
            public MapTile cur;
            public MapTile left;
            public MapTile right;
            public int turns;
            public bool prevactsuc;
            public List<MapTile> allcells;
        }
        struct Cave
        {
            public MapTile entrnc;
            public MapTile preentrnc;
            public List<MapTile> allcells;
        }
        struct Filtvalcells
        {
            public List<MapTile> bestcells;
            public List<MapTile> normdcells;
            public List<MapTile> badcells;
        }
        Filtvalcells valcells;
        public Map(int d)//map constructor. ingame maps generates when its created. Here is needed just to make aomething in begining
        {
            valcells = new Filtvalcells();
            valcells.bestcells = new List<MapTile>();
            valcells.normdcells=new List< MapTile > ();
            valcells.badcells = new List<MapTile>();
            height = 6;
            width =6;
            floornum = 0;
            floorcur = 0;
            //map generation
            MapTile[,] t = new MapTile[6, 6];
            tiles = t;
            for (int i=0;i<6;i++)
            {
                for(int j=0;j<6;j++)
                {
                    tiles[i, j] = new MapTile();
                }
            }
            iniccells();
        }

        public void generatemap(int deep)//creating new map
        {
            valcells = new Filtvalcells();
            valcells.bestcells = new List<MapTile>();
            valcells.normdcells = new List<MapTile>();
            valcells.badcells = new List<MapTile>();
            floornum = 0;
            floorcur = 0;
            plst = 1;//setting plaer pos to the entrnc. no use in mapgen
            Random r = new Random();
            sethw(deep);
            cavesize = getmaxcavesize();
            widtun = getmaxtunwidth();
            tiles = new MapTile[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    tiles[i, j] = new MapTile();
                    tiles[i, j].neighbours = new MapTile[4];
                }
            }
            iniccells();
            int entdp = (height - 3) * (width - 3);
            for (int i = 1; i < height-2; i++)
            {
                for (int j = 1; j < width-2; j++)
                {
                    if(entdp>0)
                    {
                        int t = r.Next(0, entdp);
                        if(t == 0 || entdp ==1)
                        {
                            tiles[i, j].type = 1;
                            tiles[i, j].qual = 0;
                            floorcur++;
                            updtallcells();
                            valcells.bestcells.Add(tiles[i, j]);
                            tiles[i, j].backgrnd = Brushes.Red;
                            Account.maptile.x = i; Account.maptile.y = j;//randomly selecting entrance position from non border cells
                            entdp = 0;
                            createmap();//aand starting mapgen
                            i = height ;
                            j = width;
                        }
                        entdp = entdp - 1;
                    }
                }
            }
        }

        public void sethw(int deep)//auto set map size
        {
            int size = (50 + deep)*2;
            Random r = new Random();
            int s = r.Next((int)Math.Round((size - size * 0.1), MidpointRounding.AwayFromZero), (int)Math.Round((size + size * 0.1), MidpointRounding.AwayFromZero));
            height = r.Next((int)Math.Round((s*0.3), MidpointRounding.AwayFromZero), (int)Math.Round((s * 0.7), MidpointRounding.AwayFromZero));
            width = s - height;
        }

        private int getmaxcavesize()//max cave size depending on map size
        {
            double c = ((height + width) / 2)-50;
            double s = 5;
            if(c>0)
            {
                s = s + (c / 10);
            }
            if (s > 50) { return 50; }
            return (int)Math.Round(s, MidpointRounding.AwayFromZero); 
        }

        private int getmaxtunwidth()//max tunnel width depending on map size. max sizes for tun and caves set because higher values cause weird results
        {
            double count = 0;
            double d = 12.5;
            int c = height + width;
            while(c>0)
            {
                if(c>200)
                {
                    count = count + (200 / d);
                }
                else
                {
                    count = count + (c / d);
                }
                c = c - 200;
                d = d * 2;
            }
            MessageBox.Show("cs: "+cavesize+" height: "+height+" width: "+width+"__"+count+"==int- "+(int)Math.Round(count, MidpointRounding.AwayFromZero));
            if (count > 31) { count = 31; }
            return (int)Math.Round(count, MidpointRounding.AwayFromZero);
        }

        public Canvas canvpaint(Canvas canv)
        {
            int c = 0;
            canv.Background = Brushes.Black;
            foreach (MapTile mt in tiles)
            {
                if (mt.type == 1)
                {
                    c++;
                }
            }
            tst.updt(height, width, floorcur, floornum, c);
            canv.Children.Clear();
            double hpoint=canv.Height/height;
            double wpoint = canv.Width / width;
            for(int i=0; i<height;i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if(tiles[i,j].type==1)
                    {
                        Rectangle nb = new Rectangle();
                        nb.Stroke = Brushes.Blue;
                        nb.StrokeThickness = 0;
                        nb.Margin = new Thickness(1);
                        nb.SnapsToDevicePixels = true;
                        nb.Fill = Brushes.White;
                        //===

                        if (tiles[i, j].backgrnd != Brushes.Black)
                        {
                            nb.Fill = tiles[i, j].backgrnd;
                        }

                        nb.Height = hpoint;
                        nb.Width = wpoint;
                        Canvas.SetLeft(nb, wpoint + (wpoint * j));
                        Canvas.SetTop(nb, hpoint + (hpoint * i));
                        canv.Children.Add(nb);
                    }
                }
            }
            return canv;
        }

        public void createmap()
        {
            Random r = new Random();
            floornum = r.Next((int)((tiles.Length * 0.25) - (tiles.Length * 0.05)), (int)((tiles.Length * 0.25) + (tiles.Length * 0.05)));
            Thread myThread = new Thread(new ThreadStart(startbuild));
            myThread.Priority = ThreadPriority.Highest;
            myThread.Start();
            myThread.Join();
        }

        private void startbuild()//good starbuild
        {
            Random r = new Random();
            int c = r.Next(0, 100);
            if(c==0&&floorcur==1)//chance that entrance ll be in the cave. Like this it ll be ONLY in the middle of cave, but it can be easely moved to random tile
            {
                valcells.bestcells[0].type = 0;
                int size = r.Next(5, cavesize + 1);
                cavemade(valcells.bestcells[0], valcells.bestcells[0], size);
            }
            while (floorcur < floornum)
            {
                filtvalcell(valcells);
                if (valcells.bestcells.Count > 0)
                {
                    tunnelermade(valcells.bestcells);
                }
                else if (valcells.normdcells.Count > 0)
                {
                    tunnelermade(valcells.normdcells);
                }
                else
                {
                    tunnelermade(valcells.badcells);
                }
            }
        }

        private void cavemade(MapTile startt, MapTile prest, int sz)//cavegen//creating cave
        {
            if (!cellvalid(startt)) { return; }
            Cave cv = new Cave();
            cv.entrnc = startt;
            cv.preentrnc = prest;
            cv.allcells = grabcells(cv.entrnc, sz);
            cv.allcells = cavefiltc(cv.allcells);
            Random r = new Random();
            for(int i=0;i<cv.allcells.Count;i++)
            {
                int k = r.Next(0, 2);
                cv.allcells[i].type = k;
            }
            for (int i = 0; i < 3; i++)
            {
                cv.allcells = cgen(cv.allcells);
            }
            for(int i=0; i<cv.allcells.Count;i++)
            {
                if(cv.allcells[i].countneigb()==4)
                {
                    cv.allcells[i].type = 1;
                }
            }
            cv.allcells = csetpath(cv);
            cv.preentrnc.type = 1;
            cv.entrnc.type = 1;
            floorcur = floorcur + 1;
        }

        private List<MapTile> csetpath(Cave c)//cavegen//deleting all tiles that r not directly connected with the entrpoint of cave
        {
            List<MapTile> lst = new List<MapTile>();
            lst = cavepath(c.entrnc, new List<MapTile>() {c.preentrnc}, c.allcells);
            for(int i=0;i<c.allcells.Count;i++)
            {
                if(!lst.Exists(m => m == c.allcells[i]))
                {
                    c.allcells[i].type = 0;
                }
            }
            floorcur = floorcur + lst.Count;
            updtvalidcells(createfvc(lst), valcells);
            return lst;
        }

        private List<MapTile> cgen(List<MapTile> src)//cavegen//generating cave by cellular automatha method
        {
            List<bool> bl = new List<bool>();
            Random r = new Random();
            for(int i=0;i<src.Count;i++)
            {
                bool b = false;
                int cn = src[i].countneigb();
                if (cn < 1)
                {
                    b = false;
                }
                else if(cn == 1|| cn == 2)
                {
                    int c = r.Next(0, cn + 1);
                    if(c==0)
                    {
                        b = true;
                    }
                }
                else if(cn >2)
                {
                    b = true;
                }
                else
                {
                        b = true;
                }
                bl.Add(b);
            }
            for (int i = 0; i < src.Count; i++)
            {
                if(bl[i])
                {
                    src[i].type = 1;
                }
                else
                {
                    src[i].type = 0;
                }
            }
            return src;
        }

        private List<MapTile> cavepath(MapTile mt, List<MapTile> list, List<MapTile> all)//creating list of connected cells for csetpath
        {
            list.Add(mt);
            for (int i = 0; i < 4; i++)
            {
                if(mt.neighbours[i].type==1&&!list.Exists(m => m == mt.neighbours[i])&& all.Exists(m => m == mt.neighbours[i]))
                {
                    list = cavepath(mt.neighbours[i], list, all);
                }
            }
            if(list.Count<3)
            {
                for(int i=0; i<4;i++)
                {
                    if(mt.neighbours[i].countneigb()==1&& !list.Exists(m => m == mt.neighbours[i]) && all.Exists(m => m == mt.neighbours[i]))
                    {
                        mt.neighbours[i].type = 1;
                        list = cavepath(mt.neighbours[i], list, all);
                    }
                }
            }
            return list;
        }

        private List<MapTile> cavefiltc(List<MapTile> src)//cavegen//removes surely unusable tiles from cave zone
        {
            List<MapTile> lst = new List<MapTile>();
            for(int i=0;i<src.Count;i++)
            {
                if(cellvalid(src[i])&&src[i].countneigb()==0)
                {
                    lst.Add(src[i]);
                }
            }
            return lst;
        }

        private List<MapTile> grabcells(MapTile strt, int range)//cavegen//creates a list of ALL possible cell for cave according to size(ONLY SIZE)
        {
            List<MapTile> lst = new List<MapTile>();
            MapTile temp = strt;
            lst.Add(strt);
            lst.AddRange(grabdir(strt, range, 0));
            lst.AddRange(grabdir(strt, range, 2));
            for (int i = 0; i < range; i++)
            {
                if (temp.neighbours[1].type != -1)
                {
                    temp = temp.neighbours[1];
                    lst.Add(temp);
                    lst.AddRange(grabdir(temp, range, 2));
                    lst.AddRange(grabdir(temp, range, 0));
                }
            }
            temp = strt;
            for (int i = 0; i < range; i++)
            {
                if (temp.neighbours[3].type != -1)
                {
                    temp = temp.neighbours[3];
                    lst.Add(temp);
                    lst.AddRange(grabdir(temp, range, 2));
                    lst.AddRange(grabdir(temp, range, 0));
                }
            }
            return lst;
        }

        private List<MapTile> grabdir(MapTile strt, int range, int dir)//cavegen//part of grabcells
        {
            List<MapTile> lst = new List<MapTile>();
            for (int lt = 0; lt < range; lt++)
            {
                if (strt.neighbours[dir].type != -1)
                {
                    strt = strt.neighbours[dir];
                    lst.Add(strt);
                }
                else { break; }
            }
            return lst;
        }

        private void setcellqual(List<MapTile> tls)
        {
            for(int i=0;i<tls.Count;i++)
            {
                if(tls[i].type==1&& tls[i].qual!=2)
                {
                    tls[i].qual = denscheck(tls[i]);
                }
            }
        }

        private void tunnelermade(List<MapTile> tiles)//creating tunnels
        {
            Random r = new Random();
            int chance = r.Next(0, tiles.Count);
            int d = choosetdir(tiles[chance], 1);
            if (d < 0) { return; }
            Tunneler t = new Tunneler();
            t.lenght = r.Next(2, (int)(((height+width)/2) + 3));
            t.turns = t.lenght / 10;
            if (t.turns > 20) { t.turns = 20; }
            t.cur = tiles[chance].neighbours[d];
            t.left = t.cur;
            t.right = t.cur;
            t.allcells = new List<MapTile>();
            t.wide = gettwidth();
            tunnelerlife(t); 
        }

        private int gettwidth()//randomly selects tunnel width according to current parametrs
        {
            Random r = new Random();
            int w = r.Next(1, widtun+1);
            if(w<=16)
            {
                return 1;
            }
            else if(w>16&&w<=24)
            {
                return 2;
            }
            else if (w > 24 && w <= 28)
            {
                return 3;
            }
            else if (w > 28 && w <= 30)
            {
                return 4;
            }
            else
            {
                return 5;
            }
        }

        public int denscheck(MapTile mt)//checks cell quality for creating offsprings by checking how much free space is available from this point
        {
            if(mt.countneigb()==4)
            {
                return 2;
            }
            int dens = 0;
            foreach(MapTile m in mt.neighbours)
            {
                if(m.type==0)
                {
                    int t = dcv2(m, 0, new List<MapTile>());
                    if(t>250)
                    {
                        dens = dens + 4;
                    }
                    else if(t>100&&t<=250)
                    {
                        dens = dens + 1;
                    }
                }
            }
            if(dens>=4)
            {
                return 0;
            }
            else if(dens<=3&&dens>1)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        private int dcv2(MapTile mt, int c, List<MapTile> list)//part of denscheck
        {
            int count = c;
            list.Add(mt);
            if(count>250|| mt.countneigb() >= 2)
            {
                return count;
            }
            if(mt.countneigb()==1)
            {
                for(int i=0;i<4;i++)
                {
                    if(mt.neighbours[i].type==1)
                    {
                        if(i==0&&mt.neighbours[2].neighbours[2].type==1)
                        {
                            return count;
                        }
                        else if(i == 2 && mt.neighbours[0].neighbours[0].type == 1)
                        {
                            return count;
                        }
                        else if (i == 1 && mt.neighbours[3].neighbours[3].type == 1)
                        {
                            return count;
                        }
                        else if (i == 3 && mt.neighbours[1].neighbours[1].type == 1)
                        {
                            return count;
                        }
                        break;
                    }
                }
            }
            foreach(MapTile til in mt.neighbours)
            {
                if(cellvalid(til)&&!list.Exists(m=>m==til))
                {
                    count++;
                    count = dcv2(til, count, list);
                }
            }
            return count;
        }

        private void tunnelerlife(Tunneler t)//tunnel behavior logic
        {
            while (t.lenght > 0&&floorcur<floornum)
            {
                //updtallcells();//this and others commented updtallcells may(or may not) have influence on quality of tunnels built. more updtallcels=less performance
                if (t.prev == null)
                {
                    int d = choosetdir(t.cur, 0);
                    if(d<0)
                    {
                        t.lenght = 0;
                    }
                    else
                    {
                        t.prev = t.cur;
                        t.prev.type = 1;
                        floorcur = floorcur + 1;
                        t.lenght--;
                        t.cur = t.prev.neighbours[d];
                        t.cur.type = 1;
                        floorcur=floorcur+1;
                        t.lenght--;
                        t.allcells.Add(t.prev);
                        t.allcells.Add(t.cur);
                        if (t.wide > 1) { widetun(t, 1); }
                    }
                }
                else
                {
                    bool straight = true;
                    bool turning = true;
                    t.prevactsuc = false;
                    while(straight||turning)
                    {
                        Random r = new Random();
                        int act = r.Next(0, 100);
                        int d = tnextstep(t);
                        if ((act > t.turns||turning==false)&&straight==true)
                        {
                            straight = false;
                            if(cellvalid(t.cur.neighbours[d]))
                            {
                                t = acttunneler(t, 0);
                            }
                            else
                            {
                                t = acttunneler(t, 2);
                            }
                        }
                        else if ((act <= t.turns||straight==false)&&turning==true)
                        {
                            turning = false;
                            int turn = choosetdir(t.cur, 0);
                            if (turn < 0)
                            {
                                t = acttunneler(t, 3);
                            }
                            else
                             {
                                t = acttunneler(t, 1);
                             }
                        }
                        if (t.prevactsuc) { break; }
                    }
                    if (t.prevactsuc == false) { t.lenght = 0; }
                    else { t.allcells.Add(t.cur); }
                }
                if(t.wide>1&&t.prev!=null)
                {
                    t = widetun(t, 0);
                }
                //updtallcells();//_______________________________________________________________________________________________________________
            }
            
            if (floorcur < floornum)
            {
                Filtvalcells fvc = createfvc(t.allcells);
                tmakechild(t, fvc);
                fvc = filtvalcell(fvc);
                updtvalidcells(fvc, valcells);
            }
            else { Thread.CurrentThread.Abort(); }
        }

        private void cmakechild(Filtvalcells fvc)//creating child cave
        {
            if(fvc.bestcells.Count>0)
            {
                Random rnd = new Random();
                int t = rnd.Next(0, fvc.bestcells.Count);
                int size = rnd.Next(5, cavesize + 1);
                int dir = choosetdir(fvc.bestcells[t], 1);
                MapTile b = fvc.bestcells[t].neighbours[dir];
                MapTile a = fvc.bestcells[t].neighbours[dir].neighbours[dir];
                cavemade(a, b, size);
            }
        }

        private void tmakechild(Tunneler t, Filtvalcells fvc)//creating tunnel offsprings
        {
            int childr = (t.allcells.Count / t.wide)/20;
            Random r = new Random();
            int ren = r.Next(0, 10);
            if(ren==0&&childr>1)
            {
                cmakechild(fvc);
                childr--;
                fvc = filtvalcell(fvc);
            }
            for(int i=0;i<childr;i++)
            {
                if (floorcur < floornum)
                {
                    if (fvc.bestcells.Count > 0)
                    {
                        tunnelermade(fvc.bestcells);
                    }
                    else { return; }
                    fvc = filtvalcell(fvc);
                }
                else
                {
                    Thread.CurrentThread.Abort();
                } 
            }
        }

        private Filtvalcells createfvc(List<MapTile> src)//creating filtred by quality lists of cells
        {
            Filtvalcells fvc = new Filtvalcells();
            fvc.bestcells = new List<MapTile>();
            fvc.normdcells = new List<MapTile>();
            fvc.badcells = new List<MapTile>();
            src = filtrtiles(src);
            setcellqual(src);
            for (int i = 0; i < src.Count; i++)
            {
                if (src[i].qual == 0)
                {
                    fvc.bestcells.Add(src[i]);
                }
                else if (src[i].qual == 1)
                {
                    fvc.normdcells.Add(src[i]);
                }
                else if (src[i].qual == 2)
                {
                    fvc.badcells.Add(src[i]);
                }
            }
            return fvc;
        }

        private List<MapTile> filtrtiles(List<MapTile> tils)//remove absolutely unusable tiles
        {
            List<MapTile> tunvalc = new List<MapTile>();
            for (int i = 0; i < tils.Count; i++)
            {
                if (tils[i].type == 1 && testchdir(tils[i]) > 0)
                {
                    tunvalc.Add(tils[i]);
                }
            }
            return tunvalc;
        }

        private Filtvalcells filtvalcell(Filtvalcells vc)//refresh existing Filtrcells
        {
            if(vc.bestcells.Count>0)
            {
                vc.bestcells = filtrtiles(vc.bestcells);
                setcellqual(vc.bestcells);
                for (int i = 0; i < vc.bestcells.Count; i++)
                {
                    if (vc.bestcells[i].qual == 0)
                    {

                    }
                    else if (vc.bestcells[i].qual == 1 && !valcells.normdcells.Exists(x => x == vc.bestcells[i]))
                    {
                        valcells.normdcells.Add(vc.bestcells[i]);
                        vc.bestcells.RemoveAt(i);
                        i--;
                    }
                    else if (vc.bestcells[i].qual == 2 && !valcells.badcells.Exists(x => x == vc.bestcells[i]))
                    {
                        valcells.badcells.Add(vc.bestcells[i]);
                        vc.bestcells.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        vc.bestcells.RemoveAt(i);
                        i--;
                    }
                }
            }
            if(vc.bestcells.Count==0&&vc.normdcells.Count>0)
            {
                vc.normdcells = filtrtiles(vc.normdcells);
                setcellqual(vc.normdcells);
                for (int i = 0; i < vc.normdcells.Count; i++)
                {
                    if (vc.normdcells[i].qual == 1)
                    {}
                    else if (vc.normdcells[i].qual == 2 && !valcells.badcells.Exists(x => x == vc.normdcells[i]))
                    {
                        valcells.badcells.Add(vc.normdcells[i]);
                        vc.normdcells.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        vc.normdcells.RemoveAt(i);
                        i--;
                    }
                }
            }
            else
            {
                vc.badcells = filtrtiles(vc.badcells);
            }
            return vc;
        }

        private Filtvalcells updtvalidcells(Filtvalcells source, Filtvalcells target)//add from one filtlist to another
        {
            target.bestcells.AddRange(source.bestcells);
            target.normdcells.AddRange(source.normdcells);
            target.badcells.AddRange(source.badcells);
            return target;
        }

        private Tunneler widetun(Tunneler t, int type)//wide out tunnels//type- 0=regular, 1=wideprev, 2=widestraight(for making straight nit "cavelike" tunnels)(not implemented)
        {
            updtallcells();
            int d = tnextstep(t);
            Random rnd = new Random();
            if (d == 1 || d == 3)
            {
                int w = t.wide;
                MapTile left = t.cur;
                MapTile right = t.cur;
                if (type==1)
                {
                    left = t.prev;
                    right = t.prev;
                }
                bool r = true;
                bool l = true;
                while ((l || r) && w > 1)
                {
                    int chose = rnd.Next(0, 2);
                    if (chose == 0)
                    {
                        if (cellvalid(left.neighbours[0]) && left.neighbours[0].neighbours[0].type == 0)
                        {
                            left = left.neighbours[0];
                            left.type = 1;
                            t.left = left;
                            w--;
                            floorcur++;
                            t.allcells.Add(left);
                        }
                        else
                        {
                            l = false;
                        }
                    }
                    else
                    {
                        if (cellvalid(right.neighbours[2]) && right.neighbours[2].neighbours[2].type == 0)
                        {
                            right = right.neighbours[2];
                            right.type = 1;
                            t.right = right;
                            w--;
                            floorcur++;
                            t.allcells.Add(right);
                        }
                        else
                        {
                            r = false;
                        }
                    }
                }
            }
            else
            {
                int w = t.wide;
                MapTile left = t.cur;
                MapTile right = t.cur;
                if (type == 1)
                {
                    left = t.prev;
                    right = t.prev;
                }
                bool r = true;
                bool l = true;
                while ((l || r) && w > 1)
                {
                    int chose = rnd.Next(0, 2);
                    if (chose == 0)
                    {
                        if (cellvalid(left.neighbours[1]) && left.neighbours[1].neighbours[1].type == 0)
                        {
                            left = left.neighbours[1];
                            left.type = 1;
                            t.left = left;
                            w--;
                            floorcur++;
                            t.allcells.Add(left);
                        }
                        else
                        {
                            l = false;
                        }
                    }
                    else
                    {
                        if (cellvalid(right.neighbours[3]) && right.neighbours[3].neighbours[3].type == 0)
                        {
                            right = right.neighbours[3];
                            right.type = 1;
                            t.right = right;
                            w--;
                            floorcur++;
                            t.allcells.Add(right);
                        }
                        else
                        {
                            r = false;
                        }
                    }
                }
            }
            return t;
        }

        private Tunneler acttunneler(Tunneler t, int action)//action: 0-move straight,  1-turn, 2-cross other tunnel, 3-stepback&turn
        {
            updtallcells();
            int d = tnextstep(t);
            t.prevactsuc = false;
            if (action==0)
            {
                if (d == 1 || d == 3)
                {
                    if (t.cur.neighbours[d].neighbours[0].type == 0 && t.cur.neighbours[d].neighbours[2].type == 0&&cellvalid(t.cur.neighbours[d]))
                    {
                        t.prev = t.cur;
                        t.cur = t.prev.neighbours[d];
                        t.cur.type = 1;
                        floorcur = floorcur + 1;
                        t.lenght--;
                        t.prevactsuc = true;
                    }
                }
                else if(d==2||d==0)
                {
                    if (t.cur.neighbours[d].neighbours[1].type == 0 && t.cur.neighbours[d].neighbours[3].type == 0 && cellvalid(t.cur.neighbours[d]))
                    {
                        t.prev = t.cur;
                        t.cur = t.prev.neighbours[d];
                        t.cur.type = 1;
                        floorcur = floorcur + 1;
                        t.lenght--;
                        t.prevactsuc = true;
                    }
                }
            }
            if(action==1)
            {
                if(t.wide==1)
                {
                    int turn = choosetdir(t.cur, 0);
                    if (turn < 0)
                    {
                        t.prevactsuc = false;
                    }
                    else
                    {
                        t.prev = t.cur;
                        t.cur = t.cur.neighbours[turn];
                        t.cur.type = 1;
                        floorcur = floorcur + 1;
                        t.lenght = t.lenght - 1;
                        t.prevactsuc = true;
                    }
                }
                else
                {
                    bool l = true;
                    bool r = true;
                    Random rnd = new Random();
                    while (r || l)
                    {
                        int c = rnd.Next(0, 2);
                        if (c == 0 || r == false)
                        {
                            l = false;
                            int turn = choosetdir(t.left, 0);
                            if (turn < 0)
                            {
                                t.prevactsuc = false;
                            }
                            else
                            {
                                t.prev = t.left;
                                t.cur = t.left.neighbours[turn];
                                t.cur.type = 1;
                                floorcur = floorcur + 1;
                                t.lenght = t.lenght - 1;
                                t.prevactsuc = true;
                                r = false;
                                break;
                            }
                        }
                        else
                        {
                            r = false;
                            int turn = choosetdir(t.right, 0);
                            if (turn < 0)
                            {
                                t.prevactsuc = false;
                            }
                            else
                            {
                                t.prev = t.right;
                                t.cur = t.right.neighbours[turn];
                                t.cur.type = 1;
                                floorcur = floorcur + 1;
                                t.lenght = t.lenght - 1;
                                t.prevactsuc = true;
                                l = false;
                                break;
                            }
                        }

                    }
                }
            }
            if(action==2)
            {
                for(int i=0;i<5;i++)
                {
                    t.prev = t.cur;
                    t.cur = t.prev.neighbours[d];
                    if(t.cur.neighbours[d].type==0)
                    {
                        t.prevactsuc = true;
                        t = acttunneler(t, 0);
                        break;
                    }
                    if(t.cur.type==0)
                    {
                        break;
                    } 
                    
                }
            }
            if(action==3)
            {
                MapTile temp = t.cur;
                t.cur = t.prev;
                t.prev = temp;
                int turn = choosetdir(t.cur, 0);
                if (turn < 0)
                {
                    t.prevactsuc = false;
                }
                else
                {
                    Random r = new Random();
                    int ra = r.Next(0, 2);
                    if(ra==0)
                    {
                        t.prev.type = 0;
                        floorcur = floorcur - 1;
                    }
                    else { widetun(t, 1); }
                    t.prev = t.cur;
                    t.cur = t.prev.neighbours[turn];
                    t.cur.type = 1;
                    floorcur = floorcur + 1;
                    t.lenght = t.lenght - 1;
                    t.prevactsuc = true;
                }
            }
            return t;
        }

        private bool cellvalid(MapTile mt)//checks if cell is not bordercell& is a wall
        {
            bool b = false;
            for(int i=0; i<4; i++)
            {
                if(mt.neighbours[i].type<0)
                    {
                    b = true;
                    }
            }
            if (mt.type == 0 && b==false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int tnextstep(Tunneler t)//returns neighbour index for moving straight
        {
            //updtallcells();//_______________________________________________________________________________________________________________
            int d = -1;
            for(int i=0;i<4;i++)
            {
                if(t.prev.neighbours[i]==t.cur)
                {
                    d = i;
                    break;
                }
            }
            return d;
        }

        private int testchdir(MapTile mt)//another tile validation check
        {
            for(int i=0;i<4;i++)
            {
                if(mt.neighbours[i].countneigb()<2)
                {
                    if(Array.Exists(mt.neighbours[i].neighbours, x=>x.countneigb()==0))
                    {
                        return 1;
                    }
                }
            }
            return -1;
        }

        private int choosetdir(MapTile mt, int type)//0-regular(random valid cell), 1-first cell(best valid cell)//returns neighbour index for valid direction of next move
        {
            //updtallcells();//_______________________________________________________________________________________________________________
            Random r = new Random();
            int d = r.Next(0, 4); 
            if(type==0)
            {
                bool[] b = new bool[4] { false, false, false, false };
                while (b[0] == false || b[1] == false || b[2] == false || b[3] == false)
                {
                    while (b[d])
                    {
                        d = r.Next(0, 4);
                    }
                    if (cellvalid(mt.neighbours[d]))
                    {
                        if (d == 1 || d == 3)
                        {

                            if (mt.neighbours[d].neighbours[0].type == 0 && mt.neighbours[d].neighbours[2].type == 0 && mt.neighbours[d].neighbours[d].type == 0)
                            {
                                return d;
                            }
                            else { b[d] = true; }
                        }
                        else
                        {
                            if (mt.neighbours[d].neighbours[1].type == 0 && mt.neighbours[d].neighbours[3].type == 0 && mt.neighbours[d].neighbours[d].type == 0)
                            {
                                return d;
                            }
                            else { b[d] = true; }
                        }
                    }
                    else
                    {
                        b[d] = true;
                    }
                }
                if (!Array.Exists(b, x => x == false))
                {
                    d = -1;
                }
            }
            else
            {
                int[] b = new int[4] { -1, -1, -1, -1 };
                for(int i=0;i<4;i++)
                {
                    if (cellvalid(mt.neighbours[i]))
                    {
                        if (i == 1 || i == 3)
                        {

                            if (mt.neighbours[i].neighbours[0].type == 0 && mt.neighbours[i].neighbours[2].type == 0 && mt.neighbours[i].neighbours[i].type == 0)
                            {
                                int t = dcv2(mt.neighbours[i], 0, new List<MapTile>());
                                if(t>250)
                                {
                                    b[i] = 0;
                                }
                                else if(t<=250&&t>100)
                                {
                                    b[i] = 1;
                                }
                                else { b[i] = 2; }
                            }
                            else { b[i] = -1; }
                        }
                        else
                        {
                            if (mt.neighbours[i].neighbours[1].type == 0 && mt.neighbours[i].neighbours[3].type == 0 && mt.neighbours[i].neighbours[i].type == 0)
                            {
                                int t = dcv2(mt.neighbours[i], 0, new List<MapTile>());
                                if (t > 250)
                                {
                                    b[i] = 0;
                                }
                                else if (t <= 250 && t > 100)
                                {
                                    b[i] = 1;
                                }
                                else { b[i] = 2; }
                            }
                            else { b[i] = -1; }
                        }
                    }
                    else
                    {
                        b[i] = -1;
                    }
                }
                if (!Array.Exists(b, x => x != -1))
                {
                    return -1;
                }
                for (int i=0;i<3;i++)
                {
                    if (Array.Exists(b, x => x == i))
                    {
                        while (b[d]!=i)
                        {
                            d = r.Next(0, 4);
                        }
                        return d;
                    }
                } 
            }
            return d;
        }
        public void iniccells()//inicialix=zing cells for 1st time and setting their neigbours
        {
            MapTile nul = new MapTile();
            nul.type = -1;
            nul.neighbours = new MapTile[4] { nul, nul, nul, nul };
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    tiles[i, j].neighbours = new MapTile[4];
                    if (j > 0) {tiles[i, j].neighbours[2] = tiles[i, j - 1]; }//vniz
                    else { tiles[i, j].neighbours[2] = nul; }
                    if (j < width - 2) { tiles[i, j].neighbours[0] = tiles[i, j + 1]; }//vverh
                    else { tiles[i, j].neighbours[0] = nul; }
                    if (i < height - 2) { tiles[i , j].neighbours[3] = tiles[i+1, j]; }//vpravo
                    else { tiles[i, j].neighbours[3] = nul; }
                    if (i > 0) { tiles[i , j].neighbours[1] = tiles[i-1, j]; }//vlevo
                    else { tiles[i, j].neighbours[1] = nul; }
                }
            }
        }

        private void updtallcells()//updates cells neighbour values cause changes from the stream dont apply while generatian not finishes. There probably must be better solution
        {
            for (int i = 1; i < height-2; i++)
            {
                for (int j = 1; j < width-2; j++)
                {
                    tiles[i, j].neighbours[2] = tiles[i, j - 1]; //down
                    tiles[i, j].neighbours[0] = tiles[i, j + 1]; //up
                    tiles[i, j].neighbours[3] = tiles[i + 1, j]; //right
                    tiles[i, j].neighbours[1] = tiles[i - 1, j]; //left
                }
            }
        }
    }
}
