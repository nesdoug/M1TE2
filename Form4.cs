using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace M1TE2
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        public static int start_val, end_val;
        public static bool skipTextChange = false;

        private void Form4_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1.close_it4();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (skipTextChange == true) return;

            string str = textBox1.Text;
            if (str == "") return;


            skipTextChange = true;

            int value = 0;
            int.TryParse(str, out value);
            if (value > 1023) value = 1023; // max value
            if (value < 0) value = 0; // min value
            str = value.ToString();
            textBox1.Text = str;
            start_val = value;
            skipTextChange = false;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (skipTextChange == true) return;

            string str = textBox2.Text;
            if (str == "") return;

            skipTextChange = true;

            int value = 0;
            int.TryParse(str, out value);
            if (value > 1023) value = 1023; // max value
            if (value < 0) value = 0; // min value
            str = value.ToString();
            textBox2.Text = str;
            end_val = value;
            skipTextChange = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Form1.map_view > 2)
            {
                MessageBox.Show("First select BG View = 1,2, or 3.");
                return;
            }
            if ((start_val < 0) || (start_val > 1023))
            {
                MessageBox.Show("Error. Start value is out of range.");
                return;
            }
            if ((end_val < 0) || (end_val > 1023))
            {
                MessageBox.Show("Error. End value is out of range.");
                return;
            }
            if(start_val > end_val)
            {
                MessageBox.Show("Error. Start value > end value.");
                return;
            }

            // 16 or 32 bytes per tile x 1024 possible tiles = 32768
            byte[] out_array = new byte[32768];
            int out_size = 0;
            int end_val2 = end_val + 1;
            int out_index = 0;
            int[] bit1 = new int[8]; // bit planes
            int[] bit2 = new int[8];
            int[] bit3 = new int[8];
            int[] bit4 = new int[8];
            int temp;

            if (Form1.map_view == 2) // 2bpp tiles
            {
                out_size = (end_val2 - start_val) * 16; // 16 bytes per tile

                for (int i = start_val; i < end_val2; i++)
                {
                    int z = i * 64; // 64 bytes per tile // start of current tile
                    z += 65536; // the 2bpp tiles use the 5-8 tilesets
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
                        out_array[out_index++] = (byte)(bit1[j]);
                        out_array[out_index++] = (byte)(bit2[j]);
                    }
                }
            }
            else // 4bpp tiles
            {
                out_size = (end_val2 - start_val) * 32; // 32 bytes per tile

                for (int i = start_val; i < end_val2; i++)
                {
                    int z = i * 64; // 64 bytes per tile // start of current tile
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
                        out_array[out_index++] = (byte)(bit1[j]);
                        out_array[out_index++] = (byte)(bit2[j]);
                    }
                    for (int j = 0; j < 8; j++)
                    {
                        out_array[out_index++] = (byte)(bit3[j]);
                        out_array[out_index++] = (byte)(bit4[j]);
                    }
                }
            }

            // now save it
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Tileset (*.chr)|*.chr|RLE File (*.rle)|*.rle";
            saveFileDialog1.Title = "Save all 4bpp Tilesets";
            //saveFileDialog1.ShowDialog();

            Form1 f = (this.Owner as Form1);

            if ((saveFileDialog1.ShowDialog() == DialogResult.OK) && (saveFileDialog1.FileName != ""))
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                string ext = System.IO.Path.GetExtension(saveFileDialog1.FileName);
                if (ext == ".chr")
                {
                    for (int j = 0; j < out_size; j++)
                    {
                        fs.WriteByte(out_array[j]);
                    }

                    fs.Close();
                }
                else if (ext == ".rle")
                {
                    int rle_length = f.convert_RLE(out_array, out_size);
                    // global rle_array[] now has our compressed data
                    for (int i = 0; i < rle_length; i++)
                    {
                        fs.WriteByte(Form1.rle_array[i]);
                    }

                    float percent = (float)rle_length / out_size;
                    fs.Close();

                    MessageBox.Show(String.Format("RLE size is {0}, or {1:P2}", rle_length, percent));
                }
                else
                { // something went wrong.
                    fs.Close();
                }

                // close this form, only if result = ok
                this.Close();
            }

        }

        private void Form4_Load(object sender, EventArgs e)
        {
            start_val = 0;
            end_val = 255;

            textBox1.Text = start_val.ToString();
            textBox2.Text = end_val.ToString();
        }


    }
}
