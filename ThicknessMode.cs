namespace Assets.BresenhamLine
{
    public enum ThicknessMode : byte
    {
        /**
         * Line goes through the center
         */
        MIDDLE,

        /**
         * Line goes along the border (clockwise)
         */
        CLOCKWISE,

        /**
         * Line goes along the border (counter-clockwise)
         */
        COUNTERCLOCKWISE
    }
}