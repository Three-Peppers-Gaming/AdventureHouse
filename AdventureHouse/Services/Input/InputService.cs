using AdventureHouse.Services.Models;
using Spectre.Console;

namespace AdventureHouse.Services.Input
{
    /// <summary>
    /// Input service that handles user input and command history
    /// </summary>
    public class InputService : IInputService
    {
        private readonly List<string> _commandHistory = new();

        public List<string> CommandHistory => _commandHistory;

        public void InitializeCommandBuffer()
        {
            // Pre-populate command history with common commands
            foreach (var cmd in UIConfiguration.CommonGameCommands)
            {
                _commandHistory.Add(cmd);
            }
        }

        public string GetUserInput(bool useClassicMode)
        {
            if (useClassicMode)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(UIConfiguration.NextActionPrompt);
                
                // Use enhanced input with command buffer
                var input = GetUserInputWithHistory();
                return input;
            }
            else
            {
                // For enhanced mode, use Spectre.Console prompt but then use our command buffer
                AnsiConsole.MarkupLine("[bold white]Next Action?[/]");
                Console.Write("> ");
                
                // Get input using our custom command buffer
                var input = GetUserInputWithHistorySimple();
                return input;
            }
        }

        private string GetUserInputWithHistory()
        {
            string input = string.Empty;
            int historyIndex = _commandHistory.Count;
            List<ConsoleKeyInfo> currentLine = new();
            int cursorPosition = 0;

            int startPosition = Console.CursorLeft;

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        input = new string(currentLine.Select(k => k.KeyChar).ToArray());
                        
                        // Add to history if not empty and not duplicate of last command
                        if (!string.IsNullOrWhiteSpace(input) && 
                            (_commandHistory.Count == 0 || _commandHistory.Last() != input))
                        {
                            _commandHistory.Add(input);
                        }
                        return input;

                    case ConsoleKey.UpArrow:
                        if (historyIndex > 0)
                        {
                            historyIndex--;
                            LoadFromHistory(historyIndex, ref currentLine, ref cursorPosition, startPosition);
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (historyIndex < _commandHistory.Count - 1)
                        {
                            historyIndex++;
                            LoadFromHistory(historyIndex, ref currentLine, ref cursorPosition, startPosition);
                        }
                        else if (historyIndex == _commandHistory.Count - 1)
                        {
                            historyIndex++;
                            currentLine.Clear();
                            cursorPosition = 0;
                            RefreshLine(currentLine, startPosition, cursorPosition);
                        }
                        break;

                    case ConsoleKey.LeftArrow:
                        if (cursorPosition > 0)
                        {
                            cursorPosition--;
                            Console.SetCursorPosition(startPosition + cursorPosition, Console.CursorTop);
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (cursorPosition < currentLine.Count)
                        {
                            cursorPosition++;
                            Console.SetCursorPosition(startPosition + cursorPosition, Console.CursorTop);
                        }
                        break;

                    case ConsoleKey.Home:
                        cursorPosition = 0;
                        Console.SetCursorPosition(startPosition, Console.CursorTop);
                        break;

                    case ConsoleKey.End:
                        cursorPosition = currentLine.Count;
                        Console.SetCursorPosition(startPosition + cursorPosition, Console.CursorTop);
                        break;

                    case ConsoleKey.Escape:
                        currentLine.Clear();
                        cursorPosition = 0;
                        historyIndex = _commandHistory.Count;
                        RefreshLine(currentLine, startPosition, cursorPosition);
                        break;

                    case ConsoleKey.Backspace:
                        if (cursorPosition > 0)
                        {
                            currentLine.RemoveAt(cursorPosition - 1);
                            cursorPosition--;
                            RefreshLine(currentLine, startPosition, cursorPosition);
                        }
                        break;

                    case ConsoleKey.Delete:
                        if (cursorPosition < currentLine.Count)
                        {
                            currentLine.RemoveAt(cursorPosition);
                            RefreshLine(currentLine, startPosition, cursorPosition);
                        }
                        break;

                    default:
                        // Only accept printable ASCII characters (32-126)
                        if (!char.IsControl(keyInfo.KeyChar) && 
                            keyInfo.KeyChar >= UIConfiguration.MinPrintableAscii && 
                            keyInfo.KeyChar <= UIConfiguration.MaxPrintableAscii)
                        {
                            currentLine.Insert(cursorPosition, keyInfo);
                            cursorPosition++;
                            RefreshLine(currentLine, startPosition, cursorPosition);
                        }
                        break;
                }
            }
        }

