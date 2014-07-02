namespace Assets.BresenhamLine
{
    [System.Flags]
    public enum Overlap : byte
    {
        NONE = 1 << 0,

        // Overlap - first go major then minor direction
        MAJOR = 1 << 1,

        // Overlap - first go minor then major direction
        MINOR = 1 << 2
    }
}