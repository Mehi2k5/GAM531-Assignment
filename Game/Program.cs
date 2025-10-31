using System;

namespace Game
{
    public static class Program
    {
        public static void Main()
        {
            using var game = new GameWindowWrapper(1280, 720, "Mini 3D Explorer - Midterm");
            game.Run();
        }
    }
}
