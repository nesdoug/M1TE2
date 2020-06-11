using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace M1TE2
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        //global
        Bitmap image_tile_box = new Bitmap(128, 128);

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            //trying to figure out how to send information between forms
            Form1.close_it();
            image_tile_box.Dispose();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            update_tile_box();
        }

        public void update_tile_box()
        {
            Color temp_color;
            for (int i = 0;i < 8; ++i) //row = y
            {
                for(int j = 0;j < 8; ++j) //column = x
                {
                    int color = 0;
                    int index = (Form1.tile_set*256*8*8) + (Form1.tile_num*8*8) + (i*8) + j;
                    int pal_index = Tiles.Tile_Arrays[index]; //pixel in tile array
                    if (Form1.map_view == 2) // 2bpp
                    {
                        pal_index = pal_index & 0x03; //sanitize, for my sanity
                        if (pal_index == 0)
                        {
                            color = 0; // 0th color
                        }
                        else
                        {
                            color = (Form1.pal_y * 16) + (Form1.pal_x & 0x0c) + pal_index;
                        }
                    }
                    else // 4bpp
                    {
                        pal_index = pal_index & 0x0f; //sanitize, for my sanity
                        color = (Form1.pal_y * 16) + pal_index;
                    }
                    
                    temp_color = Color.FromArgb(Palettes.pal_r[color], Palettes.pal_g[color], Palettes.pal_b[color]);

                    //TODO refactor this with graphics rectangles ?
                    for (int k = 0;k < 15; ++k) // x pixel
                    { //128 / 8 = 16, last one = white line
                        for (int m = 0;m < 15; ++m) // y pixel
                        {
                            image_tile_box.SetPixel((j*16)+m, (i*16)+k, temp_color);
                        }
                        if(j == 7)
                        {
                            image_tile_box.SetPixel((j * 16) + 15, (i * 16) + k, temp_color);
                        }
                        else
                        {
                            image_tile_box.SetPixel((j * 16) + 15, (i * 16) + k, Color.White);
                        }
                    }
                    for (int m = 0; m < 16; ++m) //bottom line
                    {
                        if (i == 7)
                        {
                            image_tile_box.SetPixel((j * 16) + m, (i * 16) + 15, temp_color);
                        }
                        else
                        {
                            image_tile_box.SetPixel((j * 16) + m, (i * 16) + 15, Color.White);
                        }
                        
                    }
                }
            }

            pictureBox1.Image = image_tile_box;
            pictureBox1.Refresh();
        }

        // changed from click event, so we can hold down and draw
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                int pixel_x = 0;
                int pixel_y = 0;

                var mouseEventArgs = e as MouseEventArgs;
                if (mouseEventArgs != null)
                {
                    pixel_x = mouseEventArgs.X >> 4;
                    pixel_y = mouseEventArgs.Y >> 4;
                }
                if (pixel_x < 0) pixel_x = 0;
                if (pixel_y < 0) pixel_y = 0;
                if (pixel_x > 7) pixel_x = 7;
                if (pixel_y > 7) pixel_y = 7;

                int index = (Form1.tile_set * 256 * 8 * 8) + (Form1.tile_num * 8 * 8) + (pixel_y * 8) + pixel_x;
                int color = 0;// (Form1.pal_y * 16) + Form1.pal_x; // which color is selected in palette
                if (Form1.map_view == 2) // 2bpp
                {
                    color = Form1.pal_x & 0x03;
                }
                else // 4bpp
                {
                    color = Form1.pal_x & 0x0f;
                }
                Tiles.Tile_Arrays[index] = color;

                //update tileset picture too
                //common_update();
                update_tile_box();
                Form1 f = (this.Owner as Form1);
                f.update_tile_image();
                f.tile_show_num();
                //skip the map, do with mouse up event
            }
        }

        // this is the mostly the same as above, pictureBox1_MouseMove
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e) 
        {
            int pixel_x = 0;
            int pixel_y = 0;
            Form1 f = (this.Owner as Form1);

            var mouseEventArgs = e as MouseEventArgs;
            if (mouseEventArgs != null)
            {
                pixel_x = mouseEventArgs.X >> 4;
                pixel_y = mouseEventArgs.Y >> 4;
            }
            if (pixel_x < 0) pixel_x = 0;
            if (pixel_y < 0) pixel_y = 0;
            if (pixel_x > 7) pixel_x = 7;
            if (pixel_y > 7) pixel_y = 7;

            int index = (Form1.tile_set * 256 * 8 * 8) + (Form1.tile_num * 8 * 8) + (pixel_y * 8) + pixel_x;

            if(e.Button == MouseButtons.Left)
            {
                int color = 0; // (Form1.pal_y * 16) + Form1.pal_x; // which color is selected in palette
                if (Form1.map_view == 2) // 2bpp
                {
                    color = Form1.pal_x & 0x03;
                }
                else // 4bpp
                {
                    color = Form1.pal_x & 0x0f;
                }
                Tiles.Tile_Arrays[index] = color;
            }
            else if(e.Button == MouseButtons.Right)
            {
                int color = Tiles.Tile_Arrays[index];
                if (Form1.map_view == 2) // 2bpp
                {
                    Form1.pal_x = (Form1.pal_x & 0x0c) + (color & 0x03);
                }
                else
                {
                    Form1.pal_x = color;
                }
                f.update_palette();

            }


            //update tileset picture too
            //common_update();
            update_tile_box();
            
            f.update_tile_image();
            f.tile_show_num();
            //skip the map
        }

        private void common_update() // for clicks and drags
        {
            update_tile_box();
            Form1 f = (this.Owner as Form1);
            f.update_tile_image();
            f.tile_show_num();
            f.update_tilemap();
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            Form1 f = (this.Owner as Form1);

            if (e.KeyCode == Keys.Left)
            {
                Tiles.shift_left();
                common_update();
            }
            else if (e.KeyCode == Keys.Up)
            {
                Tiles.shift_up();
                common_update();
            }
            else if (e.KeyCode == Keys.Right)
            {
                Tiles.shift_right();
                common_update();
            }
            else if (e.KeyCode == Keys.Down)
            {
                Tiles.shift_down();
                common_update();
            }
            else if (e.KeyCode == Keys.NumPad2) // down
            {
                if (Form1.tile_y < 15) Form1.tile_y++;
                Form1.tile_num = (Form1.tile_y * 16) + Form1.tile_x;
                common_update();
            }
            else if (e.KeyCode == Keys.NumPad4) // left
            {
                if (Form1.tile_x > 0) Form1.tile_x--;
                Form1.tile_num = (Form1.tile_y * 16) + Form1.tile_x;
                common_update();
            }
            else if (e.KeyCode == Keys.NumPad6) // right
            {
                if (Form1.tile_x < 15) Form1.tile_x++;
                Form1.tile_num = (Form1.tile_y * 16) + Form1.tile_x;
                common_update();
            }
            else if (e.KeyCode == Keys.NumPad8) // up
            {
                if (Form1.tile_y > 0) Form1.tile_y--;
                Form1.tile_num = (Form1.tile_y * 16) + Form1.tile_x;
                common_update();
            }
            else if (e.KeyCode == Keys.H)
            {
                Tiles.tile_h_flip();
                common_update();
            }
            else if (e.KeyCode == Keys.V)
            {
                Tiles.tile_v_flip();
                common_update();
            }
            else if (e.KeyCode == Keys.R)
            {
                Tiles.tile_rot_cw();
                common_update();
            }
            else if (e.KeyCode == Keys.L)
            {
                Tiles.tile_rot_ccw();
                common_update();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                Tiles.tile_delete();
                common_update();
            }
            else if (e.KeyCode == Keys.C)
            {
                Tiles.tile_copy();
                common_update();
            }
            else if (e.KeyCode == Keys.P)
            {
                Tiles.tile_paste();
                common_update();
            }
            else if (e.KeyCode == Keys.F)
            {
                Tiles.tile_fill();
                common_update();
            }
            else if (e.KeyCode == Keys.D1) // number buttons
            {
                f.set1_change(); // change the tileset
            }
            else if (e.KeyCode == Keys.D2)
            {
                f.set2_change();
            }
            else if (e.KeyCode == Keys.D3)
            {
                f.set3_change();
            }
            else if (e.KeyCode == Keys.D4)
            {
                f.set4_change();
            }
            else if (e.KeyCode == Keys.D5)
            {
                f.set5_change();
            }
            else if (e.KeyCode == Keys.D6)
            {
                f.set6_change();
            }
            else if (e.KeyCode == Keys.D7)
            {
                f.set7_change();
            }
            else if (e.KeyCode == Keys.D8)
            {
                f.set8_change();
            }

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            Form1 f = (this.Owner as Form1);
            f.update_tilemap();
        }


    }
}
