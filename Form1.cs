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

        public static void close_it()
        {
            newChild = null;
        }

        //globals
        public static Bitmap image_map = new Bitmap(256, 256);
        public static Bitmap image_tiles = new Bitmap(128, 128);
        public static Bitmap image_pal = new Bitmap(256, 256);
        public static Bitmap image_map_local = new Bitmap(256, 256);
        public static Bitmap temp_bmp = new Bitmap(256, 256); //double size
        public static Bitmap temp_bmp2 = new Bitmap(512, 512); //double size
        public static Bitmap temp_bmp3 = new Bitmap(256, 256); //double size
        public static int pal_x, pal_y, tile_x, tile_y, tile_num, tile_set;
        public static int map_view, active_map_x, active_map_y, active_map_index;
        public static int map_height = 32;
        public static int last_tile_x, last_tile_y;
        public static int brushsize;
        public const int BRUSH1x1 = 0;
        public const int BRUSH3x3 = 1;
        public const int BRUSH5x5 = 2;
        public const int BRUSHNEXT = 3;
        public static int pal_r_copy, pal_g_copy, pal_b_copy;

        private void Form1_Load(object sender, EventArgs e)
        {
            update_palette();
            update_tile_image();
            update_tilemap();
            label5.Focus();
            this.ActiveControl = label5;
        }


        // 16x16 grid
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            update_tilemap();
        }



        public void update_tilemap() // the big box on the left, 32x32
        {
            //default BG, draw color 0 all over the BG
            int r = Palettes.pal_r[0];
            int g = Palettes.pal_g[0];
            int b = Palettes.pal_b[0];
            int offset = 0;
            int temp_tile = 0;
            int temp_pal = 0;
            //int temp_h = 0;
            //int temp_v = 0;
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
                    for(int x = 0; x < 32; x++)
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


            //Bitmap temp_bmp2 = new Bitmap(512, 512); //resize double size
            using (Graphics g2 = Graphics.FromImage(temp_bmp2))
            {
                g2.InterpolationMode = InterpolationMode.NearestNeighbor;
                g2.DrawImage(image_map_local, 0, 0, 512, 512);
            } // standard resize of bmp was blurry, this makes it sharp

            //draw a box around the active tile
            if (map_view < 3)
            {
                int x2 = (active_map_x * 16) - 1;
                if (x2 < 0) x2 = 0;
                if (active_map_y >= map_height) active_map_y = map_height - 1;
                int y2 = (active_map_y * 16) - 1;
                if (y2 < 0) y2 = 0;
                for (int i = 0; i < 16; i++)
                {
                    temp_bmp2.SetPixel(x2 + i, y2, Color.White);
                    temp_bmp2.SetPixel(x2, y2 + i, Color.White);
                    temp_bmp2.SetPixel(x2 + i, y2 + 15, Color.White);
                    temp_bmp2.SetPixel(x2 + 15, y2 + i, Color.White);
                }

                //draw grid here
                if (checkBox4.Checked == true)
                {
                    //draw horizontal lines at each 16
                    for (int i = 31; i < (map_height * 16); i += 32)
                    {
                        if (i == 511) break;
                        for (int j = 0; j < 512; j++)
                        {
                            temp_bmp2.SetPixel(j, i, Color.White);
                        }
                    }
                    //draw vertical lines at each 16
                    for (int i = 0; i < (map_height * 16); i++)
                    {
                        for (int j = 31; j < 512; j += 32)
                        {
                            if (i == 511) break;
                            temp_bmp2.SetPixel(j, i, Color.White);
                        }
                    }
                }
            }

            pictureBox1.Image = temp_bmp2;
            pictureBox1.Refresh();
            //temp_bmp2.Dispose(); //crashes the program ?
        }
        // END UPDATE TILEMAP




        //for drawing the tile map
        private void big_sub(int offset, int x, int y, int temp_tile, int temp_pal)
        {
            int color = 0;
            int temp_h = Maps.h_flip[offset];
            int temp_v = Maps.v_flip[offset];
            if (temp_h == 0) // plain
            {
                if (temp_v == 0) // plain
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
        // END TILEMAP SUB

        

        private int check_num(string str) // make sure string is number
        {
            int value = 0;

            int.TryParse(str, out value);
            if (value > 255) value = 255; // max value
            if (value < 0) value = 0; // min value
            value = value & 0xf8;
            //str = value.ToString();
            return (value);
        }



        private void update_box4() // when boxes 1,2,or 3 changed
        {
            //int[] values = new int[3];
            int value_red, value_green, value_blue;
            int sum;
            int selection = get_selection();


            //int.TryParse(textBox1.Text, out values[0]); //convert string to int
            //int.TryParse(textBox2.Text, out values[1]);
            //int.TryParse(textBox3.Text, out values[2]);
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
                (!is_hex(str[3])) )
            {
                //something isn't a hex string
                return "Z";
            }

            //make sure the high byte is 0-7
            if (str[0] > '7')
            {
                char[] letters = str.ToCharArray();
                char letter;
                switch(letters[0])
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
            switch(chr)
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
            
            //string str = textBox1.Text;
            int selection = pal_x + (pal_y * 16);
            if ((map_view == 2) && ((pal_x & 3) == 0)) selection = 0; // 2bpp
            if (pal_x == 0) selection = 0;
            return selection;
            /*int value = 0;
            int.TryParse(str, out value);
            Palettes.pal_r[selection] = (byte)value;

            str = textBox2.Text;
            value = 0;
            int.TryParse(str, out value);
            Palettes.pal_g[selection] = (byte)value;

            str = textBox3.Text;
            value = 0;
            int.TryParse(str, out value);
            Palettes.pal_b[selection] = (byte)value;
            */
        }



        private void update_rgb() // when r g or b boxes change
        {
            string str = textBox1.Text;
            int value = check_num(str);
            textBox1.Text = value.ToString();

            int selection = get_selection();
            Palettes.pal_r[selection] = (byte)value;

            str = textBox2.Text;
            value = check_num(str);
            textBox2.Text = value.ToString();

            //selection = get_selection();
            Palettes.pal_g[selection] = (byte)value;

            str = textBox3.Text;
            value = check_num(str);
            textBox3.Text = value.ToString();

            //selection = get_selection();
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

                if(str.Length < 4)
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

                //update the palette values, then draw it
                //update_single_color();
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
                                if(pal_index == 0)
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
                g.DrawImage(image_tiles, 0, 0, 256, 256);
            } // standard resize of bmp was blurry, this makes it sharp

            //put a white box around the selected tile
            int pos_x = 0; int pos_y = 0;
            for (int i = 0; i < 16; i++)
            {
                pos_y = (tile_y * 16)-1; // it's doing a weird off by 1 thing
                if (pos_y < 0) pos_y = 0; // so have to adjust by 1, and not == -1
                pos_x = (tile_x * 16)-1;
                if (pos_x < 0) pos_x = 0;
                temp_bmp.SetPixel(pos_x + i, pos_y, Color.White);
                temp_bmp.SetPixel(pos_x, pos_y + i, Color.White);
                temp_bmp.SetPixel(pos_x + i, pos_y + 15, Color.White);
                temp_bmp.SetPixel(pos_x + 15, pos_y + i, Color.White);
            }
            pictureBox2.Image = temp_bmp;
            pictureBox2.Refresh();
            //temp_bmp.Dispose(); //crashes the program ?
        }
        // END REDRAW TILESET



        public void tile_show_num() // top right, above tileset
        {
            string str = "";
            str = hex_char(tile_y) + hex_char(tile_x);
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
                if(this.Location.X + 970 < xx) // set new form location
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



        private void picbox1_sub() // place a tile on the map
        {
            // apply the tile now
            int temp_y, temp_x, start_x, loop_x, loop_y;
            if(brushsize == BRUSH1x1)
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
            else // BRUSHNEXT
            {
                start_x = temp_x = active_map_x;
                temp_y = active_map_y;
                loop_x = 2;
                loop_y = 2;
            }

            int temp_set = tile_set & 3; //0-3
            int tile_num2 = tile_x + (tile_y * 16) + (256 * temp_set); // 0-1023
            // which tile is selected on right.

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

                    } // end of tile change 

                    if(brushsize == BRUSHNEXT)
                    {
                        tile_num2++;
                        tile_num2 &= 0x3ff;
                    }
                    temp_x++;
                }
                if (brushsize == BRUSHNEXT)
                {
                    tile_num2 = tile_num2 + 14;
                    tile_num2 &= 0x3ff;
                }
                temp_x = start_x;
                temp_y++;
            }

            

            //update_tilemap(); // moved
        }



        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        { // TILEMAP
            if (map_view > 2)
            {
                MessageBox.Show("Editing is disabled in Preview Mode.");
                return;
            }

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
                last_tile_x = active_map_x; // to speed up the app
                last_tile_y = active_map_y; // see mouse move event
                picbox1_sub(); // place the tile and redraw the map
                //checkBox3.Checked = false;
                update_tilemap();
            }
            else if (e.Button == MouseButtons.Right) // get the tile, tileset, and properties
            {
                int tile = (map_view * 32 * 32) + (32 * active_map_y) + active_map_x;
                int pal = Maps.palette[tile];
                textBox5.Text = pal.ToString();
                if (Maps.h_flip[tile] == 0) checkBox1.Checked = false;
                else checkBox1.Checked = true;
                if (Maps.v_flip[tile] == 0) checkBox2.Checked = false;
                else checkBox2.Checked = true;
                //if (Maps.priority[tile] == 0) checkBox3.Checked = false;
                //else checkBox3.Checked = true;
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
                    //update_tile_image();
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
                    //update_tile_image();

                    // was...
                    pal_y = (pal >> 2);
                    pal_x = (pal & 3) << 2;
                }

                update_palette();
                common_update2();
            }
        } // END CLICKED ON TILEMAP



        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (map_view > 2) return;

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

                if((last_tile_x != active_map_x)||(last_tile_y != active_map_y))
                {
                    // only update if the tile under mouse has changed.
                    last_tile_x = active_map_x;
                    last_tile_y = active_map_y;
                    picbox1_sub();
                    update_tilemap();
                }
                
            }
        }
        // END MOUSE DOWN MOVE ON TILEMAP



        
        //capure key presses on the tiles, focus is redirected to label 5
        private void label5_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            int selection = pal_x + (pal_y * 16);

            if (e.KeyCode == Keys.C)
            {
                Tiles.tile_copy();
                //common_update2();
            }
            else if (e.KeyCode == Keys.P)
            {
                Tiles.tile_paste();
                //common_update2();
            }
            else if (e.KeyCode == Keys.NumPad4) 
            {
                if (tile_x > 0) tile_x--;
                tile_num = (tile_y * 16) + tile_x;
                //common_update2();
            }
            else if (e.KeyCode == Keys.NumPad8)
            {
                if (tile_y > 0) tile_y--;
                tile_num = (tile_y * 16) + tile_x;
                //common_update2();
            }
            else if (e.KeyCode == Keys.NumPad6)
            {
                if (tile_x < 15) tile_x++;
                tile_num = (tile_y * 16) + tile_x;
                //common_update2();
            }
            else if (e.KeyCode == Keys.NumPad2)
            {
                if (tile_y < 15) tile_y++;
                tile_num = (tile_y * 16) + tile_x;
                //common_update2();
            }
            else if (e.KeyCode == Keys.Q)
            { // copy selected color
                //int selection = pal_x + (pal_y * 16);
                pal_r_copy = Palettes.pal_r[selection];
                pal_g_copy = Palettes.pal_g[selection];
                pal_b_copy = Palettes.pal_b[selection];
            }
            else if (e.KeyCode == Keys.W)
            { // paste selected to color
                //int selection = pal_x + (pal_y * 16);
                Palettes.pal_r[selection] = (byte)pal_r_copy;
                Palettes.pal_g[selection] = (byte)pal_g_copy;
                Palettes.pal_b[selection] = (byte)pal_b_copy;
                update_palette();
                textBox1.Text = Palettes.pal_r[selection].ToString();
                textBox2.Text = Palettes.pal_g[selection].ToString();
                textBox3.Text = Palettes.pal_b[selection].ToString();
                update_box4();
            }
            else if (e.KeyCode == Keys.E)
            { // clear selected to color
                //int selection = pal_x + (pal_y * 16);
                Palettes.pal_r[selection] = 0;
                Palettes.pal_g[selection] = 0;
                Palettes.pal_b[selection] = 0;
                update_palette();
                textBox1.Text = Palettes.pal_r[selection].ToString();
                textBox2.Text = Palettes.pal_g[selection].ToString();
                textBox3.Text = Palettes.pal_b[selection].ToString();
                update_box4();
            }

            common_update2();
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
        }

        private void x3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            brushsize = BRUSH3x3;
            x1ToolStripMenuItem.Checked = false;
            x3ToolStripMenuItem.Checked = true;
            x5ToolStripMenuItem.Checked = false;
            x2NextToolStripMenuItem.Checked = false;
        }

        private void x5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            brushsize = BRUSH5x5;
            x1ToolStripMenuItem.Checked = false;
            x3ToolStripMenuItem.Checked = false;
            x5ToolStripMenuItem.Checked = true;
            x2NextToolStripMenuItem.Checked = false;
        }

        private void x2NextToolStripMenuItem_Click(object sender, EventArgs e)
        { // drop current tile and it's neighbors in a 16x16 box
            brushsize = BRUSHNEXT;
            x1ToolStripMenuItem.Checked = false;
            x3ToolStripMenuItem.Checked = false;
            x5ToolStripMenuItem.Checked = false;
            x2NextToolStripMenuItem.Checked = true;
        }

        private void button2_Click(object sender, EventArgs e)
        { // shift map left
            int temp_offset, temp_tile, temp_palette, temp_h_flip, temp_v_flip, temp_priority;
            int temp_offset2;
            if (map_view > 2) return;
            temp_offset = 32 * 32 * map_view;

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
        }

        private void button3_Click(object sender, EventArgs e)
        { // shift map right
            int temp_offset, temp_tile, temp_palette, temp_h_flip, temp_v_flip, temp_priority;
            int temp_offset2;
            if (map_view > 2) return;
            temp_offset = 32 * 32 * map_view;

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
        }

        private void button4_Click(object sender, EventArgs e)
        { // shift map up
            int temp_offset, temp_tile, temp_palette, temp_h_flip, temp_v_flip, temp_priority;
            int temp_offset2, temp_offset3;
            if (map_view > 2) return;
            temp_offset = 32 * 32 * map_view;

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
        }

        private void button5_Click(object sender, EventArgs e)
        { // shift map down
            int temp_offset, temp_tile, temp_palette, temp_h_flip, temp_v_flip, temp_priority;
            int temp_offset2, temp_offset3;
            if (map_view > 2) return;
            temp_offset = 32 * 32 * map_view;

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
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Color tempcolor = colorDialog1.Color;
                int red = tempcolor.R & 0xf8;
                int green = tempcolor.G & 0xf8;
                int blue = tempcolor.B & 0xf8;
                textBox1.Text = red.ToString();
                textBox2.Text = green.ToString();
                textBox3.Text = blue.ToString();
                update_rgb();
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
            }
        }



        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        { // priority (entire map)
            if (map_view > 2) return;

            //active_map_index = active_map_x + (active_map_y * 32) + (32 * 32 * map_view);
            int offset = map_view * 32 * 32;
            if (checkBox3.Checked == false)
            {
                //Maps.priority[active_map_index] = 0;
                for(int i = 0; i < 32*32; i++)
                {
                    Maps.priority[offset++] = 0;
                }
            }
            else
            {
                //Maps.priority[active_map_index] = 1;
                for (int i = 0; i < 32 * 32; i++)
                {
                    Maps.priority[offset++] = 1;
                }
            }

            //this tool doesn't show priority differences
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
            }
        }



        private void checkBox1_Click(object sender, EventArgs e)
        { // h flip
            if (map_view > 2) return;

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
        }



        private void checkBox2_Click(object sender, EventArgs e)
        { // v flip
            if (map_view > 2) return;

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
            //temp_bm.Dispose(); //crashes the program ?
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

            //int selected = pal_x + (pal_y * 8); //which palette square
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
                textBox1.Text = Palettes.pal_r[selection].ToString();
                textBox2.Text = Palettes.pal_g[selection].ToString();
                textBox3.Text = Palettes.pal_b[selection].ToString();
                update_box4();
            }

            common_update2();
            label5.Focus();
        }


    }
}
