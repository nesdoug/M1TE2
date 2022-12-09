M1TE ver 3.6 (SNES Mode 1 Tile Editor) 
Dec 9, 2022
.NET 4.5.2 (works with MONO on non-Windows systems)
For SNES game development. Mode 1.
Freeware by Doug Fraker


The MIT License (MIT)


This app is for generating, editing, and arranging 
SNES tiles and tilemaps (and palettes).
It is designed for Mode 1, but could be used for any
mode that needs 2bpp or 4bpp graphics.



version changes
1.1 - added more palette options (color picker)
1.2 - fixed palette bug
    - made the map editor faster
    - changed keys in tile editor (P = paste, H and V are flip)
    - save/load session saves map height
    - load any (reasonable) size tileset
    - added brush size for map editor
    - fixed file type bug
1.3 - added map shift buttons
    - allow loading unusual size tile, map, palette files
    - allow loading from very large tile, map, palette files
    - bug fix loading full palette in 2bpp mode
    - added Load Map to Selected Y coordinate
      (for loading multiple small maps to the same screen)
    - added keys to copy/paste/delete colors in a palette
      Q = copy, W = paste, E = delete color 0000
    - fixed hex box to allow 1,2,or 3 digit entries.
1.4 - Grid lines color adapts to the 0th color
    - brush 2x2 next (pseudo 16x16) will flip 16x16
      with Apply H Flip or Apply V flip checked
    - added checkerboard fill if map height is < 32
1.5 - RLE option to save map or tiles
    - fixed, forgot to allow save 2bpp x 1 from sets 6,7,8
1.6 - clone from tiles or from map (brush)
    - fill map with tile (brush), or recolor entire map
      if "palette only" is checked
    - can use all key commands outside tile edit form
    - added slider bars for color
    - changed default map height to 28
1.7 - minor fix slider bar updating
    - fixed tilemap image zoom code that cut off 1/2
      a pixel at the top and left, that effected exported
      pictures also.
    - fixed bug, double clicking in a dialogue box caused
      a mouse event on the tilemap below it.
    - added hotkeys to change the tileset-1,2,3,4,5,6,7,8
2.0 - import image chr/map and import palette from image
    - much faster drawing tiles on map
    - changed the grid lines
    - remove duplicate tiles
    - load tiles to selected tile (to join multiple chr files)
    - save tiles in a range (to split into smaller files)
    - minor renaming of menu items
2.1 - fix, rt click on tile editor wasn't updating 
      palette values
    - top right tile # now displays 256*tileset+tile
      so that "save tiles in range" is easier to do
3.0 - basic undo function (press Z)
      16x16 tilesize option
3.1 - minor changes and bug fixes for 16x16 mode
3.2 - 16x16 tile functions work in 16x16...
      such as flip, rotate, delete, fill, copy
      (fails if you highlight the right most
      or the bottom most tile in a set)
    - tile editor can view a full 16x16 tile
    - 16x16 grid also highlights the tileset
3.3 - added brushes "multi select" and "map edit"
      (replaces clone brushes, now removed)
    - command keys x=cut and a=select all added.
    - change v=paste and y=vertical flip
    - slight change to "best color" formula
      (should prefer a color closer to the original hue
      rather than a wrong color of the same brightness)
    - minor bug fixes
    - added a checkbox on import options, auto-
      put imported tiles on map (now off by default)
    - importing a 128x128 image (or smaller)
      now only blanks the needed tiles,
      and starts at the selected tile
    - allow small images to be imported as palettes
      (as small as 2x1) to allow 16x1 images as a palette
3.5 - bumped up version # to show that
      there were lots of changes 
3.6 - slight changes / bug fixes


Note, the RLE is a special compression format that I wrote, 
specifically for SNES maps (but could be used for tiles).
See unrle.txt (or my SNES projects) for decompression code.


Undo
----
Edit/Undo or press Z
Note: only will undo changes to map or tiles. Palette
changes won't undo. Some other things (checkbox status,
which map view, which tile view, etc) may not undo.


