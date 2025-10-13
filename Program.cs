using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace OpenTK_FPSCamera
{
    internal static class Program
    {
        static void Main()
        {
            var nativeSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(1280, 720),
                Title = "Assignment 6 — FPS Camera (OpenTK)",
                // You can set APIVersion if needed, but it's optional for this assignment
                // APIVersion = new Version(4, 5)
            };

            // GameWindowSettings no longer needs RenderFrequency or UpdateFrequency.
            // Default behavior: uncapped render, ~60Hz update.
            var gameSettings = new GameWindowSettings();

            using (var game = new Game(gameSettings, nativeSettings))
            {
                game.Run();
            }
        }
    }
}
