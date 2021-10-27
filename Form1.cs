using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace M1TE2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Location = new Point(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y);

        }
        static Form2 newChild = null;
        static Form3 newChild3 = null;
        static Form4 newChild4 = null;

        public static void close_it()
        {
            newChild = null;
        }

        public static void close_it3()
        {
            newChild3 = null;
        }

        public static void close_it4()
        {
            newChild4 = null;
        }

        //globals
        public static Bitmap image_map = new Bitmap(256, 256);
        public static Bitmap image_tiles = new Bitmap(128, 128);
        public static Bitmap image_pal = new Bitmap(256, 256);
        public static Bitmap image_map_local = new Bitmap(256, 256);
        public static Bitmap temp_bmp = new Bitmap(256, 256); //double size
        public static Bitmap temp_bmp2 = new Bitmap(512, 512); //double size
        public static Bitmap temp_bmp3 = new Bitmap(256, 256); //double size
        public static Bitmap cool_bmp = new Bitmap(256, 256); //import
        public static int pal_x, pal_y, tile_x, tile_y, tile_num, tile_set;
        public static int map_view, active_map_x, active_map_y, active_map_index;
        public static int map_height = 28;
        public static int last_tile_x, last_tile_y;
        public static int brushsize;
        public const int BRUSH1x1 = 0;
        public const int BRUSH3x3 = 1;
        public const int BRUSH5x5 = 2;
        public const int BRUSHNEXT = 3;
        public const int BRUSH_CLONE_T = 4;
        public const int BRUSH_CLONE_M = 5;
        public const int BRUSH_FILL = 6;
        public static int pal_r_copy, pal_g_copy, pal_b_copy;
        public static byte[] rle_array = new byte[65536];
        public static int rle_index, rle_index2, rle_count;
        public static int map_clone_x, map_clone_y, clone_start_x, clone_start_y;
        public static int disable_map_click;

        public static int[] R_Array = new int[65536];
        public static int[] G_Array = new int[65536];
        public static int[] B_Array = new int[65536];
        public static int[] Count_Array = new int[65536]; // count each color
        public static int[] SixteenColorIndexes = new int[16];
        public static int[] SixteenColorsAdded = new int[16];
        //public static int[] SortedColorIndexes = new int[16];
        public static int color_count; // how many total different colors
        public static int r_val, g_val, b_val, diff_val;
        public static int c_offset, c_offset2;
        public static int image_width, image_height;
        public static int[] needy_chr_array = new int[65536]; // temp store color values of imported image

        public static int dither_factor = 0;
        public static int dither_adjust = 0;
        public static double dither_db = 0.0;
        public static bool f3_cb1 = false, f3_cb2 = false;
        public static bool flip_h = false, flip_v = false;

        public static bool undo_ready = false;
        public const int TILE_8X8 = 0;
        public const int TILE_16X16 = 1;
        public static int tilesize = TILE_8X8;
        public static bool zoom_Q_flag = false;



        public readonly int[,] BAYER_MATRIX =
        {
            { 0,48,12,60,3,51,15,63 },
            { 32,16,44,28,35,19,47,31 },
            { 8,56,4,52,11,59,7,55 },
            { 40,24,36,20,43,27,39,23 },
            { 2,50,14,62,1,49,13,61 },
            { 34,18,46,30,33,17,45,29 },
            { 10,58,6,54,9,57,5,53 },
            { 42,26,38,22,41,25,37,21 }
        }; // 1/64 times this


        // these are for the remove duplicate, flipped
        public readonly int[] H_FLIP_TABLE =
        {
            7,6,5,4,3,2,1,0,
            15,14,13,12,11,10,9,8,
            23,22,21,20,19,18,17,16,
            31,30,29,28,27,26,25,24,
            39,38,37,36,35,34,33,32,
            47,46,45,44,43,42,41,40,
            55,54,53,52,51,50,49,48,
            63,62,61,60,59,58,57,56
        };

        public readonly int[] V_FLIP_TABLE =
        {
            56,57,58,59,60,61,62,63,
            48,49,50,51,52,53,54,55,
            40,41,42,43,44,45,46,47,
            32,33,34,35,36,37,38,39,
            24,25,26,27,28,29,30,31,
            16,17,18,19,20,21,22,23,
            8,9,10,11,12,13,14,15,
            0,1,2,3,4,5,6,7
        };

        public readonly int[] HV_FLIP_TABLE =
        {
            63,62,61,60,59,58,57,56,
            55,54,53,52,51,50,49,48,
            47,46,45,44,43,42,41,40,
            39,38,37,36,35,34,33,32,
            31,30,29,28,27,26,25,24,
            23,22,21,20,19,18,17,16,
            15,14,13,12,11,10,9,8,
            7,6,5,4,3,2,1,0
        };


        



        private void Form1_Load(object sender, EventArgs e)
        {
            update_palette();
            update_tile_image();
            update_tilemap();
            label5.Focus();
            this.ActiveControl = label5;
        }


        public void Checkpoint()
        {
            // backup for undo function
            undo_ready = true;


            for(int i = 0; i < 3072; ++i)
            { // copy maps
                MapsU.tile[i] = Maps.tile[i];
                MapsU.palette[i] = Maps.palette[i];
                MapsU.h_flip[i] = Maps.h_flip[i];
                MapsU.v_flip[i] = Maps.v_flip[i];
                MapsU.priority[i] = Maps.priority[i];
            }

            for(int i = 0; i < 131072; ++i)
            { // copy tilesets
                TilesU.Tile_Arrays[i] = Tiles.Tile_Arrays[i];
            }
        }

        public void Do_Undo()
        {
            if (undo_ready == false) return;

            for (int i = 0; i < 3072; ++i)
            { // restore maps
                Maps.tile[i] = MapsU.tile[i];
                Maps.palette[i] = MapsU.palette[i];
                Maps.h_flip[i] = MapsU.h_flip[i];
                Maps.v_flip[i] = MapsU.v_flip[i];
                Maps.priority[i] = MapsU.priority[i];
            }

            for (int i = 0; i < 131072; ++i)
            { // restore tilesets
                Tiles.Tile_Arrays[i] = TilesU.Tile_Arrays[i];
            }

            active_map_index = active_map_x + (active_map_y * 32) + (32 * 32 * map_view);
            int value = Maps.palette[active_map_index];
            textBox5.Text = value.ToString();
            if (Maps.h_flip[active_map_index] == 0)
            {
                checkBox1.Checked = false;
            }
            else
            {
                checkBox1.Checked = true;
            }
            if (Maps.v_flip[active_map_index] == 0)
            {
                checkBox2.Checked = false;
            }
            else
            {
                checkBox2.Checked = true;
            }
            if (Maps.priority[active_map_index] == 0)
            {
                checkBox3.Checked = false;
            }
            else
            {
                checkBox3.Checked = true;
            }

            common_update2();

            undo_ready = false;
        }


        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Do_Undo();
        }


        // 16x16 grid
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            update_tilemap();
            label5.Focus();
        }


        public void update_tilemap8x8()
        {
            //default BG, draw color 0 all over the BG
            int r = Palettes.pal_r[0];
            int g = Palettes.pal_g[0];
            int b = Palettes.pal_b[0];
            int offset = 0;
            int temp_tile = 0;
            int temp_pal = 0;
            int z2 = 0;

            for (int y = 0; y < 256; y++) // fill with the 0th color first
            {
                for (int x = 0; x < 256; x++)
                {
                    image_map_local.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            if (map_view > 2) // preview modes
            {
                // draw all the maps, layered, with color 0 transparent
                // and draw them all together
                if (map_view == 3) // 1,2,3 = draw the 3rd in the back
                {
                    z2 = 2 * 32 * 32; // offset for current map
                    for (int y = 0; y < map_height; y++)
                    {
                        for (int x = 0; x < 32; x++)
                        {
                            offset = z2 + (y * 32) + x; // offset to current tile on the map
                            temp_tile = ((Maps.tile[offset] + 0x400) * 8 * 8); // base offset for tile                                                               // the 2bpp uses the 5th set of 256 tiles
                            temp_pal = (Maps.palette[offset] * 4); // beginning of this palette

                            big_sub(offset, x, y, temp_tile, temp_pal);
                        }
                    }
                }
                // now draw the 2nd
                z2 = 1 * 32 * 32; // offset for current map
                for (int y = 0; y < map_height; y++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        offset = z2 + (y * 32) + x; // offset to current tile on the map

                        temp_tile = (Maps.tile[offset] * 8 * 8); // base offset for tile
                        temp_pal = (Maps.palette[offset] * 16); // beginning of this palette
                        big_sub(offset, x, y, temp_tile, temp_pal);
                    }
                }
                // now draw the 1st
                z2 = 0; // offset for current map
                for (int y = 0; y < map_height; y++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        offset = z2 + (y * 32) + x; // offset to current tile on the map

                        temp_tile = (Maps.tile[offset] * 8 * 8); // base offset for tile
                        temp_pal = (Maps.palette[offset] * 16); // beginning of this palette
                        big_sub(offset, x, y, temp_tile, temp_pal);
                    }
                }
                if (map_view == 4) // 3,1,2 = draw the 3rd in the front
                {
                    z2 = 2 * 32 * 32; // offset for current map
                    for (int y = 0; y < map_height; y++)
                    {
                        for (int x = 0; x < 32; x++)
                        {
                            offset = z2 + (y * 32) + x; // offset to current tile on the map
                            temp_tile = ((Maps.tile[offset] + 0x400) * 8 * 8); // base offset for tile                                                               // the 2bpp uses the 5th set of 256 tiles
                            temp_pal = (Maps.palette[offset] * 4); // beginning of this palette

                            big_sub(offset, x, y, temp_tile, temp_pal);
                        }
                    }
                }
            }



            else // map views 0 or 1 or 2, draw one map 4bpp or 2bpp
            {
                int z = map_view * 32 * 32; // offset for current map
                for (int y = 0; y < map_height; y++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        offset = z + (y * 32) + x; // offset to current tile on the map

                        if (map_view == 2) // 2bpp
                        {
                            temp_tile = ((Maps.tile[offset] + 0x400) * 8 * 8); // base offset for tile
                            // the 2bpp uses the 5th set of 256 tiles
                            temp_pal = (Maps.palette[offset] * 4); // beginning of this palette
                        }
                        else // 4bpp
                        {
                            temp_tile = (Maps.tile[offset] * 8 * 8); // base offset for tile
                            temp_pal = (Maps.palette[offset] * 16); // beginning of this palette
                        }
                        big_sub(offset, x, y, temp_tile, temp_pal);
                    }
                }
            }

            // draw a checkerboard pattern over the unused portion of the map
            if (map_height < 32)
            {
                Color checker_color = Color.SlateGray;
                for (int y = map_height * 8; y < 256; y++)
                {
                    for (int x = 0; x < 256; x++)
                    {
                        if ((y & 4) == 0)
                        {
                            if ((x & 4) == 0)
                            {
                                checker_color = Color.SlateGray;
                            }
                            else
                            {
                                checker_color = Color.LightSlateGray;
                            }
                        }
                        else
                        {
                            if ((x & 4) == 0)
                            {
                                checker_color = Color.LightSlateGray;
                            }
                            else
                            {
                                checker_color = Color.SlateGray;
                            }
                        }

                        image_map_local.SetPixel(x, y, checker_color);
                    }
                }
            }

            //Bitmap temp_bmp2 = new Bitmap(512, 512); //resize double size
            using (Graphics g2 = Graphics.FromImage(temp_bmp2))
            {
                g2.InterpolationMode = InterpolationMode.NearestNeighbor;
                g2.PixelOffsetMode = PixelOffsetMode.Half; // fix bug, missing half a pixel on top and left
                g2.DrawImage(image_map_local, 0, 0, 512, 512);
            } // standard resize of bmp was blurry, this makes it sharp

            //draw a box around the active tile
            if (map_view < 3)
            {


                //draw grid here
                if (checkBox4.Checked == true)
                {
                    //draw horizontal lines at each 16
                    for (int i = 31; i < (map_height * 15); i += 32)
                    {
                        for (int j = 0; j < 510; j += 2)
                        {
                            temp_bmp2.SetPixel(j, i, Color.Black);
                            temp_bmp2.SetPixel(j + 1, i, Color.White);
                        }
                    }
                    //draw vertical lines at each 16
                    for (int j = 31; j < 511; j += 32)
                    {
                        for (int i = 0; i < (map_height * 16) - 2; i += 2)
                        {
                            temp_bmp2.SetPixel(j, i + 1, Color.Black);
                            temp_bmp2.SetPixel(j, i, Color.White);
                        }
                    }
                }
                // draw box around current selection
                int x2 = (active_map_x * 16);
                //if (x2 >= 496) x2 = 495;
                if (active_map_y >= map_height) active_map_y = map_height - 1;
                int y2 = (active_map_y * 16);
                //if (y2 >= 496) y2 = 495;
                for (int i = 0; i < 16; i++)
                {
                    temp_bmp2.SetPixel(x2 + i, y2, Color.White);
                    temp_bmp2.SetPixel(x2, y2 + i, Color.White);
                    temp_bmp2.SetPixel(x2 + i, y2 + 15, Color.White);
                    temp_bmp2.SetPixel(x2 + 15, y2 + i, Color.White);
                }
            }

            pictureBox1.Image = temp_bmp2;
            pictureBox1.Refresh();
        }
        // END UPDATE TILEMAP 8x8




        public void update_tilemap16x16()
        {
            label6.Text = "here1"; // debugging
            
            //default BG, draw color 0 all over the BG
            int r = Palettes.pal_r[0];
            int g = Palettes.pal_g[0];
            int b = Palettes.pal_b[0];
            int offset = 0;
            int temp_tile = 0;
            int temp_pal = 0;
            int z2 = 0;

            for (int y = 0; y < 512; y++) // fill with the 0th color first
            {
                for (int x = 0; x < 512; x++)
                {
                    temp_bmp2.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            if (map_view > 2) // preview modes
            {
                // draw all the maps, layered, with color 0 transparent
                // and draw them all together
                if (map_view == 3) // 1,2,3 = draw the 3rd in the back
                {
                    z2 = 2 * 32 * 32; // offset for current map
                    for (int y = 0; y < map_height; y++)
                    {
                        for (int x = 0; x < 32; x++)
                        {
                            offset = z2 + (y * 32) + x; // offset to current tile on the map
                            temp_tile = ((Maps.tile[offset] + 0x400) * 8 * 8); // base offset for tile                                                               // the 2bpp uses the 5th set of 256 tiles
                            temp_pal = (Maps.palette[offset] * 4); // beginning of this palette

                            big_sub16(offset, x, y, temp_tile, temp_pal);
                        }
                    }
                }
                // now draw the 2nd
                z2 = 1 * 32 * 32; // offset for current map
                for (int y = 0; y < map_height; y++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        offset = z2 + (y * 32) + x; // offset to current tile on the map

                        temp_tile = (Maps.tile[offset] * 8 * 8); // base offset for tile
                        temp_pal = (Maps.palette[offset] * 16); // beginning of this palette
                        big_sub16(offset, x, y, temp_tile, temp_pal);
                    }
                }
                // now draw the 1st
                z2 = 0; // offset for current map
                for (int y = 0; y < map_height; y++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        offset = z2 + (y * 32) + x; // offset to current tile on the map

                        temp_tile = (Maps.tile[offset] * 8 * 8); // base offset for tile
                        temp_pal = (Maps.palette[offset] * 16); // beginning of this palette
                        big_sub16(offset, x, y, temp_tile, temp_pal);
                    }
                }
                if (map_view == 4) // 3,1,2 = draw the 3rd in the front
                {
                    z2 = 2 * 32 * 32; // offset for current map
                    for (int y = 0; y < map_height; y++)
                    {
                        for (int x = 0; x < 32; x++)
                        {
                            offset = z2 + (y * 32) + x; // offset to current tile on the map
                            temp_tile = ((Maps.tile[offset] + 0x400) * 8 * 8); // base offset for tile                                                               // the 2bpp uses the 5th set of 256 tiles
                            temp_pal = (Maps.palette[offset] * 4); // beginning of this palette

                            big_sub16(offset, x, y, temp_tile, temp_pal);
                        }
                    }
                }
            }



            else // map views 0 or 1 or 2, draw one map 4bpp or 2bpp
            {
                int z = map_view * 32 * 32; // offset for current map
                for (int y = 0; y < map_height; y++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        offset = z + (y * 32) + x; // offset to current tile on the map

                        if (map_view == 2) // 2bpp
                        {
                            temp_tile = ((Maps.tile[offset] + 0x400) * 8 * 8); // base offset for tile
                            // the 2bpp uses the 5th set of 256 tiles
                            temp_pal = (Maps.palette[offset] * 4); // beginning of this palette
                        }
                        else // 4bpp
                        {
                            temp_tile = (Maps.tile[offset] * 8 * 8); // base offset for tile
                            temp_pal = (Maps.palette[offset] * 16); // beginning of this palette
                        }
                        big_sub16(offset, x, y, temp_tile, temp_pal);
                    }
                }
            }

            // draw a checkerboard pattern over the unused portion of the map
            if (map_height < 32)
            {
                Color checker_color = Color.SlateGray;
                for (int y = map_height * 16; y < 512; y++)
                {
                    for (int x = 0; x < 512; x++)
                    {
                        if ((y & 8) == 0)
                        {
                            if ((x & 8) == 0)
                            {
                                checker_color = Color.SlateGray;
                            }
                            else
                            {
                                checker_color = Color.LightSlateGray;
                            }
                        }
                        else
                        {
                            if ((x & 8) == 0)
                            {
                                checker_color = Color.LightSlateGray;
                            }
                            else
                            {
                                checker_color = Color.SlateGray;
                            }
                        }

                        temp_bmp2.SetPixel(x, y, checker_color);
                    }
                }
            }

            //draw a box around the active tile
            if (map_view < 3)
            {


                //draw grid here
                if (checkBox4.Checked == true)
                {
                    //draw horizontal lines at each 16
                    for (int i = 31; i < (map_height * 15); i += 32)
                    {
                        for (int j = 0; j < 510; j += 2)
                        {
                            temp_bmp2.SetPixel(j, i, Color.Black);
                            temp_bmp2.SetPixel(j + 1, i, Color.White);
                        }
                    }
                    //draw vertical lines at each 16
                    for (int j = 31; j < 511; j += 32)
                    {
                        for (int i = 0; i < (map_height * 16) - 2; i += 2)
                        {
                            temp_bmp2.SetPixel(j, i + 1, Color.Black);
                            temp_bmp2.SetPixel(j, i, Color.White);
                        }
                    }
                }
                // draw box around current selection
                int x2 = (active_map_x * 16);
                //if (x2 >= 496) x2 = 495;
                if (active_map_y >= map_height) active_map_y = map_height - 1;
                int y2 = (active_map_y * 16);
                //if (y2 >= 496) y2 = 495;
                for (int i = 0; i < 16; i++)
                {
                    temp_bmp2.SetPixel(x2 + i, y2, Color.White);
                    temp_bmp2.SetPixel(x2, y2 + i, Color.White);
                    temp_bmp2.SetPixel(x2 + i, y2 + 15, Color.White);
                    temp_bmp2.SetPixel(x2 + 15, y2 + i, Color.White);
                }
            }

            if(zoom_Q_flag == true)
            {
                // copy a quadrant to cool bmp, then double that to temp again
                int start_x = 0;
                int start_y = 0;
                if(active_map_y < 16)
                {
                    if(active_map_x >= 16)
                    {
                        start_x = 256;
                    }
                }
                else
                {
                    start_y = 256;
                    if (active_map_x >= 16)
                    {
                        start_x = 256;
                    }
                }

                Color temp_color = Color.Black;

                // copy the quadrant to a temp bmp
                for(int y = 0; y < 256; y++)
                {
                    int y2 = y + start_y;
                    for(int x = 0; x < 256; x++)
                    {
                        int x2 = x + start_x;
                        temp_color = temp_bmp2.GetPixel(x2, y2);
                        cool_bmp.SetPixel(x, y, temp_color);
                    }
                }

                //resize double size back to temp_bmp2
                using (Graphics g2 = Graphics.FromImage(temp_bmp2))
                {
                    g2.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g2.PixelOffsetMode = PixelOffsetMode.Half; // fix bug, missing half a pixel on top and left
                    g2.DrawImage(cool_bmp, 0, 0, 512, 512);
                } // standard resize of bmp was blurry, this makes it sharp
            }

            pictureBox1.Image = temp_bmp2;
            pictureBox1.Refresh();
        }
        // END UPDATE TILEMAP 16x16


        public void update_tilemap() // the big box on the left, 32x32
        {
            if(tilesize == TILE_8X8)
            {
                update_tilemap8x8();
            }
            else // 16x16
            {
                update_tilemap16x16();
            }
        }
        // END UPDATE TILEMAP




        //for drawing the tile map
        private void big_sub(int offset, int x, int y, int temp_tile, int temp_pal)
        { // 8x8 tiles
            int color = 0;
            int temp_h = Maps.h_flip[offset];
            int temp_v = Maps.v_flip[offset];
            if (temp_h == 0) // plain, h not flipped
            {
                if (temp_v == 0) // plain, v not flipped
                {
                    int index = temp_tile;
                    int x8 = (x * 8);
                    int y8 = (y * 8);
                    for (int tile_y = 0; tile_y < 8; tile_y++)
                    {
                        for (int tile_x = 0; tile_x < 8; tile_x++)
                        {
                            int test_color = Tiles.Tile_Arrays[index++];
                            if (test_color == 0) continue;
                            color = temp_pal + test_color; //Tiles.Tile_Arrays[index++];
                                                           //if (color == 0) continue;
                            image_map_local.SetPixel(x8 + tile_x, y8 + tile_y,
                                    Color.FromArgb(Palettes.pal_r[color], Palettes.pal_g[color], Palettes.pal_b[color]));
                        }
                    }
                }
                else // v flipped
                {
                    int index = temp_tile;
                    int x8 = (x * 8);
                    int y8 = (y * 8);
                    for (int tile_y = 0; tile_y < 8; tile_y++)
                    {
                        for (int tile_x = 0; tile_x < 8; tile_x++)
                        {
                            int test_color = Tiles.Tile_Arrays[index++];
                            if (test_color == 0) continue;
                            color = temp_pal + test_color; //Tiles.Tile_Arrays[index++];
                                                           //if (color == 0) continue;
                            image_map_local.SetPixel(x8 + tile_x, y8 + (7 - tile_y),
                                    Color.FromArgb(Palettes.pal_r[color], Palettes.pal_g[color], Palettes.pal_b[color]));
                        }
                    }
                }
            }
            else // h flipped
            {
                if (temp_v == 0) // just h
                {
                    int index = temp_tile;
                    int x8 = (x * 8);
                    int y8 = (y * 8);
                    for (int tile_y = 0; tile_y < 8; tile_y++)
                    {
                        for (int tile_x = 0; tile_x < 8; tile_x++)
                        {
                            int test_color = Tiles.Tile_Arrays[index++];
                            if (test_color == 0) continue;
                            color = temp_pal + test_color; //Tiles.Tile_Arrays[index++];
                                                           //if (color == 0) continue;
                            image_map_local.SetPixel(x8 + (7 - tile_x), y8 + tile_y,
                                    Color.FromArgb(Palettes.pal_r[color], Palettes.pal_g[color], Palettes.pal_b[color]));
                        }
                    }
                }
                else // both flipped
                {
                    int index = temp_tile;
                    int x8 = (x * 8);
                    int y8 = (y * 8);
                    for (int tile_y = 0; tile_y < 8; tile_y++)
                    {
                        for (int tile_x = 0; tile_x < 8; tile_x++)
                        {
                            int test_color = Tiles.Tile_Arrays[index++];
                            if (test_color == 0) continue;
                            color = temp_pal + test_color; //Tiles.Tile_Arrays[index++];
                                                           //if (color == 0) continue;
                            image_map_local.SetPixel(x8 + (7 - tile_x), y8 + (7 - tile_y),
                                    Color.FromArgb(Palettes.pal_r[color], Palettes.pal_g[color], Palettes.pal_b[color]));
                        }
                    }
                }
            }
        }
        // END TILEMAP SUB 8x8 tiles






        private void big_sub16(int offset, int x, int y, int temp_tile, int temp_pal)
        { // 16x16 tiles
            // there is no wrapping, except from 3ff back to 000
            // tile, tile+1, tile+16, tile+17
            // multiply 64, to get the appropriate offset in the array
            // 0x3ff -> 0xffc0

            int remember_high_bit = temp_tile & 0x10000;
            int temp_tile2 = (temp_tile + 64) & 0xffc0;
            temp_tile2 += remember_high_bit;
            int temp_tile3 = (temp_tile + 1024) & 0xffc0;
            temp_tile3 += remember_high_bit;
            int temp_tile4 = (temp_tile + 1088) & 0xffc0;
            temp_tile4 += remember_high_bit;


            int color = 0;
            int temp_h = Maps.h_flip[offset];
            int temp_v = Maps.v_flip[offset];
            if (temp_h == 0) // plain, h not flipped
            {
                if (temp_v == 0) // plain, v not flipped
                {
                    int index, x16, y16;
                    for (int do4 = 0; do4 < 4; do4++)
                    {
                        switch (do4)
                        {
                            case 0:
                            default:
                                index = temp_tile;
                                x16 = (x * 16);
                                y16 = (y * 16);
                                break;
                            case 1:
                                index = temp_tile2;
                                x16 = (x * 16) + 8;
                                y16 = (y * 16);
                                break;
                            case 2:
                                index = temp_tile3;
                                x16 = (x * 16);
                                y16 = (y * 16) + 8;
                                break;
                            case 3:
                                index = temp_tile4;
                                x16 = (x * 16) + 8;
                                y16 = (y * 16) + 8;
                                break;
                        }
                        
                        for (int tile_y = 0; tile_y < 8; tile_y++)
                        {
                            for (int tile_x = 0; tile_x < 8; tile_x++)
                            {
                                int test_color = Tiles.Tile_Arrays[index++];
                                if (test_color == 0) continue;
                                color = temp_pal + test_color; 
                                temp_bmp2.SetPixel(x16 + tile_x, y16 + tile_y,
                                        Color.FromArgb(Palettes.pal_r[color], Palettes.pal_g[color], Palettes.pal_b[color]));
                            }
                        }
                    }
                }
                else // v flipped
                {
                    int index, x16, y16;
                    for (int do4 = 0; do4 < 4; do4++)
                    {
                        switch (do4)
                        {
                            case 0:
                            default:
                                index = temp_tile3;
                                x16 = (x * 16);
                                y16 = (y * 16);
                                break;
                            case 1:
                                index = temp_tile4;
                                x16 = (x * 16) + 8;
                                y16 = (y * 16);
                                break;
                            case 2:
                                index = temp_tile;
                                x16 = (x * 16);
                                y16 = (y * 16) + 8;
                                break;
                            case 3:
                                index = temp_tile2;
                                x16 = (x * 16) + 8;
                                y16 = (y * 16) + 8;
                                break;
                        }

                        for (int tile_y = 0; tile_y < 8; tile_y++)
                        {
                            for (int tile_x = 0; tile_x < 8; tile_x++)
                            {
                                int test_color = Tiles.Tile_Arrays[index++];
                                if (test_color == 0) continue;
                                color = temp_pal + test_color; 
                                temp_bmp2.SetPixel(x16 + tile_x, y16 + (7 - tile_y),
                                        Color.FromArgb(Palettes.pal_r[color], Palettes.pal_g[color], Palettes.pal_b[color]));
                            }
                        }
                    }  
                }
            }
            else // h flipped
            {
                if (temp_v == 0) // just h
                {
                    int index, x16, y16;
                    for (int do4 = 0; do4 < 4; do4++)
                    {
                        switch (do4)
                        {
                            case 0:
                            default:
                                index = temp_tile2;
                                x16 = (x * 16);
                                y16 = (y * 16);
                                break;
                            case 1:
                                index = temp_tile;
                                x16 = (x * 16) + 8;
                                y16 = (y * 16);
                                break;
                            case 2:
                                index = temp_tile4;
                                x16 = (x * 16);
                                y16 = (y * 16) + 8;
                                break;
                            case 3:
                                index = temp_tile3;
                                x16 = (x * 16) + 8;
                                y16 = (y * 16) + 8;
                                break;
                        }

                        for (int tile_y = 0; tile_y < 8; tile_y++)
                        {
                            for (int tile_x = 0; tile_x < 8; tile_x++)
                            {
                                int test_color = Tiles.Tile_Arrays[index++];
                                if (test_color == 0) continue;
                                color = temp_pal + test_color; 
                                temp_bmp2.SetPixel(x16 + (7 - tile_x), y16 + tile_y,
                                        Color.FromArgb(Palettes.pal_r[color], Palettes.pal_g[color], Palettes.pal_b[color]));
                            }
                        }
                    }
                        
                }
                else // both flipped
                {
                    int index, x16, y16;
                    for (int do4 = 0; do4 < 4; do4++)
                    {
                        switch (do4)
                        {
                            case 0:
                            default:
                                index = temp_tile4;
                                x16 = (x * 16);
                                y16 = (y * 16);
                                break;
                            case 1:
                                index = temp_tile3;
                                x16 = (x * 16) + 8;
                                y16 = (y * 16);
                                break;
                            case 2:
                                index = temp_tile2;
                                x16 = (x * 16);
                                y16 = (y * 16) + 8;
                                break;
                            case 3:
                                index = temp_tile;
                                x16 = (x * 16) + 8;
                                y16 = (y * 16) + 8;
                                break;
                        }

                        for (int tile_y = 0; tile_y < 8; tile_y++)
                        {
                            for (int tile_x = 0; tile_x < 8; tile_x++)
                            {
                                int test_color = Tiles.Tile_Arrays[index++];
                                if (test_color == 0) continue;
                                color = temp_pal + test_color; 
                                temp_bmp2.SetPixel(x16 + (7 - tile_x), y16 + (7 - tile_y),
                                        Color.FromArgb(Palettes.pal_r[color], Palettes.pal_g[color], Palettes.pal_b[color]));
                            }
                        }
                    }
                }
            }
        }
        // END TILEMAP SUB 16x16 tiles






        private int check_num(string str) // make sure string is number
        {
            int value = 0;

            int.TryParse(str, out value);
            if (value > 255) value = 255; // max value
            if (value < 0) value = 0; // min value
            value = value & 0xf8;
            
            return (value);
        }



        public void rebuild_pal_boxes()
        {
            int selection = pal_x + (pal_y * 16);

            int red = Palettes.pal_r[selection];
            textBox1.Text = red.ToString();
            trackBar1.Value = red / 8;

            int green = Palettes.pal_g[selection];
            textBox2.Text = green.ToString();
            trackBar2.Value = green / 8;

            int blue = Palettes.pal_b[selection];
            textBox3.Text = blue.ToString();
            trackBar3.Value = blue / 8;

            update_box4();
        }



        private void update_box4() // when boxes 1,2,or 3 changed
        {
            int value_red, value_green, value_blue;
            int sum;
            int selection = get_selection();

            value_red = Palettes.pal_r[selection];
            value_green = Palettes.pal_g[selection];
            value_blue = Palettes.pal_b[selection];


            sum = ((value_red & 0xf8) >> 3) + ((value_green & 0xf8) << 2) + ((value_blue & 0xf8) << 7);
            string hexValue = sum.ToString("X");
            // may have to append zeros to beginning


            if (hexValue.Length == 3) hexValue = String.Concat("0", hexValue);
            else if (hexValue.Length == 2) hexValue = String.Concat("00", hexValue);
            else if (hexValue.Length == 1) hexValue = String.Concat("000", hexValue);
            else if (hexValue.Length == 0) hexValue = "0000";

            textBox4.Text = hexValue;
        }



        private bool is_hex(char ch1)
        {
            if ((ch1 >= '0') && (ch1 <= '9')) return true;
            if ((ch1 >= 'A') && (ch1 <= 'F')) return true;
            //should be upper case letters
            return false;
        }



        private string check_hex(string str) //str.Length should be exacly 4
        {
            if ((!is_hex(str[0])) ||
                (!is_hex(str[1])) ||
                (!is_hex(str[2])) ||
                (!is_hex(str[3])))
            {
                //something isn't a hex string
                return "Z";
            }

            //make sure the high byte is 0-7
            if (str[0] > '7')
            {
                char[] letters = str.ToCharArray();
                char letter;
                switch (letters[0])
                {
                    case 'F':
                        letter = '7'; break;
                    case 'E':
                        letter = '6'; break;
                    case 'D':
                        letter = '5'; break;
                    case 'C':
                        letter = '4'; break;
                    case 'B':
                        letter = '3'; break;
                    case 'A':
                        letter = '2'; break;
                    case '9':
                        letter = '1'; break;
                    case '8':
                    default:
                        letter = '0'; break;
                }
                letters[0] = letter;
                return string.Join("", letters);
            }
            return str;
        }



        private int hex_val(char chr) // convert single hex digit to int value
        {
            switch (chr)
            {
                case 'F':
                    return 15;
                case 'E':
                    return 14;
                case 'D':
                    return 13;
                case 'C':
                    return 12;
                case 'B':
                    return 11;
                case 'A':
                    return 10;
                case '9':
                    return 9;
                case '8':
                    return 8;
                case '7':
                    return 7;
                case '6':
                    return 6;
                case '5':
                    return 5;
                case '4':
                    return 4;
                case '3':
                    return 3;
                case '2':
                    return 2;
                case '1':
                    return 1;
                case '0':
                default:
                    return 0;
            }
        }



        private int get_selection()
        {
            int selection = pal_x + (pal_y * 16);
            if ((map_view == 2) && ((pal_x & 3) == 0)) selection = 0; // 2bpp
            if (pal_x == 0) selection = 0;
            return selection;
        }



        private void update_rgb() // when r g or b boxes change
        {
            string str = textBox1.Text;
            int value = check_num(str);
            textBox1.Text = value.ToString();
            trackBar1.Value = value / 8;

            int selection = get_selection();
            Palettes.pal_r[selection] = (byte)value;

            str = textBox2.Text;
            value = check_num(str);
            textBox2.Text = value.ToString();
            trackBar2.Value = value / 8;

            Palettes.pal_g[selection] = (byte)value;

            str = textBox3.Text;
            value = check_num(str);
            textBox3.Text = value.ToString();
            trackBar3.Value = value / 8;

            Palettes.pal_b[selection] = (byte)value;
        }



        private void textBox1_KeyPress(object sender, KeyPressEventArgs e) //Red
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                update_rgb();
                update_box4();

                update_palette();

                common_update2();

                e.Handled = true; // prevent ding on return press
            }
        }



        private void textBox2_KeyPress(object sender, KeyPressEventArgs e) //Green
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                update_rgb();
                update_box4();

                update_palette();

                common_update2();

                e.Handled = true; // prevent ding on return press
            }
        }



        private void textBox3_KeyPress(object sender, KeyPressEventArgs e) //Blue
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                update_rgb();
                update_box4();

                update_palette();

                common_update2();

                e.Handled = true; // prevent ding on return press
            }
        }



        private void textBox4_KeyPress(object sender, KeyPressEventArgs e) //Hex
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                string str = textBox4.Text;
                str = str.Trim(); // remove spaces
                int[] value = new int[4];
                int temp;

                if (str.Length < 4)
                {
                    str = str.PadLeft(4, '0');
                }

                if (str.Length != 4) return;
                str = str.ToUpper();
                str = check_hex(str); //returns "Z" if fail
                if (str == "Z") return;

                textBox4.Text = str;

                value[0] = hex_val(str[0]); //get int value, 0-15
                value[1] = hex_val(str[1]);
                value[2] = hex_val(str[2]);
                value[3] = hex_val(str[3]);

                //pass values to the other boxes
                temp = ((value[3] & 0x0f) << 3) + ((value[2] & 0x01) << 7); // red, 5 bits
                textBox1.Text = temp.ToString();
                temp = ((value[2] & 0x0e) << 2) + ((value[1] & 0x03) << 6); // green, 5 bits
                textBox2.Text = temp.ToString();
                temp = ((value[1] & 0x0c) << 1) + ((value[0] & 0x07) << 5); // blue, 5 bits
                textBox3.Text = temp.ToString();

                update_rgb();
                update_palette();
                common_update2();

                e.Handled = true; // prevent ding on return press
            }
        }



        private string hex_char(int value)
        {
            switch (value)
            {
                case 15:
                    return "F";
                case 14:
                    return "E";
                case 13:
                    return "D";
                case 12:
                    return "C";
                case 11:
                    return "B";
                case 10:
                    return "A";
                case 9:
                    return "9";
                case 8:
                    return "8";
                case 7:
                    return "7";
                case 6:
                    return "6";
                case 5:
                    return "5";
                case 4:
                    return "4";
                case 3:
                    return "3";
                case 2:
                    return "2";
                case 1:
                    return "1";
                case 0:
                default:
                    return "0";
            }
        }



        public void update_tile_image() // redraw the visible tileset
        {
            Color temp_color;
            int temp_tile_num = 0;
            for (int i = 0; i < 16; i++) //tile row = y
            {
                for (int j = 0; j < 16; j++) //tile column = x
                {
                    temp_tile_num = (i * 16) + j;
                    for (int k = 0; k < 8; k++) // pixel row = y
                    {
                        for (int m = 0; m < 8; m++) // pixel column = x
                        {
                            int color = 0;
                            int index = (Form1.tile_set * 256 * 8 * 8) + (temp_tile_num * 8 * 8) + (k * 8) + m;
                            int pal_index = Tiles.Tile_Arrays[index]; // pixel in tile array
                            if (Form1.map_view == 2) // 2bpp
                            {
                                pal_index = pal_index & 0x03; //sanitize, for my sanity
                                if (pal_index == 0)
                                {
                                    color = 0; // 0th color
                                }
                                else
                                {
                                    color = (pal_y * 16) + (pal_x & 0x0c) + pal_index;
                                }
                            }
                            else // 4bpp
                            {
                                pal_index = pal_index & 0x0f; //sanitize, for my sanity
                                color = (pal_y * 16) + pal_index;
                            }

                            temp_color = Color.FromArgb(Palettes.pal_r[color], Palettes.pal_g[color], Palettes.pal_b[color]);
                            image_tiles.SetPixel((j * 8) + m, (i * 8) + k, temp_color);
                        }
                    }
                }
            }

            //Bitmap temp_bmp = new Bitmap(256, 256); //resize double size
            using (Graphics g = Graphics.FromImage(temp_bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half; // fix bug, missing half a pixel on top and left
                g.DrawImage(image_tiles, 0, 0, 256, 256);
            } // standard resize of bmp was blurry, this makes it sharp

            //put a white box around the selected tile
            int pos_x = 0; int pos_y = 0;
            if(tilesize == TILE_8X8)
            {
                for (int i = 0; i < 16; i++)
                {
                    pos_y = (tile_y * 16) - 1; // it's doing a weird off by 1 thing
                    if (pos_y < 0) pos_y = 0; // so have to adjust by 1, and not == -1
                    pos_x = (tile_x * 16) - 1;
                    if (pos_x < 0) pos_x = 0;
                    temp_bmp.SetPixel(pos_x + i, pos_y, Color.White);
                    temp_bmp.SetPixel(pos_x, pos_y + i, Color.White);
                    temp_bmp.SetPixel(pos_x + i, pos_y + 15, Color.White);
                    temp_bmp.SetPixel(pos_x + 15, pos_y + i, Color.White);
                }
            }
            else // 16x16 tiles, draw a bigger box
            {
                for (int i = 0; i < 32; i++)
                {
                    pos_y = (tile_y * 16) - 1; // it's doing a weird off by 1 thing
                    if (pos_y < 0) pos_y = 0; // so have to adjust by 1, and not == -1
                    pos_x = (tile_x * 16) - 1;
                    if (pos_x < 0) pos_x = 0;
                    if((pos_x + i) < 256)
                    {
                        temp_bmp.SetPixel(pos_x + i, pos_y, Color.White); // top
                    }
                    if ((pos_y + i) < 256)
                    {
                        temp_bmp.SetPixel(pos_x, pos_y + i, Color.White); // left
                    }
                    if (((pos_x + i) < 256) && ((pos_y + 31) < 256))
                    {
                        temp_bmp.SetPixel(pos_x + i, pos_y + 31, Color.White); // right
                    }
                    if (((pos_x + 31) < 256) && ((pos_y + i) < 256))
                    {
                        temp_bmp.SetPixel(pos_x + 31, pos_y + i, Color.White); // bottom
                    }
                    
                }
            }
            


            pictureBox2.Image = temp_bmp;
            pictureBox2.Refresh();
            
        }
        // END REDRAW TILESET



        public void tile_show_num() // top right, above tileset
        {
            string str = "";
            int dec_num = (tile_y * 16) + tile_x + ((tile_set & 3) * 256);
            str = hex_char(tile_y) + hex_char(tile_x) + "   " + dec_num.ToString();
            label9.Text = str;
        }



        private void pictureBox2_Click(object sender, EventArgs e)
        { // tiles
            if (map_view > 2)
            {
                MessageBox.Show("Editing is disabled in Preview Mode.");
                return;
            }

            //change the label to tile number, in hex
            tile_x = 0; tile_y = 0; tile_num = 0; //globals

            var mouseEventArgs = e as MouseEventArgs;
            if (mouseEventArgs != null)
            {

                tile_x = mouseEventArgs.X >> 4;
                tile_y = mouseEventArgs.Y >> 4;
            }
            if (tile_x < 0) tile_x = 0;
            if (tile_y < 0) tile_y = 0;
            if (tile_x > 15) tile_x = 15;
            if (tile_y > 15) tile_y = 15;
            tile_num = (tile_y * 16) + tile_x;
            tile_show_num();

            //last
            if (newChild != null)
            {
                newChild.BringToFront();
                newChild.update_tile_box();
            }
            else
            {
                newChild = new Form2();
                newChild.Owner = this;
                int xx = Screen.PrimaryScreen.Bounds.Width;
                if (this.Location.X + 970 < xx) // set new form location
                {
                    newChild.Location = new Point(this.Location.X + 800, this.Location.Y + 80);
                }
                else
                {
                    newChild.Location = new Point(xx - 170, this.Location.Y);
                }

                newChild.Show();
                //update
            }

            update_tile_image();
            label5.Focus();
        } // END CLICKED ON TILES




        // this was to speed up map changes, so we don't have to draw the
        // entire map every click
        private void picbox1_sub() // place a tile on the map
        {
            // apply the tile now
            int temp_y, temp_x, start_x, loop_x, loop_y;
            int z = map_view * 32 * 32;
            int offset, temp_tile, temp_pal;
            int next_count = 0;
            int[] next_tiles = new int[5]; // actually 4

            // which tile is selected on right.
            int temp_set = tile_set & 3; //0-3
            int tile_num2 = tile_x + (tile_y * 16) + (256 * temp_set); // 0-1023

            if (brushsize == BRUSH1x1)
            {
                start_x = temp_x = active_map_x;
                temp_y = active_map_y;
                loop_x = 1;
                loop_y = 1;
            }
            else if (brushsize == BRUSH3x3)
            {
                start_x = temp_x = active_map_x - 1;
                temp_y = active_map_y - 1;
                loop_x = 3;
                loop_y = 3;
            }
            else if (brushsize == BRUSH5x5)
            {
                start_x = temp_x = active_map_x - 2;
                temp_y = active_map_y - 2;
                loop_x = 5;
                loop_y = 5;
            }
            else if (brushsize == BRUSHNEXT) // pseudo 16x16
            {
                start_x = temp_x = active_map_x;
                temp_y = active_map_y;
                loop_x = 2;
                loop_y = 2;
                if (checkBox5.Checked == false) // h flip no
                {
                    if (checkBox6.Checked == false) // v flip no
                    {
                        next_tiles[0] = tile_num2;
                        next_tiles[1] = (tile_num2 + 1) & 0x3ff;
                        next_tiles[2] = (tile_num2 + 16) & 0x3ff;
                        next_tiles[3] = (tile_num2 + 17) & 0x3ff;
                    }
                    else // v flip yes
                    {
                        next_tiles[0] = (tile_num2 + 16) & 0x3ff;
                        next_tiles[1] = (tile_num2 + 17) & 0x3ff;
                        next_tiles[2] = tile_num2;
                        next_tiles[3] = (tile_num2 + 1) & 0x3ff;
                    }
                }
                else // h flip yes
                {
                    if (checkBox6.Checked == false) // v flip no
                    {
                        next_tiles[0] = (tile_num2 + 1) & 0x3ff;
                        next_tiles[1] = tile_num2;
                        next_tiles[2] = (tile_num2 + 17) & 0x3ff;
                        next_tiles[3] = (tile_num2 + 16) & 0x3ff;
                    }
                    else // v flip yes, both flipped
                    {
                        next_tiles[0] = (tile_num2 + 17) & 0x3ff;
                        next_tiles[1] = (tile_num2 + 16) & 0x3ff;
                        next_tiles[2] = (tile_num2 + 1) & 0x3ff;
                        next_tiles[3] = tile_num2;
                    }
                }

                tile_num2 = next_tiles[0];
            }
            else if ((brushsize == BRUSH_CLONE_T) || (brushsize == BRUSH_CLONE_M))
            {

                loop_x = 1;
                loop_y = 1;

                if (brushsize == BRUSH_CLONE_T)
                { // clone from tileset
                    if(tilesize == TILE_16X16)
                    {
                        tile_x = tile_x & 0xfe; // force even
                        tile_y = tile_y & 0xfe;
                        tile_show_num(); // update number
                    }

                    // get distance in tiles 
                    temp_x = (active_map_x - clone_start_x) + tile_x;
                    if (tilesize == TILE_16X16)
                    {
                        temp_x = ((active_map_x - clone_start_x) * 2) + tile_x;
                    }
                    if ((temp_x < 0) || (temp_x > 15)) return;

                    temp_y = (active_map_y - clone_start_y) + tile_y;
                    if (tilesize == TILE_16X16) temp_y *= 2;
                    if ((temp_y < 0) || (temp_y > 15)) return;

                    tile_num2 = temp_x + (temp_y * 16) + (256 * temp_set); // 0-1023

                    start_x = temp_x = active_map_x;
                    temp_y = active_map_y;
                }
                else // clone from map
                {
                    int temp_x2, temp_y2, active_map_index2;

                    temp_x2 = (active_map_x - clone_start_x) + map_clone_x;
                    if ((temp_x2 < 0) || (temp_x2 > 31)) return;

                    temp_y2 = (active_map_y - clone_start_y) + map_clone_y;
                    if ((temp_y2 < 0) || (temp_y2 >= map_height)) return;
                    
                    temp_x = active_map_x;
                    temp_y = active_map_y;


                    active_map_index = temp_x + (temp_y * 32) + (32 * 32 * map_view);
                    active_map_index2 = temp_x2 + (temp_y2 * 32) + (32 * 32 * map_view);
                    Maps.tile[active_map_index] = Maps.tile[active_map_index2];
                    Maps.palette[active_map_index] = Maps.palette[active_map_index2];
                    Maps.h_flip[active_map_index] = Maps.h_flip[active_map_index2];
                    Maps.v_flip[active_map_index] = Maps.v_flip[active_map_index2];
                    Maps.priority[active_map_index] = Maps.priority[active_map_index2];



                    // draw 1 tile, but first fill with zero color
                    int temp_x3 = temp_x * 8;
                    int temp_y3 = temp_y * 8;
                    Color zero_color = Color.FromArgb(Palettes.pal_r[0], Palettes.pal_g[0], Palettes.pal_b[0]);

                    if(tilesize == TILE_8X8)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                image_map_local.SetPixel(temp_x3 + j, temp_y3 + i, zero_color);
                            }
                        }
                    }
                    else // 16x16
                    {
                        temp_x3 *= 2;
                        temp_y3 *= 2;
                        for (int i = 0; i < 16; i++)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                temp_bmp2.SetPixel(temp_x3 + j, temp_y3 + i, zero_color);
                            }
                        }
                    }

                    // temp_x and temp_y
                    //int z = map_view * 32 * 32; //above
                    offset = z + (temp_y * 32) + temp_x;
                    if (map_view == 2) // 2bpp
                    {
                        temp_tile = ((Maps.tile[offset] + 0x400) * 8 * 8); // base offset for tile
                                                                           // the 2bpp uses the 5th set of 256 tiles
                        temp_pal = (Maps.palette[offset] * 4); // beginning of this palette
                    }
                    else // 4bpp
                    {
                        temp_tile = (Maps.tile[offset] * 8 * 8); // base offset for tile
                        temp_pal = (Maps.palette[offset] * 16); // beginning of this palette
                    }

                    if(tilesize == TILE_8X8)
                    {
                        big_sub(offset, temp_x, temp_y, temp_tile, temp_pal);
                    }
                    else // 16x16
                    {
                        big_sub16(offset, temp_x, temp_y, temp_tile, temp_pal);
                    }
                    

                    if(tilesize == TILE_8X8)
                    {
                        //Bitmap temp_bmp2 = new Bitmap(512, 512); //resize double size
                        using (Graphics g2 = Graphics.FromImage(temp_bmp2))
                        {
                            g2.InterpolationMode = InterpolationMode.NearestNeighbor;
                            g2.PixelOffsetMode = PixelOffsetMode.Half; // fix bug, missing..
                                                                       // half a pixel on  top and left
                            g2.DrawImage(image_map_local, 0, 0, 512, 512);
                        } // standard resize of bmp was blurry, this makes it sharp
                    }
                    

                    //draw grid here
                    if (checkBox4.Checked == true)
                    {
                        //draw horizontal lines at each 16
                        for (int i = 31; i < (map_height * 15); i += 32)
                        {
                            for (int j = 0; j < 510; j += 2)
                            {
                                temp_bmp2.SetPixel(j, i, Color.Black);
                                temp_bmp2.SetPixel(j + 1, i, Color.White);
                            }
                        }
                        //draw vertical lines at each 16
                        for (int j = 31; j < 511; j += 32)
                        {
                            for (int i = 0; i < (map_height * 16) - 2; i += 2)
                            {
                                temp_bmp2.SetPixel(j, i + 1, Color.Black);
                                temp_bmp2.SetPixel(j, i, Color.White);
                            }
                        }
                    }

                    pictureBox1.Image = temp_bmp2;
                    pictureBox1.Refresh();

                    return; // must return when done.
                }
            }
            else // brush = fill screen
            {
                start_x = temp_x = 0;
                temp_y = 0;
                loop_x = 32;
                loop_y = map_height;
            }





            // nested loop of tile changes, per brush size.
            for (int yy = 0; yy < loop_y; yy++)
            {
                for (int xx = 0; xx < loop_x; xx++)
                {
                    //tile change temp_y < map_height
                    if ((temp_y >= 0) && (temp_x >= 0) &&
                        (temp_y < map_height) && (temp_x < 32))
                    {
                        active_map_index = temp_x + (temp_y * 32) + (32 * 32 * map_view);

                        // always apply the palette
                        int temp_val = 0;
                        if (map_view == 2) // 2bpp mode
                        {
                            temp_val = (pal_y * 4) + (pal_x >> 2); // 2bpp mode
                            Maps.palette[active_map_index] = temp_val;
                            textBox5.Text = temp_val.ToString();
                        }
                        else
                        {
                            Maps.palette[active_map_index] = pal_y; // 4bpp mode
                            textBox5.Text = pal_y.ToString();
                        }

                        if (checkBox7.Checked == false) // apply only palette = false
                        {
                            Maps.tile[active_map_index] = tile_num2;
                            if (checkBox5.Checked == false)
                            {
                                Maps.h_flip[active_map_index] = 0;
                                checkBox1.Checked = false;
                            }
                            else {
                                Maps.h_flip[active_map_index] = 1;
                                checkBox1.Checked = true;
                            }
                            if (checkBox6.Checked == false)
                            {
                                Maps.v_flip[active_map_index] = 0;
                                checkBox2.Checked = false;
                            }
                            else
                            {
                                Maps.v_flip[active_map_index] = 1;
                                checkBox2.Checked = true;
                            }
                            //Maps.priority[active_map_index] = 0; // not used
                        }


                        // draw 1 tile, but first fill with zero color
                        int temp_x2 = temp_x * 8;
                        int temp_y2 = temp_y * 8;
                        Color zero_color = Color.FromArgb(Palettes.pal_r[0], Palettes.pal_g[0], Palettes.pal_b[0]);

                        if(tilesize == TILE_8X8)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    image_map_local.SetPixel(temp_x2 + j, temp_y2 + i, zero_color);
                                }
                            }
                        }
                        else // 16x16
                        {
                            temp_x2 *= 2;
                            temp_y2 *= 2;
                            for (int i = 0; i < 16; i++)
                            {
                                for (int j = 0; j < 16; j++)
                                {
                                    temp_bmp2.SetPixel(temp_x2 + j, temp_y2 + i, zero_color);
                                }
                            }
                        }
                        

                        // temp_x and temp_y
                        //int z = map_view * 32 * 32; //above
                        offset = z + (temp_y * 32) + temp_x;
                        if (map_view == 2) // 2bpp
                        {
                            temp_tile = ((Maps.tile[offset] + 0x400) * 8 * 8); // base offset for tile
                            // the 2bpp uses the 5th set of 256 tiles
                            temp_pal = (Maps.palette[offset] * 4); // beginning of this palette
                        }
                        else // 4bpp
                        {
                            temp_tile = (Maps.tile[offset] * 8 * 8); // base offset for tile
                            temp_pal = (Maps.palette[offset] * 16); // beginning of this palette
                        }

                        if(tilesize == TILE_8X8)
                        {
                            big_sub(offset, temp_x, temp_y, temp_tile, temp_pal);
                        }
                        else // 16x16
                        {
                            big_sub16(offset, temp_x, temp_y, temp_tile, temp_pal);
                        }
                        


                    } // end of tile change 

                    if (brushsize == BRUSHNEXT)
                    {
                        
                        next_count++;
                        tile_num2 = next_tiles[next_count];
                    }
                    temp_x++;
                }

                temp_x = start_x;
                temp_y++;
            }

            if(tilesize == TILE_8X8)
            {
                //Bitmap temp_bmp2 = new Bitmap(512, 512); //resize double size
                using (Graphics g2 = Graphics.FromImage(temp_bmp2))
                {
                    g2.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g2.PixelOffsetMode = PixelOffsetMode.Half; // fix bug, missing..
                                                               // half a pixel on  top and left
                    g2.DrawImage(image_map_local, 0, 0, 512, 512);
                } // standard resize of bmp was blurry, this makes it sharp
            }
            

            //draw grid here
            if (checkBox4.Checked == true)
            {
                //draw horizontal lines at each 16
                for (int i = 31; i < (map_height * 15); i += 32)
                {
                    for (int j = 0; j < 510; j += 2)
                    {
                        temp_bmp2.SetPixel(j, i, Color.Black);
                        temp_bmp2.SetPixel(j + 1, i, Color.White);
                    }
                }
                //draw vertical lines at each 16
                for (int j = 31; j < 511; j += 32)
                {
                    for (int i = 0; i < (map_height * 16) - 2; i += 2)
                    {
                        temp_bmp2.SetPixel(j, i + 1, Color.Black);
                        temp_bmp2.SetPixel(j, i, Color.White);
                    }
                }
            }

            pictureBox1.Image = temp_bmp2;
            pictureBox1.Refresh();


            //update_tilemap(); // moved
        }


        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (disable_map_click == 1) // click dialog boxes were causing mouse clicks on map
            {
                disable_map_click = 0;
                return;
            }

            if (brushsize == BRUSH_CLONE_M)
            {
                var mouseEventArgs = e as MouseEventArgs;
                if (mouseEventArgs == null) return;
                if (e.Button == MouseButtons.Left)
                {
                    active_map_x = map_clone_x;
                    active_map_y = map_clone_y;
                    label12.Text = "X = " + map_clone_x.ToString(); // change the numbers at the top
                    label13.Text = "Y = " + map_clone_y.ToString();
                    update_palette();
                    common_update2();
                }
            }

            //update_tilemap();
            common_update2();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        { // tilemap
            disable_map_click = 0;
            zoom_Q_flag = false;

            if (map_view > 2)
            {
                MessageBox.Show("Editing is disabled in Preview Mode.");
                return;
            }

            //Checkpoint();

            active_map_x = 0; active_map_y = 0;
            var mouseEventArgs = e as MouseEventArgs;
            if (mouseEventArgs != null)
            {
                active_map_x = mouseEventArgs.X >> 4;
                active_map_y = mouseEventArgs.Y >> 4;
            }
            if (active_map_x < 0) active_map_x = 0;
            if (active_map_x > 31) active_map_x = 31;
            if (active_map_y < 0) active_map_y = 0;
            if (active_map_y >= map_height)
            {
                active_map_y = map_height - 1;
                return;
            }
            label12.Text = "X = " + active_map_x.ToString(); // change the numbers at the top
            label13.Text = "Y = " + active_map_y.ToString();

            if (e.Button == MouseButtons.Left)
            {
                Checkpoint(); // only if left click

                clone_start_x = active_map_x;
                clone_start_y = active_map_y;


                last_tile_x = active_map_x; // to speed up the app
                last_tile_y = active_map_y; // see mouse move event
                picbox1_sub(); // place the tile and redraw the map
                //update_tilemap();
            }
            else if (e.Button == MouseButtons.Right) // get the tile, tileset, and properties
            {
                map_clone_x = active_map_x;
                map_clone_y = active_map_y;

                int tile = (map_view * 32 * 32) + (32 * active_map_y) + active_map_x;
                int pal = Maps.palette[tile];
                textBox5.Text = pal.ToString();
                if (Maps.h_flip[tile] == 0) checkBox1.Checked = false;
                else checkBox1.Checked = true;
                if (Maps.v_flip[tile] == 0) checkBox2.Checked = false;
                else checkBox2.Checked = true;
                int set = (Maps.tile[tile] & 0x300) >> 8;
                int tile2 = Maps.tile[tile] & 0xff;
                tile_x = tile2 & 0x0f;
                tile_y = (tile2 >> 4) & 0x0f;
                tile_num = (tile_y * 16) + tile_x;
                tile_show_num();

                set14bppToolStripMenuItem.Checked = false; // set them all to false
                set24bppToolStripMenuItem.Checked = false;
                set34bppToolStripMenuItem.Checked = false;
                set44bppToolStripMenuItem.Checked = false;
                set52bppToolStripMenuItem.Checked = false;
                set62bppToolStripMenuItem.Checked = false;
                set72bppToolStripMenuItem.Checked = false;
                set82bppToolStripMenuItem.Checked = false;

                if (map_view < 2) // the 4bpp maps
                {
                    pal_y = pal;

                    if (set == 0)
                    {
                        label10.Text = "1";
                        tile_set = 0;
                        set14bppToolStripMenuItem.Checked = true;
                    }
                    else if (set == 1)
                    {
                        label10.Text = "2";
                        tile_set = 1;
                        set24bppToolStripMenuItem.Checked = true;
                    }
                    else if (set == 2)
                    {
                        label10.Text = "3";
                        tile_set = 2;
                        set34bppToolStripMenuItem.Checked = true;
                    }
                    else
                    {
                        label10.Text = "4";
                        tile_set = 3;
                        set44bppToolStripMenuItem.Checked = true;
                    }
                }
                else //2bpp
                {
                    pal_y = pal;

                    if (set == 0)
                    {
                        label10.Text = "5";
                        tile_set = 4;
                        set52bppToolStripMenuItem.Checked = true;
                    }
                    else if (set == 1)
                    {
                        label10.Text = "6";
                        tile_set = 5;
                        set62bppToolStripMenuItem.Checked = true;
                    }
                    else if (set == 2)
                    {
                        label10.Text = "7";
                        tile_set = 6;
                        set72bppToolStripMenuItem.Checked = true;
                    }
                    else
                    {
                        label10.Text = "8";
                        tile_set = 7;
                        set82bppToolStripMenuItem.Checked = true;
                    }

                    // was...
                    pal_y = (pal >> 2);
                    pal_x = (pal & 3) << 2;
                }

                update_palette();
                common_update2();
            }

        } // end tilemap

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        { // TILEMAP

        } // END CLICKED ON TILEMAP



        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (map_view > 2) return;
            if (disable_map_click == 1) return;

            if (e.Button == MouseButtons.Left)
            {
                active_map_x = 0; active_map_y = 0;
                var mouseEventArgs = e as MouseEventArgs;
                if (mouseEventArgs != null)
                {

                    active_map_x = mouseEventArgs.X >> 4;
                    active_map_y = mouseEventArgs.Y >> 4;
                }
                if (active_map_x < 0) active_map_x = 0;
                if (active_map_x > 31) active_map_x = 31;
                if (active_map_y < 0) active_map_y = 0;
                if (active_map_y >= map_height)
                {
                    active_map_y = map_height - 1;
                    return;
                }
                label12.Text = "X = " + active_map_x.ToString();
                label13.Text = "Y = " + active_map_y.ToString();

                if ((last_tile_x != active_map_x) || (last_tile_y != active_map_y))
                {
                    // only update if the tile under mouse has changed.
                    last_tile_x = active_map_x;
                    last_tile_y = active_map_y;
                    picbox1_sub();
                    //update_tilemap();
                }

            }
        }
        // END MOUSE DOWN MOVE ON TILEMAP




        //capure key presses on the tiles, focus is redirected to label 5
        private void label5_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            int selection = pal_x + (pal_y * 16);

            if (e.KeyCode == Keys.Left)
            {
                Checkpoint();
                e.IsInputKey = true;
                Tiles.shift_left();
            }
            else if (e.KeyCode == Keys.Up)
            {
                Checkpoint();
                e.IsInputKey = true;
                Tiles.shift_up();
            }
            else if (e.KeyCode == Keys.Right)
            {
                Checkpoint();
                e.IsInputKey = true;
                Tiles.shift_right();
            }
            else if (e.KeyCode == Keys.Down)
            {
                Checkpoint();
                e.IsInputKey = true;
                Tiles.shift_down();
            }

            else if (e.KeyCode == Keys.NumPad2)
            {
                if (tile_y < 15) tile_y++;
                tile_num = (tile_y * 16) + tile_x;
            }
            else if (e.KeyCode == Keys.NumPad4)
            {
                if (tile_x > 0) tile_x--;
                tile_num = (tile_y * 16) + tile_x;
            }
            else if (e.KeyCode == Keys.NumPad6)
            {
                if (tile_x < 15) tile_x++;
                tile_num = (tile_y * 16) + tile_x;
            }
            else if (e.KeyCode == Keys.NumPad8)
            {
                if (tile_y > 0) tile_y--;
                tile_num = (tile_y * 16) + tile_x;
            }
            else if (e.KeyCode == Keys.H)
            {
                Checkpoint();
                Tiles.tile_h_flip();
            }
            else if (e.KeyCode == Keys.V)
            {
                Checkpoint();
                Tiles.tile_v_flip();
            }
            else if (e.KeyCode == Keys.R)
            {
                Checkpoint();
                Tiles.tile_rot_cw();
            }
            else if (e.KeyCode == Keys.L)
            {
                Checkpoint();
                Tiles.tile_rot_ccw();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                Checkpoint();
                Tiles.tile_delete();
            }
            else if (e.KeyCode == Keys.C)
            {
                Tiles.tile_copy();
            }
            else if (e.KeyCode == Keys.P)
            {
                Checkpoint();
                Tiles.tile_paste();
            }
            else if (e.KeyCode == Keys.F)
            {
                Checkpoint();
                Tiles.tile_fill();
            }

            else if (e.KeyCode == Keys.Q)
            { // palette copy selected color
                pal_r_copy = Palettes.pal_r[selection];
                pal_g_copy = Palettes.pal_g[selection];
                pal_b_copy = Palettes.pal_b[selection];
            }
            else if (e.KeyCode == Keys.W)
            { // palette paste selected to color
                Palettes.pal_r[selection] = (byte)pal_r_copy;
                Palettes.pal_g[selection] = (byte)pal_g_copy;
                Palettes.pal_b[selection] = (byte)pal_b_copy;
                update_palette();
                rebuild_pal_boxes();
            }
            else if (e.KeyCode == Keys.E)
            { // palette clear selected to color
                Palettes.pal_r[selection] = 0;
                Palettes.pal_g[selection] = 0;
                Palettes.pal_b[selection] = 0;
                update_palette();
                rebuild_pal_boxes();
            }

            else if (e.KeyCode == Keys.D1) // number buttons
            {
                set1_change(); // change the tileset
            }
            else if (e.KeyCode == Keys.D2)
            {
                set2_change();
            }
            else if (e.KeyCode == Keys.D3)
            {
                set3_change();
            }
            else if (e.KeyCode == Keys.D4)
            {
                set4_change();
            }
            else if (e.KeyCode == Keys.D5)
            {
                set5_change();
            }
            else if (e.KeyCode == Keys.D6)
            {
                set6_change();
            }
            else if (e.KeyCode == Keys.D7)
            {
                set7_change();
            }
            else if (e.KeyCode == Keys.D8)
            {
                set8_change();
            }

            else if (e.KeyCode == Keys.Z)
            {
                Do_Undo();
            }

            common_update2();
            // prevent change in focus
            label5.Focus();
        }







        private void common_update2()
        {
            if (newChild != null)
            {
                newChild.update_tile_box();
            }

            update_tile_image();
            update_tilemap();
        }

        private void x1ToolStripMenuItem_Click(object sender, EventArgs e)
        { // brush size
            brushsize = BRUSH1x1;
            x1ToolStripMenuItem.Checked = true;
            x3ToolStripMenuItem.Checked = false;
            x5ToolStripMenuItem.Checked = false;
            x2NextToolStripMenuItem.Checked = false;
            cloneFromTilesetToolStripMenuItem.Checked = false;
            cloneFromMapToolStripMenuItem.Checked = false;
            fillScreenToolStripMenuItem.Checked = false;
        }

        private void x3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            brushsize = BRUSH3x3;
            x1ToolStripMenuItem.Checked = false;
            x3ToolStripMenuItem.Checked = true;
            x5ToolStripMenuItem.Checked = false;
            x2NextToolStripMenuItem.Checked = false;
            cloneFromTilesetToolStripMenuItem.Checked = false;
            cloneFromMapToolStripMenuItem.Checked = false;
            fillScreenToolStripMenuItem.Checked = false;
        }

        private void x5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            brushsize = BRUSH5x5;
            x1ToolStripMenuItem.Checked = false;
            x3ToolStripMenuItem.Checked = false;
            x5ToolStripMenuItem.Checked = true;
            x2NextToolStripMenuItem.Checked = false;
            cloneFromTilesetToolStripMenuItem.Checked = false;
            cloneFromMapToolStripMenuItem.Checked = false;
            fillScreenToolStripMenuItem.Checked = false;
        }

        // this should be in menuclicks.cs
        private void fillTopRowWithColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Checkpoint();

            int save_num = tile_num;
            int save_pal = pal_x;
            tile_num = 0;
            pal_x = 0;
            for (int a = 0; a < 16; a++)
            {
                Tiles.tile_fill();
                tile_num++;
                pal_x++;
            }

            tile_num = save_num;
            pal_x = save_pal;
            common_update2();
        }

        private void trackBar1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int val = trackBar1.Value * 8;
                textBox1.Text = val.ToString();

                update_rgb();
                update_box4();

                update_palette();

                common_update2();
            }
        }

        private void trackBar2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int val = trackBar2.Value * 8;
                textBox2.Text = val.ToString();

                update_rgb();
                update_box4();

                update_palette();

                common_update2();
            }
        }

        private void trackBar3_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int val = trackBar3.Value * 8;
                textBox3.Text = val.ToString();

                update_rgb();
                update_box4();

                update_palette();

                common_update2();
            }
        }

        private void fillScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //BRUSH_FILL
            brushsize = BRUSH_FILL;
            x1ToolStripMenuItem.Checked = false;
            x3ToolStripMenuItem.Checked = false;
            x5ToolStripMenuItem.Checked = false;
            x2NextToolStripMenuItem.Checked = false;
            cloneFromTilesetToolStripMenuItem.Checked = false;
            cloneFromMapToolStripMenuItem.Checked = false;
            fillScreenToolStripMenuItem.Checked = true;
        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            label5.Focus();
        }

        private void trackBar2_MouseUp(object sender, MouseEventArgs e)
        {
            label5.Focus();
        }

        private void removeDuplicateTilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Checkpoint();

            RemoveDuplicateTiles();
            common_update2();
        }

        public void RemoveDuplicateTiles()
        {
            if(tilesize == TILE_8X8)
            {
                RemoveDuplicateTiles8x8();
            }
            else
            {
                RemoveDuplicateTiles16x16();
            }
        }


        public void RemoveDuplicateTiles8x8()
        {
            // this checks each tileset 1-4 / 5-8 and removes duplicates
            // this goes through each map and reorders the tile #
            // also flipped versions of a tile are removed

            // first the 4bpp tilesets (1-4)
            int tile_so_far = 1;
            for (int tile = 1; tile < 1024; tile++) // higher val
            {
                // compare to all previous tiles
                bool match_found = false;
                int match_index = 0;
                for (int tile2 = 0; tile2 < tile_so_far; tile2++) // lower val
                {
                    match_found = Compare2Tiles(tile, tile2, 0);
                    if (match_found == true)
                    {
                        match_index = tile2; // the lower val, keeper
                        break;
                    }
                }

                if (match_found == true)
                {
                    DeleteTile(tile, 0);
                    // search map1 for the bad tile, replace with good
                    ReplaceTile(tile, match_index, 0); // bad, good, start offset
                    // search map2
                    ReplaceTile(tile, match_index, 1024);
                }
                else // no match found
                {
                    // shift the tile down, maybe
                    if (tile != tile_so_far)
                    {
                        ReplaceTile(tile, tile_so_far, 0); // bad, good, start offset
                        ReplaceTile(tile, tile_so_far, 1024); // search map2
                        CopyTile(tile, tile_so_far, 0); // bad, good, start offset
                        DeleteTile(tile, 0);
                    }

                    tile_so_far++;
                }
            }


            // now test tilesets 5-8 (2bpp)
            // first the 4bpp tilesets (1-4)
            tile_so_far = 1;
            for (int tile = 1; tile < 1024; tile++) // higher val
            {
                // compare to all previous tiles
                bool match_found = false;
                int match_index = 0;
                for (int tile2 = 0; tile2 < tile_so_far; tile2++) // lower val
                {
                    match_found = Compare2Tiles(tile, tile2, 65536);
                    if (match_found == true)
                    {
                        match_index = tile2; // the lower val, keeper
                        break;
                    }
                }

                if (match_found == true)
                {
                    DeleteTile(tile, 65536);
                    // search map3
                    ReplaceTile(tile, match_index, 2048);  // bad, good, start offset
                }
                else // no match found
                {
                    // shift the tile down, maybe
                    if (tile != tile_so_far)
                    {
                        // search map3
                        ReplaceTile(tile, tile_so_far, 2048);  // bad, good, start offset
                        CopyTile(tile, tile_so_far, 65536); // bad, good, start offset
                        DeleteTile(tile, 65536);
                    }

                    tile_so_far++;
                }
            }

            // redraw everything (tiles and map)
            //common_update2(); // handled elsewhere
        }


        public bool Compare2Tiles(int tile, int tile2, int tileset)
        { // tilset should be either 0 or 65536 (for 4bpp vs 2bpp tiles)
            // also check for flipped version of the tile

            int offset1 = tileset + (tile * 64);
            int offset2 = tileset + (tile2 * 64);
            flip_h = false;
            flip_v = false;
            bool same = true;

            for (int i = 0; i < 64; i++)
            { // 64 pixels per tile
                if (Tiles.Tile_Arrays[offset1] != Tiles.Tile_Arrays[offset2])
                {
                    same = false;
                    break;
                }

                offset1++;
                offset2++;
            }
            if (same == true) return true;

            // check H flip version
            same = true;
            offset1 = tileset + (tile * 64);
            offset2 = tileset + (tile2 * 64);
            
            for (int i = 0; i < 64; i++)
            { // 64 pixels per tile
                if (Tiles.Tile_Arrays[offset1] != Tiles.Tile_Arrays[offset2 + H_FLIP_TABLE[i]])
                {
                    same = false;
                    break;
                }

                offset1++;
            }
            if (same == true)
            {
                flip_h = true;
                return true;
            }

            // check V flip version
            same = true;
            offset1 = tileset + (tile * 64);
            offset2 = tileset + (tile2 * 64);
            
            for (int i = 0; i < 64; i++)
            { // 64 pixels per tile
                if (Tiles.Tile_Arrays[offset1] != Tiles.Tile_Arrays[offset2 + V_FLIP_TABLE[i]])
                {
                    same = false;
                    break;
                }

                offset1++;
            }
            if (same == true)
            {
                flip_v = true;
                return true;
            }

            // check HV flip version
            same = true;
            offset1 = tileset + (tile * 64);
            offset2 = tileset + (tile2 * 64);
            
            for (int i = 0; i < 64; i++)
            { // 64 pixels per tile
                if (Tiles.Tile_Arrays[offset1] != Tiles.Tile_Arrays[offset2 + HV_FLIP_TABLE[i]])
                {
                    same = false;
                    break;
                }

                offset1++;
            }
            if (same == true)
            {
                flip_h = true;
                flip_v = true;
                return true;
            }

            return false;
        }

        public void DeleteTile(int tile, int tileset)
        { // tilset should be either 0 or 65536 (for 4bpp vs 2bpp tiles)
            int offset3 = tileset + (tile * 64);
            for (int i = 0; i < 64; i++)
            {
                Tiles.Tile_Arrays[offset3++] = 0;
            }
        }


        public void CopyTile(int tile, int tile2, int tileset) // high, low, start offset
        { // shift the tile down 
            // tilset should be either 0 or 65536 (for 4bpp vs 2bpp tiles)
            int offset4 = tileset + (tile * 64);
            int offset5 = tileset + (tile2 * 64);
            for (int i = 0; i < 64; i++)
            {
                Tiles.Tile_Arrays[offset5] = Tiles.Tile_Arrays[offset4];
                offset4++;
                offset5++;
            }
        }


        public void ReplaceTile(int bad_tile, int good_tile, int map_offset)
        {
            // checks a map for a tile (to be replaced)
            // mapset should be 0, 1024, or 2048 (map1, map2, map3)
            for (int i = 0; i < 1024; i++)
            {
                if (Maps.tile[map_offset] == bad_tile)
                {
                    Maps.tile[map_offset] = good_tile;
                    if (flip_h == true)
                    {
                        Maps.h_flip[map_offset] = Maps.h_flip[map_offset] ^ 1;
                        // bitwise XOR, 1 -> 0, 0 -> 1
                    }
                    if (flip_v == true)
                    {
                        Maps.v_flip[map_offset] = Maps.v_flip[map_offset] ^ 1;
                        // bitwise XOR, 1 -> 0, 0 -> 1
                    }

                }
                map_offset++;
            }
        }




        public void Force16x16onMaps()
        {
            for(int i = 0; i < 3072; i++)
            {
                Maps.tile[i] = Maps.tile[i] & 0x3ee;
                // disallow odd x and y tiles.
            }
        }


        // made 16x16 versions of the remove duplicates functions


        public void RemoveDuplicateTiles16x16()
        { // for 16x16 mode
            // this checks each tileset 1-4 / 5-8 and removes duplicates
            // this goes through each map and reorders the tile #
            // also flipped versions of a tile are removed


            // force all tiles to 16x16 offsets
            Force16x16onMaps();


            // first the 4bpp tilesets (1-4)
            int tile_so_far = 2; // ??
            for (int tile = 2; tile < 1024; tile += 2) // higher val
            {
                if ((tile & 0x10) == 0x10) tile += 0x10;
                if (tile >= 1024) break;

                // compare to all previous tiles
                bool match_found = false;
                int match_index = 0;
                for (int tile2 = 0; tile2 < tile_so_far; tile2 += 2) // lower val
                {
                    if ((tile2 & 0x10) == 0x10) tile2 += 0x10;
                    if (tile2 >= tile_so_far) break;

                    match_found = Compare2Tiles16(tile, tile2, 0);
                    if (match_found == true)
                    {
                        match_index = tile2; // the lower val, keeper
                        break;
                    }
                }

                if (match_found == true)
                {
                    DeleteTile16(tile, 0);
                    // search map1 for the bad tile, replace with good
                    ReplaceTile16(tile, match_index, 0); // bad, good, start offset
                    // search map2
                    ReplaceTile16(tile, match_index, 1024);
                }
                else // no match found
                {
                    // shift the tile down, maybe
                    if (tile != tile_so_far)
                    {
                        ReplaceTile16(tile, tile_so_far, 0); // bad, good, start offset
                        ReplaceTile16(tile, tile_so_far, 1024); // search map2
                        CopyTile16(tile, tile_so_far, 0); // bad, good, start offset
                        DeleteTile16(tile, 0);
                    }

                    //tile_so_far++;
                    tile_so_far += 2;
                    if ((tile_so_far & 0x10) == 0x10) tile_so_far += 0x10;
                }
            }


            // now test tilesets 5-8 (2bpp)
            // first the 4bpp tilesets (1-4)
            tile_so_far = 2; // ??
            for (int tile = 2; tile < 1024; tile += 2) // higher val
            {
                if ((tile & 0x10) == 0x10) tile += 0x10;
                if (tile >= 1024) break;

                // compare to all previous tiles
                bool match_found = false;
                int match_index = 0;
                for (int tile2 = 0; tile2 < tile_so_far; tile2 += 2) // lower val
                {
                    if ((tile2 & 0x10) == 0x10) tile2 += 0x10;
                    if (tile2 >= tile_so_far) break;

                    match_found = Compare2Tiles16(tile, tile2, 65536);
                    if (match_found == true)
                    {
                        match_index = tile2; // the lower val, keeper
                        break;
                    }
                }

                if (match_found == true)
                {
                    DeleteTile16(tile, 65536);
                    // search map3
                    ReplaceTile16(tile, match_index, 2048);  // bad, good, start offset
                }
                else // no match found
                {
                    // shift the tile down, maybe
                    if (tile != tile_so_far)
                    {
                        // search map3
                        ReplaceTile16(tile, tile_so_far, 2048);  // bad, good, start offset
                        CopyTile16(tile, tile_so_far, 65536); // bad, good, start offset
                        DeleteTile16(tile, 65536);
                    }

                    //tile_so_far++;
                    tile_so_far += 2;
                    if ((tile_so_far & 0x10) == 0x10) tile_so_far += 0x10;
                }
            }

            // redraw everything (tiles and map)
            //common_update2(); // handled elsewhere
        }



        public bool Compare2Tiles16(int tile, int tile2, int tileset)
        { // tilset should be either 0 or 65536 (for 4bpp vs 2bpp tiles)
            // also check for flipped version of the tile
            // for 16x16 mode

            int tileB = tile + 1;
            int tileC = tile + 16; // there shouldn't be any wrapping past tileset
            int tileD = tile + 17;
            int tile2B = tile2 + 1;
            int tile2C = tile2 + 16;
            int tile2D = tile2 + 17;
            int offset1A = tileset + (tile * 64);
            int offset1B = tileset + (tileB * 64);
            int offset1C = tileset + (tileC * 64);
            int offset1D = tileset + (tileD * 64);
            int offset2A = tileset + (tile2 * 64);
            int offset2B = tileset + (tile2B * 64);
            int offset2C = tileset + (tile2C * 64);
            int offset2D = tileset + (tile2D * 64);
            flip_h = false;
            flip_v = false;
            bool same = true;

            // check unflipped
            
            for (int i = 0; i < 64; i++)
            { // 64 pixels per tile
                if (Tiles.Tile_Arrays[offset1A] != Tiles.Tile_Arrays[offset2A])
                {
                    same = false;
                    break;
                }
                if (Tiles.Tile_Arrays[offset1B] != Tiles.Tile_Arrays[offset2B])
                {
                    same = false;
                    break;
                }
                if (Tiles.Tile_Arrays[offset1C] != Tiles.Tile_Arrays[offset2C])
                {
                    same = false;
                    break;
                }
                if (Tiles.Tile_Arrays[offset1D] != Tiles.Tile_Arrays[offset2D])
                {
                    same = false;
                    break;
                }

                offset1A++;
                offset1B++;
                offset1C++;
                offset1D++;
                offset2A++;
                offset2B++;
                offset2C++;
                offset2D++;
            }
            if (same == true) return true;

            // check H flip version
            same = true;
            offset1A = tileset + (tile * 64);
            offset1B = tileset + (tileB * 64);
            offset1C = tileset + (tileC * 64);
            offset1D = tileset + (tileD * 64);
            offset2A = tileset + (tile2 * 64);
            offset2B = tileset + (tile2B * 64);
            offset2C = tileset + (tile2C * 64);
            offset2D = tileset + (tile2D * 64);

            for (int i = 0; i < 64; i++)
            { // 64 pixels per tile
                if (Tiles.Tile_Arrays[offset1A] != Tiles.Tile_Arrays[offset2B + H_FLIP_TABLE[i]])
                {
                    same = false;
                    break;
                }
                if (Tiles.Tile_Arrays[offset1B] != Tiles.Tile_Arrays[offset2A + H_FLIP_TABLE[i]])
                {
                    same = false;
                    break;
                }
                if (Tiles.Tile_Arrays[offset1C] != Tiles.Tile_Arrays[offset2D + H_FLIP_TABLE[i]])
                {
                    same = false;
                    break;
                }
                if (Tiles.Tile_Arrays[offset1D] != Tiles.Tile_Arrays[offset2C + H_FLIP_TABLE[i]])
                {
                    same = false;
                    break;
                }

                offset1A++;
                offset1B++;
                offset1C++;
                offset1D++;
            }
            if (same == true)
            {
                flip_h = true;
                return true;
            }

            // check V flip version
            same = true;
            offset1A = tileset + (tile * 64);
            offset1B = tileset + (tileB * 64);
            offset1C = tileset + (tileC * 64);
            offset1D = tileset + (tileD * 64);
            offset2A = tileset + (tile2 * 64);
            offset2B = tileset + (tile2B * 64);
            offset2C = tileset + (tile2C * 64);
            offset2D = tileset + (tile2D * 64);

            for (int i = 0; i < 64; i++)
            { // 64 pixels per tile
                if (Tiles.Tile_Arrays[offset1A] != Tiles.Tile_Arrays[offset2C + V_FLIP_TABLE[i]])
                {
                    same = false;
                    break;
                }
                if (Tiles.Tile_Arrays[offset1B] != Tiles.Tile_Arrays[offset2D + V_FLIP_TABLE[i]])
                {
                    same = false;
                    break;
                }
                if (Tiles.Tile_Arrays[offset1C] != Tiles.Tile_Arrays[offset2A + V_FLIP_TABLE[i]])
                {
                    same = false;
                    break;
                }
                if (Tiles.Tile_Arrays[offset1D] != Tiles.Tile_Arrays[offset2B + V_FLIP_TABLE[i]])
                {
                    same = false;
                    break;
                }

                offset1A++;
                offset1B++;
                offset1C++;
                offset1D++;
            }
            if (same == true)
            {
                flip_v = true;
                return true;
            }

            // check HV flip version
            same = true;
            offset1A = tileset + (tile * 64);
            offset1B = tileset + (tileB * 64);
            offset1C = tileset + (tileC * 64);
            offset1D = tileset + (tileD * 64);
            offset2A = tileset + (tile2 * 64);
            offset2B = tileset + (tile2B * 64);
            offset2C = tileset + (tile2C * 64);
            offset2D = tileset + (tile2D * 64);

            for (int i = 0; i < 64; i++)
            { // 64 pixels per tile
                if (Tiles.Tile_Arrays[offset1A] != Tiles.Tile_Arrays[offset2D + HV_FLIP_TABLE[i]])
                {
                    same = false;
                    break;
                }
                if (Tiles.Tile_Arrays[offset1B] != Tiles.Tile_Arrays[offset2C + HV_FLIP_TABLE[i]])
                {
                    same = false;
                    break;
                }
                if (Tiles.Tile_Arrays[offset1C] != Tiles.Tile_Arrays[offset2B + HV_FLIP_TABLE[i]])
                {
                    same = false;
                    break;
                }
                if (Tiles.Tile_Arrays[offset1D] != Tiles.Tile_Arrays[offset2A + HV_FLIP_TABLE[i]])
                {
                    same = false;
                    break;
                }

                offset1A++;
                offset1B++;
                offset1C++;
                offset1D++;
            }
            if (same == true)
            {
                flip_h = true;
                flip_v = true;
                return true;
            }

            return false;
        }

        public void DeleteTile16(int tile, int tileset)
        { // tilset should be either 0 or 65536 (for 4bpp vs 2bpp tiles)
            // for 16x16 mode
            int offset3 = tileset + (tile * 64);
            int offset3B = tileset + ((tile + 1) * 64);
            int offset3C = tileset + ((tile + 16) * 64);
            int offset3D = tileset + ((tile + 17) * 64);
            for (int i = 0; i < 64; i++)
            {
                Tiles.Tile_Arrays[offset3++] = 0;
                Tiles.Tile_Arrays[offset3B++] = 0;
                Tiles.Tile_Arrays[offset3C++] = 0;
                Tiles.Tile_Arrays[offset3D++] = 0;
            }
            
        }


        public void CopyTile16(int tile, int tile2, int tileset) // high, low, start offset
        { // shift the tile down 
            // for 16x16 mode
            // tilset should be either 0 or 65536 (for 4bpp vs 2bpp tiles)
            int offset4 = tileset + (tile * 64);
            int offset4B = tileset + ((tile + 1) * 64);
            int offset4C = tileset + ((tile + 16) * 64);
            int offset4D = tileset + ((tile + 17) * 64);
            int offset5 = tileset + (tile2 * 64);
            int offset5B = tileset + ((tile2 + 1) * 64);
            int offset5C = tileset + ((tile2 + 16) * 64);
            int offset5D = tileset + ((tile2 + 17) * 64);
            for (int i = 0; i < 64; i++)
            {
                Tiles.Tile_Arrays[offset5] = Tiles.Tile_Arrays[offset4];
                offset4++;
                offset5++;
                Tiles.Tile_Arrays[offset5B] = Tiles.Tile_Arrays[offset4B];
                offset4B++;
                offset5B++;
                Tiles.Tile_Arrays[offset5C] = Tiles.Tile_Arrays[offset4C];
                offset4C++;
                offset5C++;
                Tiles.Tile_Arrays[offset5D] = Tiles.Tile_Arrays[offset4D];
                offset4D++;
                offset5D++;
            }
        }


        public void ReplaceTile16(int bad_tile, int good_tile, int map_offset)
        { 
            // checks a map for a tile (to be replaced)
            // mapset should be 0, 1024, or 2048 (map1, map2, map3)
            // for 16x16 mode (no changes ??)
            for (int i = 0; i < 1024; i++)
            {
                if (Maps.tile[map_offset] == bad_tile)
                {
                    Maps.tile[map_offset] = good_tile;
                    if (flip_h == true)
                    {
                        Maps.h_flip[map_offset] = Maps.h_flip[map_offset] ^ 1;
                        // bitwise XOR, 1 -> 0, 0 -> 1
                    }
                    if (flip_v == true)
                    {
                        Maps.v_flip[map_offset] = Maps.v_flip[map_offset] ^ 1;
                        // bitwise XOR, 1 -> 0, 0 -> 1
                    }

                }
                map_offset++;
            }
        }




        private void checkBox5_Click(object sender, EventArgs e)
        {
            label5.Focus();
        }

        private void checkBox6_Click(object sender, EventArgs e)
        {
            label5.Focus();
        }

        private void checkBox7_Click(object sender, EventArgs e)
        {
            label5.Focus();
        }

        private void x8TilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            x8TilesToolStripMenuItem.Checked = true;
            x16TilesToolStripMenuItem.Checked = false;
            tilesize = TILE_8X8;
            label8.Text = "8x8";

            common_update2();
        }

        private void forceMapsToEvenValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Force16x16onMaps();
            common_update2();
            MessageBox.Show("done.");
        }

        private void zoomIntoQuadrantToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(tilesize != TILE_16X16)
            {
                MessageBox.Show("Select 16x16 First.");
                return;
            }
            if(map_view > 2)
            {
                MessageBox.Show("Select a single map BG View.");
                return;
            }
            zoom_Q_flag = true;

            update_tilemap();
        }

        private void x16TilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            x8TilesToolStripMenuItem.Checked = false;
            x16TilesToolStripMenuItem.Checked = true;
            tilesize = TILE_16X16;
            label8.Text = "16x16";

            if ( brushsize == BRUSHNEXT )
            {
                // disallow these in 16x16 tilesize mode
                // they don't work right
                brushsize = BRUSH1x1;
                x1ToolStripMenuItem.Checked = true;
                x3ToolStripMenuItem.Checked = false;
                x5ToolStripMenuItem.Checked = false;
                x2NextToolStripMenuItem.Checked = false;
                cloneFromTilesetToolStripMenuItem.Checked = false;
                cloneFromMapToolStripMenuItem.Checked = false;
                fillScreenToolStripMenuItem.Checked = false;
            }

            common_update2();
        }

        


        private void trackBar3_MouseUp(object sender, MouseEventArgs e)
        {
            label5.Focus();
        }

        private void getPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // generate a palette from the image
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "Image Files .png .jpg .bmp .gif)|*.png;*.jpg;*.bmp;*.gif|" + "All Files (*.*)|*.*";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Bitmap import_bmp = new Bitmap(dlg.FileName);

                    if ((import_bmp.Height < 8) || (import_bmp.Width < 8))
                    {
                        MessageBox.Show("Error. File too small?");
                        import_bmp.Dispose();
                        return;
                    }
                    if ((import_bmp.Height > 256) || (import_bmp.Width > 256))
                    {
                        MessageBox.Show("Error. File too large. 256x256 max.");
                        import_bmp.Dispose();
                        return;
                    }


                    int num_col_to_find, start_offset;

                    int RememberZeroR = Palettes.pal_r[0]; // if we use the zero color
                    int RememberZeroG = Palettes.pal_g[0];
                    int RememberZeroB = Palettes.pal_b[0];

                    if (map_view == 2) // 2bpp mode
                    {
                        num_col_to_find = 4;
                        start_offset = (pal_y * 16) + (pal_x & 0xfc);
                        if (start_offset >= 32) start_offset = 0;
                    }
                    else
                    {
                        num_col_to_find = 16;
                        start_offset = pal_y * 16;
                    }
                    

                    image_height = import_bmp.Height;
                    image_width = import_bmp.Width;
                    Color temp_color;
                    // copy the bitmap, crop but don't resize
                    // copy pixel by pixel
                    for (int xx = 0; xx < 256; xx++)
                    {
                        for (int yy = 0; yy < 256; yy++)
                        {
                            if ((xx < image_width) && (yy < image_height))
                            {
                                temp_color = import_bmp.GetPixel(xx, yy);
                            }
                            else
                            {
                                temp_color = Color.Gray;
                            }
                            cool_bmp.SetPixel(xx, yy, temp_color);
                        }
                    }



                    int color_found = 0;
                    int red = 0, blue = 0, green = 0;
                    int temp_var, closest_cnt, added;

                    // default colors

                    // blank the arrays
                    for (int i = 0; i < 65536; i++)
                    {
                        R_Array[i] = 0;
                        G_Array[i] = 0;
                        B_Array[i] = 0;
                        Count_Array[i] = 0;
                    }
                    color_count = 0;

                    Color tempcolor = Color.Black;

                    // read all possible colors from the orig image
                    // removing duplicates, keep track of how many
                    for (int yy = 0; yy < image_height; yy++)
                    {
                        for (int xx = 0; xx < image_width; xx++)
                        {
                            tempcolor = cool_bmp.GetPixel(xx, yy);
                            // speed it up, narrow the possibilities.
                            red = tempcolor.R & 0xf8;
                            blue = tempcolor.G & 0xf8;
                            green = tempcolor.B & 0xf8;
                            tempcolor = Color.FromArgb(red, blue, green);

                            // compare to all other colors, add if not present
                            if (color_count == 0)
                            {
                                Add_Color(tempcolor);
                                continue;
                            }

                            color_found = 0;
                            for (int i = 0; i < color_count; i++)
                            {
                                if ((tempcolor.R == R_Array[i] &&
                                    tempcolor.G == G_Array[i] &&
                                    tempcolor.B == B_Array[i]))
                                { // color match found
                                    Count_Array[i] = Count_Array[i] + 1;
                                    color_found = 1;
                                    break;
                                }
                            }
                            // no color match found
                            if (color_found == 0)
                            {
                                Add_Color(tempcolor);
                            }

                        }
                    }
                    //label5.Text = color_count.ToString(); // print, how many colors

                    // this mid point algorithm tends avoid extremes
                    // give extra weight to the lowest value and the highest value
                    // first find the darkest and lightest colors
                    int darkest = 999;
                    int darkest_index = 0;
                    int lightest = 0;
                    int lightest_index = 0;
                    for (int i = 0; i < color_count; i++)
                    {
                        added = R_Array[i] + G_Array[i] + B_Array[i];
                        if (added < darkest)
                        {
                            darkest = added;
                            darkest_index = i;
                        }
                        if (added > lightest)
                        {
                            lightest = added;
                            lightest_index = i;
                        }
                    }
                    // give more count to them
                    temp_var = image_width * image_height / 8; // 8 is magic
                    Count_Array[darkest_index] += temp_var;
                    Count_Array[lightest_index] += temp_var;

                    // then reduce to 4 colors, using a mid point merge with
                    // the closest neighbor color

                    int color_count2 = color_count;
                    while (color_count2 > num_col_to_find)
                    {
                        //find the least count
                        int least_index = 0;
                        int least_cnt = 99999;
                        for (int i = 0; i < color_count; i++)
                        {
                            if (Count_Array[i] == 0) continue;
                            if (Count_Array[i] < least_cnt)
                            {
                                least_cnt = Count_Array[i];
                                least_index = i;
                            }
                        }
                        // delete itself
                        Count_Array[least_index] = 0;

                        int closest_index = 0;
                        int closest_val = 999999;
                        r_val = R_Array[least_index];
                        g_val = G_Array[least_index];
                        b_val = B_Array[least_index];
                        int dR = 0, dG = 0, dB = 0;

                        // find the closest to that one
                        for (int i = 0; i < color_count; i++)
                        {
                            if (Count_Array[i] == 0) continue;
                            dR = r_val - R_Array[i];
                            dG = g_val - G_Array[i];
                            dB = b_val - B_Array[i];
                            diff_val = ((dR * dR) + (dG * dG) + (dB * dB));

                            if (diff_val < closest_val)
                            {
                                closest_val = diff_val;
                                closest_index = i;
                            }
                        }

                        closest_cnt = Count_Array[closest_index];

                        // merge closet index with least index, mid point
                        temp_var = (closest_cnt + least_cnt);
                        // the algorithm was (color1 + color2) / 2
                        // but now, multiplied each by their count, div by both counts
                        r_val = (R_Array[least_index] * least_cnt) + (R_Array[closest_index] * closest_cnt);
                        r_val = (int)Math.Round((double)r_val / temp_var);
                        g_val = (G_Array[least_index] * least_cnt) + (G_Array[closest_index] * closest_cnt);
                        g_val = (int)Math.Round((double)g_val / temp_var);
                        b_val = (B_Array[least_index] * least_cnt) + (B_Array[closest_index] * closest_cnt);
                        b_val = (int)Math.Round((double)b_val / temp_var);
                        R_Array[closest_index] = r_val;
                        G_Array[closest_index] = g_val;
                        B_Array[closest_index] = b_val;
                        Count_Array[closest_index] = closest_cnt + least_cnt;

                        color_count2--;

                    }

                    // always palette zero
                    // zero fill the palette, before filling (black)
                    for (int i = 0; i < num_col_to_find; i++)
                    {
                        int j = start_offset + i;
                        Palettes.pal_r[j] = 0;
                        Palettes.pal_g[j] = 0;
                        Palettes.pal_b[j] = 0;
                    }
                    // then go through the array and pull out 4 numbers (or 16)
                    int findindex = 0;
                    int color_count3 = 0;
                    while (color_count3 < color_count2)
                    {
                        if(Count_Array[findindex] != 0)
                        {
                            SixteenColorIndexes[color_count3] = findindex;
                            color_count3++;
                        }

                        findindex++;
                        if (findindex >= 65536) break;

                    }

                    // then sort by darkness
                    for(int i = 0; i < 16; i++) // zero them
                    {
                        SixteenColorsAdded[i] = 0;
                    }
                    for (int i = 0; i < color_count2; i++) // add them up (rough brightness)
                    {
                        SixteenColorsAdded[i] += R_Array[SixteenColorIndexes[i]];
                        SixteenColorsAdded[i] += G_Array[SixteenColorIndexes[i]];
                        SixteenColorsAdded[i] += B_Array[SixteenColorIndexes[i]];
                    }
                    int temp_val;
                    while (true)
                    {
                        bool sorted = true;
                        for (int i = 0; i < color_count2-1; i++) // add them up (rough brightness)
                        {
                            if(SixteenColorsAdded[i] > SixteenColorsAdded[i+1])
                            {
                                sorted = false;
                                // swap them
                                temp_val = SixteenColorsAdded[i];
                                SixteenColorsAdded[i] = SixteenColorsAdded[i + 1];
                                SixteenColorsAdded[i + 1] = temp_val;
                                temp_val = SixteenColorIndexes[i];
                                SixteenColorIndexes[i] = SixteenColorIndexes[i + 1];
                                SixteenColorIndexes[i + 1] = temp_val;
                            }
                        }
                        if (sorted == true) break;
                    }
                    

                    // then fill the palette with the colors
                    for (int i = 0; i < color_count2; i++)
                    {
                        int j = start_offset + i;
                        Palettes.pal_r[j] = (byte)(R_Array[SixteenColorIndexes[i]] & 0xf8);
                        Palettes.pal_g[j] = (byte)(G_Array[SixteenColorIndexes[i]] & 0xf8);
                        Palettes.pal_b[j] = (byte)(B_Array[SixteenColorIndexes[i]] & 0xf8);
                    }



                    // if checkbox to use the old zero color, put it in now
                    // review this. could be buggy.
                    if(f3_cb1 == true)
                    {
                        tempcolor = Color.FromArgb(RememberZeroR, RememberZeroG, RememberZeroB);
                        int remove_index = Best_Color(tempcolor, num_col_to_find, start_offset);
                        // we have 1 too many color, remove the one closest to the zero color
                        // from before... shuffle the lower colors upward 1 slot
                        for(int i = remove_index; i > 0; i--)
                        { 
                            int j = start_offset + i;
                            Palettes.pal_r[j] = Palettes.pal_r[j - 1];
                            Palettes.pal_g[j] = Palettes.pal_g[j - 1];
                            Palettes.pal_b[j] = Palettes.pal_b[j - 1];
                        }

                        // insert the zero color at the zero offset
                        Palettes.pal_r[start_offset] = (byte)RememberZeroR;
                        Palettes.pal_g[start_offset] = (byte)RememberZeroG;
                        Palettes.pal_b[start_offset] = (byte)RememberZeroB;
                    }
                    

                    // copy the 0th color to zero
                    Palettes.pal_r[0] = Palettes.pal_r[start_offset];
                    Palettes.pal_g[0] = Palettes.pal_g[start_offset];
                    Palettes.pal_b[0] = Palettes.pal_b[start_offset];
                    // then update the palette image
                    update_palette();

                    //update the boxes
                    rebuild_pal_boxes();

                    common_update2();
                    import_bmp.Dispose();
                }
            }
        }

        public void Add_Color(Color tempcolor)
        {
            R_Array[color_count] = tempcolor.R;
            G_Array[color_count] = tempcolor.G;
            B_Array[color_count] = tempcolor.B;
            Count_Array[color_count] = 1;

            color_count++;
        }

        private void imageToCHRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // import an image, generate CHR based on existing palette
            // also fill current map with it.
            if (map_view > 2)
            {
                MessageBox.Show("Select BG View 1,2, or 3.");
                return;
            }
            
            // load image, generate CHR from it
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "Image Files .png .jpg .bmp .gif)|*.png;*.jpg;*.bmp;*.gif|" + "All Files (*.*)|*.*";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    undo_ready = false;
                    //x8TilesToolStripMenuItem.Checked = true;
                    //x16TilesToolStripMenuItem.Checked = false;
                    //tilesize = TILE_8X8;

                    Bitmap import_bmp = new Bitmap(dlg.FileName);

                    if ((import_bmp.Height < 8) || (import_bmp.Width < 8))
                    {
                        MessageBox.Show("Error. File too small?");
                        import_bmp.Dispose();
                        return;
                    }
                    if ((import_bmp.Height > 256) || (import_bmp.Width > 256))
                    {
                        MessageBox.Show("Error. File too large. 256x256 max.");
                        import_bmp.Dispose();
                        return;
                    }

                    int num_col, start_offset;

                    if (map_view == 2) // 2bpp mode
                    {
                        num_col = 4;
                        start_offset = (pal_y * 16) + (pal_x & 0xfc);
                        if (start_offset >= 32) start_offset = 0;
                    }
                    else
                    {
                        num_col = 16;
                        start_offset = pal_y * 16;
                    }

                    // make sure color zero is correct
                    Palettes.pal_r[start_offset] = Palettes.pal_r[0];
                    Palettes.pal_g[start_offset] = Palettes.pal_g[0];
                    Palettes.pal_b[start_offset] = Palettes.pal_b[0];

                    image_height = import_bmp.Height;
                    image_width = import_bmp.Width;

                    Color temp_color;
                    // copy the bitmap, crop but don't resize
                    // copy pixel by pixel
                    for (int xx = 0; xx < 256; xx++)
                    {
                        for (int yy = 0; yy < 256; yy++)
                        {
                            if ((xx < image_width) && (yy < image_height))
                            {
                                temp_color = import_bmp.GetPixel(xx, yy);
                            }
                            else
                            {
                                temp_color = Color.Gray;
                            }
                            cool_bmp.SetPixel(xx, yy, temp_color);
                        }
                    }
                    

                    int final_y, final_x, best_index, chr_index, tile_num, pixel_num;
                    //int temp_set = 0;
                    int count = 0;

                    // get best color for each pixel
                    // copied to int array, needy_chr_array

                    if(map_view == 2) // 2bpp mode
                    {
                        dither_db = dither_factor / 10.0;
                    }
                    else // 4bpp mode
                    {
                        dither_db = dither_factor / 28.0;
                    }
                    
                    dither_adjust = (int)(dither_db * 32.0);
                    int red, green, blue, bayer_val;

                    for (int y = 0; y < 256; y++)
                    {
                        for (int x = 0; x < 256; x++)
                        {
                            if((x >= image_width) || (y >= image_height))
                            {
                                needy_chr_array[count] = 0;
                            }
                            else
                            {
                                // get the pixel and find its best color
                                temp_color = cool_bmp.GetPixel(x, y);

                                if(dither_factor != 0)
                                {
                                    // add dithering
                                    red = temp_color.R - dither_adjust; // keep it from lightening
                                    green = temp_color.G - dither_adjust;
                                    blue = temp_color.B - dither_adjust;
                                    bayer_val = BAYER_MATRIX[x % 8, y % 8];
                                    bayer_val = (int)((double)bayer_val * dither_db);
                                    red += bayer_val;
                                    red = Math.Max(0, red); // clamp min max
                                    red = Math.Min(255, red);
                                    green += bayer_val;
                                    green = Math.Max(0, green);
                                    green = Math.Min(255, green);
                                    blue += bayer_val;
                                    blue = Math.Max(0, blue);
                                    blue = Math.Min(255, blue);
                                    temp_color = Color.FromArgb(red, green, blue);
                                }

                                best_index = Best_Color(temp_color, num_col, start_offset);
                                needy_chr_array[count] = best_index;
                            }
                            
                            count++;
                        }
                    }

                    // copy image to CHR
                    // do in 128x128 segments so it looks pretty
                    // working through each tile, one at a time
                    // this is a bit of a mess. ugh.
                    // the tile system goes tile by tile, y*8 + x
                    tile_num = 0;
                    int tile_offset = 0;
                    if(map_view == 2)
                    {
                        tile_offset = 65536; // start of tileset 5
                    }

                    for (int segment_y = 0; segment_y < 256; segment_y += 128)
                    {
                        for (int segment_x = 0; segment_x < 256; segment_x += 128)
                        {
                            for (int y1 = 0; y1 < 128; y1 += 8) // 16 tiles of 8x8
                            {
                                for (int x1 = 0; x1 < 128; x1 += 8) // ditto
                                {
                                    for (int y2 = 0; y2 < 8; y2++) // 8 pixels tall
                                    {
                                        for (int x2 = 0; x2 < 8; x2++) // 8 pixels wide
                                        {
                                            final_x = segment_x + x1 + x2;
                                            final_y = segment_y + y1 + y2;
                                            
                                            pixel_num = (final_y * 256) + final_x;
                                            // 64 bytes per tile
                                            chr_index = (tile_num * 64) + (y2 * 8) + x2;
                                            chr_index += tile_offset;

                                            Tiles.Tile_Arrays[chr_index] = needy_chr_array[pixel_num];
                                        }
                                    }
                                    tile_num++;
                                }
                            }

                            //temp_set++;
                        }
                    }
                    
                    // fill the map with the palette # and the CHR values
                    // do we want layer 1,2, or 3 ?

                    int pal_sel = 0;
                    int map_offset = map_view * 1024;
                    if(map_view == 2) // 2bpp mode
                    {
                        pal_sel = (pal_y * 4) + (pal_x >> 2);
                    }
                    else // 4 bpp
                    {
                        pal_sel = pal_y;
                    }
                    
                    for(int i = 0; i < 1024; i++) // fill with palette #
                    {
                        Maps.palette[i + map_offset] = pal_sel;
                        Maps.h_flip[i + map_offset] = 0;
                        Maps.v_flip[i + map_offset] = 0;
                        Maps.priority[i + map_offset] = 0;
                    }
                    // each 128x128 block separately
                    tile_num = 0;
                    int map_offset2 = 0;

                    if(tilesize == TILE_8X8)
                    {
                        for (int y = 0; y < 16; y++)
                        {
                            for (int x = 0; x < 16; x++)
                            {
                                map_offset2 = (y * 32) + x + map_offset;
                                Maps.tile[map_offset2] = tile_num++;
                                
                            }
                        }

                        for (int y = 0; y < 16; y++)
                        {
                            for (int x = 16; x < 32; x++)
                            {
                                map_offset2 = (y * 32) + x + map_offset;
                                Maps.tile[map_offset2] = tile_num++;
                            }
                        }
                        for (int y = 16; y < 32; y++)
                        {
                            for (int x = 0; x < 16; x++)
                            {
                                map_offset2 = (y * 32) + x + map_offset;
                                Maps.tile[map_offset2] = tile_num++;
                            }
                        }
                        for (int y = 16; y < 32; y++)
                        {
                            for (int x = 16; x < 32; x++)
                            {
                                map_offset2 = (y * 32) + x + map_offset;
                                Maps.tile[map_offset2] = tile_num++;
                            }
                        }
                    }
                    else
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            for (int x = 0; x < 8; x++)
                            {
                                map_offset2 = (y * 32) + x + map_offset;
                                Maps.tile[map_offset2] = tile_num;
                                tile_num += 2;
                                if((tile_num & 0x10) == 0x10) tile_num += 0x10;
                            }
                        }

                        for (int y = 0; y < 8; y++)
                        {
                            for (int x = 8; x < 16; x++)
                            {
                                map_offset2 = (y * 32) + x + map_offset;
                                Maps.tile[map_offset2] = tile_num;
                                tile_num += 2;
                                if ((tile_num & 0x10) == 0x10) tile_num += 0x10;
                            }
                        }
                        for (int y = 8; y < 16; y++)
                        {
                            for (int x = 0; x < 8; x++)
                            {
                                map_offset2 = (y * 32) + x + map_offset;
                                Maps.tile[map_offset2] = tile_num;
                                tile_num += 2;
                                if ((tile_num & 0x10) == 0x10) tile_num += 0x10;
                            }
                        }
                        for (int y = 8; y < 16; y++)
                        {
                            for (int x = 8; x < 16; x++)
                            {
                                map_offset2 = (y * 32) + x + map_offset;
                                Maps.tile[map_offset2] = tile_num;
                                tile_num += 2;
                                if ((tile_num & 0x10) == 0x10) tile_num += 0x10;
                            }
                        }
                    }
                    

                    // change map height and box
                    image_height = (image_height + 7) & 0x1f8;
                    if (image_height > 256) image_height = 256;
                    if (image_height < 8) image_height = 8;
                    
                    map_height = image_height / 8; //1-32
                    if(tilesize == TILE_16X16)
                    {
                        map_height /= 2;

                        // blank the unused part of the map, ? it looks weird
                        int m_offset = map_view * 1024;
                        for (int y = 0; y < 16; y++)
                        {
                            for(int x = 16; x < 32; x++) // top right
                            {
                                int m_offset2 = (y * 32) + x + m_offset;
                                Maps.tile[m_offset2] = 0;
                                Maps.palette[m_offset2] = 0;
                                Maps.h_flip[m_offset2] = 0;
                                Maps.v_flip[m_offset2] = 0;
                            }
                        }
                        for (int y = 16; y < 32; y++)
                        {
                            for (int x = 0; x < 32; x++) // bottom left and right
                            {
                                int m_offset2 = (y * 32) + x + m_offset;
                                Maps.tile[m_offset2] = 0;
                                Maps.palette[m_offset2] = 0;
                                Maps.h_flip[m_offset2] = 0;
                                Maps.v_flip[m_offset2] = 0;
                            }
                        }
                    }
                    textBox6.Text = map_height.ToString();

                    // remove duplicates
                    if(f3_cb2 == true)
                    {
                        RemoveDuplicateTiles();
                    }

                    // redraw everything
                    common_update2();

                    import_bmp.Dispose();
                }
            }
        }

        public int Best_Color(Color temp_color, int num_col, int start_offset)
        {
            int best_index = 0;
            int best_count = 99999;

            for(int i = 0; i < num_col; i++)
            {
                int i2 = start_offset + i;
                int red = Palettes.pal_r[i2] - temp_color.R;
                red = Math.Abs(red);
                int green = Palettes.pal_g[i2] - temp_color.G;
                green = Math.Abs(green);
                int blue = Palettes.pal_b[i2] - temp_color.B;
                blue = Math.Abs(blue);
                int sum = red + green + blue;
                if(sum < best_count)
                {
                    best_count = sum;
                    best_index = i;
                }
            }

            return best_index;
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // open Form3, Options for importing images
            if (newChild3 != null)
            {
                newChild3.BringToFront();
            }
            else
            {
                newChild3 = new Form3();
                newChild3.Owner = this;
                
                newChild3.Top = this.Top + 100;
                newChild3.Left = this.Left + 300;

                newChild3.Show();
                
            }
        }

        private void x2NextToolStripMenuItem_Click(object sender, EventArgs e)
        { // drop current tile and it's neighbors in a 16x16 box
            if(tilesize != TILE_8X8)
            {
                MessageBox.Show("This brush is disallowed in 16x16 Mode.");
                return;
            }

            brushsize = BRUSHNEXT;
            x1ToolStripMenuItem.Checked = false;
            x3ToolStripMenuItem.Checked = false;
            x5ToolStripMenuItem.Checked = false;
            x2NextToolStripMenuItem.Checked = true;
            cloneFromTilesetToolStripMenuItem.Checked = false;
            cloneFromMapToolStripMenuItem.Checked = false;
            fillScreenToolStripMenuItem.Checked = false;
        }

        private void cloneFromTilesetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*if (tilesize != TILE_8X8)
            {
                MessageBox.Show("This brush is disallowed in 16x16 Mode.");
                return;
            }*/

            brushsize = BRUSH_CLONE_T;
            x1ToolStripMenuItem.Checked = false;
            x3ToolStripMenuItem.Checked = false;
            x5ToolStripMenuItem.Checked = false;
            x2NextToolStripMenuItem.Checked = false;
            cloneFromTilesetToolStripMenuItem.Checked = true;
            cloneFromMapToolStripMenuItem.Checked = false;
            fillScreenToolStripMenuItem.Checked = false;
        }

        private void cloneFromMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*if (tilesize != TILE_8X8)
            {
                MessageBox.Show("This brush is disallowed in 16x16 Mode.");
                return;
            }*/

            brushsize = BRUSH_CLONE_M;
            x1ToolStripMenuItem.Checked = false;
            x3ToolStripMenuItem.Checked = false;
            x5ToolStripMenuItem.Checked = false;
            x2NextToolStripMenuItem.Checked = false;
            cloneFromTilesetToolStripMenuItem.Checked = false;
            cloneFromMapToolStripMenuItem.Checked = true;
            fillScreenToolStripMenuItem.Checked = false;
        }

        private void button2_Click(object sender, EventArgs e)
        { // shift map left
            int temp_offset, temp_tile, temp_palette, temp_h_flip, temp_v_flip, temp_priority;
            int temp_offset2;
            if (map_view > 2) return;
            temp_offset = 32 * 32 * map_view;

            Checkpoint();

            for(int yy = 0; yy < 32; yy++)
            {
                temp_offset2 = temp_offset + (yy * 32);
                // save left most column
                temp_tile = Maps.tile[temp_offset2];
                temp_palette = Maps.palette[temp_offset2];
                temp_h_flip = Maps.h_flip[temp_offset2];
                temp_v_flip = Maps.v_flip[temp_offset2];
                temp_priority = Maps.priority[temp_offset2];

                for (int xx = 0; xx < 31; xx++)
                {
                    // shift every tile left 1
                    Maps.tile[temp_offset2 + xx] = Maps.tile[temp_offset2 + xx + 1];
                    Maps.palette[temp_offset2 + xx] = Maps.palette[temp_offset2 + xx + 1];
                    Maps.h_flip[temp_offset2 + xx] = Maps.h_flip[temp_offset2 + xx + 1];
                    Maps.v_flip[temp_offset2 + xx] = Maps.v_flip[temp_offset2 + xx + 1];
                    Maps.priority[temp_offset2 + xx] = Maps.priority[temp_offset2 + xx + 1];
                }
                // put left most column on right
                Maps.tile[temp_offset2+31] = temp_tile;
                Maps.palette[temp_offset2+31] = temp_palette;
                Maps.h_flip[temp_offset2+31] = temp_h_flip;
                Maps.v_flip[temp_offset2+31] = temp_v_flip;
                Maps.priority[temp_offset2+31] = temp_priority;
            }
            common_update2();
            label5.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        { // shift map right
            int temp_offset, temp_tile, temp_palette, temp_h_flip, temp_v_flip, temp_priority;
            int temp_offset2;
            if (map_view > 2) return;
            temp_offset = 32 * 32 * map_view;

            Checkpoint();

            for (int yy = 0; yy < 32; yy++)
            {
                temp_offset2 = temp_offset + (yy * 32);
                // save right most column
                temp_tile = Maps.tile[temp_offset2 + 31];
                temp_palette = Maps.palette[temp_offset2 + 31];
                temp_h_flip = Maps.h_flip[temp_offset2 + 31];
                temp_v_flip = Maps.v_flip[temp_offset2 + 31];
                temp_priority = Maps.priority[temp_offset2 + 31];

                for (int xx = 30; xx >= 0; xx--)
                {
                    // shift every tile right 1
                    Maps.tile[temp_offset2 + xx + 1] = Maps.tile[temp_offset2 + xx];
                    Maps.palette[temp_offset2 + xx + 1] = Maps.palette[temp_offset2 + xx];
                    Maps.h_flip[temp_offset2 + xx + 1] = Maps.h_flip[temp_offset2 + xx];
                    Maps.v_flip[temp_offset2 + xx + 1] = Maps.v_flip[temp_offset2 + xx];
                    Maps.priority[temp_offset2 + xx + 1] = Maps.priority[temp_offset2 + xx];
                }
                // put right most column on left
                Maps.tile[temp_offset2] = temp_tile;
                Maps.palette[temp_offset2] = temp_palette;
                Maps.h_flip[temp_offset2] = temp_h_flip;
                Maps.v_flip[temp_offset2] = temp_v_flip;
                Maps.priority[temp_offset2] = temp_priority;
            }
            common_update2();
            label5.Focus();
        }

        private void button4_Click(object sender, EventArgs e)
        { // shift map up
            int temp_offset, temp_tile, temp_palette, temp_h_flip, temp_v_flip, temp_priority;
            int temp_offset2, temp_offset3;
            if (map_view > 2) return;
            temp_offset = 32 * 32 * map_view;

            Checkpoint();

            for (int xx = 0; xx < 32; xx++)
            {
                temp_offset2 = temp_offset + xx;
                // save top most row
                temp_tile = Maps.tile[temp_offset2];
                temp_palette = Maps.palette[temp_offset2];
                temp_h_flip = Maps.h_flip[temp_offset2];
                temp_v_flip = Maps.v_flip[temp_offset2];
                temp_priority = Maps.priority[temp_offset2];

                for (int yy = 0; yy < 31; yy++)
                {
                    // shift every tile up 1
                    temp_offset3 = temp_offset2 + (yy * 32);
                    Maps.tile[temp_offset3] = Maps.tile[temp_offset3 + 32];
                    Maps.palette[temp_offset3] = Maps.palette[temp_offset3 + 32];
                    Maps.h_flip[temp_offset3] = Maps.h_flip[temp_offset3 + 32];
                    Maps.v_flip[temp_offset3] = Maps.v_flip[temp_offset3 + 32];
                    Maps.priority[temp_offset3] = Maps.priority[temp_offset3 + 32];
                }
                // put top most row on bottom
                Maps.tile[temp_offset2 + (31 * 32)] = temp_tile;
                Maps.palette[temp_offset2 + (31 * 32)] = temp_palette;
                Maps.h_flip[temp_offset2 + (31 * 32)] = temp_h_flip;
                Maps.v_flip[temp_offset2 + (31 * 32)] = temp_v_flip;
                Maps.priority[temp_offset2 + (31 * 32)] = temp_priority;
            }
            common_update2();
            label5.Focus();
        }

        private void button5_Click(object sender, EventArgs e)
        { // shift map down
            int temp_offset, temp_tile, temp_palette, temp_h_flip, temp_v_flip, temp_priority;
            int temp_offset2, temp_offset3;
            if (map_view > 2) return;
            temp_offset = 32 * 32 * map_view;

            Checkpoint();

            for (int xx = 0; xx < 32; xx++)
            {
                temp_offset2 = temp_offset + xx;
                // save bottom most row
                temp_tile = Maps.tile[temp_offset2 + (31 * 32)];
                temp_palette = Maps.palette[temp_offset2 + (31 * 32)];
                temp_h_flip = Maps.h_flip[temp_offset2 + (31 * 32)];
                temp_v_flip = Maps.v_flip[temp_offset2 + (31 * 32)];
                temp_priority = Maps.priority[temp_offset2 + (31 * 32)];

                for (int yy = 30; yy >= 0; yy--)
                {
                    // shift every tile down 1
                    temp_offset3 = temp_offset2 + (yy * 32);
                    Maps.tile[temp_offset3 + 32] = Maps.tile[temp_offset3];
                    Maps.palette[temp_offset3 + 32] = Maps.palette[temp_offset3];
                    Maps.h_flip[temp_offset3 + 32] = Maps.h_flip[temp_offset3];
                    Maps.v_flip[temp_offset3 + 32] = Maps.v_flip[temp_offset3];
                    Maps.priority[temp_offset3 + 32] = Maps.priority[temp_offset3];
                }
                // put bottom most row on top
                Maps.tile[temp_offset2] = temp_tile;
                Maps.palette[temp_offset2] = temp_palette;
                Maps.h_flip[temp_offset2] = temp_h_flip;
                Maps.v_flip[temp_offset2] = temp_v_flip;
                Maps.priority[temp_offset2] = temp_priority;
            }
            common_update2();
            label5.Focus();
        }

        

        private void button1_Click(object sender, EventArgs e)
        { // color picker
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Color tempcolor = colorDialog1.Color;
                int red = tempcolor.R & 0xf8;
                int green = tempcolor.G & 0xf8;
                int blue = tempcolor.B & 0xf8;
                textBox1.Text = red.ToString();
                textBox2.Text = green.ToString();
                textBox3.Text = blue.ToString();
                
                update_rgb(); //updates trackbars too
                update_box4();
                update_palette();
                common_update2();
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        { // the tile attribute box, palette 0-7
            if (map_view > 2) return;

            if (e.KeyChar == (char)Keys.Return)
            {
                Checkpoint();
                
                string str = textBox5.Text;
                int value = 0;
                int.TryParse(str, out value);
                if (value > 7) value = 7; // max value
                if (value < 0) value = 0; // min value
                str = value.ToString();
                textBox5.Text = str;

                active_map_index = active_map_x + (active_map_y * 32) + (32 * 32 * map_view);
                Maps.palette[active_map_index] = value;

                //also change the palette selection
                if(map_view == 2)
                {
                    pal_y = (value >> 2);
                    pal_x = (value & 3) << 2;
                }
                else
                {
                    pal_y = value;
                }
                
                update_palette();
                common_update2();

                e.Handled = true; // prevent ding on return press
                label5.Focus();
            }
        }



        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        { // priority (entire map)
            if (map_view > 2) return;

            Checkpoint();

            int offset = map_view * 32 * 32;
            if (checkBox3.Checked == false)
            {
                for(int i = 0; i < 32*32; i++)
                {
                    Maps.priority[offset++] = 0;
                }
            }
            else
            {
                for (int i = 0; i < 32 * 32; i++)
                {
                    Maps.priority[offset++] = 1;
                }
            }

            //this tool doesn't show priority differences
            label5.Focus();
        }



        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        { // the map height box, 1-32
            if (e.KeyChar == (char)Keys.Return)
            {
                string str = textBox6.Text;
                int value = 32; //default
                int.TryParse(str, out value);
                if (value > 32) value = 32; // max value
                if (value < 1) value = 32; // just use default if error
                str = value.ToString();
                textBox6.Text = str;

                map_height = value; //1-32

                update_tilemap();
                e.Handled = true; // prevent ding on return press
                label5.Focus();
            }
        }



        private void checkBox1_Click(object sender, EventArgs e)
        { // h flip
            if (map_view > 2) return;

            Checkpoint();

            active_map_index = active_map_x + (active_map_y * 32) + (32 * 32 * map_view);
            if (checkBox1.Checked == false)
            {
                Maps.h_flip[active_map_index] = 0;
            }
            else
            {
                Maps.h_flip[active_map_index] = 1;
            }

            update_tilemap();
            label5.Focus();
        }



        private void checkBox2_Click(object sender, EventArgs e)
        { // v flip
            if (map_view > 2) return;

            Checkpoint();

            active_map_index = active_map_x + (active_map_y * 32) + (32 * 32 * map_view);
            if (checkBox2.Checked == false)
            {
                Maps.v_flip[active_map_index] = 0;
            }
            else
            {
                Maps.v_flip[active_map_index] = 1;
            }

            update_tilemap();
            label5.Focus();
        }

        

        public static void draw_palettes() // sub routine of update palette
        {
            int count = 0;
            //Bitmap temp_bm = new Bitmap(256, 256); // very small, will zoom it later
            SolidBrush temp_brush = new SolidBrush(Color.White);
            if(tile_set > 3) // 2 bpp
            {
                for (int i = 0; i < 64; i += 32) //2 rows
                {
                    for (int j = 0; j < 256; j += 16) //each box in the row
                    {
                        // draw a rectangle
                        using (Graphics g = Graphics.FromImage(temp_bmp3))
                        {
                            if((j & 0x30) == 0)
                            {
                                temp_brush.Color = Color.FromArgb(Palettes.pal_r[0], Palettes.pal_g[0], Palettes.pal_b[0]);
                            }
                            else
                            {
                                temp_brush.Color = Color.FromArgb(Palettes.pal_r[count], Palettes.pal_g[count], Palettes.pal_b[count]);
                            }
                            g.FillRectangle(temp_brush, j, i, 16, 16);
                        }
                        count++;
                    }
                }

                //draw the other colors anyway, need if load palette from menu
                for (int i = 64; i < 256; i += 32) //each row
                {
                    for (int j = 0; j < 256; j += 16) //each box in the row
                    {
                        // draw a rectangle
                        using (Graphics g = Graphics.FromImage(temp_bmp3))
                        {
                            temp_brush.Color = Color.FromArgb(Palettes.pal_r[count], Palettes.pal_g[count], Palettes.pal_b[count]);
                            g.FillRectangle(temp_brush, j, i, 16, 16);
                        }
                        count++;
                    }
                }
            }
            else // 4bpp
            {
                for (int i = 0; i < 256; i += 32) //each row
                {
                    for (int j = 0; j < 256; j += 16) //each box in the row
                    {
                        // draw a rectangle
                        using (Graphics g = Graphics.FromImage(temp_bmp3))
                        {
                            temp_brush.Color = Color.FromArgb(Palettes.pal_r[count], Palettes.pal_g[count], Palettes.pal_b[count]);
                            g.FillRectangle(temp_brush, j, i, 16, 16);
                        }
                        count++;
                    }
                }
            }
            
            
            image_pal = temp_bmp3;
            temp_brush.Dispose();
        } // END DRAW PALETTES



        public void update_palette() // use this one
        {
            byte zero = Palettes.pal_r[0]; // copy the first element to all the firsts
            Palettes.pal_r[16] = zero;
            Palettes.pal_r[16 * 1] = zero;
            Palettes.pal_r[16 * 2] = zero;
            Palettes.pal_r[16 * 3] = zero;
            Palettes.pal_r[16 * 4] = zero;
            Palettes.pal_r[16 * 5] = zero;
            Palettes.pal_r[16 * 6] = zero;
            Palettes.pal_r[16 * 7] = zero;
            zero = Palettes.pal_g[0];
            Palettes.pal_g[16] = zero;
            Palettes.pal_g[16 * 1] = zero;
            Palettes.pal_g[16 * 2] = zero;
            Palettes.pal_g[16 * 3] = zero;
            Palettes.pal_g[16 * 4] = zero;
            Palettes.pal_g[16 * 5] = zero;
            Palettes.pal_g[16 * 6] = zero;
            Palettes.pal_g[16 * 7] = zero;
            zero = Palettes.pal_b[0];
            Palettes.pal_b[16] = zero;
            Palettes.pal_b[16 * 1] = zero;
            Palettes.pal_b[16 * 2] = zero;
            Palettes.pal_b[16 * 3] = zero;
            Palettes.pal_b[16 * 4] = zero;
            Palettes.pal_b[16 * 5] = zero;
            Palettes.pal_b[16 * 6] = zero;
            Palettes.pal_b[16 * 7] = zero;

            // which palette square
            int xx = pal_x * 16;
            int yy = pal_y * 32;

            draw_palettes();

            // draw a square on selected box
            for(int i = 0; i < 16; i++)
            {
                image_pal.SetPixel(xx + i, yy, Color.White); //top line
                image_pal.SetPixel(xx, yy + i, Color.White); //left line
                
                image_pal.SetPixel(xx + i, yy + 15, Color.White); //bottom line
                image_pal.SetPixel(xx + 15, yy + i, Color.White); //right line

                if (i == 15) continue;
                image_pal.SetPixel(xx + 14, yy + i, Color.Black); //black right line
                image_pal.SetPixel(xx + i, yy + 14, Color.Black); //black bottom line
            }

            pictureBox3.Image = image_pal;
            pictureBox3.Refresh();
        }



        private void pictureBox3_Click(object sender, EventArgs e)
        { // palettes            

            int selection;
            var mouseEventArgs = e as MouseEventArgs;
            if (mouseEventArgs != null)
            {
                if ((mouseEventArgs.Y & 0x10) == 0)
                {
                    pal_x = (mouseEventArgs.X & 0xf0) >> 4;
                    pal_y = (mouseEventArgs.Y & 0xe0) >> 5;
                }
                if (pal_x < 0) pal_x = 0;
                if (pal_y < 0) pal_y = 0;
                if (pal_x > 15) pal_x = 15;
                if (pal_y > 7) pal_y = 7;

                if ((map_view == 2) && (pal_y > 1))
                {
                    // 2bpp and off the 2 rows
                    return;
                }
                selection = pal_x + (pal_y * 16);
                if ((map_view == 2) && ((pal_x & 3) == 0)) selection = 0; // 2bpp
                update_palette();

                //update the boxes
                rebuild_pal_boxes();
            }

            common_update2();
            label5.Focus();
        }


    }
}