16x16 Tile Mode
---------------
In order to minimize the changes to the UI, this mode
will appear zoomed out compared to 8x8 mode. Also, one
of the brushes won't work right (the 2x2 pseudo 16x16) 
so it has been disabled in this mode.
"Remove unused tile" will work differently than 8x8 mode.
*this mode affects all layers. There is no way to mix
and match here, even though you can do that on the SNES.
If you need that, you will have to create 2 projects. One
at 8x8 and one at 16x16.
(some other features)
-Tilesize/Force Maps to Even Values-- is not persistent,
like you might expect, but it changes all the current
map values to even X and Y values, since you probably
have your 16x16 tiles aligned to the even values.
-Tilesize/Zoom Into Quadrant-- in 16x16 mode, since the
map is zoomed out from what you expect, this option
will give a quick zoomed in view, to the selected map
quadrant (to show what your actual screen will be).
Please, rt click on the map to exit this mode
before doing anything else, which will be
buggy (or broken) with maps stuck in Zoomed In Mode.

In order for 16x16 tiles to work correctly on the SNES,
make sure you select that when writing BGMODE ($2105).
Maps will be twice as large in this mode, allowing
further scrolling before reaching the end of the map.





Tilemaps
-------- 
3 layers. 2 of them 4bpp (16 color). 1 of them 2bpp (4 color).
Only 32x32 mode. It can't do 64x32, 32x64, or 64x64. You will have 
to make multiple files for each 32x32 region needed.
You can also change the map height at the bottom left (1-32).

Change the BG View to select a layer.
**Editing is disabled in preview modes.** (except palettes and map height)

Left click to add tiles to the tile map.
Right click to select a given tile, and get info on it.
You can edit the attributes of the selected tile by clicking H Flip,
V Flip, Priority, or Palette (0-7).
Checking "Apply H Flip" or "Apply V Flip" will cause future placed tiles to 
be flipped.
Checking "Apply Only Palette" means left click (or drag) on the map will
only change the palette of tiles on the map.

The priority button changes all the priority bits on the selected map. You 
won't be able to see this except the 3/1/2 preview shows what would happen 
if you have all the priority bits set on BG3 and the priority bit set on 
register $2105.
Be sure to click the priority checkbox on BG3 before saving, if you want 
BG3 to be on top.



Tilesets
--------
4 sets for 4bpp, (sets 1,2,3,4). 4 sets for 2bpp, (sets 5,6,7,8).
4bpp are for layers 1 and 2. 2bpp is for layer 3.

Left/Right click to open an editing box.
Numberpad 2,4,6,8 to move to adjacent tile. (some brushes only)
(see the key commands below)
1,2,3,4,5,6,7,8 - to change the tileset.

*note - 2bpp SNES tilesets are NOT like NES. They are like Gameboy, GB,
so, if you use YY-CHR, set the tile mode to 2bpp GB. You can easily
convert NES to SNES in YY-CHR by loading NES, copy all the visible tiles,
switch to 2bpp GB, then paste the tiles again.



Tile Edit Box
-------------
Left click - place the currently selected color on the grid
Right click - get the color under the pointer
Numberpad 2,4,6,8 to move to adjacent tile.
Arrow keys to shift the image.
F - fills a tile with selected color
H - flip horizontal
Y - flip vertical
R - rotate clockwise
L - rotate counter clockwise
Delete - fills with color 0 (transparent)
C - copy
X - cut
V - paste
A - select all (only works for some brushes)



Palette
-------
In 2bpp mode, BG View BG3, each palette has only 4 colors (3 + transparent)
It is recommended that the first row be reserved for 2bpp tiles.
In 4bpp we have 16 colors (15 + transparent) per palette.

