# MapGenerator 
Map generator for dungeons 
Creates random dungeons. Can be used in rogue-like rpgs. 
I used something similar to drunkard walk algorithm for tunnel generation combined with cellular automata for caves. 
In code settings for size, frequensy of elements an so on is set for my taste, but can be easily changed by changing couple of variables. Algorithm is pretty flexible. 
I tested it on different sizes up to 800x800 map and it works fine, just takes more time. Less then 200x200 can be generated in less then minute, but higher vakues takes much more time, up to 40mins on 800. So i suggest, if using, start generating new map in background thread while player walks throught previous one. Or just use <200 size. Also, on higher then 700 size some of tunnels on canvas became not visible cause of to big scale. 
Currently it generates only "natural cave"-like structure, but i may add later options for making tunnels more like corridors and caves like rooms, for more "industrial" settings. There is unused variable in MapTile - prefab. It`s for using prefab structures. Just needs to add "prefab check" so tunnels won`t dig through pre designed zones
