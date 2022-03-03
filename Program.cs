using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using Somewhere2.ApplicationState;
using Somewhere2.System;

namespace Somewhere2
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandHandler handler = new CommandHandler();
            handler.Start();
        }
    }
    
    class CommandHandler
    {
        #region Construction
        public CommandHandler()
        {
            CurrentWorkingDirectory = Directory.GetCurrentDirectory();
        }
        #endregion
        
        #region Interface
        public void Start()
        {
            PrintWelcomeText();
            while (!ShouldExit)
            {
                Console.Write($"> {CurrentWorkingDirectory}: ");
                string input = Console.ReadLine();
                PreProcessInput(input);
            }
        }

        public void PreProcessInput(string input)
        {
            HandleCommands(input.ToLower(), new string[]{}, new Dictionary<string, string>());
        }
        #endregion

        #region States
        public bool ShouldExit { get; set; }
        public string CurrentWorkingDirectory { get; set; }
        #endregion

        #region Utilities

        #endregion

        #region Commands

        #endregion

        #region Routines

        private void ColorfulPrint(string text)
        {
            // Save previous color
            var previous = Console.ForegroundColor;

            StringBuilder buffer = new StringBuilder();
            string currentStyle = "Default";
            void SwitchStyle()
            {
                switch (currentStyle)
                {
                    // Special names
                    default:
                    case "Default":
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        break;
                    case "Bold":
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    // Color names
                    case "Blue":
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case "Green":
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                }
            }
            void EnterSpan(string style)
            {
                // Output existing buffer
                if (buffer.Length != 0)
                {
                    SwitchStyle();
                    Console.Write(buffer);
                }
                // Start new buffer
                currentStyle = style;
                buffer.Clear();
            }

            // Parse and output each character as buffer
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                char cNext = i < text.Length - 1 ? text[i + 1] : char.MinValue;
                char cNextNext = i < text.Length - 2 ? text[i + 2] : char.MinValue;
                
                // Bolding
                if (c == '*' && cNext == '*')
                {
                    if (currentStyle != "Bold")
                        EnterSpan("Bold");    
                    else 
                        EnterSpan("Default");
                    i++;    // Skip next
                }
                // Tag starting
                else if (c == '<' && cNext != '/')
                {
                    string remaining = text.Substring(i + 1);
                    int endingBracket = remaining.IndexOf('>');
                    if (endingBracket > 1)
                    {
                        string tag = remaining.Substring(0, endingBracket);
                        EnterSpan(tag);
                        i += endingBracket + 1;
                    }
                }
                // Tag ending
                else if (c == '<' && cNext == '/' && cNextNext == '>')
                {
                    EnterSpan("Default");
                    i += 2; // Skip next 2
                }
                else
                    buffer.Append(c);
            }
            
            // Print remaining
            if (buffer.Length != 0)
                EnterSpan("Finish");
            // Reset
            Console.ForegroundColor = previous;
        }
        private void ColorfulPrintLine(string text)
        {
            ColorfulPrint(text);
            Console.WriteLine();
        }
        private void HandleCommands(string command, string[] rawArguments, Dictionary<string, string> kepMaps)
        {
            switch (command)
            {
                case "exit":
                    ShouldExit = true;
                    break;
                case "gui":
                    RunGUI();
                    break;
                case "help":
                    break;
            }
        }
        private void HideConsole()
            => WindowHelper.HideConsole();
        private void PrintWelcomeText()
        {
            string text = Helpers.ReadTextResource("Somewhere2.Documentation.Welcome.txt");
            ColorfulPrintLine(text);
        }
        private void RunGUI()
        {
            HideConsole();
            
            Application window = new Application();
            window.Run();
        }
        #endregion
    }
}