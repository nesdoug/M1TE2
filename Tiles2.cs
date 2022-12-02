namespace M1TE2
{
    public static class TilesU // undo backup copy
    {
        public static int[] Tile_Arrays = new int[8 * 256 * 8 * 8]; // 131072 
    }

        public static class Tiles
    {
        public static int[] Tile_Arrays = new int[8 * 256 * 8 * 8]; // 131072 
                                                                    // 8 sets, 256 tiles, 8 high, 8 wide
                                                                    // 4 sets 4bpp allow values 0-15, 4 sets 2bpp allow values 0-3
        public static int[] Tile_Copier = new int[8 * 8]; // one tile
        public static int[] Tile_Copier16 = new int[16 * 16]; // one tile
        public static int[] Tile_Temp16 = new int[16 * 16]; // one tile
        public static bool Has_Copied = false;


        public static int[] trans_array = new int[16384]; // linear
        public static int[] copy_array = new int[16384];
        public static int trans_x, trans_y; // in tiles
        public static int trans_w = 1; // in tiles
        public static int trans_h = 1;
        public static int copy_x, copy_y; // in tiles
        public static int copy_w = 1; // in tiles
        public static int copy_h = 1;


        // you know, I regret the way I set up the tile array
        // and I think a linear format woud be easier to
        // work with for large transformations
        // this converts from tile array to a linear format
        public static void Tiles_2_Linear()
        {
            //in - globals trans_x, trans_y, trans_w, trans_h

            int start_x = trans_x * 8;
            int final_x = (trans_w * 8) + start_x;
            int start_y = trans_y * 8;
            int final_y = (trans_h * 8) + start_y;
            // should be values 0-127

            int offset = Form1.tile_set * 16384;

            for (int y1 = start_y; y1 < final_y; y1++)
            {
                for (int x1 = start_x; x1 < final_x; x1++)
                {
                    int tile_xH = x1 >> 3;
                    int tile_xL = x1 & 7;
                    int tile_yH = y1 >> 3;
                    int tile_yL = y1 & 7;
                    int index1 = (tile_yH * 1024) + (tile_xH * 64) + (tile_yL * 8) + tile_xL;
                    index1 += offset;
                    int index2 = (y1 * 128) + x1;
                    trans_array[index2] = Tile_Arrays[index1];
                }
            }

        }


        // this converts the linear / transformation array
        // back to the old tile format
        public static void Linear_2_Tiles()
        {
            //in - globals trans_x, trans_y, trans_w, trans_h

            int start_x = trans_x * 8;
            int final_x = (trans_w * 8) + start_x;
            int start_y = trans_y * 8;
            int final_y = (trans_h * 8) + start_y;
            // should be values 0-127

            int offset = Form1.tile_set * 16384;

            for (int y1 = start_y; y1 < final_y; y1++)
            {
                for (int x1 = start_x; x1 < final_x; x1++)
                {
                    int tile_xH = x1 >> 3;
                    int tile_xL = x1 & 7;
                    int tile_yH = y1 >> 3;
                    int tile_yL = y1 & 7;
                    int index1 = (tile_yH * 1024) + (tile_xH * 64) + (tile_yL * 8) + tile_xL;
                    index1 += offset;
                    int index2 = (y1 * 128) + x1;
                    Tile_Arrays[index1] = trans_array[index2];
                }
            }
        }


        public static void Make_Box_Same()
        {
            trans_x = Form1.BE_x1;
            trans_y = Form1.BE_y1;
            trans_w = Form1.BE_x2 - Form1.BE_x1;
            if (trans_w < 1) trans_y = 1;
            trans_h = Form1.BE_y2 - Form1.BE_y1;
            if (trans_h < 1) trans_h = 1;
        }


        public static void Nix_Copy()
        {
            Has_Copied = false;
        }

        public static void shift_left_big()
        {
            Tiles_2_Linear();

            int start_x = trans_x * 8;
            int final_x = (trans_w * 8) + start_x;
            int start_y = trans_y * 8;
            int final_y = (trans_h * 8) + start_y;

            for (int y1 = start_y; y1 < final_y; y1++)
            {
                int index = (y1 * 128) + start_x;
                int temp = trans_array[index];
                for (int x1 = start_x; x1 < final_x - 1; x1++)
                {
                    index = (y1 * 128) + x1;
                    trans_array[index] = trans_array[index + 1];
                }
                index = (y1 * 128) + final_x - 1;
                trans_array[index] = temp;
            }

            Linear_2_Tiles();
        }

        public static void shift_left()
        {
            if(Form1.brushsize == Form1.BRUSH_MULTI)
            {
                shift_left_big();
                return;
            }
            
            int z = (Form1.tile_set * 256 * 8 * 8) + (Form1.tile_num * 8 * 8); // base index

            if (Form1.tilesize == Form1.TILE_8X8)
            {
                for (int y = 0; y < 8; y++)
                {
                    int temp = Tile_Arrays[z + (y * 8)]; // save the left most
                    for (int x = 0; x < 7; x++)
                    {
                        Tile_Arrays[z + (y * 8) + x] = Tile_Arrays[z + (y * 8) + x + 1];
                    }
                    Tile_Arrays[z + (y * 8) + 7] = temp; // put it on the right
                }
            }
            else // 16x16
            {
                if ((Form1.tile_x > 14) || (Form1.tile_y > 14)) return;
                // this should never wrap around now

                int z2 = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 1) * 8 * 8); // base index
                for (int y = 0; y < 8; y++)
                {
                    int temp = Tile_Arrays[z + (y * 8)]; // save the left most
                    for (int x = 0; x < 7; x++)
                    {
                        Tile_Arrays[z + (y * 8) + x] = Tile_Arrays[z + (y * 8) + x + 1];
                    }
                    //transfer 1 pixel from R tile to L tile
                    Tile_Arrays[z + (y * 8) + 7] = Tile_Arrays[z2 + (y * 8)];

                    for (int x = 0; x < 7; x++)
                    {
                        Tile_Arrays[z2 + (y * 8) + x] = Tile_Arrays[z2 + (y * 8) + x + 1];
                    }
                    Tile_Arrays[z2 + (y * 8) + 7] = temp; // put it on the right
                }
                // one tile down
                z = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 16) * 8 * 8); // base index
                z2 = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 17) * 8 * 8); // base index
                for (int y = 0; y < 8; y++)
                {
                    int temp = Tile_Arrays[z + (y * 8)]; // save the left most
                    for (int x = 0; x < 7; x++)
                    {
                        Tile_Arrays[z + (y * 8) + x] = Tile_Arrays[z + (y * 8) + x + 1];
                    }
                    //transfer 1 pixel from R tile to L tile
                    Tile_Arrays[z + (y * 8) + 7] = Tile_Arrays[z2 + (y * 8)];

                    for (int x = 0; x < 7; x++)
                    {
                        Tile_Arrays[z2 + (y * 8) + x] = Tile_Arrays[z2 + (y * 8) + x + 1];
                    }
                    Tile_Arrays[z2 + (y * 8) + 7] = temp; // put it on the right
                }

            }

        }


        public static void shift_right_big()
        {
            Tiles_2_Linear();

            int start_x = trans_x * 8;
            int final_x = (trans_w * 8) + start_x;
            int start_y = trans_y * 8;
            int final_y = (trans_h * 8) + start_y;

            for (int y1 = start_y; y1 < final_y; y1++)
            {
                int index = (y1 * 128) + final_x - 1;
                int temp = trans_array[index];
                for (int x1 = final_x - 2; x1 >= start_x; x1--)
                {
                    index = (y1 * 128) + x1;
                    trans_array[index + 1] = trans_array[index];
                }
                index = (y1 * 128) + start_x;
                trans_array[index] = temp;
            }

            Linear_2_Tiles();
        }

        public static void shift_right()
        {
            if (Form1.brushsize == Form1.BRUSH_MULTI)
            {
                shift_right_big();
                return;
            }

            int z = (Form1.tile_set * 256 * 8 * 8) + (Form1.tile_num * 8 * 8); // base index

            if (Form1.tilesize == Form1.TILE_8X8)
            {
                for (int y = 0; y < 8; y++)
                {
                    int temp = Tile_Arrays[z + (y * 8) + 7]; // save the right most
                    for (int x = 6; x >= 0; x--)
                    {
                        Tile_Arrays[z + (y * 8) + x + 1] = Tile_Arrays[z + (y * 8) + x];
                    }
                    Tile_Arrays[z + (y * 8)] = temp; // put it on the left
                }
            }
            else // 16x16
            {
                if ((Form1.tile_x > 14) || (Form1.tile_y > 14)) return;
                // this should never wrap around now

                int z2 = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 1) * 8 * 8); // base index
                for (int y = 0; y < 8; y++)
                {
                    int temp = Tile_Arrays[z2 + (y * 8) + 7]; // save the right most
                    for (int x = 6; x >= 0; x--)
                    {
                        Tile_Arrays[z2 + (y * 8) + x + 1] = Tile_Arrays[z2 + (y * 8) + x];
                    }
                    //transfer 1 pixel from L tile to R tile
                    Tile_Arrays[z2 + (y * 8)] = Tile_Arrays[z + (y * 8) + 7];

                    for (int x = 6; x >= 0; x--)
                    {
                        Tile_Arrays[z + (y * 8) + x + 1] = Tile_Arrays[z + (y * 8) + x];
                    }
                    Tile_Arrays[z + (y * 8)] = temp; // put it on the left
                }
                // one tile down
                z = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 16) * 8 * 8); // base index
                z2 = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 17) * 8 * 8); // base index
                for (int y = 0; y < 8; y++)
                {
                    int temp = Tile_Arrays[z2 + (y * 8) + 7]; // save the right most
                    for (int x = 6; x >= 0; x--)
                    {
                        Tile_Arrays[z2 + (y * 8) + x + 1] = Tile_Arrays[z2 + (y * 8) + x];
                    }
                    //transfer 1 pixel from L tile to R tile
                    Tile_Arrays[z2 + (y * 8)] = Tile_Arrays[z + (y * 8) + 7];

                    for (int x = 6; x >= 0; x--)
                    {
                        Tile_Arrays[z + (y * 8) + x + 1] = Tile_Arrays[z + (y * 8) + x];
                    }
                    Tile_Arrays[z + (y * 8)] = temp; // put it on the left
                }
            }
        }


        public static void shift_up_big()
        {
            Tiles_2_Linear();

            int start_x = trans_x * 8;
            int final_x = (trans_w * 8) + start_x;
            int start_y = trans_y * 8;
            int final_y = (trans_h * 8) + start_y;

            for (int x1 = start_x; x1 < final_x; x1++)
            {
                int index = (start_y * 128) + x1;
                int temp = trans_array[index];
                for (int y1 = start_y; y1 < final_y - 1; y1++)
                {
                    index = (y1 * 128) + x1;
                    trans_array[index] = trans_array[index + 128];
                }
                index = ((final_y - 1) * 128) + x1;
                trans_array[index] = temp;
            }

            Linear_2_Tiles();
        }

        public static void shift_up()
        {
            if (Form1.brushsize == Form1.BRUSH_MULTI)
            {
                shift_up_big();
                return;
            }

            int z = (Form1.tile_set * 256 * 8 * 8) + (Form1.tile_num * 8 * 8); // base index
            if (Form1.tilesize == Form1.TILE_8X8) 
            {
                for (int x = 0; x < 8; x++)
                {
                    int temp = Tile_Arrays[z + x]; // save the top most
                    for (int y = 0; y < 7; y++)
                    {
                        Tile_Arrays[z + (y * 8) + x] = Tile_Arrays[z + ((y + 1) * 8) + x];
                    }
                    Tile_Arrays[z + 56 + x] = temp; // put it on the bottom
                }
            }
            else // 16x16
            {
                if ((Form1.tile_x > 14) || (Form1.tile_y > 14)) return;
                // this should never wrap around now

                int z2 = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 16) * 8 * 8); // base index
                for (int x = 0; x < 8; x++)
                {
                    int temp = Tile_Arrays[z + x]; // save the top most
                    for (int y = 0; y < 7; y++)
                    {
                        Tile_Arrays[z + (y * 8) + x] = Tile_Arrays[z + ((y + 1) * 8) + x];
                    }
                    //transfer 1 pixel from bottom tile to top
                    Tile_Arrays[z + 56 + x] = Tile_Arrays[z2 + x];
                    for (int y = 0; y < 7; y++)
                    {
                        Tile_Arrays[z2 + (y * 8) + x] = Tile_Arrays[z2 + ((y + 1) * 8) + x];
                    }
                    Tile_Arrays[z2 + 56 + x] = temp; // put it on the bottom
                }
                // one to the right
                z = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 1) * 8 * 8); // base index
                z2 = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 17) * 8 * 8); // base index
                for (int x = 0; x < 8; x++)
                {
                    int temp = Tile_Arrays[z + x]; // save the top most
                    for (int y = 0; y < 7; y++)
                    {
                        Tile_Arrays[z + (y * 8) + x] = Tile_Arrays[z + ((y + 1) * 8) + x];
                    }
                    //transfer 1 pixel from bottom tile to top
                    Tile_Arrays[z + 56 + x] = Tile_Arrays[z2 + x];
                    for (int y = 0; y < 7; y++)
                    {
                        Tile_Arrays[z2 + (y * 8) + x] = Tile_Arrays[z2 + ((y + 1) * 8) + x];
                    }
                    Tile_Arrays[z2 + 56 + x] = temp; // put it on the bottom
                }
            }
        }


        public static void shift_down_big()
        {
            Tiles_2_Linear();

            int start_x = trans_x * 8;
            int final_x = (trans_w * 8) + start_x;
            int start_y = trans_y * 8;
            int final_y = (trans_h * 8) + start_y;

            for (int x1 = start_x; x1 < final_x; x1++)
            {
                int index = ((final_y - 1) * 128) + x1;
                int temp = trans_array[index];
                for (int y1 = final_y - 2; y1 >= start_y; y1--)
                {
                    index = (y1 * 128) + x1;
                    trans_array[index + 128] = trans_array[index];
                }
                index = (start_y * 128) + x1;
                trans_array[index] = temp;
            }

            Linear_2_Tiles();
        }

        public static void shift_down()
        {
            if (Form1.brushsize == Form1.BRUSH_MULTI)
            {
                shift_down_big();
                return;
            }

            int z = (Form1.tile_set * 256 * 8 * 8) + (Form1.tile_num * 8 * 8); // base index
            if (Form1.tilesize == Form1.TILE_8X8)
            {
                for (int x = 0; x < 8; x++)
                {
                    int temp = Tile_Arrays[z + 56 + x]; // save the bottom most
                    for (int y = 6; y >= 0; y--)
                    {
                        Tile_Arrays[z + ((y + 1) * 8) + x] = Tile_Arrays[z + (y * 8) + x];
                    }
                    Tile_Arrays[z + x] = temp; // put it on the top
                }
            }
            else // 16x16
            {
                if ((Form1.tile_x > 14) || (Form1.tile_y > 14)) return;
                // this should never wrap around now

                int z2 = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 16) * 8 * 8); // base index
                for (int x = 0; x < 8; x++)
                {
                    int temp = Tile_Arrays[z2 + 56 + x]; // save the bottom most
                    for (int y = 6; y >= 0; y--)
                    {
                        Tile_Arrays[z2 + ((y + 1) * 8) + x] = Tile_Arrays[z2 + (y * 8) + x];
                    }
                    //transfer 1 pixel from top to bottom
                    Tile_Arrays[z2 + x] = Tile_Arrays[z + 56 + x];
                    for (int y = 6; y >= 0; y--)
                    {
                        Tile_Arrays[z + ((y + 1) * 8) + x] = Tile_Arrays[z + (y * 8) + x];
                    }
                    Tile_Arrays[z + x] = temp; // put it on the top
                }
                // one to the right
                z = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 1) * 8 * 8); // base index
                z2 = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 17) * 8 * 8); // base index
                for (int x = 0; x < 8; x++)
                {
                    int temp = Tile_Arrays[z2 + 56 + x]; // save the bottom most
                    for (int y = 6; y >= 0; y--)
                    {
                        Tile_Arrays[z2 + ((y + 1) * 8) + x] = Tile_Arrays[z2 + (y * 8) + x];
                    }
                    //transfer 1 pixel from top to bottom
                    Tile_Arrays[z2 + x] = Tile_Arrays[z + 56 + x];
                    for (int y = 6; y >= 0; y--)
                    {
                        Tile_Arrays[z + ((y + 1) * 8) + x] = Tile_Arrays[z + (y * 8) + x];
                    }
                    Tile_Arrays[z + x] = temp; // put it on the top
                }
            }
        }


        public static void tile_copy_big()
        {
            // just copy the entire set
            int offset = Form1.tile_set * 16384;
            for (int i = 0; i < 16384; i++)
            {
                copy_array[i] = Tile_Arrays[i + offset];
            }
            // remember the bounds
            copy_x = trans_x;
            copy_y = trans_y;
            copy_w = trans_w;
            copy_h = trans_h;

            Has_Copied = true;
        }

        public static void tile_copy()
        {
            if (Form1.brushsize == Form1.BRUSH_MULTI)
            {
                tile_copy_big();
                return;
            }

            int z = (Form1.tile_set * 256 * 8 * 8) + (Form1.tile_num * 8 * 8); // base index
            if (Form1.tilesize == Form1.TILE_8X8)
            {
                for (int x = 0; x < 64; x++)
                {
                    Tile_Copier[x] = Tile_Arrays[z + x];
                }
            }
            else // 16x16
            {
                if ((Form1.tile_x > 14) || (Form1.tile_y > 14)) return;
                // this should never wrap around now
                
                for (int x = 0; x < 64; x++)
                {
                    Tile_Copier16[x] = Tile_Arrays[z + x];
                }
                int temp_tile_num = (Form1.tile_num + 1);
                z = (Form1.tile_set * 256 * 8 * 8) + (temp_tile_num * 8 * 8);
                for (int x = 0; x < 64; x++)
                {
                    Tile_Copier16[x + 64] = Tile_Arrays[z + x];
                }
                temp_tile_num = (Form1.tile_num + 16);
                z = (Form1.tile_set * 256 * 8 * 8) + (temp_tile_num * 8 * 8);
                for (int x = 0; x < 64; x++)
                {
                    Tile_Copier16[x + 128] = Tile_Arrays[z + x];
                }
                temp_tile_num = (Form1.tile_num + 17);
                z = (Form1.tile_set * 256 * 8 * 8) + (temp_tile_num * 8 * 8);
                for (int x = 0; x < 64; x++)
                {
                    Tile_Copier16[x + 192] = Tile_Arrays[z + x];
                }
            }
            
            Has_Copied = true;
        }


        public static void tile_paste_big()
        {
            // copy_x, copy_y, copy_w, copy_h = bounds
            // copy_array[] = where, usual tile format
            int offset = Form1.tile_set * 16384;

            // 0, 64, 128, etc.
            // 1024

            for (int y1 = 0; y1 < copy_h; y1++) // in tiles
            {
                for (int x1 = 0; x1 < copy_w; x1++)
                {
                    if ((Form1.tile_x + x1) >= 16) continue; // no wrapping
                    if ((Form1.tile_y + y1) >= 16) continue; // no overflow
                    int src_index = ((copy_y + y1) * 1024) + ((copy_x + x1) * 64);
                    int dest_index = ((Form1.tile_y + y1) * 1024) + ((Form1.tile_x + x1) * 64);
                    if (dest_index >= 16384) continue; // no overflow
                    dest_index += offset;

                    //copy tile by tile, which is 64 values

                    if (Form1.map_view == 2) // 2bpp
                    {
                        for (int i = 0; i < 64; i++)
                        {
                            Tile_Arrays[dest_index + i] = copy_array[src_index + i] & 3;
                        }
                    }
                    else // 4bpp
                    {
                        for (int i = 0; i < 64; i++)
                        {
                            Tile_Arrays[dest_index + i] = copy_array[src_index + i];
                        }
                    }
                    
                }
            }
        }

        public static void tile_paste()
        {
            if (Has_Copied == false) return;

            if (Form1.brushsize == Form1.BRUSH_MULTI)
            {
                tile_paste_big();
                return;
            }

            int z = (Form1.tile_set * 256 * 8 * 8) + (Form1.tile_num * 8 * 8); // base index
            if (Form1.tilesize == Form1.TILE_8X8)
            {
                if (Form1.map_view == 2) // 2bpp
                {
                    for (int x = 0; x < 64; x++)
                    {
                        Tile_Arrays[z + x] = Tile_Copier[x] & 3; //values 0-3
                    }
                }
                else // 4bpp
                {
                    for (int x = 0; x < 64; x++)
                    {
                        Tile_Arrays[z + x] = Tile_Copier[x];
                    }
                }
            }
            else // 16x16
            {
                if ((Form1.tile_x > 14) || (Form1.tile_y > 14)) return;
                // this should never wrap around now

                if (Form1.map_view == 2) // 2bpp
                {
                    for (int x = 0; x < 64; x++)
                    {
                        Tile_Arrays[z + x] = Tile_Copier16[x] & 3;
                    }
                    int temp_tile_num = (Form1.tile_num + 1);
                    z = (Form1.tile_set * 256 * 8 * 8) + (temp_tile_num * 8 * 8);
                    for (int x = 0; x < 64; x++)
                    {
                        Tile_Arrays[z + x] = Tile_Copier16[x + 64] & 3;
                    }
                    temp_tile_num = (Form1.tile_num + 16);
                    z = (Form1.tile_set * 256 * 8 * 8) + (temp_tile_num * 8 * 8);
                    for (int x = 0; x < 64; x++)
                    {
                        Tile_Arrays[z + x] = Tile_Copier16[x + 128] & 3;
                    }
                    temp_tile_num = (Form1.tile_num + 17);
                    z = (Form1.tile_set * 256 * 8 * 8) + (temp_tile_num * 8 * 8);
                    for (int x = 0; x < 64; x++)
                    {
                        Tile_Arrays[z + x] = Tile_Copier16[x + 192] & 3;
                    }
                }
                else // 4bpp
                {
                    for (int x = 0; x < 64; x++)
                    {
                        Tile_Arrays[z + x] = Tile_Copier16[x];
                    }
                    int temp_tile_num = (Form1.tile_num + 1);
                    z = (Form1.tile_set * 256 * 8 * 8) + (temp_tile_num * 8 * 8);
                    for (int x = 0; x < 64; x++)
                    {
                        Tile_Arrays[z + x] = Tile_Copier16[x + 64];
                    }
                    temp_tile_num = (Form1.tile_num + 16);
                    z = (Form1.tile_set * 256 * 8 * 8) + (temp_tile_num * 8 * 8);
                    for (int x = 0; x < 64; x++)
                    {
                        Tile_Arrays[z + x] = Tile_Copier16[x + 128];
                    }
                    temp_tile_num = (Form1.tile_num + 17);
                    z = (Form1.tile_set * 256 * 8 * 8) + (temp_tile_num * 8 * 8);
                    for (int x = 0; x < 64; x++)
                    {
                        Tile_Arrays[z + x] = Tile_Copier16[x + 192];
                    }
                }
            }
        }


        public static void tile_delete_big()
        {
            Tiles_2_Linear();

            int start_x = trans_x * 8;
            int final_x = (trans_w * 8) + start_x;
            int start_y = trans_y * 8;
            int final_y = (trans_h * 8) + start_y;

            for (int y1 = start_y; y1 < final_y; y1++)
            {
                for (int x1 = start_x; x1 < final_x; x1++)
                {
                    int index = (y1 * 128) + x1;
                    trans_array[index] = 0;
                }
            }

            Linear_2_Tiles();
        }

        public static void tile_delete()
        {
            if (Form1.brushsize == Form1.BRUSH_MULTI)
            {
                tile_delete_big();
                return;
            }

            int z = (Form1.tile_set * 256 * 8 * 8) + (Form1.tile_num * 8 * 8); // base index
            if (Form1.tilesize == Form1.TILE_8X8)
            {
                for (int x = 0; x < 64; x++)
                {
                    Tile_Arrays[z + x] = 0;
                }
            }
            else // 16x16
            {
                if ((Form1.tile_x > 14) || (Form1.tile_y > 14)) return;
                // this should never wrap around now

                for (int x = 0; x < 64; x++)
                {
                    Tile_Arrays[z + x] = 0;
                }
                int temp_tile_num = (Form1.tile_num + 1);
                z = (Form1.tile_set * 256 * 8 * 8) + (temp_tile_num * 8 * 8);
                for (int x = 0; x < 64; x++)
                {
                    Tile_Arrays[z + x] = 0;
                }
                temp_tile_num = (Form1.tile_num + 16);
                z = (Form1.tile_set * 256 * 8 * 8) + (temp_tile_num * 8 * 8);
                for (int x = 0; x < 64; x++)
                {
                    Tile_Arrays[z + x] = 0;
                }
                temp_tile_num = (Form1.tile_num + 17);
                z = (Form1.tile_set * 256 * 8 * 8) + (temp_tile_num * 8 * 8);
                for (int x = 0; x < 64; x++)
                {
                    Tile_Arrays[z + x] = 0;
                }
            }
        }


        public static void tile_h_flip_big()
        {
            Tiles_2_Linear();

            int start_x = trans_x * 8;
            int final_x = (trans_w * 8) + start_x;
            int start_y = trans_y * 8;
            int final_y = (trans_h * 8) + start_y;
            int mid_x = trans_w * 4;

            for (int y1 = start_y; y1 < final_y; y1++)
            {
                for (int x1 = 0; x1 < mid_x; x1++)
                {
                    int left_x = start_x + x1;
                    int right_x = (final_x - x1) - 1;
                    int index1 = (y1 * 128) + left_x;
                    int index2 = (y1 * 128) + right_x;
                    int temp = trans_array[index1];
                    trans_array[index1] = trans_array[index2];
                    trans_array[index2] = temp;
                }
            }

            Linear_2_Tiles();
        }

        public static void tile_h_flip()
        {
            if (Form1.brushsize == Form1.BRUSH_MULTI)
            {
                tile_h_flip_big();
                return;
            }

            int z = (Form1.tile_set * 256 * 8 * 8) + (Form1.tile_num * 8 * 8); // base index
            if (Form1.tilesize == Form1.TILE_8X8)
            {
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        int temp = Tile_Arrays[z + (y * 8) + x];
                        Tile_Arrays[z + (y * 8) + x] = Tile_Arrays[z + (y * 8) + (7 - x)];
                        Tile_Arrays[z + (y * 8) + (7 - x)] = temp;
                    }
                }
            }
            else // 16x16
            {
                if ((Form1.tile_x > 14) || (Form1.tile_y > 14)) return;
                // this should never wrap around now
                int z2 = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 1) * 8 * 8); // base index
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        int temp = Tile_Arrays[z + (y * 8) + x];
                        Tile_Arrays[z + (y * 8) + x] = Tile_Arrays[z2 + (y * 8) + (7 - x)];
                        Tile_Arrays[z2 + (y * 8) + (7 - x)] = temp;
                    }
                }
                //lower
                z = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 16) * 8 * 8); // base index
                z2 = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 17) * 8 * 8); // base index
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        int temp = Tile_Arrays[z + (y * 8) + x];
                        Tile_Arrays[z + (y * 8) + x] = Tile_Arrays[z2 + (y * 8) + (7 - x)];
                        Tile_Arrays[z2 + (y * 8) + (7 - x)] = temp;
                    }
                }
            }
        }


        public static void tile_v_flip_big()
        {
            Tiles_2_Linear();

            int start_x = trans_x * 8;
            int final_x = (trans_w * 8) + start_x;
            int start_y = trans_y * 8;
            int final_y = (trans_h * 8) + start_y;
            int mid_y = trans_h * 4;


            for (int x1 = start_x; x1 < final_x; x1++)
            {
                for (int y1 = 0; y1 < mid_y; y1++)
                {
                    int top_y = start_y + y1;
                    int low_y = (final_y - y1) - 1;
                    int index1 = (top_y * 128) + x1;
                    int index2 = (low_y * 128) + x1;
                    int temp = trans_array[index1];
                    trans_array[index1] = trans_array[index2];
                    trans_array[index2] = temp;
                }
            }

            Linear_2_Tiles();
        }

        public static void tile_v_flip()
        {
            if (Form1.brushsize == Form1.BRUSH_MULTI)
            {
                tile_v_flip_big();
                return;
            }

            int z = (Form1.tile_set * 256 * 8 * 8) + (Form1.tile_num * 8 * 8); // base index
            if (Form1.tilesize == Form1.TILE_8X8)
            {
                for (int y = 0; y < 4; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        int temp = Tile_Arrays[z + (y * 8) + x];
                        Tile_Arrays[z + (y * 8) + x] = Tile_Arrays[z + ((7 - y) * 8) + x];
                        Tile_Arrays[z + ((7 - y) * 8) + x] = temp;
                    }
                }
            }
            else // 16x16
            {
                if ((Form1.tile_x > 14) || (Form1.tile_y > 14)) return;
                // this should never wrap around now

                int z2 = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 16) * 8 * 8); // base index
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        int temp = Tile_Arrays[z + (y * 8) + x];
                        Tile_Arrays[z + (y * 8) + x] = Tile_Arrays[z2 + ((7 - y) * 8) + x];
                        Tile_Arrays[z2 + ((7 - y) * 8) + x] = temp;
                    }
                }
                z = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 1) * 8 * 8); // base index
                z2 = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 17) * 8 * 8); // base index
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        int temp = Tile_Arrays[z + (y * 8) + x];
                        Tile_Arrays[z + (y * 8) + x] = Tile_Arrays[z2 + ((7 - y) * 8) + x];
                        Tile_Arrays[z2 + ((7 - y) * 8) + x] = temp;
                    }
                }
            }
        }


        public static void tile_rot_cw_big()
        {
            Tiles_2_Linear();

            // make it a square, or else this function will mangle it
            if (trans_w > trans_h)
            {
                trans_w = trans_h;
                Form1.BE_x2 = Form1.BE_x1 + trans_w;
            }
            if (trans_h > trans_w)
            {
                trans_h = trans_w;
                Form1.BE_y2 = Form1.BE_y1 + trans_h;
            }

            int start_x = trans_x * 8;
            int final_x = (trans_w * 8) + start_x;
            int start_y = trans_y * 8;
            int final_y = (trans_h * 8) + start_y;
            int mid_x = trans_w * 4;
            int mid_y = trans_h * 4;

            for (int y1 = 0; y1 < mid_y; y1++)
            {
                for (int x1 = 0; x1 < mid_x; x1++)
                {
                    int index1, index2, index3, index4;
                    int y2 = start_y + y1;
                    int x2 = start_x + x1;
                    index1 = (y2 * 128) + x2;
                    int temp = trans_array[index1];

                    int y3 = final_y - x1 - 1;
                    int x3 = start_x + y1;
                    index3 = (y3 * 128) + x3;
                    trans_array[index1] = trans_array[index3];

                    y2 = final_y - y1 - 1;
                    x2 = final_x - x1 - 1;
                    index4 = (y2 * 128) + x2;
                    trans_array[index3] = trans_array[index4];

                    y3 = start_y + x1;
                    x3 = final_x - y1 - 1;
                    index2 = (y3 * 128) + x3;
                    trans_array[index4] = trans_array[index2];

                    trans_array[index2] = temp;

                }
            }

            Linear_2_Tiles();
        }

        public static void tile_rot_cw() // R, rotate clockwise
        {
            if (Form1.brushsize == Form1.BRUSH_MULTI)
            {
                tile_rot_cw_big();
                return;
            }

            int z = (Form1.tile_set * 256 * 8 * 8) + (Form1.tile_num * 8 * 8); // base index
            int[] temp_arr = new int[64];
            if (Form1.tilesize == Form1.TILE_8X8)
            {
                int count = 0;
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 7; y >= 0; y--)
                    {
                        temp_arr[count++] = Tile_Arrays[z + (y * 8) + x];
                    }
                }
                for (int i = 0; i < 64; i++)
                {
                    Tile_Arrays[z + i] = temp_arr[i];
                }
            }
            else // 16x16
            {
                if ((Form1.tile_x > 14) || (Form1.tile_y > 14)) return;
                // this should never wrap around now
                
                // first copy tiles in to next over in a clockwise fashion
                for(int i = 0; i < 64; i++)
                {  // save top left
                    temp_arr[i] = Tile_Arrays[z + i];
                }
                int z2 = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 16) * 8 * 8); // base index
                for (int i = 0; i < 64; i++)
                {  // copy BL to TL
                    Tile_Arrays[z + i] = Tile_Arrays[z2 + i];
                }
                z = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 17) * 8 * 8); // base index
                for (int i = 0; i < 64; i++)
                {  // copy BR to BL
                    Tile_Arrays[z2 + i] = Tile_Arrays[z + i];
                }
                z2 = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 1) * 8 * 8); // base index
                for (int i = 0; i < 64; i++)
                {  // copy TR to BR
                    Tile_Arrays[z + i] = Tile_Arrays[z2 + i];
                }
                for (int i = 0; i < 64; i++)
                {  // copy temp to TR
                    Tile_Arrays[z2 + i] = temp_arr[i];
                }

                //now rotate each individual tile
                z = (Form1.tile_set * 256 * 8 * 8) + (Form1.tile_num * 8 * 8); // base index
                int count = 0;
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 7; y >= 0; y--)
                    {
                        temp_arr[count++] = Tile_Arrays[z + (y * 8) + x];
                    }
                }
                for (int i = 0; i < 64; i++)
                {
                    Tile_Arrays[z + i] = temp_arr[i];
                }
                z = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 1) * 8 * 8); // base index
                count = 0;
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 7; y >= 0; y--)
                    {
                        temp_arr[count++] = Tile_Arrays[z + (y * 8) + x];
                    }
                }
                for (int i = 0; i < 64; i++)
                {
                    Tile_Arrays[z + i] = temp_arr[i];
                }
                z = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 16) * 8 * 8); // base index
                count = 0;
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 7; y >= 0; y--)
                    {
                        temp_arr[count++] = Tile_Arrays[z + (y * 8) + x];
                    }
                }
                for (int i = 0; i < 64; i++)
                {
                    Tile_Arrays[z + i] = temp_arr[i];
                }
                z = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 17) * 8 * 8); // base index
                count = 0;
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 7; y >= 0; y--)
                    {
                        temp_arr[count++] = Tile_Arrays[z + (y * 8) + x];
                    }
                }
                for (int i = 0; i < 64; i++)
                {
                    Tile_Arrays[z + i] = temp_arr[i];
                }
            }
        }


        public static void tile_rot_ccw_big()
        {
            Tiles_2_Linear();

            // make it a square, or else this function will mangle it
            if (trans_w > trans_h)
            {
                trans_w = trans_h;
                Form1.BE_x2 = Form1.BE_x1 + trans_w;
            }
            if (trans_h > trans_w)
            {
                trans_h = trans_w;
                Form1.BE_y2 = Form1.BE_y1 + trans_h;
            }

            int start_x = trans_x * 8;
            int final_x = (trans_w * 8) + start_x;
            int start_y = trans_y * 8;
            int final_y = (trans_h * 8) + start_y;
            int mid_x = trans_w * 4;
            int mid_y = trans_h * 4;

            for (int y1 = 0; y1 < mid_y; y1++)
            {
                for (int x1 = 0; x1 < mid_x; x1++)
                {
                    int index1, index2, index3, index4;
                    int y2 = start_y + y1;
                    int x2 = start_x + x1;
                    index1 = (y2 * 128) + x2;
                    int temp = trans_array[index1];

                    int y3 = start_y + x1;
                    int x3 = final_x - y1 - 1;
                    index2 = (y3 * 128) + x3;
                    trans_array[index1] = trans_array[index2];

                    y2 = final_y - y1 - 1;
                    x2 = final_x - x1 - 1;
                    index4 = (y2 * 128) + x2;
                    trans_array[index2] = trans_array[index4];

                    y3 = final_y - x1 - 1;
                    x3 = start_x + y1;
                    index3 = (y3 * 128) + x3;
                    trans_array[index4] = trans_array[index3];

                    trans_array[index3] = temp;
                }
            }

            Linear_2_Tiles();
        }

        public static void tile_rot_ccw() // L, rotate counter clockwise
        {
            if (Form1.brushsize == Form1.BRUSH_MULTI)
            {
                tile_rot_ccw_big();
                return;
            }

            int z = (Form1.tile_set * 256 * 8 * 8) + (Form1.tile_num * 8 * 8); // base index
            int[] temp_arr = new int[64];
            if (Form1.tilesize == Form1.TILE_8X8)
            {
                int count = 0;
                for (int x = 7; x >= 0; x--)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        temp_arr[count++] = Tile_Arrays[z + (y * 8) + x];
                    }
                }
                for (int i = 0; i < 64; i++)
                {
                    Tile_Arrays[z + i] = temp_arr[i];
                }
            }
            else // 16x16
            {
                if ((Form1.tile_x > 14) || (Form1.tile_y > 14)) return;
                // this should never wrap around now

                // first copy tiles in to next over in a clockwise fashion
                for (int i = 0; i < 64; i++)
                {  // save top left
                    temp_arr[i] = Tile_Arrays[z + i];
                }
                int z2 = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 1) * 8 * 8); // base index
                for (int i = 0; i < 64; i++)
                {  // copy TR to TL
                    Tile_Arrays[z + i] = Tile_Arrays[z2 + i];
                }
                z = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 17) * 8 * 8); // base index
                for (int i = 0; i < 64; i++)
                {  // copy BR to TR
                    Tile_Arrays[z2 + i] = Tile_Arrays[z + i];
                }
                z2 = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 16) * 8 * 8); // base index
                for (int i = 0; i < 64; i++)
                {  // copy BL to BR
                    Tile_Arrays[z + i] = Tile_Arrays[z2 + i];
                }
                for (int i = 0; i < 64; i++)
                {  // copy temp to BL
                    Tile_Arrays[z2 + i] = temp_arr[i];
                }

                //now rotate each individual tile
                z = (Form1.tile_set * 256 * 8 * 8) + (Form1.tile_num * 8 * 8); // base index
                int count = 0;
                for (int x = 7; x >= 0; x--)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        temp_arr[count++] = Tile_Arrays[z + (y * 8) + x];
                    }
                }
                for (int i = 0; i < 64; i++)
                {
                    Tile_Arrays[z + i] = temp_arr[i];
                }
                z = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 1) * 8 * 8); // base index
                count = 0;
                for (int x = 7; x >= 0; x--)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        temp_arr[count++] = Tile_Arrays[z + (y * 8) + x];
                    }
                }
                for (int i = 0; i < 64; i++)
                {
                    Tile_Arrays[z + i] = temp_arr[i];
                }
                z = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 16) * 8 * 8); // base index
                count = 0;
                for (int x = 7; x >= 0; x--)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        temp_arr[count++] = Tile_Arrays[z + (y * 8) + x];
                    }
                }
                for (int i = 0; i < 64; i++)
                {
                    Tile_Arrays[z + i] = temp_arr[i];
                }
                z = (Form1.tile_set * 256 * 8 * 8) + ((Form1.tile_num + 17) * 8 * 8); // base index
                count = 0;
                for (int x = 7; x >= 0; x--)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        temp_arr[count++] = Tile_Arrays[z + (y * 8) + x];
                    }
                }
                for (int i = 0; i < 64; i++)
                {
                    Tile_Arrays[z + i] = temp_arr[i];
                }
            }
        }


        public static void tile_fill_big()
        {
            Tiles_2_Linear();

            int start_x = trans_x * 8;
            int final_x = (trans_w * 8) + start_x;
            int start_y = trans_y * 8;
            int final_y = (trans_h * 8) + start_y;

            int color = Form1.pal_x;

            for (int y1 = start_y; y1 < final_y; y1++)
            {
                for (int x1 = start_x; x1 < final_x; x1++)
                {
                    int index = (y1 * 128) + x1;
                    trans_array[index] = color;
                }
            }

            Linear_2_Tiles();
        }

        public static void tile_fill()
        { // fill with currently selected color.
            if (Form1.brushsize == Form1.BRUSH_MULTI)
            {
                tile_fill_big();
                return;
            }

            int z = (Form1.tile_set * 256 * 8 * 8) + (Form1.tile_num * 8 * 8); // base index
            int color = Form1.pal_x;
            if (Form1.tile_set > 3) color = color & 3;
            if (Form1.tilesize == Form1.TILE_8X8)
            {
                for (int x = 0; x < 64; x++)
                {
                    Tile_Arrays[z + x] = color;
                }
            }
            else // 16x16
            {
                if ((Form1.tile_x > 14) || (Form1.tile_y > 14)) return;
                // this should never wrap around now

                for (int x = 0; x < 64; x++)
                {
                    Tile_Arrays[z + x] = color;
                }
                int temp_tile_num = (Form1.tile_num + 1);
                z = (Form1.tile_set * 256 * 8 * 8) + (temp_tile_num * 8 * 8);
                for (int x = 0; x < 64; x++)
                {
                    Tile_Arrays[z + x] = color;
                }
                temp_tile_num = (Form1.tile_num + 16);
                z = (Form1.tile_set * 256 * 8 * 8) + (temp_tile_num * 8 * 8);
                for (int x = 0; x < 64; x++)
                {
                    Tile_Arrays[z + x] = color;
                }
                temp_tile_num = (Form1.tile_num + 17);
                z = (Form1.tile_set * 256 * 8 * 8) + (temp_tile_num * 8 * 8);
                for (int x = 0; x < 64; x++)
                {
                    Tile_Arrays[z + x] = color;
                }
            }

        }


        public static void select_all()
        {
            if (Form1.brushsize != Form1.BRUSH_MULTI)
            {
                return;
            }

            trans_x = 0;
            trans_y = 0;
            trans_w = 16;
            trans_h = 16;
            Form1.BE_x1 = 0;
            Form1.BE_y1 = 0;
            Form1.BE_x2 = 16;
            Form1.BE_y2 = 16;
            // seems redundant to have 2 sets of vars basically the same
            // decided to leave duplicate vars anyway
            Form1.tile_x = 0;
            Form1.tile_y = 0;
            Form1.tile_num = 0;
        }

        // these functions were done peice meal over seveeral years
        // with multiple version doing basically the same thing
        // and could probably be merged into single functions
    }
}
