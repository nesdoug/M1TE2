
namespace M1TE2
{
    public static class Maps
    {
        public static int[] tile = new int[32 * 32 * 3]; //x, y, 3 layers
        //tile can be value 0-1023, high 2 bits references the tileset
        //the 2bpp set will be fixed at 0-255, this is not the true limit
        public static int[] palette = new int[32 * 32 * 3]; //x, y, 3 layers
        public static int[] h_flip = new int[32 * 32 * 3]; //x, y, 3 layers
        public static int[] v_flip = new int[32 * 32 * 3]; //x, y, 3 layers
        public static int[] priority = new int[32 * 32 * 3]; //x, y, 3 layers
        // priority affects how sprite layers show above or below
        // but, you can't see the difference here

        //if the height is not 32, it will save a shorter map
    }
}