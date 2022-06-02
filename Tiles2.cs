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

        public static void Nix_Copy()
        {
            Has_Copied = false;
        }

        public static void shift_left()
        {
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

        public static void shift_right()
        {
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

        public static void shift_up()
        {
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

        public static void shift_down()
        {
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

        public static void tile_copy()
        {
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

        public static void tile_paste()
        {
            if (Has_Copied == true)
            {
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
        }

        public static void tile_delete()
        {
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

        public static void tile_h_flip()
        {
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

        public static void tile_v_flip()
        {
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

        public static void tile_rot_cw() // R, rotate clockwise
        {
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

        public static void tile_rot_ccw() // L, rotate counter clockwise
        {
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

        public static void tile_fill()
        { // fill with currently selected color.
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


    }
}
