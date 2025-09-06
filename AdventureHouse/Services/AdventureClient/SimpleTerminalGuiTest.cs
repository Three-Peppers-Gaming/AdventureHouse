using System;
using Terminal.Gui;

namespace AdventureHouse.Services.AdventureClient
{
    /// <summary>
    /// Simple Terminal.Gui test client to verify Terminal.Gui compatibility
    /// </summary>
    public class SimpleTerminalGuiTest
    {
        public static void RunTest()
        {
            try
            {
                Console.WriteLine("Testing Terminal.Gui compatibility...");
                
                // Initialize Terminal.Gui
                Application.Init();
                
                Console.WriteLine($"Driver: {Application.Driver?.GetType().Name ?? "null"}");
                Console.WriteLine($"Top: {Application.Top?.GetType().Name ?? "null"}");
                
                if (Application.Driver != null)
                {
                    Console.WriteLine($"Terminal size: {Application.Driver.Cols}x{Application.Driver.Rows}");
                }
                
                // Create a simple window without using Application.Top initially
                var window = new Window("Test Window")
                {
                    X = 0,
                    Y = 0,
                    Width = Dim.Fill(),
                    Height = Dim.Fill()
                };
                
                var label = new Label("Terminal.Gui is working!")
                {
                    X = Pos.Center(),
                    Y = Pos.Center()
                };
                
                var quitButton = new Button("Quit")
                {
                    X = Pos.Center(),
                    Y = Pos.Center() + 2
                };
                
                quitButton.Clicked += () => Application.RequestStop();
                
                window.Add(label);
                window.Add(quitButton);
                
                // Run the application directly with the window
                Application.Run(window);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Terminal.Gui test failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                try
                {
                    Application.Shutdown();
                }
                catch
                {
                    // Ignore shutdown errors
                }
            }
        }
    }
}