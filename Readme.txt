M1TE ver 1.7 (SNES Mode 1 Tile Editor) 
June 11, 2020
.NET 4.5.2 (works with MONO on non-Windows systems)
For SNES game development. Mode 1.
Freeware by Doug Fraker


The MIT License (MIT)


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
    - fixed tilemap image zoom code that cut of 1/2
      a pixel at the top and left, that effected exported
      pictures also.
    - fixed bug, double clicking in a dialogue box caused
      a mouse event on the tilemap below it.
    - added hotkeys to change the tileset-1,2,3,4,5,6,7,8


Note, the RLE is a special compression format that I wrote, 
specifically for SNES maps (but could be used for tiles).
See unrle.txt (or my SNES projects) for decompression code.



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
Numberpad 2,4,6,8 to move to adjacent tile.
C - copy, P - paste.
1,2,3,4,5,6,7,8 - to change the tilset.

(these only work if focus is not on one of the text boxes on the form)

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
H - flip horizontal (notice the symmetric shape of the letter W)
V - flip vertical (notice the symmetric shape of the letter E)
R - rotate clockwise
L - rotate counter clockwise
Delete - fills with color 0 (transparent)
C - copy
P - paste



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

Clone from Tiles and Clone from Map. Right click to select the
starting tile. Click and drag on the map will place tiles,
copying from the source. No wrap around allowed.

Fill the Map - click on a map to fill it with the selected tile.
If "palette only" is checked, it will change the palette of the map.

HELPFUL TIP! - Use Number pad 2,4,6,8 to switch to adjacent tile.



Menu
----
All the menu options should be self-explanatory. Some of them won't work if
you are in the wrong mode. The message box should explain the problem.

Loading just 32 bytes palette loads to the currently selected palette row.
Same with saving 32 bytes. It saves the currently selected palette row.

Saving Maps only saves the currently selected map. Loading maps only loads to
the currently selected map.

Loading/Saving 1 tileset will load/save the currently selected set. The bit
depth needs to match, so consider marking each tileset with a 2 or 4 to
keep them separated.

File/Export Image saves the current view on the Tilemap as .png .bmp or .jpg



Native .M1 file format details...
16 byte header = 
 2 bytes = "M1"
 1 byte = # of the file version (should be 1)
 1 byte = # of palettes (should be 1)
 1 byte = # of maps (should be 3)
 1 byte = # of 4bpp tilesets (should be 4)
 1 byte = # of 2bpp tilesets (should be 4)
 1 byte = map height
 pad 8 zeros
256 bytes per palette (should be 256 total)
2048 bytes per tile map (x3 should be 6144 total)
8192 bytes per 4bpp tile set (x4 should be 32768 total)
4096 bytes per 2bpp tile set (x4 should be 16384 total)
16 + 256 + 6144 + 32768 + 16384 = 55568 bytes per file, exactly.




///////////////////////////////////////////////
TODO-
-16x16 view mode
///////////////////////////////////////////////



Credits -
I used Klarth's Console Graphics Document...
https://mrclick.zophar.net/TilEd/download/consolegfx.txt
in making this software. 