        // Simpler version for enhanced mode
        private string GetUserInputWithHistorySimple()
        {
            string input = string.Empty;
            int historyIndex = _commandHistory.Count;
            List<ConsoleKeyInfo> currentLine = new();
            int cursorPosition = 0;

            int startPosition = Console.CursorLeft;

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        input = new string(currentLine.Select(k => k.KeyChar).ToArray());
                        
                        // Add to history if not empty and not duplicate of last command
                        if (!string.IsNullOrWhiteSpace(input) && 
                            (_commandHistory.Count == 0 || _commandHistory.Last() != input))
                        {
                            _commandHistory.Add(input);
                        }
                        return input;

                    case ConsoleKey.UpArrow:
                        if (historyIndex > 0)
                        {
                            historyIndex--;
                            LoadFromHistory(historyIndex, ref currentLine, ref cursorPosition, startPosition);
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (historyIndex < _commandHistory.Count - 1)
                        {
                            historyIndex++;
                            LoadFromHistory(historyIndex, ref currentLine, ref cursorPosition, startPosition);
                        }
                        else if (historyIndex == _commandHistory.Count - 1)
                        {
                            historyIndex++;
                            currentLine.Clear();
                            cursorPosition = 0;
                            RefreshLine(currentLine, startPosition, cursorPosition);
                        }
                        break;

                    case ConsoleKey.LeftArrow:
                        if (cursorPosition > 0)
                        {
                            cursorPosition--;
                            Console.SetCursorPosition(startPosition + cursorPosition, Console.CursorTop);
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (cursorPosition < currentLine.Count)
                        {
                            cursorPosition++;
                            Console.SetCursorPosition(startPosition + cursorPosition, Console.CursorTop);
                        }
                        break;

                    case ConsoleKey.Home:
                        cursorPosition = 0;
                        Console.SetCursorPosition(startPosition, Console.CursorTop);
                        break;

                    case ConsoleKey.End:
                        cursorPosition = currentLine.Count;
                        Console.SetCursorPosition(startPosition + cursorPosition, Console.CursorTop);
                        break;

                    case ConsoleKey.Escape:
                        currentLine.Clear();
                        cursorPosition = 0;
                        historyIndex = _commandHistory.Count;
                        RefreshLine(currentLine, startPosition, cursorPosition);
                        break;

                    case ConsoleKey.Backspace:
                        if (cursorPosition > 0)
                        {
                            currentLine.RemoveAt(cursorPosition - 1);
                            cursorPosition--;
                            RefreshLine(currentLine, startPosition, cursorPosition);
                        }
                        break;

                    case ConsoleKey.Delete:
                        if (cursorPosition < currentLine.Count)
                        {
                            currentLine.RemoveAt(cursorPosition);
                            RefreshLine(currentLine, startPosition, cursorPosition);
                        }
                        break;

                    default:
                        // Only accept printable ASCII characters (32-126)
                        if (!char.IsControl(keyInfo.KeyChar) && 
                            keyInfo.KeyChar >= UIConfiguration.MinPrintableAscii && 
                            keyInfo.KeyChar <= UIConfiguration.MaxPrintableAscii)
                        {
                            currentLine.Insert(cursorPosition, keyInfo);
                            cursorPosition++;
                            RefreshLine(currentLine, startPosition, cursorPosition);
                        }
                        break;
                }
            }
        }

        private void LoadFromHistory(int historyIndex, ref List<ConsoleKeyInfo> currentLine, ref int cursorPosition, int startPosition)
        {
            if (historyIndex >= 0 && historyIndex < _commandHistory.Count)
            {
                currentLine.Clear();
                string historyCommand = _commandHistory[historyIndex];
                foreach (char c in historyCommand)
                {
                    currentLine.Add(new ConsoleKeyInfo(c, (ConsoleKey)c, false, false, false));
                }
                cursorPosition = currentLine.Count;
                RefreshLine(currentLine, startPosition, cursorPosition);
            }
        }

        private static void RefreshLine(List<ConsoleKeyInfo> currentLine, int startPosition, int cursorPosition)
        {
            try
            {
                // Clear the line
                Console.SetCursorPosition(startPosition, Console.CursorTop);
                Console.Write(new string(' ', Math.Min(Console.WindowWidth - startPosition - 1, UIConfiguration.SafeConsoleWidth)));
                Console.SetCursorPosition(startPosition, Console.CursorTop);
                
                // Write current line
                foreach (var keyInfo in currentLine)
                {
                    Console.Write(keyInfo.KeyChar);
                }
                
                // Set cursor position
                Console.SetCursorPosition(startPosition + cursorPosition, Console.CursorTop);
            }
            catch
            {
                // If console operations fail, just continue
            }
        }
    }
}