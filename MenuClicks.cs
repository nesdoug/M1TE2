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
    public partial class Form1
    {
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        { // FILE / OPEN SESSION

            // note, we are ignoring the header, maybe change later
            // all sizes are fixed for now

            byte[] big_array = new byte[55568];
            int temp = 0;
            int[] bit1 = new int[8]; // bit planes
            int[] bit2 = new int[8];
            int[] bit3 = new int[8];
            int[] bit4 = new int[8];

            int temp1 = 0;
            int temp2 = 0;
            int temp3 = 0;
            int temp4 = 0;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Open an M1 Session";
            openFileDialog1.Filter = "M1 File (*.M1)|*.M1|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.FileStream fs = (System.IO.FileStream)openFileDialog1.OpenFile();
                if (fs.Length == 55568)
                {
                    for (int i = 0; i < 55568; i++)
                    {
                        big_array[i] = (byte)fs.ReadByte();
                    }

                    if ((big_array[0] == (byte)'M') && (big_array[1] == (byte)'1'))
                    {
                        //copy the map height
                        map_height = big_array[7];
                        if((map_height < 1) || (map_height > 32))
                        {
                            map_height = 32;
                        }
                        textBox6.Text = map_height.ToString();

                        //copy the palette
                        int offset = 16;
                        for (int i = 0; i < 256; i += 2)
                        {
                            int j;
                            temp1 = big_array[offset++];
                            temp2 = big_array[offset++] << 8;
                            temp = temp1 + temp2;
                            if ((i == 0x20) || (i == 0x40) || (i == 0x60) || (i == 0x80) ||
                                (i == 0xa0) || (i == 0xc0) || (i == 0xe0)) temp = 0;
                            // make the left most boxes black, but not the top most
                            j = i / 2;
                            Palettes.pal_r[j] = (byte)((temp & 0x001f) << 3);
                            Palettes.pal_g[j] = (byte)((temp & 0x03e0) >> 2);
                            Palettes.pal_b[j] = (byte)((temp & 0x7c00) >> 7);
                        }

                        // update the numbers in the boxes
                        temp = pal_x + (pal_y * 16);
                        textBox1.Text = Palettes.pal_r[temp].ToString();
                        textBox2.Text = Palettes.pal_g[temp].ToString();
                        textBox3.Text = Palettes.pal_b[temp].ToString();
                        update_box4();

                        //copy the tile maps
                        for (int i = 0; i < 32 * 32 * 3; i++)
                        {
                            temp1 = big_array[offset++];
                            temp2 = big_array[offset++];
                            byte weird_byte = (byte)temp2;
                            int tile = temp1 + ((weird_byte & 3) << 8);
                            Maps.tile[i] = tile;
                            int pal = (weird_byte >> 2) & 7;
                            Maps.palette[i] = pal;
                            int pri = (weird_byte >> 5) & 1;
                            Maps.priority[i] = pri;
                            int h_flip = (weird_byte >> 6) & 1;
                            Maps.h_flip[i] = h_flip;
                            int v_flip = (weird_byte >> 7) & 1;
                            Maps.v_flip[i] = v_flip;
                        }

                        // copy the 4bpp tile sets
                        for (int temp_set = 0; temp_set < 4; temp_set++) // 4 sets
                        {
                            for (int i = 0; i < 256; i++) // 256 tiles
                            {
                                int index = offset + (temp_set * 256 * 32) + (32 * i); // start of current tile
                                for (int y = 0; y < 8; y++) // get 8 sets of bitplanes
                                {
                                    // get the 4 bitplanes for each tile row
                                    int y2 = y * 2; //0,2,4,6,8,10,12,14
                                    bit1[y] = big_array[index + y2];
                                    bit2[y] = big_array[index + y2 + 1];
                                    bit3[y] = big_array[index + y2 + 16];
                                    bit4[y] = big_array[index + y2 + 17];

                                    for (int x = 7; x >= 0; x--) // right to left
                                    {
                                        temp1 = bit1[y] & 1;    // get a bit from each bitplane
                                        bit1[y] = bit1[y] >> 1;
                                        temp2 = bit2[y] & 1;
                                        bit2[y] = bit2[y] >> 1;
                                        temp3 = bit3[y] & 1;
                                        bit3[y] = bit3[y] >> 1;
                                        temp4 = bit4[y] & 1;
                                        bit4[y] = bit4[y] >> 1;
                                        Tiles.Tile_Arrays[(temp_set * 256 * 8 * 8) + (i * 8 * 8) + (y * 8) + x] =
                                            (temp4 << 3) + (temp3 << 2) + (temp2 << 1) + temp1;
                                    }
                                }
                            }
                        }
                        offset += 32768;

                        //copy the 2bpp tileset2
                        for (int temp_set = 0; temp_set < 4; temp_set++) // 4 sets
                        {
                            for (int i = 0; i < 256; i++) // 256 tiles
                            {
                                int index = offset + (temp_set * 4096) + (16 * i); // start of current tile
                                for (int y = 0; y < 8; y++) // get 8 sets of bitplanes
                                {
                                    // get the 4 bitplanes for each tile row
                                    int y2 = y * 2; //0,2,4,6,8,10,12,14
                                    bit1[y] = big_array[index + y2];
                                    bit2[y] = big_array[index + y2 + 1];

                                    for (int x = 7; x >= 0; x--) // right to left
                                    {
                                        temp1 = bit1[y] & 1;    // get a bit from each bitplane
                                        bit1[y] = bit1[y] >> 1;
                                        temp2 = bit2[y] & 1;
                                        bit2[y] = bit2[y] >> 1;
                                        Tiles.Tile_Arrays[((temp_set + 4) * 256 * 8 * 8) + (i * 8 * 8) + (y * 8) + x] =
                                            (temp2 << 1) + temp1;
                                    }
                                }
                            }
                        }
                        if (map_view < 3)
                        {
                            if (Maps.priority[map_view * 32 * 32] == 0) checkBox3.Checked = false;
                            else checkBox3.Checked = true;
                        }
                        else checkBox3.Checked = false;

                        //end, updates are below

                    }
                    else
                    {
                        MessageBox.Show("Error. Not an M1 File.");
                    }


                }
                else
                {
                    MessageBox.Show("File size error. Expected 55568 bytes.",
                    "File size error", MessageBoxButtons.OK);
                }

                fs.Close();

                update_palette();
                common_update2();
            }
        } // end of OPEN SESSION



        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        { // FILE / SAVE SESSION

            byte[] big_array = new byte[55568];
            int temp, count;
            int[] bit1 = new int[8]; // bit planes
            int[] bit2 = new int[8];
            int[] bit3 = new int[8];
            int[] bit4 = new int[8];

            big_array[0] = (byte)'M';
            big_array[1] = (byte)'1';
            big_array[2] = 1; // M1 file version
            big_array[3] = 1; // # palettes
            big_array[4] = 3; // # maps
            big_array[5] = 4; // # 4bpp tilesets
            big_array[6] = 1; // # 4bpp tilesets
            big_array[7] = (byte)map_height; // save map height
            // I don't use these values currently, but maybe will later.

            for (int i = 8; i < 16; i++)
            {
                big_array[i] = 0;
            }

            count = 16;
            for (int i = 0; i < 128; i++) // palettes
            {
                temp = ((Palettes.pal_r[i] & 0xf8) >> 3) + ((Palettes.pal_g[i] & 0xf8) << 2) + ((Palettes.pal_b[i] & 0xf8) << 7);
                big_array[count++] = (byte)(temp & 0xff); // little end first
                big_array[count++] = (byte)((temp >> 8) & 0x7f); // 15 bit palette
            }

            for (int i = 0; i < 32 * 32 * 3; i++) // 3 background maps
            {
                big_array[count++] = (byte)(Maps.tile[i] & 0xff); // the low byte
                temp = ((Maps.tile[i] >> 8) & 3) + ((Maps.palette[i] & 7) << 2) +
                    ((Maps.priority[i] & 1) << 5) + ((Maps.h_flip[i] & 1) << 6) +
                    ((Maps.v_flip[i] & 1) << 7); // mishmash of weird bits
                // VHoP PPcc
                // VH flip, o priority, palette, upper 2 bits of tile number
                big_array[count++] = (byte)temp;
            }

            for (int temp_set = 0; temp_set < 4; temp_set++) // 4 tilesets 4bpp
            {
                for (int i = 0; i < 256; i++) // 256 tiles
                {
                    int z = (temp_set * 256 * 8 * 8) + (64 * i); // start of current tile
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            temp = Tiles.Tile_Arrays[z + (y * 8) + x];
                            bit1[y] = (bit1[y] << 1) + (temp & 1);
                            bit2[y] = (bit2[y] << 1) + ((temp & 2) >> 1);
                            bit3[y] = (bit3[y] << 1) + ((temp & 4) >> 2);
                            bit4[y] = (bit4[y] << 1) + ((temp & 8) >> 3);
                        }
                    }
                    for (int j = 0; j < 8; j++)
                    {
                        big_array[count++] = (byte)bit1[j];
                        big_array[count++] = (byte)bit2[j];
                    }
                    for (int j = 0; j < 8; j++)
                    {
                        big_array[count++] = (byte)bit3[j];
                        big_array[count++] = (byte)bit4[j];
                    }
                }
            }

            for (int temp_set = 4; temp_set < 8; temp_set++) // 4 tilesets 2bpp
            {
                for (int i = 0; i < 256; i++)
                {
                    int z = (temp_set * 256 * 8 * 8) + (64 * i); // start of current tile
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            temp = Tiles.Tile_Arrays[z + (y * 8) + x];
                            bit1[y] = (bit1[y] << 1) + (temp & 1);
                            bit2[y] = (bit2[y] << 1) + ((temp & 2) >> 1);
                        }
                    }
                    for (int j = 0; j < 8; j++)
                    {
                        big_array[count++] = (byte)bit1[j];
                        big_array[count++] = (byte)bit2[j];
                    }
                }
            }


            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "M1 File (*.M1)|*.M1|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save this Session";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                for (int i = 0; i < 55568; i++)
                {
                    fs.WriteByte(big_array[i]);
                }
                fs.Close();
            }
        } // END OF SAVE SESSION



        private void savePhotoToolStripMenuItem_Click(object sender, EventArgs e)
        { // FILE / EXPORT IMAGE
            
            // export image pic of the current view
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Images|*.png;*.bmp;*.jpg";
            //ImageFormatConverter format = ImageFormatConverter.StandardValuesCollection;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(sfd.FileName);
                switch (ext)
                {
                    case ".jpg":
                        pictureBox1.Image.Save(sfd.FileName, ImageFormat.Jpeg);
                        break;
                    case ".bmp":
                        pictureBox1.Image.Save(sfd.FileName, ImageFormat.Bmp);
                        break;
                    default:
                        pictureBox1.Image.Save(sfd.FileName, ImageFormat.Png);
                        break;

                }
            }
        } // END EXPORT IMAGE



        private void endSessionToolStripMenuItem_Click(object sender, EventArgs e)
        { // FILE / CLOSE PROGRAM

            // close the program
            Application.Exit();
        }



        // MAPS **************************************************

        private void loadMapToolStripMenuItem_Click(object sender, EventArgs e)
        { // MAPS / LOAD A MAP
            
            // load a map 2 * 32 * height (1-32)
            if (map_view > 2)
            {
                MessageBox.Show("Select View: BG1, BG2, or BG3.");
                return;
            }

            byte[] map_array = new byte[2 * 32 * 32]; // 128 entries * 2 bytes, little endian
            int height, map_size, map_mod;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Select a Tile Map";
            openFileDialog1.Filter = "Tile Map (*.map)|*.map|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.FileStream fs = (System.IO.FileStream)openFileDialog1.OpenFile();

                if ((fs.Length > 2048) || (fs.Length < 64))
                {
                    MessageBox.Show("File size error. Expected 64 - 2048 bytes.",
                    "File size error", MessageBoxButtons.OK);
                }
                else
                {
                    map_size = (int)fs.Length; // how many bytes we need to copy
                    height = map_size / 64;
                    map_mod = map_size % 64; // should be exactly divisible by 64
                    if (map_mod != 0)
                    {
                        MessageBox.Show("File size error. Expected multiple of 64.",
                        "File size error", MessageBoxButtons.OK);
                    }
                    else
                    {

                        for (int i = 0; i < map_size; i++)
                        {
                            map_array[i] = (byte)fs.ReadByte();
                        }

                        int offset = map_view * 32 * 32;

                        //copy it here
                        //int half_size = map_size / 2;
                        for (int i = 0; i < map_size; i += 2)
                        {
                            byte weird_byte = map_array[i + 1];
                            int tile = map_array[i] + ((weird_byte & 3) << 8);
                            Maps.tile[offset] = tile;
                            int pal = (weird_byte >> 2) & 7;
                            Maps.palette[offset] = pal;
                            int pri = (weird_byte >> 5) & 1;
                            Maps.priority[offset] = pri;
                            int h_flip = (weird_byte >> 6) & 1;
                            Maps.h_flip[offset] = h_flip;
                            int v_flip = (weird_byte >> 7) & 1;
                            Maps.v_flip[offset] = v_flip;
                            offset++;
                        }

                        offset = map_view * 32 * 32;
                        if (Maps.priority[offset] == 0) checkBox3.Checked = false;
                        else checkBox3.Checked = true;
                    }
                }

                fs.Close();

                update_tilemap();
            }
        } // END OF LOAD A MAP



        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        { // MAPS / SAVE A 32 x 32 MAP
            
            // save a full size map, the current one
            if (map_view > 2)
            {
                MessageBox.Show("Select View: BG1, BG2, or BG3.");
                return;
            }

            byte[] map_array = new byte[2048]; // 2 bytes * 32 * 32
            int temp, offset;

            offset = (map_view * 32 * 32);
            for (int i = 0; i < 1024; i++)
            {
                map_array[(i * 2)] = (byte)(Maps.tile[offset] & 0xff); // the low byte
                temp = ((Maps.tile[offset] >> 8) & 3) + ((Maps.palette[offset] & 7) << 2) +
                    ((Maps.priority[offset] & 1) << 5) + ((Maps.h_flip[offset] & 1) << 6) +
                    ((Maps.v_flip[offset] & 1) << 7); // mishmash of weird bits
                // VHoP PPcc
                // VH flip, o priority, palette, upper 2 bits of tile number
                map_array[(i * 2) + 1] = (byte)temp;
                offset++;
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Tile Map (*.map)|*.map|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save a Tile Map 32x32";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                for (int i = 0; i < 2048; i++)
                {
                    fs.WriteByte(map_array[i]);
                }
                fs.Close();
            }
        } // END OF SAVE 32 x 32 MAP



        private void saveAMap224ToolStripMenuItem_Click(object sender, EventArgs e)
        { // MAPS / SAVE 32 x HEIGHT MAP
            
            // save a map at a specific height
            if (map_view > 2)
            {
                MessageBox.Show("Select View: BG1, BG2, or BG3.");
                return;
            }
            if ((map_height < 1) || (map_height > 32))
            {
                MessageBox.Show("Map Height needs to be 1-32");
                return;
            }

            int size_h = map_height * 32 * 2;
            byte[] map_array = new byte[size_h]; // 2 bytes * 32 * 32
            int temp, offset;
            int size_h2 = size_h / 2;

            offset = (map_view * 32 * 32);
            for (int i = 0; i < size_h2; i++)
            {
                map_array[(i * 2)] = (byte)(Maps.tile[offset] & 0xff); // the low byte
                temp = ((Maps.tile[offset] >> 8) & 3) + ((Maps.palette[offset] & 7) << 2) +
                    ((Maps.priority[offset] & 1) << 5) + ((Maps.h_flip[offset] & 1) << 6) +
                    ((Maps.v_flip[offset] & 1) << 7); // mishmash of weird bits
                // VHoP PPcc
                // VH flip, o priority, palette, upper 2 bits of tile number
                map_array[(i * 2) + 1] = (byte)temp;
                offset++;
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Tile Map (*.map)|*.map|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save a Tile Map 32xHeight";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                for (int i = 0; i < size_h; i++)
                {
                    fs.WriteByte(map_array[i]);
                }
                fs.Close();
            }
        } // END OF SAVE 32 x HEIGHT MAP



        private void clearAllMapsToolStripMenuItem_Click(object sender, EventArgs e)
        { // MAPS / CLEAR ALL MAPS

            for (int i = 0; i < 3 * 32 * 32; i++)
            {
                Maps.tile[i] = 0;
                Maps.palette[i] = 0;
                Maps.priority[i] = 0;
                Maps.h_flip[i] = 0;
                Maps.v_flip[i] = 0;
            }

            common_update2();
            checkBox3.Checked = false; //priority
        }



        // TILES **************************************************

        private void load2bppSNESToolStripMenuItem_Click(object sender, EventArgs e)
        { // TILES / LOAD 2bpp
            if (map_view != 2)
            {
                MessageBox.Show("Select the 2bpp tileset first. View BG3.");
                return;
            }

            int[] bit1 = new int[8]; // bit planes
            int[] bit2 = new int[8];
            int temp1 = 0;
            int temp2 = 0;
            int[] temp_tiles = new int[0x4000];
            int size_temp_tiles = 0;
            
            // tile_set assumed to be 4-7
            // so offset_tiles_ar = 10000, 14000, 18000, or 1c000
            int offset_tiles_ar = 0x4000 * tile_set; // Tile_Arrays is 1 byte per pixel
            int num_sets = 1;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Select a 2bpp Tileset";
            openFileDialog1.Filter = "Tileset (*.chr)|*.chr|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.FileStream fs = (System.IO.FileStream)openFileDialog1.OpenFile();
                if (fs.Length >= 16) // at least one tile.
                {
                    size_temp_tiles = (int)fs.Length & 0xf000; // round down to nearest 1000
                    
                    if(((int)fs.Length & 0x0fff) > 0) // handle weird sizes.
                    {
                        // just bump up to next size.
                        size_temp_tiles = size_temp_tiles + 0x1000;
                    }
                    if (size_temp_tiles < 0x1000) // min, 1 tileset worth.
                    {
                        size_temp_tiles = 0x1000;
                    }
                    if (size_temp_tiles > 0x4000)
                    {
                        size_temp_tiles = 0x4000; // max, 4 tilesets worth.
                    }
                    if(size_temp_tiles == 0x4000)
                    {
                        // offset_tiles_ar = 10000, 14000, 18000, or 1c000
                        offset_tiles_ar = 0x10000;
                        num_sets = 4;
                    }
                    if (size_temp_tiles == 0x3000) // 3 sets
                    {
                        if (tile_set == 4)
                        {
                            offset_tiles_ar = 0x10000;
                        }
                        else
                        {
                            offset_tiles_ar = 0x14000;
                        }
                        num_sets = 3;
                    }
                    if (size_temp_tiles == 0x2000) // middle size, 2 sets
                    {
                        if(tile_set < 6)
                        {
                            offset_tiles_ar = 0x10000;
                        }
                        else
                        {
                            offset_tiles_ar = 0x18000;
                        }
                        num_sets = 2;
                    }
                    //note, if 0x1000, already set above

                    // make sure don't try to copy more bytes than exist.
                    int min_size = size_temp_tiles;
                    if(min_size > (int)fs.Length)
                    {
                        min_size = (int)fs.Length;
                    }

                    // copy file to the temp array.
                    for (int i = 0; i < min_size; i++)
                    {
                        temp_tiles[i] = (byte)fs.ReadByte();
                    }


                    for(int temp_set = 0; temp_set < num_sets; temp_set++)
                    {
                        for (int i = 0; i < 256; i++) // 256 tiles
                        {
                            int index = (temp_set * 0x1000) + (16 * i); // start of current tile
                            for (int y = 0; y < 8; y++) // get 8 sets of bitplanes
                            {
                                // get the 4 bitplanes for each tile row
                                int y2 = y * 2; //0,2,4,6,8,10,12,14
                                bit1[y] = temp_tiles[index + y2];
                                bit2[y] = temp_tiles[index + y2 + 1];

                                int offset = offset_tiles_ar + (temp_set * 256 * 8 * 8) + (i * 8 * 8) + (y * 8);
                                for (int x = 7; x >= 0; x--) // right to left
                                {
                                    temp1 = bit1[y] & 1;    // get a bit from each bitplane
                                    bit1[y] = bit1[y] >> 1;
                                    temp2 = bit2[y] & 1;
                                    bit2[y] = bit2[y] >> 1;
                                    Tiles.Tile_Arrays[offset  + x] =
                                        (temp2 << 1) + temp1;
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("File size error. Too small.",
                    "File size error", MessageBoxButtons.OK);
                }

                fs.Close();

                common_update2();
            }
        } // END OF LOAD 2bpp



        private void load4bppSNESToolStripMenuItem_Click(object sender, EventArgs e)
        { // TILES / LOAD 4bpp
            if (map_view > 1)
            {
                MessageBox.Show("Select a 4bpp tileset first. View BG1 or BG2.");
                return;
            }

            int[] bit1 = new int[8]; // bit planes
            int[] bit2 = new int[8];
            int[] bit3 = new int[8];
            int[] bit4 = new int[8];
            int temp1 = 0;
            int temp2 = 0;
            int temp3 = 0;
            int temp4 = 0;
            int[] temp_tiles = new int[0x8000];
            int size_temp_tiles = 0;

            // tile_set assumed to be 0-3
            // so offset_tiles_ar = 0, 4000, 8000, or c000
            int offset_tiles_ar = 0x4000 * tile_set; // Tile_Arrays is 1 byte per pixel
            int num_sets = 1;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Select a 4bpp Tileset";
            openFileDialog1.Filter = "Tileset (*.chr)|*.chr|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.FileStream fs = (System.IO.FileStream)openFileDialog1.OpenFile();
                if (fs.Length >= 16) // at least one tile.
                {
                    size_temp_tiles = (int)fs.Length & 0xf000; // round down to nearest 1000

                    if (((int)fs.Length & 0x1fff) > 0) // handle weird sizes.
                    {
                        // just bump up to next size.
                        size_temp_tiles = size_temp_tiles + 0x2000;
                    }
                    if (size_temp_tiles < 0x2000) // min, 1 tileset worth.
                    {
                        size_temp_tiles = 0x2000;
                    }
                    if (size_temp_tiles > 0x8000)
                    {
                        size_temp_tiles = 0x8000; // max, 4 tilesets worth.
                    }
                    if (size_temp_tiles == 0x8000)
                    {
                        // offset_tiles_ar = 10000, 14000, 18000, or 1c000
                        offset_tiles_ar = 0;
                        num_sets = 4;
                    }
                    if (size_temp_tiles == 0x6000) // 3 sets
                    {
                        if (tile_set == 0)
                        {
                            offset_tiles_ar = 0;
                        }
                        else
                        {
                            offset_tiles_ar = 0x4000;
                        }
                        num_sets = 3;
                    }
                    if (size_temp_tiles == 0x4000) // middle size, 2 sets
                    {
                        if (tile_set < 2)
                        {
                            offset_tiles_ar = 0;
                        }
                        else
                        {
                            offset_tiles_ar = 0x8000;
                        }
                        num_sets = 2;
                    }
                    //note, if 0x2000, already set above

                    // make sure don't try to copy more bytes than exist.
                    int min_size = size_temp_tiles;
                    if (min_size > (int)fs.Length)
                    {
                        min_size = (int)fs.Length;
                    }

                    // copy file to the temp array.
                    for (int i = 0; i < min_size; i++)
                    {
                        temp_tiles[i] = (byte)fs.ReadByte();
                    }


                    for (int temp_set = 0; temp_set < num_sets; temp_set++)
                    {
                        for (int i = 0; i < 256; i++) // 256 tiles
                        {
                            int index = (temp_set * 0x2000) + (32 * i); // start of current tile
                            for (int y = 0; y < 8; y++) // get 8 sets of bitplanes
                            {
                                // get the 4 bitplanes for each tile row
                                int y2 = y * 2; //0,2,4,6,8,10,12,14
                                bit1[y] = temp_tiles[index + y2];
                                bit2[y] = temp_tiles[index + y2 + 1];
                                bit3[y] = temp_tiles[index + y2 + 16];
                                bit4[y] = temp_tiles[index + y2 + 17];

                                int offset = offset_tiles_ar + (temp_set * 256 * 8 * 8) + (i * 8 * 8) + (y * 8);
                                for (int x = 7; x >= 0; x--) // right to left
                                {
                                    temp1 = bit1[y] & 1;    // get a bit from each bitplane
                                    bit1[y] = bit1[y] >> 1;
                                    temp2 = bit2[y] & 1;
                                    bit2[y] = bit2[y] >> 1;
                                    temp3 = bit3[y] & 1;
                                    bit3[y] = bit3[y] >> 1;
                                    temp4 = bit4[y] & 1;
                                    bit4[y] = bit4[y] >> 1;
                                    Tiles.Tile_Arrays[offset + x] =
                                        (temp4 << 3) + (temp3 << 2) + (temp2 << 1) + temp1;
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("File size error. Too small.",
                    "File size error", MessageBoxButtons.OK);
                }

                fs.Close();

                common_update2();
            }
        } // END OF LOAD 4bpp



        



        private void save2bppSNESToolStripMenuItem_Click(object sender, EventArgs e)
        { // TILES / SAVE 2bpp x 1

            int[] bit1 = new int[8]; // bit planes
            int[] bit2 = new int[8];
            int temp;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Tileset (*.chr)|*.chr|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save a 2bpp Tileset";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                for (int i = 0; i < 256; i++) // 256 tiles
                {
                    int z = (4 * 256 * 8 * 8) + (64 * i); // start of current tile
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            temp = Tiles.Tile_Arrays[z + (y * 8) + x];
                            bit1[y] = (bit1[y] << 1) + (temp & 1);
                            bit2[y] = (bit2[y] << 1) + ((temp & 2) >> 1);
                        }
                    }
                    for (int j = 0; j < 8; j++)
                    {
                        fs.WriteByte((byte)bit1[j]);
                        fs.WriteByte((byte)bit2[j]);
                    }
                }
                // 
                fs.Close();
            }
        } // END OF SAVE 2bpp x 1



        private void save2bppX4ToolStripMenuItem_Click(object sender, EventArgs e)
        { // TILES / SAVE 2bpp x 4
            int[] bit1 = new int[8]; // bit planes
            int[] bit2 = new int[8];
            int temp;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Tileset (*.chr)|*.chr|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save a 2bpp Tileset";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                for (int temp_set = 4; temp_set < 8; temp_set++)
                {
                    for (int i = 0; i < 256; i++) // 256 tiles
                    {
                        int z = (temp_set * 256 * 8 * 8) + (64 * i); // start of current tile
                        for (int y = 0; y < 8; y++)
                        {
                            for (int x = 0; x < 8; x++)
                            {
                                temp = Tiles.Tile_Arrays[z + (y * 8) + x];
                                bit1[y] = (bit1[y] << 1) + (temp & 1);
                                bit2[y] = (bit2[y] << 1) + ((temp & 2) >> 1);
                            }
                        }
                        for (int j = 0; j < 8; j++)
                        {
                            fs.WriteByte((byte)bit1[j]);
                            fs.WriteByte((byte)bit2[j]);
                        }
                    }
                }

                // 
                fs.Close();
            }
        } // END OF SAVE 2bpp x 4



        private void save4bppSNESToolStripMenuItem_Click(object sender, EventArgs e)
        { // TILES / SAVE 4bpp x 1
            if (map_view > 1)
            {
                MessageBox.Show("Select a 4bpp tileset first. View BG1 or BG2.");
                return;
            }

            int[] bit1 = new int[8]; // bit planes
            int[] bit2 = new int[8];
            int[] bit3 = new int[8];
            int[] bit4 = new int[8];
            int temp;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Tileset (*.chr)|*.chr|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save a 4bpp Tileset";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                for (int i = 0; i < 256; i++) // 256 tiles
                {
                    int z = (tile_set * 256 * 8 * 8) + (64 * i); // start of current tile
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            temp = Tiles.Tile_Arrays[z + (y * 8) + x];
                            bit1[y] = (bit1[y] << 1) + (temp & 1);
                            bit2[y] = (bit2[y] << 1) + ((temp & 2) >> 1);
                            bit3[y] = (bit3[y] << 1) + ((temp & 4) >> 2);
                            bit4[y] = (bit4[y] << 1) + ((temp & 8) >> 3);
                        }
                    }
                    for (int j = 0; j < 8; j++)
                    {
                        fs.WriteByte((byte)bit1[j]);
                        fs.WriteByte((byte)bit2[j]);
                    }
                    for (int j = 0; j < 8; j++)
                    {
                        fs.WriteByte((byte)bit3[j]);
                        fs.WriteByte((byte)bit4[j]);
                    }
                }
                // 
                fs.Close();
            }
        } // END OF SAVE 4bpp x 1



        private void save4bppX4ToolStripMenuItem_Click(object sender, EventArgs e)
        { // TILES / SAVE 4bpp x 4

            int[] bit1 = new int[8]; // bit planes
            int[] bit2 = new int[8];
            int[] bit3 = new int[8];
            int[] bit4 = new int[8];
            int temp;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Tileset (*.chr)|*.chr|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save all 4bpp Tilesets";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                for (int temp_set = 0; temp_set < 4; temp_set++)
                {
                    for (int i = 0; i < 256; i++) // 256 tiles
                    {
                        int z = (temp_set * 256 * 8 * 8) + (64 * i); // start of current tile
                        for (int y = 0; y < 8; y++)
                        {
                            for (int x = 0; x < 8; x++)
                            {
                                temp = Tiles.Tile_Arrays[z + (y * 8) + x];
                                bit1[y] = (bit1[y] << 1) + (temp & 1);
                                bit2[y] = (bit2[y] << 1) + ((temp & 2) >> 1);
                                bit3[y] = (bit3[y] << 1) + ((temp & 4) >> 2);
                                bit4[y] = (bit4[y] << 1) + ((temp & 8) >> 3);
                            }
                        }
                        for (int j = 0; j < 8; j++)
                        {
                            fs.WriteByte((byte)bit1[j]);
                            fs.WriteByte((byte)bit2[j]);
                        }
                        for (int j = 0; j < 8; j++)
                        {
                            fs.WriteByte((byte)bit3[j]);
                            fs.WriteByte((byte)bit4[j]);
                        }
                    }
                }


                // 
                fs.Close();
            }
        } // END OF SAVE 4bpp x 4



        private void clearAllTilesToolStripMenuItem1_Click(object sender, EventArgs e)
        { // TILES / CLEAR ALL TILES
            for (int i = 0; i < 131072; i++)
            {
                Tiles.Tile_Arrays[i] = 0;
            }
            common_update2();
        }



        // PALETTES **************************************************

        private void loadPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        { // PALETTE / LOAD FULL PALETTE
            byte[] pal_array = new byte[256]; // 128 entries * 2 bytes, little endian
            int temp;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Select a Palette file";
            openFileDialog1.Filter = "Palette files (*.pal)|*.pal|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.FileStream fs = (System.IO.FileStream)openFileDialog1.OpenFile();
                if (fs.Length == 256)
                {
                    for (int i = 0; i < 256; i++)
                    {
                        pal_array[i] = (byte)fs.ReadByte();
                    }

                    for (int i = 0; i < 256; i += 2)
                    {
                        int j;
                        temp = pal_array[i] + (pal_array[i + 1] << 8);
                        if ((i == 0x20) || (i == 0x40) || (i == 0x60) || (i == 0x80) ||
                            (i == 0xa0) || (i == 0xc0) || (i == 0xe0)) temp = 0;
                        // make the left most boxes black, but not the top most
                        j = i / 2;
                        Palettes.pal_r[j] = (byte)((temp & 0x001f) << 3);
                        Palettes.pal_g[j] = (byte)((temp & 0x03e0) >> 2);
                        Palettes.pal_b[j] = (byte)((temp & 0x7c00) >> 7);
                    }

                    // update the numbers in the boxes
                    temp = pal_x + (pal_y * 16);
                    textBox1.Text = Palettes.pal_r[temp].ToString();
                    textBox2.Text = Palettes.pal_g[temp].ToString();
                    textBox3.Text = Palettes.pal_b[temp].ToString();
                    
                    update_box4();
                    update_palette();
                    common_update2();
                }
                else
                {
                    MessageBox.Show("File size error. Expected 256 bytes.",
                    "File size error", MessageBoxButtons.OK);
                }

                fs.Close();
            }
        } // END OF LOAD FULL PALETTE



        private void load32BytesToolStripMenuItem_Click(object sender, EventArgs e)
        { // PALETTE / LOAD 32 bytes
            // load just 1 palette (16 colors = 32 bytes)
            byte[] pal_array = new byte[32]; // 16 entries * 2 bytes, little endian
            int temp;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Select a Palette file";
            openFileDialog1.Filter = "Palette files (*.pal)|*.pal|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.FileStream fs = (System.IO.FileStream)openFileDialog1.OpenFile();
                if (fs.Length == 32)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        pal_array[i] = (byte)fs.ReadByte();
                    }

                    for (int i = 0; i < 32; i += 2)
                    {
                        int j;
                        temp = pal_array[i] + (pal_array[i + 1] << 8);
                        if ((i == 0) && (pal_y != 0)) continue;
                        // skip the left most boxes, but not the top most

                        j = (i / 2) + (pal_y * 16);
                        Palettes.pal_r[j] = (byte)((temp & 0x001f) << 3);
                        Palettes.pal_g[j] = (byte)((temp & 0x03e0) >> 2);
                        Palettes.pal_b[j] = (byte)((temp & 0x7c00) >> 7);
                    }

                    // update the numbers in the boxes
                    temp = pal_x + (pal_y * 16);
                    textBox1.Text = Palettes.pal_r[temp].ToString();
                    textBox2.Text = Palettes.pal_g[temp].ToString();
                    textBox3.Text = Palettes.pal_b[temp].ToString();

                    update_box4();
                    update_palette();
                    common_update2();
                }
                else
                {
                    MessageBox.Show("File size error. Expected 32 bytes.",
                    "File size error", MessageBoxButtons.OK);
                }

                fs.Close();
            }
        } // END LOAD 32 byte palette



        private void loadPaletteFromRGBToolStripMenuItem_Click(object sender, EventArgs e)
        { // PALETTE / LOAD FROM RGB
            byte[] pal_array = new byte[384]; // 128 entries * 3 colors
            int temp;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Select a Palette file";
            openFileDialog1.Filter = "Palette files (*.pal)|*.pal|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.FileStream fs = (System.IO.FileStream)openFileDialog1.OpenFile();
                if ((fs.Length >= 3) && (fs.Length <= 768))
                {
                    for (int i = 0; i < 384; i++)
                    {
                        if (i == fs.Length) break;
                        pal_array[i] = (byte)fs.ReadByte();
                    }

                    int max = (int)fs.Length;
                    max = (max / 3) * 3; // should now be divisible by 3
                    int offset = 0;

                    for (int i = 0; i < 384; i += 3) //128 * 3 color
                    {
                        if (i >= max) break;
                        Palettes.pal_r[offset] = (byte)(pal_array[i] & 0xf8);
                        Palettes.pal_g[offset] = (byte)(pal_array[i + 1] & 0xf8);
                        Palettes.pal_b[offset] = (byte)(pal_array[i + 2] & 0xf8);
                        offset++;
                    }

                    // update the numbers in the boxes
                    temp = pal_x + (pal_y * 16);
                    textBox1.Text = Palettes.pal_r[temp].ToString();
                    textBox2.Text = Palettes.pal_g[temp].ToString();
                    textBox3.Text = Palettes.pal_b[temp].ToString();

                    update_box4();
                    update_palette();
                    common_update2();
                }
                else
                {
                    MessageBox.Show("File size error. Expected 3 - 768 bytes.",
                    "File size error", MessageBoxButtons.OK);
                }

                fs.Close();
            }
        } // END PALETTE LOAD FROM RGB



        private void savePaletteToolStripMenuItem_Click(object sender, EventArgs e)
        { // PALETTE / SAVE PALETTE
            byte[] pal_array = new byte[256]; // 128 entries * 2 bytes, little endian
            int temp;

            for (int i = 0; i < 128; i++)
            {
                temp = ((Palettes.pal_r[i] & 0xf8) >> 3) + ((Palettes.pal_g[i] & 0xf8) << 2) + ((Palettes.pal_b[i] & 0xf8) << 7);
                pal_array[(i * 2)] = (byte)(temp & 0xff); // little end first
                pal_array[(i * 2) + 1] = (byte)((temp >> 8) & 0x7f); // 15 bit palette
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Palette files (*.pal)|*.pal|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save a Palette";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                for (int i = 0; i < 256; i++)
                {
                    fs.WriteByte(pal_array[i]);
                }
                fs.Close();
            }
        }



        private void save32BytesToolStripMenuItem_Click(object sender, EventArgs e)
        { // PALETTE / SAVE 32 bytes
            // save just 1 palette (16 colors = 32 bytes)
            byte[] pal_array = new byte[32]; // 16 entries * 2 bytes, little endian
            int temp;

            for (int i = 0; i < 16; i++)
            {
                int j = i + (pal_y * 16);
                temp = ((Palettes.pal_r[j] & 0xf8) >> 3) + ((Palettes.pal_g[j] & 0xf8) << 2) + ((Palettes.pal_b[j] & 0xf8) << 7);
                pal_array[(i * 2)] = (byte)(temp & 0xff); // little end first
                pal_array[(i * 2) + 1] = (byte)((temp >> 8) & 0x7f); // 15 bit palette
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Palette files (*.pal)|*.pal|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save a Palette";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                for (int i = 0; i < 32; i++)
                {
                    fs.WriteByte(pal_array[i]);
                }
                fs.Close();
            }
        }



        private void savePaletteAsASMToolStripMenuItem_Click(object sender, EventArgs e)
        { // PALETTE / SAVE PALETTE AS ASM
            byte[] pal_array = new byte[256]; // 128 entries * 2 bytes, little endian
            int temp;

            for (int i = 0; i < 128; i++)
            {
                temp = ((Palettes.pal_r[i] & 0xf8) >> 3) + ((Palettes.pal_g[i] & 0xf8) << 2) + ((Palettes.pal_b[i] & 0xf8) << 7);
                pal_array[(i * 2)] = (byte)(temp & 0xff); // little end first
                pal_array[(i * 2) + 1] = (byte)((temp >> 8) & 0x7f); // 15 bit palette
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "ASM File (*.asm)|*.asm|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save a Palette as ASM";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.OpenFile()))
                {
                    int count = 0;
                    string str = "";
                    sw.Write("Palette:\r\n");
                    for (int i = 0; i < 16; i++)
                    {
                        sw.Write("\r\n.byte ");
                        for (int j = 0; j < 8; j++)
                        {
                            str = pal_array[count].ToString("X2"); // convert int to hex string
                            sw.Write("$" + str + ", ");
                            count++;
                            str = pal_array[count].ToString("X2");
                            sw.Write("$" + str);
                            if (j < 7)
                            {
                                sw.Write(", ");
                            }
                            count++;
                        }
                    }
                    sw.Write("\r\n\r\n");
                    sw.Close();
                }
            }
        } // END SAVE PALETTE AS ASM



        private void savePalAsRGBToolStripMenuItem_Click(object sender, EventArgs e)
        { // PALETTE / SAVE PAL AS RBG (for YY-CHR)
            byte[] pal_array = new byte[384]; // 128 entries * 3 = r,g,b
            int temp;
            int offset = 0;
            for (int i = 0; i < 128; i++)
            {
                pal_array[offset++] = (byte)(Palettes.pal_r[i] & 0xf8);
                pal_array[offset++] = (byte)(Palettes.pal_g[i] & 0xf8);
                pal_array[offset++] = (byte)(Palettes.pal_b[i] & 0xf8);
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Palette (*.pal)|*.pal|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save a Palette";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                for (int i = 0; i < 384; i++)
                {
                    fs.WriteByte(pal_array[i]);
                }
                fs.Close();
            }
        } // END SAVE PAL AS RGB



        // BG VIEW **************************************************

        private void bG1TopToolStripMenuItem_Click(object sender, EventArgs e)
        { // BG VIEW / BG1

            bG1TopToolStripMenuItem.Checked = true;
            bG2ToolStripMenuItem.Checked = false;
            bG3ToolStripMenuItem.Checked = false;
            previewAllToolStripMenuItem.Checked = false;
            preview312ToolStripMenuItem.Checked = false;
            label11.Text = "BG1";
            map_view = 0; //view BG1
            //if tile set is 2bpp, switch it to 0th
            if (tile_set > 3)
            {
                set14bppToolStripMenuItem.Checked = true;
                set24bppToolStripMenuItem.Checked = false;
                set34bppToolStripMenuItem.Checked = false;
                set44bppToolStripMenuItem.Checked = false;
                set52bppToolStripMenuItem.Checked = false;
                set62bppToolStripMenuItem.Checked = false;
                set72bppToolStripMenuItem.Checked = false;
                set82bppToolStripMenuItem.Checked = false;
                label10.Text = "1";
                tile_set = 0;
                common_update2(); // includes map
                update_palette();
            }
            else
            {
                update_tilemap();
            }

            if (Maps.priority[0] == 0) checkBox3.Checked = false;
            else checkBox3.Checked = true; //priority
        }



        private void bG2ToolStripMenuItem_Click(object sender, EventArgs e)
        { // BG VIEW / BG2

            bG1TopToolStripMenuItem.Checked = false;
            bG2ToolStripMenuItem.Checked = true;
            bG3ToolStripMenuItem.Checked = false;
            previewAllToolStripMenuItem.Checked = false;
            preview312ToolStripMenuItem.Checked = false;
            label11.Text = "BG2";
            map_view = 1; //view BG2
            //if tile set is 2bpp, switch it to 0th
            if (tile_set > 3)
            {
                set14bppToolStripMenuItem.Checked = true;
                set24bppToolStripMenuItem.Checked = false;
                set34bppToolStripMenuItem.Checked = false;
                set44bppToolStripMenuItem.Checked = false;
                set52bppToolStripMenuItem.Checked = false;
                set62bppToolStripMenuItem.Checked = false;
                set72bppToolStripMenuItem.Checked = false;
                set82bppToolStripMenuItem.Checked = false;
                label10.Text = "1";
                tile_set = 0;
                common_update2(); // includes map
                update_palette();
            }
            else
            {
                update_tilemap();
            }
            if (Maps.priority[32 * 32] == 0) checkBox3.Checked = false;
            else checkBox3.Checked = true; //priority
        }



        private void bG3ToolStripMenuItem_Click(object sender, EventArgs e)
        { // BG VIEW / BG3

            bG1TopToolStripMenuItem.Checked = false;
            bG2ToolStripMenuItem.Checked = false;
            bG3ToolStripMenuItem.Checked = true;
            previewAllToolStripMenuItem.Checked = false;
            preview312ToolStripMenuItem.Checked = false;
            label11.Text = "BG3";
            map_view = 2; //view BG3 2bpp mode
            if (pal_y > 1) pal_y = 1; // palette y selection
            //make sure correct tileset
            if (tile_set < 4)
            {
                set14bppToolStripMenuItem.Checked = false;
                set24bppToolStripMenuItem.Checked = false;
                set34bppToolStripMenuItem.Checked = false;
                set44bppToolStripMenuItem.Checked = false;
                set52bppToolStripMenuItem.Checked = true;
                set62bppToolStripMenuItem.Checked = false;
                set72bppToolStripMenuItem.Checked = false;
                set82bppToolStripMenuItem.Checked = false;
                label10.Text = "5";
                tile_set = 4;
                common_update2(); // includes map
                update_palette();
            }
            else
            {
                update_tilemap();
            }
            if (Maps.priority[2 * 32 * 32] == 0) checkBox3.Checked = false;
            else checkBox3.Checked = true; //priority
        }



        private void previewAllToolStripMenuItem_Click(object sender, EventArgs e)
        { // BG VIEW / preview 1/2/3

            bG1TopToolStripMenuItem.Checked = false;
            bG2ToolStripMenuItem.Checked = false;
            bG3ToolStripMenuItem.Checked = false;
            previewAllToolStripMenuItem.Checked = true;
            preview312ToolStripMenuItem.Checked = false;
            label11.Text = "Preview 1/2/3";
            map_view = 3; // preview all
            if (newChild != null)
            {
                newChild.Close();
                newChild = null;
            }
            if (tile_set > 3)
            {
                set14bppToolStripMenuItem.Checked = true;
                set24bppToolStripMenuItem.Checked = false;
                set34bppToolStripMenuItem.Checked = false;
                set44bppToolStripMenuItem.Checked = false;
                set52bppToolStripMenuItem.Checked = false;
                set62bppToolStripMenuItem.Checked = false;
                set72bppToolStripMenuItem.Checked = false;
                set82bppToolStripMenuItem.Checked = false;
                tile_set = 0;
                label10.Text = "1";
                update_palette();
            }
            common_update2(); // includes tile map
            checkBox3.Checked = false; //priority
        }



        private void preview312ToolStripMenuItem_Click(object sender, EventArgs e)
        { // BG VIEW / preview 3/1/2

            bG1TopToolStripMenuItem.Checked = false;
            bG2ToolStripMenuItem.Checked = false;
            bG3ToolStripMenuItem.Checked = false;
            previewAllToolStripMenuItem.Checked = false;
            preview312ToolStripMenuItem.Checked = true;
            label11.Text = "Preview 3/1/2";
            map_view = 4; // preview all
            if (newChild != null)
            {
                newChild.Close();
                newChild = null;
            }
            if (tile_set > 3)
            {
                set14bppToolStripMenuItem.Checked = true;
                set24bppToolStripMenuItem.Checked = false;
                set34bppToolStripMenuItem.Checked = false;
                set44bppToolStripMenuItem.Checked = false;
                set52bppToolStripMenuItem.Checked = false;
                set62bppToolStripMenuItem.Checked = false;
                set72bppToolStripMenuItem.Checked = false;
                set82bppToolStripMenuItem.Checked = false;
                tile_set = 0;
                label10.Text = "1";
                update_palette();
            }
            common_update2(); // includes tile map
            checkBox3.Checked = false; //priority
        }



        // TILESET **************************************************

        private void set14bppToolStripMenuItem_Click(object sender, EventArgs e)
        { // TILESET / SET 1

            set14bppToolStripMenuItem.Checked = true;
            set24bppToolStripMenuItem.Checked = false;
            set34bppToolStripMenuItem.Checked = false;
            set44bppToolStripMenuItem.Checked = false;
            set52bppToolStripMenuItem.Checked = false;
            set62bppToolStripMenuItem.Checked = false;
            set72bppToolStripMenuItem.Checked = false;
            set82bppToolStripMenuItem.Checked = false;
            tile_set = 0;
            label10.Text = "1";

            if (newChild != null)
            {
                newChild.update_tile_box();
            }
            if (map_view > 1)
            {
                bG1TopToolStripMenuItem.Checked = true;
                bG2ToolStripMenuItem.Checked = false;
                bG3ToolStripMenuItem.Checked = false;
                previewAllToolStripMenuItem.Checked = false;
                preview312ToolStripMenuItem.Checked = false;
                label11.Text = "BG1";
                map_view = 0;
                update_tilemap();
            }

            common_update2(); // includes map
            update_palette();
        }



        private void set24bppToolStripMenuItem_Click(object sender, EventArgs e)
        { // TILESET / SET 2

            set14bppToolStripMenuItem.Checked = false;
            set24bppToolStripMenuItem.Checked = true;
            set34bppToolStripMenuItem.Checked = false;
            set44bppToolStripMenuItem.Checked = false;
            set52bppToolStripMenuItem.Checked = false;
            set62bppToolStripMenuItem.Checked = false;
            set72bppToolStripMenuItem.Checked = false;
            set82bppToolStripMenuItem.Checked = false;
            tile_set = 1;
            label10.Text = "2";

            if (newChild != null)
            {
                newChild.update_tile_box();
            }
            if (map_view > 1)
            {
                bG1TopToolStripMenuItem.Checked = true;
                bG2ToolStripMenuItem.Checked = false;
                bG3ToolStripMenuItem.Checked = false;
                previewAllToolStripMenuItem.Checked = false;
                preview312ToolStripMenuItem.Checked = false;
                label11.Text = "BG1";
                map_view = 0;
                update_tilemap();
            }

            common_update2(); // includes map
            update_palette();
        }



        private void set32bppToolStripMenuItem_Click(object sender, EventArgs e)
        { // TILESET / SET 3

            set14bppToolStripMenuItem.Checked = false;
            set24bppToolStripMenuItem.Checked = false;
            set34bppToolStripMenuItem.Checked = true;
            set44bppToolStripMenuItem.Checked = false;
            set52bppToolStripMenuItem.Checked = false;
            set62bppToolStripMenuItem.Checked = false;
            set72bppToolStripMenuItem.Checked = false;
            set82bppToolStripMenuItem.Checked = false;
            tile_set = 2;
            label10.Text = "3";

            if (newChild != null)
            {
                newChild.update_tile_box();
            }
            if (map_view > 1)
            {
                bG1TopToolStripMenuItem.Checked = true;
                bG2ToolStripMenuItem.Checked = false;
                bG3ToolStripMenuItem.Checked = false;
                previewAllToolStripMenuItem.Checked = false;
                preview312ToolStripMenuItem.Checked = false;
                label11.Text = "BG1";
                map_view = 0;
                update_tilemap();
            }

            common_update2(); // includes map
            update_palette();
        }



        private void set44bppToolStripMenuItem_Click(object sender, EventArgs e)
        { // TILESET / SET 4

            set14bppToolStripMenuItem.Checked = false;
            set24bppToolStripMenuItem.Checked = false;
            set34bppToolStripMenuItem.Checked = false;
            set44bppToolStripMenuItem.Checked = true;
            set52bppToolStripMenuItem.Checked = false;
            set62bppToolStripMenuItem.Checked = false;
            set72bppToolStripMenuItem.Checked = false;
            set82bppToolStripMenuItem.Checked = false;
            tile_set = 3;
            label10.Text = "4";

            if (newChild != null)
            {
                newChild.update_tile_box();
            }
            if (map_view > 1)
            {
                bG1TopToolStripMenuItem.Checked = true;
                bG2ToolStripMenuItem.Checked = false;
                bG3ToolStripMenuItem.Checked = false;
                previewAllToolStripMenuItem.Checked = false;
                preview312ToolStripMenuItem.Checked = false;
                label11.Text = "BG1";
                map_view = 0;
                update_tilemap();
            }

            common_update2(); // includes map
            update_palette();
        }



        private void set52bppToolStripMenuItem_Click(object sender, EventArgs e)
        { // TILESET / SET 5

            set14bppToolStripMenuItem.Checked = false;
            set24bppToolStripMenuItem.Checked = false;
            set34bppToolStripMenuItem.Checked = false;
            set44bppToolStripMenuItem.Checked = false;
            set52bppToolStripMenuItem.Checked = true;
            set62bppToolStripMenuItem.Checked = false;
            set72bppToolStripMenuItem.Checked = false;
            set82bppToolStripMenuItem.Checked = false;
            tile_set = 4;
            label10.Text = "5";

            if (newChild != null)
            {
                newChild.update_tile_box();
            }

            //assume that the BG map isn't right, and set it, and update all
            bG1TopToolStripMenuItem.Checked = false;
            bG2ToolStripMenuItem.Checked = false;
            bG3ToolStripMenuItem.Checked = true;
            previewAllToolStripMenuItem.Checked = false;
            preview312ToolStripMenuItem.Checked = false;
            label11.Text = "BG3";
            map_view = 2;
            if (pal_y > 1) pal_y = 1; // palette y selection

            common_update2(); // includes map
            update_palette();
        }



        private void set62bppToolStripMenuItem_Click(object sender, EventArgs e)
        { // TILESET / SET 6

            set14bppToolStripMenuItem.Checked = false;
            set24bppToolStripMenuItem.Checked = false;
            set34bppToolStripMenuItem.Checked = false;
            set44bppToolStripMenuItem.Checked = false;
            set52bppToolStripMenuItem.Checked = false;
            set62bppToolStripMenuItem.Checked = true;
            set72bppToolStripMenuItem.Checked = false;
            set82bppToolStripMenuItem.Checked = false;
            tile_set = 5;
            label10.Text = "6";

            if (newChild != null)
            {
                newChild.update_tile_box();
            }

            //assume that the BG map isn't right, and set it, and update all
            bG1TopToolStripMenuItem.Checked = false;
            bG2ToolStripMenuItem.Checked = false;
            bG3ToolStripMenuItem.Checked = true;
            previewAllToolStripMenuItem.Checked = false;
            preview312ToolStripMenuItem.Checked = false;
            label11.Text = "BG3";
            map_view = 2;
            if (pal_y > 1) pal_y = 1; // palette y selection

            common_update2(); // includes map
            update_palette();
        }



        private void set72bppToolStripMenuItem_Click(object sender, EventArgs e)
        { // TILESET / SET 7

            set14bppToolStripMenuItem.Checked = false;
            set24bppToolStripMenuItem.Checked = false;
            set34bppToolStripMenuItem.Checked = false;
            set44bppToolStripMenuItem.Checked = false;
            set52bppToolStripMenuItem.Checked = false;
            set62bppToolStripMenuItem.Checked = false;
            set72bppToolStripMenuItem.Checked = true;
            set82bppToolStripMenuItem.Checked = false;
            tile_set = 6;
            label10.Text = "7";

            if (newChild != null)
            {
                newChild.update_tile_box();
            }

            //assume that the BG map isn't right, and set it, and update all
            bG1TopToolStripMenuItem.Checked = false;
            bG2ToolStripMenuItem.Checked = false;
            bG3ToolStripMenuItem.Checked = true;
            previewAllToolStripMenuItem.Checked = false;
            preview312ToolStripMenuItem.Checked = false;
            label11.Text = "BG3";
            map_view = 2;
            if (pal_y > 1) pal_y = 1; // palette y selection

            common_update2(); // includes map
            update_palette();
        }



        private void set82bppToolStripMenuItem_Click(object sender, EventArgs e)
        { // TILESET / SET 8

            set14bppToolStripMenuItem.Checked = false;
            set24bppToolStripMenuItem.Checked = false;
            set34bppToolStripMenuItem.Checked = false;
            set44bppToolStripMenuItem.Checked = false;
            set52bppToolStripMenuItem.Checked = false;
            set62bppToolStripMenuItem.Checked = false;
            set72bppToolStripMenuItem.Checked = false;
            set82bppToolStripMenuItem.Checked = true;
            tile_set = 7;
            label10.Text = "8";

            if (newChild != null)
            {
                newChild.update_tile_box();
            }

            //assume that the BG map isn't right, and set it, and update all
            bG1TopToolStripMenuItem.Checked = false;
            bG2ToolStripMenuItem.Checked = false;
            bG3ToolStripMenuItem.Checked = true;
            previewAllToolStripMenuItem.Checked = false;
            preview312ToolStripMenuItem.Checked = false;
            label11.Text = "BG3";
            map_view = 2;
            if (pal_y > 1) pal_y = 1; // palette y selection

            common_update2(); // includes map
            update_palette();
        }



        // INFO ******************************

        
        private void aboutM1TEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("M1TE = Mode 1 Tile Editor for SNES, by Doug Fraker, 2019.\n\nVersion 1.2");
        }
        


    }
}
