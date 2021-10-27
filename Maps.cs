
namespace M1TE2
{
    public static class Maps
    {
        public static int[] tile = new int[32 * 32 * 3]; //x, y, 3 layers
        //tile can be value 0-1023, high 2 bits references the tileset
        
        public static int[] palette = new int[32 * 32 * 3]; //x, y, 3 layers
        public static int[] h_flip = new int[32 * 32 * 3]; //x, y, 3 layers
        public static int[] v_flip = new int[32 * 32 * 3]; //x, y, 3 layers
        public static int[] priority = new int[32 * 32 * 3]; //x, y, 3 layers
        // priority affects how sprite layers show above or below
        // but, you can't see the difference here

        //if the height is not 32, it will save a shorter map
    }

    public static class MapsU // undo backup copy
    {
        public static int[] tile = new int[32 * 32 * 3]; //x, y, 3 layers
        //tile can be value 0-1023, high 2 bits references the tileset
        
        public static int[] palette = new int[32 * 32 * 3]; //x, y, 3 layers
        public static int[] h_flip = new int[32 * 32 * 3]; //x, y, 3 layers
        public static int[] v_flip = new int[32 * 32 * 3]; //x, y, 3 layers
        public static int[] priority = new int[32 * 32 * 3]; //x, y, 3 layers
        // priority affects how sprite layers show above or below
        // but, you can't see the difference here

        //if the height is not 32, it will save a shorter map
    }
}