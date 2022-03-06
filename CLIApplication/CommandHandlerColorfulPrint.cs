using System;
using System.Text;

namespace Somewhere2.CLIApplication
{
    internal partial class CommandHandler
    {
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
                    case "Code":
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                    default:
                    case "Default":
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        break;
                    case "Bold":
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case "Body":
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case "Emphasis":
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case "Warning":
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case "Error":
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                    // Color names
                    case "Blue":
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case "Cyan":
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case "Gray":
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case "Green":
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                    case "Orange":
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case "Yellow":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case "White":
                        Console.ForegroundColor = ConsoleColor.White;
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
        #endregion
    }
}