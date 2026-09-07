public static class GameResultData
{
    public static int GoldCount;
    public static int RubyCount;
    public static int SaphireCount;
    public static int EmeraldCount;
    public static int DiamondCount;

    public static int Score;

    public static void Reset()
    {
        GoldCount = 0;
        RubyCount = 0;
        SaphireCount = 0;
        EmeraldCount = 0;
        DiamondCount = 0;

        Score = 0;
    }
}