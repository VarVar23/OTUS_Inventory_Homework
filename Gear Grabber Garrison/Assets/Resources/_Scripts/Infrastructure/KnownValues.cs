public static class KnownValues
{
    public static class Scene
    {
        public const string LOADING = "Loading";
        public const string MENU = "Menu";
        public const string GAME = "1. Game";

        public static bool IsKnown(string value) =>
            value == LOADING || value == MENU || value == GAME; 
    }

    public static class Addresables
    {
        public const string GAME = "Game";
        public const string CANVAS = "UI_Canvas";
        public const string GAMEPLAY = "Gameplay";
    }
}