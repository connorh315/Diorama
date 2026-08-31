global using Common;
using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.OpenGL;
using BrickVault.Types;
using Diorama.Core;
using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.SHADERS;
using Diorama.Core.Filetypes.TEXTURES;
using Diorama.Core.IO;
using Diorama.Editor;
using Diorama.Editor.glTF;
using Diorama.Editor.ShaderSystem;
using OpenTK.Graphics.ES11;
using OpenTK.Platform.Windows;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection.PortableExecutable;


namespace Diorama
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .UseSkia()
                .LogToTrace();

        private static readonly Stopwatch AppTimer = Stopwatch.StartNew();

        public static float TimeSinceStart => (float)AppTimer.Elapsed.TotalSeconds;

        public static void Mainaqa(string[] args)
        {
            AppSettings.Initialize();

            //var gFile = FileProvider.GetFile("chars\\minifigs\\super_characters\\super_city\\super_city_dx11.ghg");

            //GScene.Parse(gFile);

            //return;


            int total = 0;
            int success = 0;

            foreach (var file in FileProvider.EnumerateFiles("gsc", "ghg"))
            {
                total++;
                try
                {
                    GScene scene = GScene.Parse(file);

                    success++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Skipped file: {ex.Message}");
                }
            }

            Console.WriteLine($"{success} / {total}");
        }
    }
}