Left/Right click - select a color
R - edit red
G - edit green
B - edit blue
Hex - manually type the SNES color code (2 bytes)
(the color doesn't update until you hit Return in one of these boxes)

Key presses...(click a palette color first)
Q = copy selected color
W = paste to the selected color
E = delete the selected color (sets 0000 black)

* use caution naming palettes the same as your tileset, if you use YY-CHR
like I do. YY-CHR will auto-create a palette, if you load a .chr and it
also finds a .pal of the same name. However, it assumes RGB and not the
15 bit SNES style palette, so the palette will be junk colors.
The load/save palette as RGB options are specifically for YY-CHR. THAT
palette can be the same name as the tileset.



Brushsize
---------
Brushes are for the map. 1x1 means place the current tile.
3x3 and 5x5 will place multiples of the same tile. It is for painting
larger areas of the screen with the same tile.

2x2 next is a pseudo 16x16 placement. It places x, x+1, x+$10, x+$11
of the selected tile in a 2x2 block on the screen. This might be
useful if the tileset has tiles arranged in 16x16 blocks.

(Clone from Tiles and Clone from Map. Removed/Replaced)

Fill the Map - click on a map to fill it with the selected tile.
If "palette only" is checked, it will change the palette of the map.

Multi Tile Select (this is what you should be using mostly) - 
you can now select multiple tiles at once, and flip/shift/copy/etc
and place them on the map all as a block. But, the code to update 
the map is slower, so if you are only placing a single tile at a 
time, it will be smoother using the 1x1 brush.

Map Edit Only - You can now select multiple tiles on the map view
with this brush, and copy/cut/paste/fill/delete and flip them
all at once. 
(some of the checkboxes are disabled with this brush/mode.
Use key commands to flip - h and y)
Tip - Combine fill (f) with "palette only" to change 
the palette of the selected area.
You can't rotate or pixel shift in this mode.



Menu
----
All the menu options should be self-explanatory. Some of them won't work if
you are in the wrong mode. The message box should explain the problem.

Loading a 16 color palette loads to the currently selected palette row.
Same with saving 16 color. It saves the currently selected palette row.

Saving Maps only saves the currently selected map. Loading maps only loads to
the currently selected map.

Loading/Saving 1 tileset will load/save the currently selected set. The bit
depth needs to match, so consider marking each tileset with a 2 or 4 to
keep them separated.

File/Export Image saves the current view on the Tilemap as .png .bmp .gif or .jpg

Tiles/Remove Duplicate Tiles - will look within the same bit depth for 
duplicates (including flipped versions of the same tile), and remove them. 
It will also scan the maps and renumber them to match the new tilesets.

Tiles/Load to Selected Tile - first select a tile in the tileset. Then click
this to load a CHR file at the selected point. You can combine CHR files
this way, or use it as a paste option.

Tiles/Save Tiles in a Range - if you only want to save a portion of a tileset,
or maybe 1 1/2 of a tileset. Also, can use like a copy/paste with the above.

Maps/Load to Selected Y - first select a location in the
tile map, then this will load a map starting at that row

Maps/Load to Selected Y, Offset to Selected Tile - First
select a tile map location, then select a tile in the set,
then choose this to load a map at the specific map row,
and it will also change the tile numbers in the map you
are loading to start at the selected tile. Why? Let's say
you are working on one project, which has a tileset and
map saved, and you want to import those into another
project, so you load the tileset below the current tileset,
and now the corresponding map has the tile numbers wrong.
This would adjust the tile numbers on the imported map,
for the now relocated tiles.



Import an image
---------------
.png .jpg .bmp .gif - files need to be 256x256 or smaller
This will be a 3 step process. First, select options and set a dither level.
Then, get the palette from the image (or make your own palette).
Finally, get the tiles/map from the image.
-CAUTION, it will erase the entire tileset and the current map
 (only if it is larger than 128x128, a small image will only
  delete the needed area)
-if a file has an indexed palette, it will not read it... it always
auto-generates an optimized palette
-if you have BG View 1 or 2, it will generate a 4bpp tileset and 16 colors
-if you have BG View 3, it will generate a 2bpp tileset and 4 colors
-the dithering is with brightness, and is not very good with hue shifts
-dithered graphics don't compress very well



Native .M1 file format details...
16 byte header = 
 2 bytes = "M1"
 1 byte = # of the file version (should be 1)
 1 byte = # of palettes (should be 1)
 1 byte = # of maps (should be 3)
 1 byte = # of 4bpp tilesets (should be 4)
 1 byte = # of 2bpp tilesets (should be 4)
 1 byte = map height
 1 byte = tilesize (0 or 1)
 pad 7 zeros
256 bytes per palette (should be 256 total)
2048 bytes per tile map (x3 should be 6144 total)
8192 bytes per 4bpp tile set (x4 should be 32768 total)
4096 bytes per 2bpp tile set (x4 should be 16384 total)
16 + 256 + 6144 + 32768 + 16384 = 55568 bytes per file, exactly.




///////////////////////////////////////////////
TODO-
-color replace tool
-more control of priority
///////////////////////////////////////////////



Credits -
I used Klarth's Console Graphics Document...
https://mrclick.zophar.net/TilEd/download/consolegfx.txt
in making this software. 


